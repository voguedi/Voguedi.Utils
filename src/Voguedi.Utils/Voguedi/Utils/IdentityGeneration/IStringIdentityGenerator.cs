using Voguedi.Utils.DependencyInjection;

namespace Voguedi.Utils.IdentityGeneration
{
    public interface IStringIdentityGenerator : ITransientDependency
    {
        #region Methods

        string Generate();

        #endregion
    }
}
