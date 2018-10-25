﻿namespace System.Collections.Concurrent
{
    public static class ConcurrentDictionaryExtensions
    {
        #region Private Fields

        static readonly object syncObj = new object();

        #endregion

        #region Public Methods

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (key.Equals(default(TKey)))
                throw new ArgumentNullException(nameof(key));

            return dictionary.TryRemove(key, out var value);
        }

        public static TValue GetOrAddIfNotNull<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (key.Equals(default(TKey)))
                throw new ArgumentNullException(nameof(key));

            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            lock (syncObj)
            {
                var value = dictionary.GetOrAdd(key, valueFactory);

                if (value.Equals(default(TValue)))
                    dictionary.TryRemove(key);

                return value;
            }
        }

        public static TValue GetOrAddIfNotNull<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (key.Equals(default(TKey)))
                throw new ArgumentNullException(nameof(key));

            if (value.Equals(default(TValue)))
                throw new ArgumentNullException(nameof(value));

            lock (syncObj)
            {
                var result = dictionary.GetOrAdd(key, value);

                if (result.Equals(default(TValue)))
                    dictionary.TryRemove(key);

                return result;
            }
        }

        #endregion
    }
}
