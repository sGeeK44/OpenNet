using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Queries;

namespace OpenNet.Orm.Sql.Queries
{
    public class HomeMadeSqlClause: IClause
    {
        private readonly List<IDataParameter> _params;
        private readonly string _query;

        public HomeMadeSqlClause(string query, List<IDataParameter> @params)
        {
            _query = query;
            _params = @params;
        }

        public string ToStatement(List<IDataParameter> @params)
        {
            @params.AddRange(_params);
            return _query;
        }
    }
}
