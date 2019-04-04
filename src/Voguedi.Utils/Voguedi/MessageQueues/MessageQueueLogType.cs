namespace Voguedi.MessageQueues
{
    public enum MessageQueueLogType
    {
        RabbitMQConsumerCancelled,
        RabbitMQRegistered,
        RabbitMQShutdown,
        RabbitMQUnregistered,
        KafkaOnConsumeError,
        KafkaOnError
    }
}
