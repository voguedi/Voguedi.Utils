namespace System.Linq
{
    public static class QueryableExtensions
    {
        #region Public Methods

        public static IQueryable<T> PageBy<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (pageNumber <= 0)
                throw new ArgumentNullException(nameof(pageNumber));

            if (pageSize <= 0)
                throw new ArgumentNullException(nameof(pageSize));

            return queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        #endregion
    }
}
