namespace System.Collections.Concurrent
{
    public static class BlockingCollectionExtensions
    {
        #region Public Methods

        public static void Clear<T>(this BlockingCollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            while (collection.TryTake(out _)) ;
        }

        #endregion
    }
}
