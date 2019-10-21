using OpenNet.Orm.Entity.Constraints;

namespace OpenNet.Orm.Sql.Schema
{
    public abstract class SchemaChecker : ISchemaChecker
    {
        protected ISqlDataStore SqlDataStore { get; private set; }

        protected SchemaChecker(ISqlDataStore sqlDataStore)
        {
            SqlDataStore = sqlDataStore;
        }

        protected abstract bool IsPrimaryKeyExist(PrimaryKey primaryKey);
        protected abstract bool IsForeignKeyExist(ForeignKey foreignKey);
        protected abstract bool IsIndexExist(Index index);
        protected abstract void CreatePrimaryKey(PrimaryKey primaryKey);
        protected abstract void CreateForeignKey(ForeignKey foreignKey);

        /// <summary>
        /// Ensure primary key constraint exist in store
        /// </summary>
        /// <param name="primaryKey">Primary key to check</param>
        public void VerifyPrimaryKey(PrimaryKey primaryKey)
        {
            if (primaryKey == null)
                return;

            var isExist = IsPrimaryKeyExist(primaryKey);

            if (isExist)
                return;

            CreatePrimaryKey(primaryKey);
        }

        /// <summary>
        /// Ensure foreign key constraint exist in store
        /// </summary>
        /// <param name="foreignKey">Foreign key to check</param>
        public void VerifyForeignKey(ForeignKey foreignKey)
        {
            var isExist = IsForeignKeyExist(foreignKey);

            if (isExist)
                return;

            CreateForeignKey(foreignKey);
        }

        /// <summary>
        /// Ensure index constraint exist in store
        /// </summary>
        /// <param name="index">Index to check</param>
        public void VerifyIndex(Index index)
        {
            var isExist = IsIndexExist(index);

            if (isExist)
                return;

            CreateIndex(index);
        }

        private void CreateIndex(Index index)
        {
            var connection = SqlDataStore.GetConnection();

            using (var command = connection.CreateCommand())
            {

                var sql = index.GetCreateSqlQuery();
                OrmDebug.Trace(sql);

                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }
    }
}
