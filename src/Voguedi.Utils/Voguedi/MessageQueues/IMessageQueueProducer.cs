using System.Threading.Tasks;
using Voguedi.Infrastructure;

namespace Voguedi.MessageQueues
{
    public interface IMessageQueueProducer
    {
        #region Methods

        Task<AsyncExecutedResult> ProduceAsync(string queueTopic, string queueMessage);

        #endregion
    }
}
