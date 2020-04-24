using System.Collections.Generic;

namespace BackOfficeBase.Utilities.Collections
{
    public interface IPagedListResult<T>
    {
        int TotalCount { get; set; }

        IEnumerable<T> Items { get; set; }
    }
}