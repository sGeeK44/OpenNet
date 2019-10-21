using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Text;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Sql;

// ReSharper disable UseNullPropagation

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.SqlCe
{
    public class SqlCeAccessStrategy : IDbAccessStrategy
    {
        private readonly Dictionary<string, Dictionary<string, int>> _entityOrdinal;
        private readonly ISqlDataStore _datastore;

        public SqlCeAccessStrategy(ISqlDataStore datastore)
        {
            _entityOrdinal = new Dictionary<string, Dictionary<string, int>>();
            _datastore = datastore;
        }

        public object SelectByPrimayKey(Type objectType, PrimaryKey primaryKey, object value)
        {
            var entityName = _datastore.Entities.GetNameForType(objectType);

            var command = new SqlCeCommand
            {
                CommandText = entityName,
                CommandType = CommandType.TableDirect,
                IndexName = primaryKey.ConstraintName,
                Connection = _datastore.GetConnection() as SqlCeConnection,
                Transaction = _datastore.CurrentTransaction as SqlCeTransaction
            };

            try
            {
                using (var results = command.ExecuteReader())
                {

                    if (!results.Seek(DbSeekOptions.FirstEqual, value))
                        return null;

                    results.Read();
                    var entity = _datastore.Entities[entityName];
                    var serializer = entity.GetSerializer();
                    serializer.UseFullName = false;
                    serializer.EntityCache = _datastore.Cache;
                    return serializer.Deserialize(results);
                }
            }
            finally
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Inserts the provided entity instance into the underlying data store.
        /// </summary>
        /// <remarks>
        /// If the entity has an identity field, calling Insert will populate that field with the identity vale vefore returning
        /// </remarks>
        public void Insert(object item)
        {
            var itemType = item.GetType();
            var entityName = _datastore.Entities.GetNameForType(itemType);

            if (entityName == null)
            {
                throw new EntityNotFoundException(item.GetType());
            }

            // we'll use table direct for inserts - no point in getting the query parser involved in this
            var connection = _datastore.GetConnection();
            using (var command = new SqlCeCommand())
            {
                command.Connection = connection as SqlCeConnection;
                command.Transaction = _datastore.CurrentTransaction as SqlCeTransaction;
                command.CommandText = entityName;
                command.CommandType = CommandType.TableDirect;

                using (var results = command.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                    var record = results.CreateRecord();

                    OrmDebug.Info("".PadRight(entityName.Length + 11, '='));
                    OrmDebug.Info(string.Format("| Insert {0} |", entityName));
                    FillEntity(record.SetValue, entityName, item);

                    results.Insert(record);

                    // did we have an identity field?  If so, we need to update that value in the item
                    var primaryKey = _datastore.Entities[entityName].PrimaryKey;
                    if (primaryKey.KeyScheme == KeyScheme.Identity)
                    {
                        var id = GetIdentity(connection);
                        primaryKey.SetEntityValue(item, id);
                    }

                    if (_datastore.Cache != null)
                        _datastore.Cache.Cache(item, primaryKey.GetEntityValue(item));
                }

                command.Dispose();
            }
        }

        public void Update(object item)
        {
            var itemType = item.GetType();

            var entityName = _datastore.Entities.GetNameForType(itemType);

            if (entityName == null)
            {
                throw new EntityNotFoundException(itemType);
            }

            if (_datastore.Entities[entityName].PrimaryKey == null)
            {
                throw new PrimaryKeyRequiredException("A primary key is required on an Entity in order to perform Updates");
            }

            var connection = _datastore.GetConnection();
            using (var command = new SqlCeCommand())
            {
                command.Connection = connection as SqlCeConnection;
                command.CommandText = entityName;
                command.CommandType = CommandType.TableDirect;
                command.IndexName = _datastore.Entities[entityName].PrimaryKey.ConstraintName;
                command.Transaction = _datastore.CurrentTransaction as SqlCeTransaction;

                using (var results = command.ExecuteResultSet(ResultSetOptions.Scrollable | ResultSetOptions.Updatable))
                {
                    var keyValue = _datastore.Entities[entityName].PrimaryKey.GetEntityValue(item);

                    // seek on the PK
                    var found = results.Seek(DbSeekOptions.BeforeEqual, keyValue);

                    if (!found)
                    {
                        // TODO: the PK value has changed - we need to store the original value in the entity or diallow this kind of change
                        throw new RecordNotFoundException("Cannot locate a record with the provided primary key.  You cannot update a primary key value through the Update method");
                    }

                    results.Read();

                    OrmDebug.Info("".PadRight(entityName.Length + 11, '='));
                    OrmDebug.Info(string.Format("| Update {0} |", entityName));
                    FillEntity(results.SetValue, entityName, item);

                    results.Update();
                }
            }
        }

        public void Delete(object item)
        {
            var type = item.GetType();
            var entityName = _datastore.Entities.GetNameForType(type);
            if (entityName == null)
            {
                throw new EntityNotFoundException(type);
            }

            if (_datastore.Entities[entityName].PrimaryKey == null)
            {
                throw new PrimaryKeyRequiredException("A primary key is required on an Entity in order to perform a Delete");
            }

            var connection = _datastore.GetConnection();
            using (var command = connection.CreateCommand() as SqlCeCommand)
            {
                if (command == null)
                    throw new NotSupportedException("Can only work with sql ce.");

                command.CommandText = entityName;
                command.CommandType = CommandType.TableDirect;
                command.IndexName = _datastore.Entities[entityName].PrimaryKey.ConstraintName;
                command.Transaction = _datastore.CurrentTransaction as SqlCeTransaction;

                using (var results = command.ExecuteResultSet(ResultSetOptions.Scrollable | ResultSetOptions.Updatable))
                {

                    // seek on the PK
                    var primaryKeyValue = _datastore.Entities[entityName].PrimaryKey.GetEntityValue(item);
                    var found = results.Seek(DbSeekOptions.BeforeEqual, primaryKeyValue);

                    if (!found)
                    {
                        throw new RecordNotFoundException("Cannot locate a record with the provided primary key.  Unable to delete the item");
                    }

                    results.Read();
                    results.Delete();
                    if (_datastore.Cache != null)
                        _datastore.Cache.Invalidate(item);
                }
            }
        }

        private void FillEntity(Action<int, object> setter, string entityName, object item)
        {
#if DEBUG
            var header = string.Empty;
            var row = string.Empty;
#endif

            var fieldsOrdinal = GetOrdinalsField(entityName);

            foreach (var field in _datastore.Entities[entityName].Fields)
            {
                if (!field.Settable)
                    continue;

                var fieldOrdinal = fieldsOrdinal[field.FieldName];

                var sqlValue = field.ToSqlValue(item);
                setter(fieldOrdinal, sqlValue);
#if DEBUG
                string rowValue;
                if (sqlValue == DBNull.Value)
                    rowValue = string.Format("{0}", "NULL");
                else if (sqlValue is DateTime)
                    rowValue = string.Format("{0:MM/dd/yyyy hh:mm:ss.fff}", (DateTime)sqlValue);
                else
                    rowValue = string.Format("{0}", sqlValue);

                var maxLength = rowValue.Length >= field.FieldName.Length
                              ? rowValue.Length
                              : field.FieldName.Length;

                row += string.Format("| {0} ", rowValue.PadRight(maxLength));
                header += string.Format("| {0} ", field.FieldName.PadRight(maxLength));
#endif
            }
#if DEBUG
            var totalLength = row.Length + 2;
            OrmDebug.Info("".PadRight(totalLength, '='));
            OrmDebug.Info(header + " |");
            OrmDebug.Info(row + " |");
            OrmDebug.Info("".PadRight(totalLength, '='));
#endif
        }

        private int GetIdentity(IDbConnection connection)
        {
            using (var command = new SqlCeCommand("SELECT @@IDENTITY", connection as SqlCeConnection))
            {
                command.Transaction = _datastore.CurrentTransaction as SqlCeTransaction;
                var id = command.ExecuteScalar();
                return Convert.ToInt32(id);
            }
        }

        public string[] GetTableNames()
        {
            var names = new List<string>();

            var connection = _datastore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = _datastore.CurrentTransaction as SqlCeTransaction;
                command.Connection = connection;
                const string sql = "SELECT table_name FROM information_schema.tables";
                command.CommandText = sql;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        names.Add(reader.GetString(0));
                    }
                }

                return names.ToArray();
            }
        }

        public void ValidateTable(IEntityInfo entity)
        {
            var entityName = entity.GetNameInStore();
            var connection = _datastore.GetConnection();

            // first make sure the table exists
            if (!_datastore.TableExists(entityName))
            {
                _datastore.CreateTable(connection, entity);
                return;
            }

            using (var command = new SqlCeCommand())
            {
                command.Transaction = _datastore.CurrentTransaction as SqlCeTransaction;
                command.Connection = connection as SqlCeConnection;

                foreach (var field in entity.Fields)
                {
                    // yes, I realize hard-coded ordinals are not a good practice, but the SQL isn't changing, it's method specific
                    var sql = string.Format("SELECT column_name, "  // 0
                          + "data_type, "                       // 1
                          + "character_maximum_length, "        // 2
                          + "numeric_precision, "               // 3
                          + "numeric_scale, "                   // 4
                          + "is_nullable "
                          + "FROM information_schema.columns "
                          + "WHERE (table_name = '{0}' AND column_name = '{1}')",
                          entityName, field.FieldName);

                    command.CommandText = sql;

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            // field doesn't exist - we must create it
                            var alter = new StringBuilder(string.Format("ALTER TABLE [{0}] ", entity.GetNameInStore()));
                            alter.Append(string.Format("ADD {0}", field.GetFieldDefinitionSqlQuery()));

                            using (var altercmd = new SqlCeCommand(alter.ToString(), connection as SqlCeConnection))
                            {
                                altercmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                _datastore.VerifiyPrimaryKey(entity.PrimaryKey);

                foreach (var foreignKey in entity.ForeignKeys)
                {
                    _datastore.VerifyForeignKey(foreignKey);
                }

                foreach (var index in entity.Indexes)
                {
                    _datastore.VerifyIndex(index);
                }
            }
        }

        private Dictionary<string, int> GetOrdinalsField(string entityName)
        {
            if (_entityOrdinal.ContainsKey(entityName))
                return _entityOrdinal[entityName];

            var ordinal = new Dictionary<string, int>();
            var connection = _datastore.GetConnection();
            using (var command = new SqlCeCommand())
            {
                command.Transaction = _datastore.CurrentTransaction as SqlCeTransaction;
                command.Connection = connection as SqlCeConnection;
                command.CommandText = entityName;
                command.CommandType = CommandType.TableDirect;

                using (var reader = command.ExecuteReader())
                {
                    foreach (var field in _datastore.Entities[entityName].Fields)
                    {
                        ordinal.Add(field.FieldName, reader.GetOrdinal(field.FieldName));
                    }
                }
                _entityOrdinal.Add(entityName, ordinal);
                command.Dispose();
                return ordinal;
            }
        }
    }
}
