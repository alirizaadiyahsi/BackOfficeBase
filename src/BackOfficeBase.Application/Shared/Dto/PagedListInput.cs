using System.Collections.Generic;

namespace BackOfficeBase.Application.Shared.Dto
{
    public class PagedListInput
    {
        public PagedListInput()
        {
            PageIndex = 0;
            PageSize = 10;
        }

        public PagedListInput(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public List<string> Filters { get; set; }

        public List<string> Sorts { get; set; } = new List<string> { "Id" };

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}