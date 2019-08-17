namespace System.Linq
{
    public static class QueryableExtensions
    {
        #region Public Methods

        public static IQueryable<T> PageBy<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            return queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        #endregion
    }
}
