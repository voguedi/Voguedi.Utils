using System;
using System.Threading;

namespace Voguedi.Messaging
{
    public interface IMessageConsumer : IDisposable
    {
        #region Events

        event EventHandler<ReceivingMessage> Received;

        #endregion

        #region Methods

        void Subscribe(params string[] queueTopics);

        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        void Commit();

        void Reject();

        #endregion
    }
}
