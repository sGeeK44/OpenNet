using System;
using System.Collections.Generic;

namespace OpenNet.Orm.Caches
{
    public class EntitiesCache : IEntityCache
    {
        private readonly Dictionary<Type, EntityCache> _cache = new Dictionary<Type, EntityCache>();

        /// <summary>
        /// Get cached entityType of for specified cacheKey
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="cacheKey">A unique key that represent entity</param>
        /// <returns>entity cached if exist, else null</returns>
        public object GetOrDefault(Type entityType, object cacheKey)
        {
            return !_cache.ContainsKey(entityType) ? null : _cache[entityType].GetOrDefault(cacheKey);
        }

        /// <summary>
        /// Update specified item from RepositoryCache
        /// </summary>
        /// <param name="entity">Entity to Add in cache</param>
        /// <param name="cacheKey">A unique key that represent specified entity</param>
        public void Cache(object entity, object cacheKey)
        {
            if (entity == null)
                return;

            var entityType = entity.GetType();
            EntityCache entityCache;
            if (!_cache.ContainsKey(entityType))
            {
                entityCache = new EntityCache();
                _cache.Add(entityType, entityCache);
            }
            else
            {
                entityCache = _cache[entityType];
            }

            entityCache.Add(entity, cacheKey);
        }

        /// <summary>
        /// Delete specified item from Cache
        /// </summary>
        /// <param name="itemToRemove">Item to remove from cache</param>
        public void Invalidate(object itemToRemove)
        {
            if (itemToRemove == null)
                return;

            var entityType = itemToRemove.GetType();
            if (!_cache.ContainsKey(entityType))
                return;

            _cache[entityType].Remove(itemToRemove);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }
    }
}