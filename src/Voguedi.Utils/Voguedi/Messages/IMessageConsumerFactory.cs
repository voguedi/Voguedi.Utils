namespace Voguedi.Messages
{
    public interface IMessageConsumerFactory
    {
        #region Methods

        IMessageConsumer Create(string group);

        #endregion
    }
}
