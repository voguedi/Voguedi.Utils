using System;
using System.Threading;

namespace Voguedi.Messaging
{
    public interface IMessageConsumer : IDisposable
    {
        #region Events

        event EventHandler<MessageReceiveEventArgs> Received;

        #endregion

        #region Methods

        void Subscribe(string queueTopic);

        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        void Commit();

        void Reject();

        #endregion
    }
}
