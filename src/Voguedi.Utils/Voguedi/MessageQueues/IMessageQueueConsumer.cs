using System;
using System.Threading;

namespace Voguedi.MessageQueues
{
    public interface IMessageQueueConsumer : IDisposable
    {
        #region Events

        event EventHandler<MessageQueueReceivedEventArgs> Received;
        event EventHandler<MessageQueueLoggedEventArgs> Logged;

        #endregion

        #region Properties

        string BrokerAddress { get; }

        #endregion

        #region Methods

        void Subscribe(params string[] queueTopics);

        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        void Commit();

        void Reject();

        #endregion
    }
}
