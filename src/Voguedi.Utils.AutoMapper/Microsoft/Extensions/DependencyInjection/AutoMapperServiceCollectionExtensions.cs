using System;
using System.Reflection;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.ObjectMappers;
using Voguedi.ObjectMappers.AutoMapper;
using Voguedi.Reflections;
using AutoMapperMapper = AutoMapper.Mapper;
using AutoMapperMapperConfiguration = AutoMapper.MapperConfiguration;
using IAutoMapperConfigurationProvider = AutoMapper.IConfigurationProvider;
using IAutoMapperMapper = AutoMapper.IMapper;
using IAutoMapperMapperConfigurationExpression = AutoMapper.IMapperConfigurationExpression;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutoMapperServiceCollectionExtensions
    {
        #region Private Methods

        static void CreateMap(MapperConfigurationExpression configurationExpression, Assembly[] assemblies)
        {
            foreach (var type in new TypeFinder().GetTypesByAttribute<ObjectMapperAttribute>(assemblies))
                CreateMap(configurationExpression, type);
        }

        static void CreateMap(MapperConfigurationExpression configurationExpression, Type type)
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute<ObjectMapperAttribute>();
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

        public static IServiceCollection AddAutoMapper(
            this IServiceCollection services,
            Action<IAutoMapperMapperConfigurationExpression> configAction,
            params Assembly[] assemblies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var configurationExpression = new MapperConfigurationExpression();
            configAction?.Invoke(configurationExpression);
            CreateMap(configurationExpression, assemblies);
            services.TryAddSingleton(configurationExpression);
            services.TryAddSingleton<IAutoMapperConfigurationProvider, AutoMapperMapperConfiguration>();
            services.TryAddSingleton<IAutoMapperMapper, AutoMapperMapper>();
            services.TryAddSingleton<IObjectMapper, AutoMapperObjectMapper>();
            return services;
        }

        #endregion
    }
}
