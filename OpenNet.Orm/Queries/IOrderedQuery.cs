using OpenNet.Orm.Filters;

namespace OpenNet.Orm.Queries
{
    public interface IOrderedQuery<TEntity> : ISelectQuery<TEntity>
    {
        IOrderedQuery<TEntity> ThenBy(ColumnValue field);
        IOrderedQuery<TEntity> ThenByDesc(ColumnValue field);
    }
}