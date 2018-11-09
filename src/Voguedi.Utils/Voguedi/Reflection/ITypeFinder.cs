using System;
using System.Collections.Generic;
using System.Reflection;
using Voguedi.DependencyInjection;

namespace Voguedi.Reflection
{
    public interface ITypeFinder : ISingletonDependency
    {
        #region Methods

        IReadOnlyList<Assembly> GetAssemblies();

        IReadOnlyList<Type> GetTypes();

        #endregion
    }
}
