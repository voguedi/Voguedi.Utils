using System;
using System.Threading;

namespace Voguedi.Utils.MessageQueue
{
    public interface IMessageQueueConsumer
    {
        #region Events

        event EventHandler<MessageQueueReceiveEventArgs> Received;

        #endregion

        #region Methods

        void Subscribe(string queueTopic);

        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        void Commit();

        void Reject();

        #endregion
    }
}
