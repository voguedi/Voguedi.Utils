using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Voguedi;
using Voguedi.MongoDB;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddMongoDB<TMongoDBContext>(this IServiceCollection services, Action<MongoDBOptions> setupAction)
            where TMongoDBContext : class, IMongoDBContext
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            var options = new MongoDBOptions();
            setupAction(options);
            services.AddSingleton(options);
            services.TryAddSingleton<IMongoClient>(new MongoClient(options.ConnectionString));
            services.TryAddScoped<TMongoDBContext>();
            return services;
        }

        public static IServiceCollection AddMongoDB(this IServiceCollection services, Action<MongoDBOptions> setupAction) => services.AddMongoDB<MongoDBContext>(setupAction);

        #endregion
    }
}
