using System.Collections.Concurrent;
using System.Threading;
using RabbitMQ.Client;

namespace Voguedi.RabbitMQ
{
    class RabbitMQChannelPool : DisposableObject, IRabbitMQChannelPool
    {
        #region Private Fields

        static readonly object syncLock = new object();
        readonly IRabbitMQConnectionProvider connectionProvider;
        readonly ConcurrentQueue<IModel> pool;
        int poolSize;
        int count;
        bool disposed;

        #endregion

        #region Ctors

        public RabbitMQChannelPool(IRabbitMQConnectionProvider connectionProvider, RabbitMQOptions options)
        {
            this.connectionProvider = connectionProvider;
            pool = new ConcurrentQueue<IModel>();
            poolSize = options.ChannelPoolSize;
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                while (pool.TryDequeue(out var channel))
                    channel.Dispose();

                poolSize = 0;
                disposed = true;
            }
        }

        #endregion

        #region IRabbitMQChannelPool

        public IModel Get()
        {
            lock (syncLock)
            {
                while (count > poolSize)
                    Thread.SpinWait(1);

                if (pool.TryDequeue(out var channel))
                {
                    Interlocked.Decrement(ref count);
                    return channel;
                }

                channel = connectionProvider.Get().CreateModel();
                return channel;
            }
        }

        public bool TryReturn(IModel channel)
        {
            if (Interlocked.Increment(ref count) <= poolSize)
            {
                pool.Enqueue(channel);
                return true;
            }

            Interlocked.Decrement(ref count);
            return false;
        }

        #endregion
    }
}
