using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Voguedi.Reflection
{
    public class TypeFinder : ITypeFinder
    {
        #region Private Fields

        const string ignoredAssemblies = "^System|^Mscorlib|^Netstandard|^Microsoft|^Autofac|^AutoMapper|^EntityFramework|^Newtonsoft|^Castle|^NLog|^Pomelo|^AspectCore|^Xunit|^Nito|^Npgsql|^Exceptionless|^MySqlConnector|^Anonymously Hosted|^libuv|^api-ms|^clrcompression|^clretwrc|^clrjit|^coreclr|^dbgshim|^e_sqlite3|^hostfxr|^hostpolicy|^MessagePack|^mscordaccore|^mscordbi|^mscorrc|sni|sos|SOS.NETCore|^sos_amd64|^SQLitePCLRaw|^StackExchange|^Swashbuckle|WindowsBase|ucrtbase";

        #endregion

        #region Private Methods

        bool IsIgnoredAssembly(Assembly assembly) => Regex.IsMatch(assembly.FullName, ignoredAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        IReadOnlyList<Type> GetTypes(Assembly assembly)
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

        bool IsAssignedBySpecifiedType(Type type, Type specifiedType)
        {
            if (specifiedType.IsAssignableFrom(type))
                return true;

            if (specifiedType.GetTypeInfo().IsGenericTypeDefinition)
            {
                var definition = specifiedType.GetTypeInfo().GetGenericTypeDefinition();

                foreach (var i in type.GetTypeInfo().ImplementedInterfaces)
                {
                    if (i.IsGenericType)
                        return definition.IsAssignableFrom(i.GetTypeInfo().GetGenericTypeDefinition());
                }
            }

            return false;
        }

        #endregion

        #region ITypeFinder

        public IReadOnlyList<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!IsIgnoredAssembly(assembly))
                    assemblies.Add(assembly);
            }
            
            return assemblies.Distinct().ToList();
        }

        public IReadOnlyList<Type> GetTypes(params Assembly[] assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies?.Length > 0 ? assemblies : GetAssemblies())
            {
                if (!IsIgnoredAssembly(assembly))
                    types.AddRange(GetTypes(assembly));
            }

            return types.Distinct().ToList();
        }

        public IReadOnlyList<Type> GetTypesBySpecifiedType(Type specifiedType, params Assembly[] assemblies)
            => GetTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && IsAssignedBySpecifiedType(t, specifiedType))?.ToList();

        public IReadOnlyList<Type> GetTypesBySpecifiedType<TSpecified>(params Assembly[] assemblies) where TSpecified : class => GetTypesBySpecifiedType(typeof(TSpecified), assemblies);

        public IReadOnlyList<Type> GetTypesByAttribute<TAttribute>(params Assembly[] assemblies)
            where TAttribute : Attribute
            => GetTypes(assemblies).Where(t => t.GetTypeInfo().IsDefined(typeof(TAttribute)))?.ToList();

        #endregion
    }
}
