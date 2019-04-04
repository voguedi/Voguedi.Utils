using AutoMapper;

namespace Voguedi.ObjectMappers.AutoMapper
{
    class AutoMapperObjectMapper : IObjectMapper
    {
        #region Private Fields

        readonly IMapper mapper;

        #endregion

        #region Ctors

        public AutoMapperObjectMapper(IMapper mapper) => this.mapper = mapper;

        #endregion

        #region IObjectMapper

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source == null)
                return default;

            return mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null)
                return default;

            return mapper.Map(source, destination);
        }

        #endregion
    }
}
