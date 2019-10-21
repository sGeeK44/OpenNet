using System.Collections.Generic;
using System.Data;

namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to OR condition
    /// </summary>
    public class Or : Condition
    {
        private const string OrValue = " OR ";

        public Or(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, OrValue) { }

        /// <summary>
        /// Convert part to sql string equivalent
        /// </summary>
        /// <param name="params">existing param list to populate in case of part object value</param>
        /// <returns>Sql string representation</returns>
        public override string ToStatement(List<IDataParameter> @params)
        {
            return string.Concat("(", base.ToStatement(@params), ")");
        }
    }
}