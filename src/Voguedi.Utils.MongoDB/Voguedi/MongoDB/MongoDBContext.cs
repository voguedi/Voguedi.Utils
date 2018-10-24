using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Voguedi.DisposableObjects;

namespace Voguedi.MongoDB
{
    public class MongoDBContext : DisposableObject, IMongoDBContext
    {
        #region Private Fields

        bool disposed = false;

        #endregion

        #region Ctors

        public MongoDBContext(IServiceProvider serviceProvider, MongoDBOptions options)
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            Database = client.GetDatabase(options.DatabaseName);
            Session = client.StartSession();
            Session.StartTransaction();
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    Session.Dispose();

                disposed = true;
            }
        }

        #endregion

        #region IMongoDBContext

        public virtual IMongoDatabase Database { get; }

        public virtual IClientSessionHandle Session { get; }

        public virtual void SaveChanges() => Session.CommitTransaction();

        public virtual async Task SaveChangesAsync() => await Session.CommitTransactionAsync();

        #endregion
    }
}
