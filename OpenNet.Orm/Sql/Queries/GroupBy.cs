using System.Text;
using OpenNet.Orm.Filters;

namespace OpenNet.Orm.Sql.Queries
{
    public class GroupBy
    {
        private ColumnValue[] _columns;

        /// <summary>
        /// Calculate sql statement equivalent to current Group By
        /// </summary>
        /// <returns>Sql string statement</returns>
        public string ToStatement()
        {
            if (_columns == null || _columns.Length == 0)
                return string.Empty;

            StringBuilder statement = null;
            foreach (var column in _columns)
            {
                if (statement == null)
                    statement = new StringBuilder(" GROUP BY ");
                else
                    statement.Append(", ");

                statement.Append(column.ToStatement(null));
            }

            // ReSharper disable once PossibleNullReferenceException
            return statement.ToString();
        }

        /// <summary>
        /// Add specified columns to group by clause
        /// </summary>
        /// <param name="columns">Columns to add</param>
        public void AddColumn(params ColumnValue[] columns)
        {
            _columns = columns;
        }
    }
}