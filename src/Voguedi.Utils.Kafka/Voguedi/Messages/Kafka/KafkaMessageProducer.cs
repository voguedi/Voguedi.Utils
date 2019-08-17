using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Voguedi.Kafka;

namespace Voguedi.Messages.Kafka
{
    class KafkaMessageProducer : IMessageProducer
    {
        #region Private Fields

        readonly IKafkaProducerPool producerPool;

        #endregion

        #region Ctors

        public KafkaMessageProducer(IKafkaProducerPool producerPool) => this.producerPool = producerPool;

        #endregion

        #region IMessageProducer

        public async Task<OperatedResult> ProduceAsync(string topic, string content)
        {
            var producer = default(IProducer<Null, string>);

            try
            {
                producer = producerPool.Get();
                var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = content });

                if (result.Status == PersistenceStatus.Persisted || result.Status == PersistenceStatus.PossiblyPersisted)
                    return OperatedResult.Success;

                throw new Exception("Kafka topic message persisted failed!");
            }
            catch (Exception ex)
            {
                return OperatedResult.Failed(ex);
            }
            finally
            {
                if (producer != null && !producerPool.TryReturn(producer))
                    producer.Dispose();
            }
        }

        #endregion
    }
}
