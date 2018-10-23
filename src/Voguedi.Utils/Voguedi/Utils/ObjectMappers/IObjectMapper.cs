namespace Voguedi.Utils.ObjectMappers
{
    public interface IObjectMapper
    {
        #region Methods

        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        #endregion
    }
}
