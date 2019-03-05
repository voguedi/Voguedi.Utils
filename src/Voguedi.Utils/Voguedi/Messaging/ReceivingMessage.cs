namespace Voguedi.Messaging
{
    public class ReceivingMessage
    {
        #region Ctors

        public ReceivingMessage(string queueName, string queueTopic, string queueMessage)
        {
            QueueName = queueName;
            QueueTopic = queueTopic;
            QueueMessage = queueMessage;
        }

        #endregion

        #region Public Properties

        public string QueueName { get; }

        public string QueueTopic { get; }

        public string QueueMessage { get; }

        #endregion

        #region Public Methods

        public override string ToString() => $"[QueueName = {QueueName}, QueueTopic = {QueueTopic}, MessageContent = {QueueMessage}]";

        #endregion
    }
}
