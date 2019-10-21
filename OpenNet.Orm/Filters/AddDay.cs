using System.Collections.Generic;
using System.Data;

namespace OpenNet.Orm.Filters
{
    public class AddDay : IFilter
    {
        private readonly ObjectValue _value;
        private readonly ColumnValue _buildColumnValue;

        public AddDay(ObjectValue value, ColumnValue buildColumnValue)
        {
            _value = value;
            _buildColumnValue = buildColumnValue;
        }

        public string ToStatement(List<IDataParameter> @params)
        {
            return string.Format("DATEADD(DAY, {0}, {1})",
                _buildColumnValue.ToStatement(@params),
                _value.ToStatement(@params));
        }
    }
}