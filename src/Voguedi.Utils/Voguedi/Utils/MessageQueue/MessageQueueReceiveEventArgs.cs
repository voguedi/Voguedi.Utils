namespace Voguedi.Utils.MessageQueue
{
    public class MessageQueueReceiveEventArgs
    {
        #region Public Properties

        public string QueueName { get; }

        public string QueueTopic { get; }

        public string QueueMessage { get; }

        #endregion

        #region Ctors

        public MessageQueueReceiveEventArgs(string queueName, string queueTopic, string queueMessage)
        {
            QueueName = queueName;
            QueueTopic = queueTopic;
            QueueMessage = queueMessage;
        }

        #endregion

        #region Public Methods

        public override string ToString() => $"[QueueName = {QueueName}, QueueTopic = {QueueTopic}, MessageContent = {QueueMessage}]";

        #endregion
    }
}
