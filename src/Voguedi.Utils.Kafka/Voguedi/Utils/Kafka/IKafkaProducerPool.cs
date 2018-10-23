using Confluent.Kafka;

namespace Voguedi.Utils.Kafka
{
    public interface IKafkaProducerPool
    {
        #region Methods

        Producer Pull();

        bool Push(Producer producer);

        #endregion
    }
}
