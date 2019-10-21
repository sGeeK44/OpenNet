using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using OpenNet.Orm.Caches;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql.Queries;
using OpenNet.Orm.Sql.Schema;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UseStringInterpolation
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable IntroduceOptionalParameters.Local

namespace OpenNet.Orm.Sql
{
    public class SqlDataStore : DataStore, ISqlDataStore
    {
        private readonly object _transactionSyncRoot = new object();
        private readonly IDbEngine _dbEngine;
        private readonly ISqlFactory _sqlFactory;
        private readonly IDbAccessStrategy _dbAccessStrategy;
        private readonly IFieldPropertyFactory _sqlFieldPropertyFactory;
        private readonly ISchemaChecker _schemaChecker;

        private IDbConnection _connection;
        private int _connectionCount;
        private readonly List<IDbConnection> _connectionPool;

        public IDbTransaction CurrentTransaction { get; set; }

        public int ConnectionPoolSize { get; set; }

        public override ISqlFactory SqlFactory { get { return _sqlFactory; } }
        public override IFieldPropertyFactory FieldPropertyFactory { get { return _sqlFieldPropertyFactory; } }
        public override IEntityCache Cache { get; set; }

        public SqlDataStore(IDbEngine dbEngine, ISqlFactory sqlFactory)
        {
            _dbEngine = dbEngine;
            _sqlFactory = sqlFactory;
            _dbAccessStrategy = _sqlFactory.CreateDbAccessStrategy(this);
            _sqlFieldPropertyFactory = _sqlFactory.CreateFieldPropertyFactory();
            _schemaChecker = _sqlFactory.CreateSchemaChecker(this);
            _connectionPool = new List<IDbConnection>();
            ConnectionPoolSize = 20;
        }

        ~SqlDataStore()
        {
            Dispose();
        }

        public void ValidateTable(IEntityInfo entity)
        {
            _dbAccessStrategy.ValidateTable(entity);
        }

        public override bool StoreExists
        {
            get { return _dbEngine.DatabaseExists; }
        }

        public override string Name
        {
            get { return _dbEngine.Name; }
        }

        /// <summary>
        /// Deletes the underlying DataStore
        /// </summary>
        public override void DeleteStore()
        {
            _dbEngine.DeleteDatabase();
        }

        /// <summary>
        /// Creates the underlying DataStore
        /// </summary>
        public override void CreateStore()
        {
            _dbEngine.CreateDatabase();

            var connection = GetConnection();
            foreach (var entity in Entities)
            {
                CreateTable(connection, entity);
            }
        }

        public override void CloseConnections()
        {
            if (_connectionPool == null)
                return;

            foreach (var connection in _connectionPool)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                connection.Dispose();
            }
            _connectionPool.Clear();
        }

        public override void Dispose()
        {
            try
            {
                base.Dispose();
                foreach (var connection in _connectionPool)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
                _connectionPool.Clear();
            }
            catch (Exception ex)
            {
                OrmDebug.Info(ex.Message);
                if (Debugger.IsAttached) Debugger.Break();
            }

            GC.SuppressFinalize(this);
        }

        private IDbConnection GetPoolConnection()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("SqlStoreBase");

            lock(_connectionPool)
            {
                IDbConnection connection;

                do
                {
                    connection = GetFreeConnectionInPool();

                    if (connection != null)
                    {
                        if (Open(connection))
                            return connection;

                        // Broken connection, maybe disposed
                        _connectionPool.Remove(connection);
                    }

                    if (_connectionPool.Count < ConnectionPoolSize)
                    {
                        connection = _dbEngine.GetNewConnection();
                        connection.Open();
                        _connectionPool.Add(connection);
                        Interlocked.Increment(ref _connectionCount);
                        OrmDebug.Trace("Creating pooled connection");
                        return connection;
                    }

                    // pool is full, we have to wait
                    Thread.Sleep(1000);

                    // TODO: add a timeout?
                } while (connection == null);

                // this should never happen
                throw new TimeoutException("Unable to get a pooled connection.");
            }
        }

        private static bool Open(IDbConnection connection)
        {
            return Open(connection, false);
        }

        private static bool Open(IDbConnection connection, bool isRetry)
        {
            if (connection == null)
                return false;

            // make sure the connection is open (in the event we has some network condition that closed it, etc.)
            if (connection.State == ConnectionState.Open)
                return true;

            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                if (isRetry)
                    return false;

                connection.Dispose();

                // retry once
                Thread.Sleep(1000);
                return Open(connection, true);
            }
        }

        public IDbConnection GetConnection()
        {
            return GetPoolConnection();
        }

        private IDbConnection GetFreeConnectionInPool()
        {
            return _connectionPool.FirstOrDefault(IsFreeForUse);
        }

        private static bool IsFreeForUse(IDbConnection connection)
        {
            return connection.State != ConnectionState.Executing
                && connection.State != ConnectionState.Fetching;
        }

        public int ExecuteNonQuery(string sql)
        {
            using (var command = SqlFactory.CreateCommand())
            {
                command.CommandText = sql;
                return ExecuteNonQuery(command);
            }
        }

        public int ExecuteNonQuery(IDbCommand command)
        {
            var connection = GetConnection();
            command.Connection = connection;
            command.Transaction = CurrentTransaction;
            return command.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql)
        {
            using (var command = SqlFactory.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = CurrentTransaction;
                return ExecuteScalar(command);
            }
        }

        public object ExecuteScalar(IDbCommand command)
        {
            var connection = GetConnection();
            command.Connection = connection;
            command.Transaction = CurrentTransaction;
            return command.ExecuteScalar();
        }

        protected string[] ReservedWords { get { return SqlReserved; } }

        private static readonly string[] SqlReserved = {
            "IDENTITY" ,"ENCRYPTION" ,"ORDER" ,"ADD" ,"END" ,"OUTER" ,"ALL" ,"ERRLVL" ,"OVER" ,"ALTER" ,"ESCAPE" ,"PERCENT" ,"AND" ,"EXCEPT" ,"PLAN" ,"ANY" ,"EXEC" ,"PRECISION" ,"AS" ,"EXECUTE" ,"PRIMARY" ,"ASC",
            "EXISTS" ,"PRINT" ,"AUTHORIZATION" ,"EXIT" ,"PROC" ,"AVG" ,"EXPRESSION" ,"PROCEDURE" ,"BACKUP" ,"FETCH" ,"PUBLIC" ,"BEGIN" ,"FILE" ,"RAISERROR" ,"BETWEEN" ,"FILLFACTOR" ,"READ" ,"BREAK" ,"FOR" ,"READTEXT",
            "BROWSE" ,"FOREIGN" ,"RECONFIGURE" ,"BULK" ,"FREETEXT" ,"BY" ,"FREETEXTTABLE" ,"REPLICATION" ,"CASCADE" ,"FROM" ,"RESTORE" ,"CASE" ,"FULL" ,"RESTRICT" ,"CHECK" ,"FUNCTION" ,"RETURN" ,"CHECKPOINT",
            "GOTO" ,"REVOKE" ,"CLOSE" ,"GRANT" ,"RIGHT" ,"CLUSTERED" ,"GROUP" ,"ROLLBACK" ,"COALESCE" ,"HAVING" ,"ROWCOUNT" ,"COLLATE" ,"HOLDLOCK" ,"ROWGUIDCOL" ,"COLUMN" ,"IDENTITY" ,"RULE",
            "COMMIT" ,"IDENTITY_INSERT" ,"SAVE" ,"COMPUTE" ,"IDENTITYCOL" ,"SCHEMA" ,"CONSTRAINT" ,"IF" ,"SELECT" ,"CONTAINS" ,"IN" ,"SESSION_USER" ,"CONTAINSTABLE" ,"SET" ,"CONTINUE" ,"INNER" ,"SETUSER",
            "CONVERT" ,"INSERT" ,"SHUTDOWN" ,"COUNT" ,"INTERSECT" ,"SOME" ,"CREATE" ,"INTO" ,"STATISTICS" ,"CROSS" ,"IS" ,"SUM" ,"CURRENT" ,"JOIN" ,"SYSTEM_USER" ,"CURRENT_DATE" ,"TABLE" ,"CURRENT_TIME" ,"KILL",
            "TEXTSIZE" ,"CURRENT_TIMESTAMP" ,"LEFT" ,"THEN" ,"CURRENT_USER" ,"LIKE" ,"TO" ,"CURSOR" ,"LINENO" ,"TOP" ,"DATABASE" ,"LOAD" ,"TRAN" ,"DATABASEPASSWORD" ,"MAX" ,"TRANSACTION" ,"DATEADD" ,"MIN" ,"TRIGGER",
            "DATEDIFF" ,"NATIONAL" ,"TRUNCATE" ,"DATENAME" ,"NOCHECK" ,"TSEQUAL" ,"DATEPART" ,"NONCLUSTERED" ,"UNION" ,"DBCC" ,"NOT" ,"DEALLOCATE", "NULL", "UPDATE", "DECLARE", "NULLIF", "UPDATETEXT",
            "DEFAULT", "OF", "USE", "DELETE", "OFF", "USER", "DENY", "OFFSETS", "VALUES", "DESC", "ON", "VARYING", "DISK", "OPEN", "VIEW", "DISTINCT", "OPENDATASOURCE", "WAITFOR", "DISTRIBUTED", "OPENQUERY", "WHEN",
            "DOUBLE", "OPENROWSET", "WHERE", "DROP", "OPENXML", "WHILE", "DUMP", "OPTION", "WITH", "ELSE", "OR", "WRITETEXT"
        };

        public void CreateTable(IDbConnection connection, IEntityInfo entity)
        {
            var sql = new StringBuilder();
            var entityNameInStore = entity.GetNameInStore();

            if (ReservedWords.Contains(entityNameInStore, StringComparer.InvariantCultureIgnoreCase))
            {
                throw new ReservedWordException(entityNameInStore);
            }

            sql.AppendFormat("CREATE TABLE [{0}] (", entityNameInStore);

            var first = true;

            foreach (var field in entity.Fields)
            {
                if (ReservedWords.Contains(field.FieldName, StringComparer.InvariantCultureIgnoreCase))
                {
                    throw new ReservedWordException(field.FieldName);
                }

                if (first)
                    first = false;
                else
                    sql.Append(", ");

                sql.AppendFormat(field.GetFieldDefinitionSqlQuery());

            }

            foreach (var foreignKey in entity.ForeignKeys)
            {
                sql.Append(", ");
                sql.AppendFormat(foreignKey.GetTableCreateSqlQuery());
            }

            sql.Append(")");

            OrmDebug.Info(sql.ToString());


            using (var command = SqlFactory.CreateCommand())
            {
                command.CommandText = sql.ToString();
                command.Connection = connection;
                command.Transaction = CurrentTransaction;
                command.ExecuteNonQuery();
            }

            VerifiyPrimaryKey(entity.PrimaryKey);

            foreach (var foreignKey in entity.ForeignKeys)
            {
                VerifyForeignKey(foreignKey);
            }

            foreach (var index in entity.Indexes)
            {
                VerifyIndex(index);
            }
        }

        public void VerifiyPrimaryKey(PrimaryKey primaryKey)
        {
            _schemaChecker.VerifyPrimaryKey(primaryKey);
        }

        public void VerifyForeignKey(ForeignKey foreignKey)
        {
            _schemaChecker.VerifyForeignKey(foreignKey);
        }

        public void VerifyIndex(Index index)
        {
            _schemaChecker.VerifyIndex(index);
        }

        /// <summary>
        /// Retrieves a single entity instance from the DataStore identified by the specified primary key value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public override T Select<T>(object primaryKey)
        {
            var objectType = typeof(T);
            var entityName = Entities.GetNameForType(objectType);
            var entity = Entities[entityName];
            var field = entity.PrimaryKey;
            return (T)_dbAccessStrategy.SelectByPrimayKey(objectType, field, primaryKey);
        }

        protected override void OnInsert(object item)
        {
            _dbAccessStrategy.Insert(item);
        }

        protected override void OnUpdate(object item)
        {
            _dbAccessStrategy.Update(item);
        }

        protected override void OnDelete(object item)
        {
            _dbAccessStrategy.Delete(item);
        }

        public override IJoinable<TEntity> Select<TEntity>()
        {
            return new Selectable<TEntity>(this, Entities);
        }

        public override IJoinable<TIEntity> Select<TEntity, TIEntity>()
        {
            return new Selectable<TIEntity>(this, Entities, typeof(TEntity));
        }

        public override IJoinable<object> Select(Type objectType)
        {
            return new Selectable<object>(this, Entities, objectType);
        }

        public override IEnumerable<TIEntity> ExecuteQuery<TIEntity>(IClause sqlClause, IEntityBuilder<TIEntity> select)
        {
            var command = BuildCommand(sqlClause);
            return ExecuteReader(command, select);
        }

        public override IEnumerable<KeyValuePair<object[], long>> ExecuteQuery(IClause sqlAggregateClause)
        {
            var command = BuildCommand(sqlAggregateClause);
            // ReSharper disable once RedundantTypeArgumentsOfMethod // Need for wince.Proj
            return ExecuteCommandReader(command);
        }

        private IEnumerable<KeyValuePair<object[], long>> ExecuteCommandReader(IDbCommand command)
        {
            try
            {
                command.Connection = GetConnection();
                command.Transaction = CurrentTransaction;
                DisplayDebug(command);

                using (var results = command.ExecuteReader())
                {
                    while (results.Read())
                    {
                        var key = new List<object>();
                        var value = results.GetInt32(0);
                        for (var i = 1; i < results.FieldCount; i++)
                        {
                            var dbColumnValue = results.GetValue(i);
                            key.Add(dbColumnValue == DBNull.Value ? null : dbColumnValue);
                        }
                        yield return new KeyValuePair<object[], long>(key.ToArray(), value);
                    }
                }
            }
            finally
            {
                command.Dispose();
            }
        }

        private static void DisplayDebug(IDbCommand command)
        {
#if DEBUG
            OrmDebug.Trace(command.CommandText);
            foreach (IDbDataParameter commandParameter in command.Parameters)
            {
                string value;
                if (commandParameter.Value == DBNull.Value)
                    value = "NULL";
                else if (commandParameter.Value is DateTime)
                    value = ((DateTime) commandParameter.Value).ToString("MM/dd/yyyy HH:mm:ss.fff");
                else
                    value = commandParameter.Value.ToString();

                OrmDebug.Trace(string.Format("\t{0}:{1}", commandParameter.ParameterName, value));
            }
#endif
        }

        public override int ExecuteNonQuery(IClause sqlClause)
        {
            var command = BuildCommand(sqlClause);
            return ExecuteNonQuery(command);
        }

        public override int ExecuteScalar(IClause sqlClause)
        {
            var command = BuildCommand(sqlClause);
            var count = ExecuteScalar(command);
            return Convert.ToInt32(count);
        }

        private IDbCommand BuildCommand(IClause sqlClause)
        {
            var @params = new List<IDataParameter>();
            var sql = sqlClause.ToStatement(@params);

            var command = SqlFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            foreach (var param in @params)
            {
                command.Parameters.Add(param);
            }
            return command;
        }

        public IEnumerable<TIEntity> ExecuteReader<TIEntity>(IDbCommand command, IEntityBuilder<TIEntity> builder)
            where TIEntity : class
        {
            builder.EntityCache = Cache ?? new EntitiesCache();
            // Need for VS2008 compilation
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            return ExecuteCommandReader<TIEntity>(command, builder.Deserialize, builder.Offset);
        }

        private IEnumerable<TIEntity> ExecuteCommandReader<TIEntity>(IDbCommand command, Func<IDataReader, TIEntity> deserialize, int offset)
            where TIEntity : class
        {
            try
            {
                command.Connection = GetConnection();
                command.Transaction = CurrentTransaction;
                DisplayDebug(command);

                using (var results = command.ExecuteReader())
                {
                    var currentOffset = 0;

                    while (results.Read())
                    {
                        if (currentOffset < offset)
                        {
                            currentOffset++;
                            continue;
                        }

                        yield return deserialize(results);
                    }
                }
            }
            finally
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Deletes all rows from the specified Table
        /// </summary>
        public void TruncateTable(string tableName)
        {
            var connection = GetConnection();
            using (var command = SqlFactory.CreateCommand())
            {
                command.Connection = connection;
                command.CommandText = string.Format("DELETE FROM {0}", tableName);
                command.ExecuteNonQuery();
            }
        }

        public bool TableExists(string tableName)
        {
            var tables = _dbAccessStrategy.GetTableNames();
            return tables.Contains(tableName, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Ensures that the underlying database tables contain all of the Fields to represent the known entities.
        /// This is useful if you need to add a Field to an existing store.  Just add the Field to the Entity, then
        /// call EnsureCompatibility to have the field added to the database.
        /// </summary>
        public override void EnsureCompatibility()
        {
            if (!StoreExists)
            {
                CreateStore();
                return;
            }

            var connection = GetConnection();
            foreach (var entity in Entities)
            {
                EnsureCompatibility(connection, entity);
            }
        }

        /// <summary>
        /// Ensures that the underlying database contain table with all Fields to represent specified entity.
        /// This is useful if you need to add a Field to an existing store.  Just add the Field to the Entity, then
        /// call EnsureCompatibility to have the field added to the database.
        /// </summary>
        protected override void EnsureCompatibility(Type entityType)
        {
            if (!StoreExists)
                return;

            var connection = GetConnection();
            var name = Entities.GetNameForType(entityType);

            EnsureCompatibility(connection, Entities[name]);

        }

        private void EnsureCompatibility(IDbConnection connection, IEntityInfo entity)
        {
            var tableName = entity.GetNameInStore();
            if (!TableExists(tableName))
            {
                CreateTable(connection, entity);
            }
            else
            {
                ValidateTable(entity);
            }
        }

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.Unspecified);
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            lock (_transactionSyncRoot)
            {
                if (CurrentTransaction != null)
                {
                    throw new InvalidOperationException("Parallel transactions are not supported");
                }

                if (_connection == null)
                {
                    _connection = GetConnection();
                }

                CurrentTransaction = _connection.BeginTransaction(isolationLevel);
            }
        }

        public void Commit()
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException();
            }

            lock (_transactionSyncRoot)
            {
                CurrentTransaction.Commit();
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }

        public void Rollback()
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException();
            }

            lock (_transactionSyncRoot)
            {
                CurrentTransaction.Rollback();
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        /// <remarks>You <b>MUST</b> call CloseReader after calling this method to prevent a leak</remarks>
        public virtual IDataReader ExecuteReader(string sql)
        {
            try
            {
                var connection = GetConnection();
                using (var command = SqlFactory.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Connection = connection;
                    command.Transaction = CurrentTransaction;

                    var reader = command.ExecuteReader(CommandBehavior.Default);
                    return reader;
                }
            }
            catch (Exception ex)
            {
                OrmDebug.Trace("SQLStoreBase::ExecuteReader threw: " + ex.Message);
                throw;
            }
        }
    }
}
