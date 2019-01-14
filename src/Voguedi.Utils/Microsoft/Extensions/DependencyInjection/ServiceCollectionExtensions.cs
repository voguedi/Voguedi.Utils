using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.AsyncExecution;
using Voguedi.BackgroundWorkers;
using Voguedi.ObjectSerialization;
using Voguedi.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddUitls(this IServiceCollection services)
        {
            services.TryAddSingleton<IAsyncExecutor, AsyncExecutor>();
            services.TryAddSingleton<IBackgroundWorker, BackgroundWorker>();
            services.TryAddSingleton<IBinaryObjectSerializer, BinaryObjectSerializer>();
            services.TryAddSingleton<ITypeFinder, TypeFinder>();
            return services;
        }

        #endregion
    }
}
