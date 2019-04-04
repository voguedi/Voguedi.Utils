using System;

namespace Voguedi.MessageQueues
{
    public class MessageQueueReceivedEventArgs : EventArgs
    {
        #region Ctors

        public MessageQueueReceivedEventArgs(string queueName, string queueTopic, string queueMessage)
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
