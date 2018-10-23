using System.Threading.Tasks;
using Voguedi.Utils.AsyncExecutors;

namespace Voguedi.Utils.MessageQueue
{
    public interface IMessageQueueProducer
    {
        #region Methods

        Task<AsyncExecutionResult> ProduceAsync(string queueTopic, string queueMessage);

        #endregion
    }
}
