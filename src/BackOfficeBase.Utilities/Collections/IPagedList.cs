using System.Collections.Generic;

namespace BackOfficeBase.Utilities.Collections
{
    public interface IPagedList<T>
    {
        int TotalCount { get; set; }

        IEnumerable<T> Items { get; set; }
    }
}