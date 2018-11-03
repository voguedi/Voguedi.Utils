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

            if (Equals(key, default(TKey)))
                throw new ArgumentNullException(nameof(key));

            return dictionary.TryRemove(key, out var value);
        }

        public static TValue GetOrAddIfNotNull<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (Equals(key, default(TKey)))
                throw new ArgumentNullException(nameof(key));

            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            lock (syncLock)
            {
                var value = dictionary.GetOrAdd(key, valueFactory);

                if (Equals(value, default(TValue)))
                    dictionary.TryRemove(key);

                return value;
            }
        }

        public static TValue GetOrAddIfNotNull<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (Equals(key, default(TKey)))
                throw new ArgumentNullException(nameof(key));

            if (Equals(value, default(TValue)))
                throw new ArgumentNullException(nameof(value));

            lock (syncLock)
            {
                var result = dictionary.GetOrAdd(key, value);

                if (Equals(result, default(TValue)))
                    dictionary.TryRemove(key);

                return result;
            }
        }

        #endregion
    }
}
