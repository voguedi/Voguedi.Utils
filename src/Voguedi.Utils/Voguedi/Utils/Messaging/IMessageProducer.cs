using System.Threading.Tasks;
using Voguedi.Utils.AsyncExecution;

namespace Voguedi.Utils.Messaging
{
    public interface IMessageProducer
    {
        #region Methods

        Task<AsyncExecutionResult> ProduceAsync(string queueTopic, string queueMessage);

        #endregion
    }
}
