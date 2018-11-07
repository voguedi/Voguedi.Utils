using System;
using System.Collections.Concurrent;

namespace Voguedi.Caching
{
    public abstract class DistributedCacheProvider : IDistributedCacheProvider
    {
        #region Private Fields

        readonly ConcurrentDictionary<Type, object> cacheMapping = new ConcurrentDictionary<Type, object>();

        #endregion

        #region Protected Methods

        protected abstract IDistributedCache<TCacheValue> Create<TCacheValue>() where TCacheValue : class;

        #endregion

        #region IDistributedCacheProvider

        public IDistributedCache<TCacheValue> Get<TCacheValue>() where TCacheValue : class => (IDistributedCache<TCacheValue>)cacheMapping.GetOrAdd(typeof(TCacheValue), Create<TCacheValue>());

        #endregion
    }
}
