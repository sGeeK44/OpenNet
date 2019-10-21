using OpenNet.Orm.Filters;

namespace OpenNet.Orm.Queries
{
    public interface IAggragableQuery<TEntity> : IOrderableQuery<TEntity>
    {
        IOrderableQuery<TEntity> GroupBy(params ColumnValue[] columns);
    }
}