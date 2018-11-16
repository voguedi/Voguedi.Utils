using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Voguedi.Reflection
{
    public class TypeFinder : ITypeFinder
    {
        #region Private Fields

        const string ignoredAssemblies = "^System|^Mscorlib|^Netstandard|^Microsoft|^Autofac|^AutoMapper|^EntityFramework|^Newtonsoft|^Castle|^NLog|^Pomelo|^AspectCore|^Xunit|^Nito|^Npgsql|^Exceptionless|^MySqlConnector|^Anonymously Hosted|^libuv|^api-ms|^clrcompression|^clretwrc|^clrjit|^coreclr|^dbgshim|^e_sqlite3|^hostfxr|^hostpolicy|^MessagePack|^mscordaccore|^mscordbi|^mscorrc|sni|sos|SOS.NETCore|^sos_amd64|^SQLitePCLRaw|^StackExchange|^Swashbuckle|WindowsBase|ucrtbase";
        readonly Lazy<List<Assembly>> assemblies = new Lazy<List<Assembly>>();
        readonly Lazy<List<Type>> types = new Lazy<List<Type>>();

        #endregion

        #region Public Properties

        public static TypeFinder Instance => new TypeFinder();

        #endregion

        #region Private Methods

        void LoadAssemblies(string path)
        {
            foreach (var assembly in Directory.GetFiles(path, "*.dll"))
            {
                if (!IsIgnoredAssembly(Path.GetFileName(assembly)))
                    AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assembly));
            }
        }

        bool IsIgnoredAssembly(string assemblyName) => Regex.IsMatch(assemblyName, ignoredAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        bool IsIgnoredAssembly(Assembly assembly) => IsIgnoredAssembly(assembly.FullName);

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

        #endregion

        #region ITypeFinder

        public IReadOnlyList<Assembly> GetAssemblies()
        {
            if (!assemblies.IsValueCreated)
            {
                var assembliesValue = new List<Assembly>();
                LoadAssemblies(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!IsIgnoredAssembly(assembly))
                        assembliesValue.Add(assembly);
                }

                assemblies.Value.AddRange(assembliesValue.Distinct());
            }

            return assemblies.Value;
        }

        public IReadOnlyList<Type> GetTypes()
        {
            if (!types.IsValueCreated)
            {
                var typesValue = new List<Type>();

                foreach (var assembly in GetAssemblies())
                    typesValue.AddRange(GetTypes(assembly));

                types.Value.AddRange(typesValue);
            }

            return types.Value;
        }

        #endregion
    }
}
