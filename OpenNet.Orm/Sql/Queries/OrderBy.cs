using System.Collections.Generic;
using System.Text;
using OpenNet.Orm.Filters;

// ReSharper disable UseStringInterpolation

namespace OpenNet.Orm.Sql.Queries
{
    public class OrderBy
    {
        private readonly List<OrderedField> _fields = new List<OrderedField>();

        private class OrderedField
        {
            private readonly ColumnValue _field;

            public OrderedField(ColumnValue field)
            {
                _field = field;
            }

            public virtual string ToStatement()
            {
                return _field.ToStatement(null);
            }
        }

        private class OrderedFieldDesc : OrderedField
        {
            private const string DescClause = "DESC";

            public OrderedFieldDesc(ColumnValue field) : base(field) { }
            
            public override string ToStatement()
            {
                return string.Format("{0} {1}", base.ToStatement(), DescClause);
            }
        }
        
        /// <summary>
        /// Set fields involve in order by (default asc)
        /// </summary>
        /// <param name="field">Ordered field for order by statement</param>
        public void AddField(ColumnValue field)
        {
            _fields.Add(new OrderedField(field));
        }
        
        /// <summary>
        /// Set fields involve in order by (default asc)
        /// </summary>
        /// <param name="field">Ordered field for order by statement</param>
        public void AddFieldDesc(ColumnValue field)
        {
            _fields.Add(new OrderedFieldDesc(field));
        }

        /// <summary>
        /// Calculate sql statement equivalent to current Order By
        /// </summary>
        /// <returns>Sql string statement</returns>
        public string ToStatement()
        {
            if (_fields == null || _fields.Count == 0)
                return string.Empty;

            StringBuilder result = null;
            foreach (var orderedField in _fields)
            {
                if (result == null)
                    result = new StringBuilder(" ORDER BY ");
                else
                    result.Append(", ");

                result.Append(orderedField.ToStatement());
            }

            // ReSharper disable once PossibleNullReferenceException
            return result.ToString();
        }
    }
}