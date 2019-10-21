using System.Data;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sql.Schema;

namespace OpenNet.Orm.SqlCe
{
    public class SqlCeSchemaChecker : SchemaChecker
    {
        public SqlCeSchemaChecker(ISqlDataStore sqlDataStore)
            : base(sqlDataStore) { }

        protected override bool IsPrimaryKeyExist(PrimaryKey primaryKey)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                var sql = string.Format(
                    "SELECT COUNT(*) FROM information_schema.table_constraints WHERE TABLE_NAME = \'{0}\' AND CONSTRAINT_TYPE = 'PRIMARY KEY'",
                    primaryKey.Entity.GetNameInStore()
                );
                command.CommandText = sql;

                return (int) command.ExecuteScalar() != 0;
            }
        }

        protected override bool IsForeignKeyExist(ForeignKey foreignKey)
        {
            return IsForeignKeyExist(foreignKey.ConstraintName);
        }

        public bool IsForeignKeyExist(string constraintName)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                var sql = string.Format(
                    "SELECT COUNT(*) FROM information_schema.table_constraints WHERE CONSTRAINT_NAME = \'{0}\'",
                    constraintName
                );
                command.CommandText = sql;

                return (int)command.ExecuteScalar() != 0;
            }
        }

        protected override bool IsIndexExist(Index index)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                var sql = string.Format(
                    "SELECT COUNT(*) FROM information_schema.indexes WHERE INDEX_NAME = '{0}'",
                    index.GetNameInStore()
                );
                command.CommandText = sql;

                return (int) command.ExecuteScalar() != 0;
            }
        }

        protected override void CreatePrimaryKey(PrimaryKey primaryKey)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {

                var sql = primaryKey.GetCreateSqlQuery();
                OrmDebug.Trace(sql);

                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        protected override void CreateForeignKey(ForeignKey foreignKey)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {

                var sql = foreignKey.GetCreateSqlQuery();
                OrmDebug.Trace(sql);

                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        public TableDefinition GetTableFormat(string entityName)
        {
            var sql = string.Format("SELECT COLUMN_NAME, ORDINAL_POSITION, IS_NULLABLE, DATA_TYPE FROM information_schema.columns WHERE TABLE_NAME = '{0}'", entityName);

            using (var reader = SqlDataStore.ExecuteReader(sql))
            {
                TableDefinition tableFormat = null;
                while (reader.Read())
                {
                    if (tableFormat == null)
                        tableFormat = new TableDefinition(entityName);

                    var columnFormat = GetColumnFormat(reader);
                    tableFormat.AddColumn(columnFormat);
                }
                return tableFormat;
            }
        }

        private static ColumnDefinition GetColumnFormat(IDataReader reader)
        {
            return new ColumnDefinition
            {
                ColumnName = reader.GetString(0),
                Ordinal = reader.GetInt32(1),
                IsNullable = reader.GetString(2) == "YES",
                DbType = reader.GetString(3)
            };
        }
    }
}