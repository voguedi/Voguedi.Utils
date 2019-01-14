using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Voguedi.DependencyInjection.Autofac
{
    public class AutofacObjectContainer : ObjectContainer, IAutofacObjectContainer
    {
        #region Private Fields

        readonly ContainerBuilder containerBuilder = new ContainerBuilder();
        IContainer container;
        bool disposed;

        #endregion

        #region ObjectContainer

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    container.Dispose();

                disposed = true;
            }
        }

        public override IServiceProvider Build()
        {
            container = containerBuilder.Build();
            return ServiceProvider = new AutofacServiceProvider(container);
        }

        public override void Register(IServiceCollection services)
        {
            if (services?.Count > 0)
                containerBuilder.Populate(services);
        }

        public override void RegisterNamed(Type serviceType, string serviceName, Lifetime lifetime = Lifetime.Singleton)
        {
            if (serviceType.IsGenericType)
            {
                var registrationBuilder = containerBuilder.RegisterGeneric(serviceType).AsSelf();

                if (!string.IsNullOrWhiteSpace(serviceName))
                    registrationBuilder.Named(serviceName, serviceType);

                if (lifetime == Lifetime.Scoped)
                    registrationBuilder.InstancePerLifetimeScope();
                else if (lifetime == Lifetime.Singleton)
                    registrationBuilder.SingleInstance();
            }
            else
            {
                var registrationBuilder = containerBuilder.RegisterType(serviceType).AsSelf();

                if (!string.IsNullOrWhiteSpace(serviceName))
                    registrationBuilder.Named(serviceName, serviceType);

                if (lifetime == Lifetime.Scoped)
                    registrationBuilder.InstancePerLifetimeScope();
                else if (lifetime == Lifetime.Singleton)
                    registrationBuilder.SingleInstance();
            }
        }

        public override void RegisterNamed(Type serviceType, Type implementationType, string serviceName, Lifetime lifetime = Lifetime.Singleton)
        {
            if (implementationType.IsGenericType)
            {
                var registrationBuilder = containerBuilder.RegisterGeneric(implementationType).As(serviceType);

                if (!string.IsNullOrWhiteSpace(serviceName))
                    registrationBuilder.Named(serviceName, implementationType);

                if (lifetime == Lifetime.Scoped)
                    registrationBuilder.InstancePerLifetimeScope();
                else if (lifetime == Lifetime.Singleton)
                    registrationBuilder.SingleInstance();
            }
            else
            {
                var registrationBuilder = containerBuilder.RegisterType(implementationType).As(serviceType);

                if (!string.IsNullOrWhiteSpace(serviceName))
                    registrationBuilder.Named(serviceName, implementationType);

                if (lifetime == Lifetime.Scoped)
                    registrationBuilder.InstancePerLifetimeScope();
                else if (lifetime == Lifetime.Singleton)
                    registrationBuilder.SingleInstance();
            }
        }

        public override void RegisterTypesNamed(Type serviceType, IReadOnlyList<Type> implementationTypes, string serviceName, Lifetime lifetime = Lifetime.Singleton)
        {
            var registrationBuilder = containerBuilder.RegisterTypes(implementationTypes.ToArray()).As(serviceType);

            if (!string.IsNullOrWhiteSpace(serviceName))
                registrationBuilder.Named(serviceName, serviceType);

            if (lifetime == Lifetime.Scoped)
                registrationBuilder.InstancePerLifetimeScope();
            else if (lifetime == Lifetime.Singleton)
                registrationBuilder.SingleInstance();
        }

        public override object ResolveNamed(Type serviceType, string serviceName)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
                return container.ResolveNamed(serviceName, serviceType);

            return container.Resolve(serviceType);
        }

        public override TService ResolveNamed<TService>(string serviceName)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
                return container.ResolveNamed<TService>(serviceName);

            return container.Resolve<TService>();
        }

        public override IReadOnlyList<object> ResolveAllNamed(Type serviceType, string serviceName)
        {
            var implementations = default(IEnumerable<object>);

            if (!string.IsNullOrWhiteSpace(serviceName))
                implementations = (IEnumerable<object>)ResolveNamed(typeof(IEnumerable<>).MakeGenericType(serviceType), serviceName);
            else
                implementations = (IEnumerable<object>)Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType));

            return implementations?.ToList();
        }

        public override IReadOnlyList<TService> ResolveAllNamed<TService>(string serviceName) => ResolveAllNamed(typeof(TService), serviceName)?.Cast<TService>()?.ToList();

        public override bool TryResolveNamed(Type serviceType, string serviceName, out object service)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
                return container.TryResolveNamed(serviceName, serviceType, out service);

            return container.TryResolve(serviceType, out service);
        }

        public override bool TryResolveNamed<TService>(string serviceName, out TService service)
        {
            service = ResolveNamed<TService>(serviceName);

            if (service != null)
                return true;

            return false;
        }

        public override bool TryResolveAllNamed(Type serviceType, string serviceName, out IReadOnlyList<object> services)
        {
            services = ResolveAllNamed(serviceType, serviceName);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        public override bool TryResolveAllNamed<TService>(string serviceName, out IReadOnlyList<TService> services)
        {
            services = ResolveAllNamed<TService>(serviceName);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        public override IScopedResolver CreateScope() => new AutofacScopeResolver(container.BeginLifetimeScope());

        #endregion

        #region IAutofacObjectContainer

        public void RegisterContainerBuilder(Action<ContainerBuilder> containerBuilderAction) => containerBuilderAction?.Invoke(containerBuilder);


        public void RegisterModules(params IModule[] modules)
        {
            if (modules?.Length > 0)
            {
                foreach (var module in modules)
                    containerBuilder.RegisterModule(module);
            }
        }

        #endregion
    }
}
