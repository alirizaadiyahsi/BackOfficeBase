using System.Collections.Generic;

namespace BackOfficeBase.Utilities.Collections
{
    public class PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
            Items = new List<T>();
        }

        public int TotalCount { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}