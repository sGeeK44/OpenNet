using System.Collections.Generic;
using System.Linq;

namespace OpenNet.Orm.Caches
{
    public class EntityCache
    {
        private readonly Dictionary<object, object> _cache = new Dictionary<object, object>();

        public object GetOrDefault(object cacheKey)
        {
            return _cache.ContainsKey(cacheKey)
                 ? _cache[cacheKey]
                 : null;
        }

        public void Add(object entity, object cacheKey)
        {
            if (_cache.ContainsKey(cacheKey))
                _cache[cacheKey] = entity;
            else
                _cache.Add(cacheKey, entity);
        }

        public void Remove(object itemToRemove)
        {
            if (!_cache.ContainsValue(itemToRemove))
                return;

            var itemCached = _cache.First(_ => _.Value == itemToRemove);
            _cache.Remove(itemCached.Key);
        }
    }
}