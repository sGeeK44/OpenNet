using OpenNet.Orm.Filters;

namespace OpenNet.Orm.Queries
{
    public interface IOrderableQuery<TEntity> : IOrderedQuery<TEntity>
    {
        IOrderedQuery<TEntity> OrderBy(ColumnValue field);
        IOrderedQuery<TEntity> OrderByDesc(ColumnValue field);
    }
}