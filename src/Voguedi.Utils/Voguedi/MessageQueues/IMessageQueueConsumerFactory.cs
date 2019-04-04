namespace Voguedi.MessageQueues
{
    public interface IMessageQueueConsumerFactory
    {
        #region Methods

        IMessageQueueConsumer Create(string queueName);

        #endregion
    }
}
