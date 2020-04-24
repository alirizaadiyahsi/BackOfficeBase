using System.Collections.Generic;

namespace BackOfficeBase.Utilities.Collections
{
    public class PagedListResult<T> : IPagedListResult<T>
    {
        public PagedListResult()
        {
            Items = new List<T>();
        }

        public int TotalCount { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}