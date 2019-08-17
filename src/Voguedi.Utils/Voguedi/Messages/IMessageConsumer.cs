using System;
using System.Collections.Generic;
using System.Threading;

namespace Voguedi.Messages
{
    public interface IMessageConsumer : IDisposable
    {
        #region Events

        event EventHandler<MessageConsumerReceivedEventArgs> Received;
        event EventHandler<MessageConsumerLoggedEventArgs> Logged;

        #endregion

        #region Methods

        void Subscribe(IEnumerable<string> topics);

        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        void Commit();

        void Reject();

        #endregion
    }
}
