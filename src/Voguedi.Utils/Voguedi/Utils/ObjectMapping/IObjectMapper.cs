﻿namespace Voguedi.Utils.ObjectMapping
{
    public interface IObjectMapper
    {
        #region Methods

        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        #endregion
    }
}