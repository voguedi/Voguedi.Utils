﻿using Voguedi.DependencyInjection;

namespace Voguedi.IdentityGeneration
{
    public interface IStringIdentityGenerator : ISingletonDependency
    {
        #region Methods

        string Generate();

        #endregion
    }
}
