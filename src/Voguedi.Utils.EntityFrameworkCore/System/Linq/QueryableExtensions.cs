using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        #region Public Methods

        public static async Task<IReadOnlyList<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            return await queryable.ToListAsync(cancellationToken);
        }

        #endregion
    }
}
