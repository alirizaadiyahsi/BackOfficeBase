using System.Collections.Generic;
using System.Threading;

namespace BackOfficeBase.Utilities.Collections.Extensions
{
    // TODO: Write tests
    public static class PagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(
            this IEnumerable<T> source,
            int count,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var pagedList = new PagedList<T>
            {
                TotalCount = count,
                Items = source
            };

            return pagedList;
        }
    }
}
