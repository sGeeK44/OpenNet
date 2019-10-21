using System.Collections.Generic;
using OpenNet.Orm.Filters;

namespace OpenNet.Orm.Queries
{
    public interface ISelectQuery<TEntity>
    {
        IEnumerable<TEntity> GetValues();
        IEnumerable<TEntity> Top(int quantity);
        int Count();
        int Delete();
        IEnumerable<KeyValuePair<object[], long>> Count(params ColumnValue[] columns);
    }
}