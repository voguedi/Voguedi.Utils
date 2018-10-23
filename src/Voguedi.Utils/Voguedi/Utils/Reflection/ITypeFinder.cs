using System;
using System.Collections.Generic;
using System.Reflection;

namespace Voguedi.Utils.Reflection
{
    public interface ITypeFinder
    {
        #region Methods

        IReadOnlyList<Assembly> GetAssemblies();

        IReadOnlyList<Type> GetTypes();

        #endregion
    }
}
