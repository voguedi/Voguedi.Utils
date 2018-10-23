using System.Collections.Concurrent;
using System.Threading;
using RabbitMQ.Client;
using Voguedi.Utils.DisposableObjects;

namespace Voguedi.Utils.RabbitMQ
{
    class RabbitMQChannelPool : DisposableObject, IRabbitMQChannelPool
    {
        #region Private Fields

        readonly IRabbitMQConnectionPool connectionPool;
        readonly int poolSize;
        readonly ConcurrentQueue<IModel> pool = new ConcurrentQueue<IModel>();
        int count = 0;
        bool disposed = false;

        #endregion

        #region Ctors

        public RabbitMQChannelPool(IRabbitMQConnectionPool connectionPool, RabbitMQOptions options)
        {
            this.connectionPool = connectionPool;
            poolSize = options.ChannelPoolSize;
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    count = 0;

                    while (pool.TryDequeue(out var channel))
                        channel.Dispose();
                }

                disposed = true;
            }
        }

        #endregion

        #region IRabbitMQChannelPool

        public IModel Pull()
        {
            if (pool.TryDequeue(out var channel))
            {
                Interlocked.Decrement(ref count);
                return channel;
            }

            var connection = connectionPool.Pull();
            channel = connection.CreateModel();
            return channel;
        }

        public bool Push(IModel channel)
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
