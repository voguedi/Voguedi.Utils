using System;

namespace Voguedi.MessageQueues
{
    public class MessageQueueLoggedEventArgs : EventArgs
    {
        #region Ctors

        public MessageQueueLoggedEventArgs(MessageQueueLogType logType, string logMessage)
        {
            LogType = logType;
            LogMessage = logMessage;
        }

        #endregion

        #region Public Properties

        public MessageQueueLogType LogType { get; }

        public string LogMessage { get; }

        #endregion
    }
}
