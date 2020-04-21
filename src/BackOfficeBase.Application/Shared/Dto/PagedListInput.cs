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

        public Dictionary<string, string> Filters { get; set; }

        public Dictionary<string, string> Sorts { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}