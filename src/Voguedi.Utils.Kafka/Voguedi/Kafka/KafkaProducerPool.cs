using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using Voguedi.Infrastructure;

namespace Voguedi.Kafka
{
    class KafkaProducerPool : DisposableObject, IKafkaProducerPool
    {
        #region Private Fields

        readonly Func<Producer> producerFactory;
        readonly int poolSize;
        readonly ConcurrentQueue<Producer> pool;
        int count;
        bool disposed;

        #endregion

        #region Ctors

        public KafkaProducerPool(KafkaOptions options)
        {
            producerFactory = BuildProducerFactory(options.GetConfig());
            poolSize = options.ProducerPoolSize;
            pool = new ConcurrentQueue<Producer>();
        }

        #endregion

        #region Private Methods

        static Func<Producer> BuildProducerFactory(IEnumerable<KeyValuePair<string, object>> config) => () => new Producer(config);

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    count = 0;

                    while (pool.TryDequeue(out var producer))
                        producer.Dispose();
                }
            }
        }

        #endregion

        #region IKafkaProducerPool

        public Producer Pull()
        {
            if (pool.TryDequeue(out var producer))
            {
                Interlocked.Decrement(ref count);
                return producer;
            }

            producer = producerFactory();
            return producer;
        }

        public bool Push(Producer producer)
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
