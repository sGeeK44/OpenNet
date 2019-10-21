using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sql.Schema;
using OpenNet.Orm.SqlCe.Fields;

// ReSharper disable IntroduceOptionalParameters.Global

// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.SqlCe
{
    public class SqlCeFactory : ISqlFactory
    {
        public string ParameterPrefix
        {
            get { return "@"; }
        }

        public IDataParameter CreateParameter()
        {
            return new SqlCeParameter();
        }

        public IDataParameter CreateParameter(string paramName, object value)
        {
            return new SqlCeParameter
            {
                ParameterName = paramName,
                Value = value ?? DBNull.Value
            };
        }

        public IDataParameter CreateParameter(string paramName, ICustomSqlField customField)
        {
            return new SqlCeParameter
            {
                ParameterName = paramName,
                Value = customField.ToSqlValue(),
                Precision = customField.Precision,
                Scale = customField.Scale
            };
        }

        public string AddParam(object paramToAdd, ICollection<IDataParameter> @params)
        {
            var paramName = string.Format("{0}p{1}", ParameterPrefix, @params.Count);

            var customField = paramToAdd as ICustomSqlField;
            var param = customField != null
                ? CreateParameter(paramName, customField)
                : CreateParameter(paramName, paramToAdd);
            @params.Add(param);
            return paramName;
        }

        public IDbCommand CreateCommand()
        {
            return new SqlCeCommand();
        }

        public IFieldPropertyFactory CreateFieldPropertyFactory()
        {
            return new FieldPropertyFactory();
        }

        public IDbAccessStrategy CreateDbAccessStrategy(ISqlDataStore sqlDataStore)
        {
            return new SqlCeAccessStrategy(sqlDataStore);
        }

        public ISchemaChecker CreateSchemaChecker(ISqlDataStore sqlDataStore)
        {
            return new SqlCeSchemaChecker(sqlDataStore);
        }

        public static ISqlDataStore CreateStore(string datasource)
        {
            return CreateStore(datasource, null);
        }

        public static ISqlDataStore CreateStore(string datasource, string password)
        {
            return new SqlDataStore(new SqlCeDbEngine(datasource, password), new SqlCeFactory());
        }
    }
}
