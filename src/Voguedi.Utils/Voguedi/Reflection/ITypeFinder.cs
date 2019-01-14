using System;
using System.Collections.Generic;
using System.Reflection;

namespace Voguedi.Reflection
{
    public interface ITypeFinder
    {
        #region Methods

        IReadOnlyList<Assembly> GetAssemblies();

        IReadOnlyList<Type> GetTypes(params Assembly[] assemblies);

        IReadOnlyList<Type> GetTypesBySpecifiedType(Type specifiedType, params Assembly[] assemblies);

        IReadOnlyList<Type> GetTypesBySpecifiedType<TSpecified>(params Assembly[] assemblies) where TSpecified : class;

        IReadOnlyList<Type> GetTypesByAttribute<TAttribute>(params Assembly[] assemblies) where TAttribute : Attribute;

        #endregion
    }
}
