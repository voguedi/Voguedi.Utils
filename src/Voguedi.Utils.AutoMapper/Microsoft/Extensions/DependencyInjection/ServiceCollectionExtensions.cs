using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.ObjectMapping;
using Voguedi.ObjectMapping.AutoMapper;
using Voguedi.Reflection;
using Voguedi.Utils.AutoMapper;
using AutoMapperMapper = AutoMapper.Mapper;
using AutoMapperMapperConfigurationExpression = AutoMapper.Configuration.MapperConfigurationExpression;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Private Methods

        static void CreateMap(AutoMapperMapperConfigurationExpression configurationExpression, params Assembly[] assemblies)
        {
            foreach (var type in new TypeFinder().GetTypesByAttribute<MapperAttribute>(assemblies))
                CreateMap(configurationExpression, type);
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

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<AutoMapperOptions> setupAction = null)
        {
            var options = new AutoMapperOptions();
            setupAction?.Invoke(options);
            var configurationExpression = new AutoMapperMapperConfigurationExpression();

            foreach (var mapAction in options.MapConfigs)
                mapAction?.Invoke(configurationExpression);

            CreateMap(configurationExpression, options.Assemblies);
            AutoMapperMapper.Initialize(configurationExpression);
            services.TryAddSingleton(AutoMapperMapper.Instance);
            services.TryAddSingleton<IObjectMapper, AutoMapperObjectMapper>();
            return services;
        }

        #endregion
    }
}
