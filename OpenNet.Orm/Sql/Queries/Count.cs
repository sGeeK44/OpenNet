using System.Text;
using OpenNet.Orm.Filters;
using OpenNet.Orm.Queries;

namespace OpenNet.Orm.Sql.Queries
{
    public class Count : ISelectable
    {
        private readonly IFilter _count;
        private readonly ColumnValue[] _columns;

        private Count(IFilter count, params ColumnValue[] columns)
        {
            _count = count;
            _columns = columns;
        }

        public string SelectStatement()
        {
            var statement = new StringBuilder("SELECT ");

            statement.Append(_count.ToStatement(null));

            if (_columns == null || _columns.Length == 0)
                return statement.ToString();
            
            foreach (var column in _columns)
            {
                statement.Append(", ");
                statement.Append(column.ToStatement(null));
            }

            return statement.ToString();
        }

        public static Count CreateTableCount(params ColumnValue[] columns)
        {
            return new Count(new WildCardCound(), columns);
        }

        public static Count CreateColumnCount(ColumnValue column)
        {
            return new Count(column.ToCountedColumn());
        }
    }
}
