using System.Collections.Generic;
using System.Data;

namespace OpenNet.Orm.Filters
{
    public class WildCardCound : IFilter
    {
        public string ToStatement(List<IDataParameter> @params)
        {
            return "COUNT(*)";
        }
    }
}