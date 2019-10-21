using System;
using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Queries;

// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sql.Queries
{
    public class Where : IClause
    {
        private readonly IFilter _filter;

        public Where() { }
        
        public Where(IFilter filter)
        {
            _filter = filter;
        }

        public string ToStatement()
        {
            throw new NotSupportedException();
        }

        public string ToStatement(List<IDataParameter> @params)
        {
            return _filter != null
                 ? " WHERE " + _filter.ToStatement(@params)
                 : string.Empty;
        }
    }
}