using System;
using Confluent.Kafka;

namespace Voguedi.Kafka
{
    public interface IKafkaProducerPool : IDisposable
    {
        #region Methods

        IProducer<Null, string> Get();

        bool TryReturn(IProducer<Null, string> producer);

        #endregion
    }
}
