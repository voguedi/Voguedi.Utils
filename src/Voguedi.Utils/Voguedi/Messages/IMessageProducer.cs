using System.Threading.Tasks;

namespace Voguedi.Messages
{
    public interface IMessageProducer
    {
        #region Methods

        Task<OperatedResult> ProduceAsync(string queueTopic, string queueMessage);

        #endregion
    }
}
