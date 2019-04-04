namespace Voguedi.MessageQueues.Kafka
{
    class KafkaMessageQueueConsumerFactory : IMessageQueueConsumerFactory
    {
        #region Private Fields

        readonly KafkaOptions options;

        #endregion

        #region Ctors

        public KafkaMessageQueueConsumerFactory(KafkaOptions options) => this.options = options;

        #endregion

        #region IMessageQueueConsumerFactory

        public IMessageQueueConsumer Create(string queueName) => new KafkaMessageQueueConsumer(queueName, options);

        #endregion
    }
}
