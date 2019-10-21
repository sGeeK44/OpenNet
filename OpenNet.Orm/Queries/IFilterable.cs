using OpenNet.Orm.Filters;

namespace OpenNet.Orm.Queries
{
    public interface IFilterable<TEntity> : IAggragableQuery<TEntity>
    {
        IAggragableQuery<TEntity> Where(IFilter filter);
    }
}