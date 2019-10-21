using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage column value part filter
    /// </summary>
    public class ColumnValue : IFilter
    {
        private readonly string _columnName;
        private readonly IEntityInfo _entity;

        public ColumnValue(IEntityInfo entity, string columnName)
        {
            _entity = entity;
            _columnName = columnName;
        }

        /// <summary>
        /// Convert part to sql string equivalent
        /// </summary>
        /// <param name="params">existing param list to populate in case of part object value</param>
        /// <returns>Sql string representation</returns>
        public string ToStatement(List<IDataParameter> @params)
        {
            return string.Concat("[", _entity.GetNameInStore(), "]", ".", "[", _columnName, "]");
        }

        /// <summary>
        /// Convert column value to Count aggragated column (ex: [Table].[Col] ==> COUNT([Table].[Col])
        /// </summary>
        /// <returns>Converted column Value</returns>
        public IFilter ToCountedColumn()
        {
            return new CountValue(this);
        }
    }
}
