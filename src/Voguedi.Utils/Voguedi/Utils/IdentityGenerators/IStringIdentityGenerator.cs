using Voguedi.Utils.DependencyInjection;

namespace Voguedi.Utils.IdentityGenerators
{
    public interface IStringIdentityGenerator : ITransientDependency
    {
        #region Methods

        string Generate();

        #endregion
    }
}
