using System;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddEntityFrameworkCore<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> setupAction)
            where TDbContext : DbContext
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            services.AddDbContext<TDbContext>(setupAction);
            return services;
        }

        #endregion
    }
}
