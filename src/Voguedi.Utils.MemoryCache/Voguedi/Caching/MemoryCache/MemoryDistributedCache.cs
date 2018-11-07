using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Voguedi.ObjectSerialization;

namespace Voguedi.Caching.MemoryCache
{
    class MemoryDistributedCache<TCacheValue> : DistributedCache<TCacheValue>
        where TCacheValue : class
    {
        #region Private Fields

        readonly IDistributedCache cache;
        readonly IBinaryObjectSerializer objectSerializer;
        readonly MemoryCacheOptions options;

        #endregion

        #region Ctors

        public MemoryDistributedCache(IDistributedCache cache, IBinaryObjectSerializer objectSerializer, MemoryCacheOptions options)
        {
            this.cache = cache;
            this.objectSerializer = objectSerializer;
            this.options = options;
        }

        #endregion

        #region DistributedCache<TCacheValue>

        public override TCacheValue Get(string key)
        {
            var content = cache.Get(key);
            return content != null ? objectSerializer.Deserialize(content, typeof(TCacheValue)) as TCacheValue : null;
        }

        public override async Task<TCacheValue> GetAsync(string key)
        {
            var content = await cache.GetAsync(key);
            return content != null ? objectSerializer.Deserialize(content, typeof(TCacheValue)) as TCacheValue : null;
        }

        public override void Set(string key, TCacheValue value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null)
        {
            cache.Set(
                key,
                objectSerializer.Serialize(typeof(TCacheValue), value),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = slidingExpiration ?? options.DefaultSlidingExpiration,
                    AbsoluteExpiration = options.DefaultAbsoluteExpiration
                });
        }

        public override async Task SetAsync(string key, TCacheValue value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null)
        {
            await cache.SetAsync(
                key,
                objectSerializer.Serialize(typeof(TCacheValue), value),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = slidingExpiration ?? options.DefaultSlidingExpiration,
                    AbsoluteExpiration = options.DefaultAbsoluteExpiration
                });
        }

        public override void Remove(string key) => cache.Remove(key);

        public override async Task RemoveAsync(string key) => await cache.RemoveAsync(key);

        public override void Refresh(string key) => cache.Refresh(key);

        public override async Task RefreshAsync(string key) => await cache.RefreshAsync(key);

        #endregion
    }
}
