using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Voguedi.Reflections
{
    public class TypeFinder : ITypeFinder
    {
        #region Private Fields

        const string ignoredAssemblies = "^System|^Mscorlib|^Netstandard|^Microsoft|^Autofac|^AutoMapper|^EntityFramework|^Newtonsoft|^Castle|^NLog|^Pomelo|^AspectCore|^Xunit|^Nito|^Npgsql|^Exceptionless|^MySqlConnector|^Anonymously Hosted|^libuv|^api-ms|^clrcompression|^clretwrc|^clrjit|^coreclr|^dbgshim|^e_sqlite3|^hostfxr|^hostpolicy|^MessagePack|^mscordaccore|^mscordbi|^mscorrc|sni|sos|SOS.NETCore|^sos_amd64|^SQLitePCLRaw|^StackExchange|^Swashbuckle|WindowsBase|ucrtbase";

        #endregion

        #region Private Methods

        bool IsIgnoredAssembly(Assembly assembly) => Regex.IsMatch(assembly.FullName, ignoredAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        IEnumerable<Assembly> GetAssemblies(Assembly[] assemblies)
        {
            var result = new List<Assembly>();

            if (assemblies?.Length > 0)
                result.AddRange(assemblies);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!IsIgnoredAssembly(assembly))
                    result.Add(assembly);
            }

            return result.Distinct().Where(a => !IsIgnoredAssembly(a));
        }

        IEnumerable<Type> TryGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }

        IEnumerable<Type> GetTypes(Assembly[] assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in GetAssemblies(assemblies))
                types.AddRange(TryGetTypes(assembly));

            return types.Distinct();
        }

        bool IsImplemented(Type type, Type implementedType)
        {
            if (type.IsAssignableFrom(implementedType))
                return true;

            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                var definition = type.GetTypeInfo().GetGenericTypeDefinition();

                foreach (var i in implementedType.GetTypeInfo().ImplementedInterfaces)
                {
                    if (i.IsGenericType)
                        return definition.IsAssignableFrom(i.GetTypeInfo().GetGenericTypeDefinition());
                }
            }

            return false;
        }

        #endregion

        #region ITypeFinder

        public IReadOnlyList<Type> GetTypesByAttribute<TAttribute>(params Assembly[] assemblies)
            where TAttribute : Attribute
            => GetTypes(assemblies).Where(t => t.GetTypeInfo().IsDefined(typeof(TAttribute)))?.ToList();

        public IEnumerable<Type> GetImplementedTypes(Type type, params Assembly[] assemblies)
            => GetTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && IsImplemented(type, t));

        public IEnumerable<Type> GetImplementedTypes<T>(params Assembly[] assemblies) where T : class => GetImplementedTypes(typeof(T), assemblies);

        public IEnumerable<Type> GetTypesByAttribute(Type attributeType, params Assembly[] assemblies)
            => GetTypes(assemblies).Where(t => t.GetTypeInfo().IsDefined(attributeType));

        IEnumerable<Type> ITypeFinder.GetTypesByAttribute<TAttribute>(params Assembly[] assemblies) => GetTypesByAttribute(typeof(TAttribute), assemblies);

        #endregion
    }
}
