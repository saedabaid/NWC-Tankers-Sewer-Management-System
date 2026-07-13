using Infrastructure;
using NWC.DTO.Common;

namespace NWC.DTO.SearchCriteria
{
    public class SearchDto
    {
        public PageFilter PageFilter { get; set; }
        public enum OrderByExepression { }
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public bool IsDeleted { get; set; }
        public string SearchKeyword { get; set; }
    }
}