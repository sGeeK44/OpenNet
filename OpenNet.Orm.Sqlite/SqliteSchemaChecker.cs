using System.Data;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sql.Schema;

namespace OpenNet.Orm.Sqlite
{
    public class SqliteSchemaChecker : SchemaChecker
    {
        public SqliteSchemaChecker(ISqlDataStore sqlDataStore)
            : base(sqlDataStore) { }


        public TableDefinition GetTableFormat(string entityName)
        {
            var sql = string.Format("PRAGMA table_info([{0}])", entityName);

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
                Ordinal = reader.GetInt32(0),
                ColumnName = reader.GetString(1),
                DbType = reader.GetString(2),
                IsNullable = !reader.GetBoolean(3),
            };
        }

        protected override bool IsPrimaryKeyExist(PrimaryKey primaryKey)
        {
            return IsPrimaryKeyExist(primaryKey.Entity.GetNameInStore());
        }

        public bool IsPrimaryKeyExist(string entityName)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                var sql = string.Format("PRAGMA table_info([{0}])", entityName);
                command.CommandText = sql;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetBoolean(5))
                            return true;
                    }
                }
                return false;
            }
        }

        protected override bool IsForeignKeyExist(ForeignKey foreignKey)
        {
            return IsForeignKeyExist(foreignKey.Entity.GetNameInStore(), foreignKey.ForeignEntityInfo.GetNameInStore());
        }

        public bool IsForeignKeyExist(string entityName, string entityRef)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                var sql = string.Format("PRAGMA foreign_key_list([{0}])", entityName);
                command.CommandText = sql;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(2) != entityRef)
                            continue;

                        return true;
                    }
                }
                return false;
            }
        }

        protected override bool IsIndexExist(Index index)
        {
            return IsIndexExist(index.GetNameInStore()) != 0;
        }

        public int IsIndexExist(string indexName)
        {
            var connection = SqlDataStore.GetConnection();
            using (var command = connection.CreateCommand())
            {
                var sql = string.Format("PRAGMA index_info({0})", indexName);
                command.CommandText = sql;

                using (var reader = command.ExecuteReader())
                {
                    var count = 0;
                    while (reader.Read())
                    {
                        count++;
                    }
                    return count;
                }
            }
        }

        protected override void CreatePrimaryKey(PrimaryKey primaryKey)
        {
            // Not supported by sqlite
            // https://www.sqlite.org/omitted.html
        }

        protected override void CreateForeignKey(ForeignKey foreignKey)
        {
            // Not supported by sqlite
            // https://www.sqlite.org/omitted.html
        }
    }
}