using System.Collections.Generic;
using System.Data;

namespace OpenNet.Orm.Filters
{
    public class CountValue : IFilter
    {
        private readonly ColumnValue _columnToCount;

        public CountValue(ColumnValue columnToCount)
        {
            _columnToCount = columnToCount;
        }

        /// <summary>
        /// Convert part to sql string equivalent
        /// </summary>
        /// <param name="params">existing param list to populate in case of part object value</param>
        /// <returns>Sql string representation</returns>
        public string ToStatement(List<IDataParameter> @params)
        {
            return string.Format("COUNT({0})", _columnToCount.ToStatement(@params));
        }
    }
}