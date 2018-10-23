using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.Utils.ObjectMapping;
using Voguedi.Utils.ObjectMapping.AutoMapper;
using Voguedi.Utils.Reflection;
using AutoMapperMapperConfiguration = AutoMapper.MapperConfiguration;
using AutoMapperMapperConfigurationExpression = AutoMapper.Configuration.MapperConfigurationExpression;
using IAutoMapperConfigurationProvider = AutoMapper.IConfigurationProvider;
using IAutoMapperMapperConfigurationExpression = AutoMapper.IMapperConfigurationExpression;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Private Methods

        static void CreateMap(AutoMapperMapperConfigurationExpression configurationExpression)
        {
            var types = TypeFinder.Instance.GetTypes().Where(t => t.GetTypeInfo().IsDefined(typeof(MapperAttribute)));

            if (types?.Count() > 0)
            {
                foreach (var type in types)
                    CreateMap(configurationExpression, type);
            }
        }

        static void CreateMap(AutoMapperMapperConfigurationExpression configurationExpression, Type type)
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute<MapperAttribute>();
            var targetTypes = attribute.TargetTypes;

            if (targetTypes?.Length > 0)
            {
                foreach (var targetType in targetTypes)
                {
                    if (attribute.IsSource)
                        configurationExpression.CreateMap(type, targetType);

                    if (attribute.IsDestination)
                        configurationExpression.CreateMap(targetType, type);
                }
            }
        }

        #endregion

        #region Public Methods

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<AutoMapperMapperConfigurationExpression> setupAction)
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            var configurationExpression = new AutoMapperMapperConfigurationExpression();
            setupAction(configurationExpression);
            CreateMap(configurationExpression);
            services.TryAddSingleton<IAutoMapperMapperConfigurationExpression>(configurationExpression);
            var configuration = new AutoMapperMapperConfiguration(configurationExpression);
            services.TryAddSingleton<IAutoMapperConfigurationProvider>(configuration);
            services.TryAddSingleton(configuration.CreateMapper());
            services.TryAddSingleton<IObjectMapper, AutoMapperObjectMapper>();
            return services;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services) => services.AddAutoMapper(_ => { });

        #endregion
    }
}
