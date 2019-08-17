using System.Collections.Concurrent;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Voguedi.Kafka
{
    class KafkaProducerPool : DisposableObject, IKafkaProducerPool
    {
        #region Private Fields

        static readonly object syncLock = new object();
        readonly KafkaOptions options;
        readonly ConcurrentQueue<IProducer<Null, string>> pool;
        int poolSize;
        int count;
        bool disposed;

        #endregion

        #region Ctors

        public KafkaProducerPool(IOptions<KafkaOptions> options)
        {
            this.options = options.Value;
            pool = new ConcurrentQueue<IProducer<Null, string>>();
            poolSize = this.options.ProducerPoolSize;
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                while (pool.TryDequeue(out var producer))
                    producer.Dispose();

                poolSize = 0;
                disposed = true;
            }
        }

        #endregion

        #region IKafkaProducerPool

        public IProducer<Null, string> Get()
        {
            lock (syncLock)
            {
                while (count > poolSize)
                    Thread.SpinWait(1);

                if (pool.TryDequeue(out var producer))
                {
                    Interlocked.Decrement(ref count);
                    return producer;
                }

                producer = new ProducerBuilder<Null, string>(options.GetConfig()).Build();
                return producer;
            }
        }

        public bool TryReturn(IProducer<Null, string> producer)
        {
            if (Interlocked.Increment(ref count) <= poolSize)
            {
                pool.Enqueue(producer);
                return true;
            }

            Interlocked.Decrement(ref count);
            return false;
        }

        #endregion
    }
}
