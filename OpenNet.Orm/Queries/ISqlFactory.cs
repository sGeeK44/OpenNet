using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sql.Schema;

namespace OpenNet.Orm.Queries
{
    public interface ISqlFactory
    {
        string ParameterPrefix { get; }
        IDataParameter CreateParameter();
        IDataParameter CreateParameter(string paramName, object value);
        IDataParameter CreateParameter(string paramName, ICustomSqlField customField);
        IFieldPropertyFactory CreateFieldPropertyFactory();
        string AddParam(object paramToAdd, ICollection<IDataParameter> @params);
        IDbCommand CreateCommand();
        IDbAccessStrategy CreateDbAccessStrategy(ISqlDataStore sqlDataStore);
        ISchemaChecker CreateSchemaChecker(ISqlDataStore sqlDataStore);
    }
}