using System.Threading.Tasks;
using Voguedi.AsyncExecution;

namespace Voguedi.Messaging
{
    public interface IMessageProducer
    {
        #region Methods

        Task<AsyncExecutedResult> ProduceAsync(string queueTopic, string queueMessage);

        #endregion
    }
}
