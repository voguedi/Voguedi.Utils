namespace Voguedi.Utils.MessageQueue
{
    public interface IMessageQueueConsumerFactory
    {
        #region Methods

        IMessageQueueConsumer Create(string queueName);

        #endregion
    }
}
