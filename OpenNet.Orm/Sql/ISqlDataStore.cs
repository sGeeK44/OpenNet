using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Entity.Constraints;
using OpenNet.Orm.Interfaces;
using OpenNet.Orm.Queries;

namespace OpenNet.Orm.Sql
{
    public interface ISqlDataStore : IDataStore
    {
        bool TableExists(string tableName);
        void CreateTable(IDbConnection connection, IEntityInfo entity);
        void VerifiyPrimaryKey(PrimaryKey primaryKey);
        void VerifyForeignKey(ForeignKey foreignKey);
        void VerifyIndex(Index index);
        void TruncateTable(string tableName);
        int ExecuteNonQuery(string sql);
        int ExecuteNonQuery(IDbCommand command);
        object ExecuteScalar(string sql);
        IDataReader ExecuteReader(string sql);
        IEnumerable<TIEntity> ExecuteReader<TIEntity>(IDbCommand command, IEntityBuilder<TIEntity> builder) where TIEntity : class;
        IDbConnection GetConnection();
        IDbTransaction CurrentTransaction { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
        void BeginTransaction(IsolationLevel unspecified);
    }
}
