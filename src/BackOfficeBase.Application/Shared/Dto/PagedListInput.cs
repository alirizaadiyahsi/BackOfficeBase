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

        public List<string> Filters { get; set; }

        public List<string> Sorts { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}