namespace System.Collections.Concurrent
{
    public static class ConcurrentQueueExtensions
    {
        #region Public Methods

        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            while (queue.TryDequeue(out _)) ;
        }

        #endregion
    }
}
