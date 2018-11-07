namespace Voguedi.Caching
{
    public interface IDistributedCacheProvider
    {
        #region Methods

        IDistributedCache<TCacheValue> Get<TCacheValue>() where TCacheValue : class;

        #endregion
    }
}
