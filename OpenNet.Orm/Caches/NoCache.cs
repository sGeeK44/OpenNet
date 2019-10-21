
using System;

namespace OpenNet.Orm.Caches
{
    public class NoCache : IEntityCache
    {
        /// <summary>
        /// Get cached entityType of for specified cacheKey
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="cacheKey">A unique key that represent entity</param>
        /// <returns>entity cached if exist, else null</returns>
        public object GetOrDefault(Type entityType, object cacheKey)
        {
            return null;
        }

        /// <summary>
        /// Update specified item from RepositoryCache
        /// </summary>
        /// <param name="entity">Entity to Add in cache</param>
        /// <param name="cacheKey">A unique key that represent specified entity</param>
        public void Cache(object entity, object cacheKey) { }

        /// <summary>
        /// Delete specified item from Cache
        /// </summary>
        /// <param name="itemToRemove">Item to remove from cache</param>
        public void Invalidate(object itemToRemove) { }

        /// <summary>
        /// Clear cache
        /// </summary>
        public void Clear() { }
    }
}
