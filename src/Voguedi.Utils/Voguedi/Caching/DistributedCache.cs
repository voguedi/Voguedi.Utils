using System;
using System.Threading.Tasks;

namespace Voguedi.Caching
{
    public abstract class DistributedCache<TCacheValue> : IDistributedCache<TCacheValue>
        where TCacheValue : class
    {
        #region IDistributedCache<TCacheValue>

        public abstract TCacheValue Get(string key);

        public virtual Task<TCacheValue> GetAsync(string key) => Task.FromResult(Get(key));

        public abstract void Set(string key, TCacheValue value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null);

        public virtual Task SetAsync(string key, TCacheValue value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null)
        {
            Set(key, value, slidingExpiration, absoluteExpiration);
            return Task.CompletedTask;
        }

        public abstract void Remove(string key);

        public virtual Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public abstract void Refresh(string key);

        public virtual Task RefreshAsync(string key)
        {
            Refresh(key);
            return Task.CompletedTask;
        }

        #endregion
    }
}
