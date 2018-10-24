using Voguedi.DependencyInjection;

namespace Voguedi.IdentityGeneration
{
    public interface IStringIdentityGenerator : ITransientDependency
    {
        #region Methods

        string Generate();

        #endregion
    }
}
