namespace Voguedi.Messaging
{
    public interface IMessageConsumerFactory
    {
        #region Methods

        IMessageConsumer Create(string queueName);

        #endregion
    }
}
