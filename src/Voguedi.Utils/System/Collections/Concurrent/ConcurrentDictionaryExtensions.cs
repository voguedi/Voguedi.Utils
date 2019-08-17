namespace System.Collections.Concurrent
{
    public static class ConcurrentDictionaryExtensions
    {
        #region Private Fields

        static readonly object syncLock = new object();

        #endregion

        #region Public Methods

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.TryRemove(key, out _);
        }

        public static TValue GetOrAddIfNotNull<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            lock (syncLock)
            {
                var result = dictionary.GetOrAdd(key, valueFactory);

                if (result == null)
                    dictionary.TryRemove(key);

                return result;
            }
        }

        public static TValue GetOrAddIfNotNull<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            lock (syncLock)
            {
                var result = dictionary.GetOrAdd(key, value);

                if (value == null)
                    dictionary.TryRemove(key);

                return result;
            }
        }

        #endregion
    }
}
