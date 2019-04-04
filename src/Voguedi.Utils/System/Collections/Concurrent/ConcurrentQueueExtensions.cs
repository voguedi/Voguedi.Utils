namespace System.Collections.Concurrent
{
    public static class ConcurrentQueueExtensions
    {
        #region Public Methods

        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (queue.Count > 0)
                while (queue.TryDequeue(out _)) ;
        }

        #endregion
    }
}
