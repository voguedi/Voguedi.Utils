namespace Voguedi.Utils.Messaging
{
    public interface IMessageConsumerFactory
    {
        #region Methods

        IMessageConsumer Create(string queueName);

        #endregion
    }
}
