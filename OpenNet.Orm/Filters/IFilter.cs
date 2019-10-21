using System.Collections.Generic;
using System.Data;

namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Expose method needed by a sql part filter
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Convert part to sql string equivalent
        /// </summary>
        /// <param name="params">existing param list to populate in case of part object value</param>
        /// <returns>Sql string representation</returns>
        string ToStatement(List<IDataParameter> @params);
    }
}

