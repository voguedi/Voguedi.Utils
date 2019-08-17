using System;

namespace Voguedi.Messages
{
    public class MessageConsumerLoggedEventArgs : EventArgs
    {
        #region Ctors

        public MessageConsumerLoggedEventArgs(string logMessage) => LogMessage = logMessage;

        #endregion

        #region Public Properties

        public string LogMessage { get; }

        #endregion
    }
}
