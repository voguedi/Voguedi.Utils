using System;
using System.Threading.Tasks;

namespace Voguedi.Caching
{
    public interface IDistributedCache<TCacheValue>
        where TCacheValue : class
    {
        #region Methods

        TCacheValue Get(string key);

        Task<TCacheValue> GetAsync(string key);

        void Set(string key, TCacheValue value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null);

        Task SetAsync(string key, TCacheValue value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null);

        void Remove(string key);

        Task RemoveAsync(string key);

        void Refresh(string key);

        Task RefreshAsync(string key);

        #endregion
    }
}
