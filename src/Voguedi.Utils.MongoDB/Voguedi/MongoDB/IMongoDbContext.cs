using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Voguedi.MongoDB
{
    public interface IMongoDBContext : IDisposable
    {
        #region Properties

        IMongoDatabase Database { get; }

        IClientSessionHandle Session { get; }

        #endregion

        #region Methods

        void SaveChanges();

        Task SaveChangesAsync();

        #endregion
    }
}
