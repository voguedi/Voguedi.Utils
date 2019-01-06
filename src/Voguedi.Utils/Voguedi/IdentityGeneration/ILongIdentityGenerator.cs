using Voguedi.DependencyInjection;

namespace Voguedi.IdentityGeneration
{
    public interface ILongIdentityGenerator : ISingletonDependency
    {
        #region Methods

        long Generate();

        #endregion
    }
}
