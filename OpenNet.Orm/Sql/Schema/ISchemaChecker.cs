using OpenNet.Orm.Entity.Constraints;

namespace OpenNet.Orm.Sql.Schema
{
    public interface ISchemaChecker
    {
        /// <summary>
        /// Ensure primary key constraint exist in store
        /// </summary>
        /// <param name="primaryKey">Primary key to check</param>
        void VerifyPrimaryKey(PrimaryKey primaryKey);

        /// <summary>
        /// Ensure foreign key constraint exist in store
        /// </summary>
        /// <param name="foreignKey">Foreign key to check</param>
        void VerifyForeignKey(ForeignKey foreignKey);

        /// <summary>
        /// Ensure index constraint exist in store
        /// </summary>
        /// <param name="index">Index to check</param>
        void VerifyIndex(Index index);
    }
}