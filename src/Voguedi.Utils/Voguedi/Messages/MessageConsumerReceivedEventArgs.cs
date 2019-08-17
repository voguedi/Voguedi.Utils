using System;

namespace Voguedi.Messages
{
    public class MessageConsumerReceivedEventArgs : EventArgs
    {
        #region Ctors

        public MessageConsumerReceivedEventArgs(string queueName, string queueTopic, string queueMessage)
        {
            Group = queueName;
            Topic = queueTopic;
            Content = queueMessage;
        }

        #endregion

        #region Public Properties

        public string Group { get; }

        public string Topic { get; }

        public string Content { get; }

        #endregion

        #region Public Methods

        public override string ToString() => $"[Group = {Group}, Topic = {Topic}, Content = {Content}]";

        #endregion
    }
}
