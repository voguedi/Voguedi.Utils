using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Voguedi.BackgroundWorkers;
using Voguedi.DependencyInjections;
using Voguedi.ObjectMappers;
using Voguedi.ObjectSerializers;

namespace Voguedi.Utils.Samples
{
    #region DependencyInjection

    interface IScoped1 { }

    class Scoped1 : IScoped1, IScopedDependency { }

    interface IScoped2 : IScopedDependency { }

    class Scoped2 : IScoped2 { }

    interface ITransient1 { }

    class Transient1 : ITransient1, ITransientDependency { }

    interface ITransient2 : ITransientDependency { }

    class Transient2 : ITransient2 { }

    interface ISingleton1 { }

    class Singleton1 : ISingleton1, ISingletonDependency { }

    interface ISingleton2 : ISingletonDependency { }

    class Singleton2 : ISingleton2 { }

    #endregion

    #region AutoMapper

    class Entity1
    {
        public string Value { get; set; } = "1";
    }

    class Dto1
    {
        public string Value { get; set; }
    }

    class Entity2
    {
        public string Value { get; set; }
    }

    [ObjectMapper(typeof(Entity2))]
    class Dto2
    {
        public string Value { get; set; } = "2";
    }

    #endregion

    #region JsonNet

    class SerializedEntity
    {
        public string Value { get; set; }
    }

    #endregion

    class Program
    {
        #region BackgroundWorker

        static void BackgroundWorker_Sample()
        {
            var backgroundWorker = new ServiceCollection()
                .AddUitls()
                .AddLogging()
                .BuildServiceProvider()
                .GetService<IBackgroundWorker>();
            var id = Guid.NewGuid().ToString();
            backgroundWorker.Start(id, () => Console.WriteLine(DateTime.Now.Ticks), 10, 10);
            Thread.Sleep(10000);
            backgroundWorker.Stop(id);
        }

        #endregion

        #region DependencyInjection

        static void DependencyInjection_Sample()
        {
            var serviceProvider = new ServiceCollection()
                .AddUitls()
                .AddLogging()
                .BuildServiceProvider();

            using (var serviceScope = serviceProvider.CreateScope())
            {
                foreach (var service in serviceScope.ServiceProvider.GetServices<IScoped1>())
                    Console.WriteLine(service.GetType().FullName);

                foreach (var service in serviceScope.ServiceProvider.GetServices<IScoped2>())
                    Console.WriteLine(service.GetType().FullName);
            }

            foreach (var service in serviceProvider.GetServices<ITransient1>())
                Console.WriteLine(service.GetType().FullName);

            foreach (var service in serviceProvider.GetServices<ITransient2>())
                Console.WriteLine(service.GetType().FullName);

            foreach (var service in serviceProvider.GetServices<ISingleton1>())
                Console.WriteLine(service.GetType().FullName);

            foreach (var service in serviceProvider.GetServices<ISingleton2>())
                Console.WriteLine(service.GetType().FullName);
        }

        #endregion

        #region AutoMapper

        static void AutoMapper_Sample()
        {
            var objectMapper = new ServiceCollection()
                .AddAutoMapper(o =>
                {
                    o.CreateMap<Dto1, Entity1>();
                    o.CreateMap<Entity1, Dto1>();
                })
                .BuildServiceProvider()
                .GetService<IObjectMapper>();

            var entity1 = new Entity1();
            var dto1 = objectMapper.Map<Entity1, Dto1>(entity1);
            Console.WriteLine($"Entity1.Value = {entity1.Value}, Dto1.Value = {dto1.Value}");

            var dto2 = new Dto2();
            var entity2 = objectMapper.Map<Dto2, Entity2>(dto2);
            Console.WriteLine($"Dto2.Value = {dto2.Value}, Entity2.Value = {entity2.Value}");
        }

        #endregion

        #region JsonNet

        static void JsonNet_Sample()
        {
            var stringSerializer = new ServiceCollection()
                .AddJsonNet()
                .BuildServiceProvider()
                .GetService<IStringSerializer>();
            var entity = new SerializedEntity { Value = "1" };
            var content = stringSerializer.Serialize(entity);
            var entity1 = stringSerializer.Deserialize<SerializedEntity>(content);
            Console.WriteLine($"Entity.Value = {entity.Value}");
            Console.WriteLine($"Content = {content}");
            Console.WriteLine($"Entity1.Value = {entity1.Value}");
            Console.WriteLine($"{entity.GetHashCode() == entity1.GetHashCode()}");
        }

        #endregion

        static void Main(string[] args)
        {
            //BackgroundWorker_Sample();
            //DependencyInjection_Sample();
            //AutoMapper_Sample();
            JsonNet_Sample();
            Console.ReadKey();
        }
    }
}
