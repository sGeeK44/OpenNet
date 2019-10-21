using System;
using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Entity.Fields;
using OpenNet.Orm.Queries;
using OpenNet.Orm.Sql;
using OpenNet.Orm.Sql.Schema;
using OpenNet.Orm.Sqlite.Fields;
#if XAMARIN
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#else
using SQLiteCommand = System.Data.SQLite.SQLiteCommand;
using SQLiteParameter = System.Data.SQLite.SQLiteParameter;
#endif

namespace OpenNet.Orm.Sqlite
{
    public class SqliteFactory : ISqlFactory
    {
        public string ParameterPrefix
        {
            get { return "@"; }
        }

        public IDataParameter CreateParameter()
        {
            return new SQLiteParameter();
        }

        public IDataParameter CreateParameter(string paramName, object value)
        {
            return new SQLiteParameter
            {
                ParameterName = paramName,
                Value = value ?? DBNull.Value
            };
        }

        public IDataParameter CreateParameter(string paramName, ICustomSqlField customField)
        {
            return new SQLiteParameter
            {
                ParameterName = paramName,
                Value = customField.ToSqlValue(),
                //TODO
                //Precision = customField.Precision,
                //Scale = customField.Scale
            };
        }

        public IFieldPropertyFactory CreateFieldPropertyFactory()
        {
            return new FieldPropertyFactory();
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
            return new SQLiteCommand();
        }

        public IDbAccessStrategy CreateDbAccessStrategy(ISqlDataStore sqlDataStore)
        {
            return new StandardDbAccessStrategy(sqlDataStore);
        }

        public ISchemaChecker CreateSchemaChecker(ISqlDataStore sqlDataStore)
        {
            return new SqliteSchemaChecker(sqlDataStore);
        }

        public static ISqlDataStore CreateStore(string datasource)
        {
            return CreateStore(datasource, null);
        }

        public static ISqlDataStore CreateStore(string datasource, string password)
        {
            return new SqlDataStore(new SqliteDbEngine(datasource, password), new SqliteFactory());
        }
    }
}