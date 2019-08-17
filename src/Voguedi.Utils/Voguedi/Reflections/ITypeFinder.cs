using System;
using System.Collections.Generic;
using System.Reflection;

namespace Voguedi.Reflections
{
    public interface ITypeFinder
    {
        #region Methods

        IEnumerable<Type> GetImplementedTypes(Type type, params Assembly[] assemblies);

        IEnumerable<Type> GetImplementedTypes<T>(params Assembly[] assemblies) where T : class;

        IEnumerable<Type> GetTypesByAttribute(Type attributeType, params Assembly[] assemblies);

        IEnumerable<Type> GetTypesByAttribute<TAttribute>(params Assembly[] assemblies) where TAttribute : Attribute;

        #endregion
    }
}
