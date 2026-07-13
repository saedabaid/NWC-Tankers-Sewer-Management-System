using Infrastructure;
using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class StaffRolesSearchCriteriaDTO
    {
        public enum OrderByExepression { ID = 1, name = 2}
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public bool IsDeleted { get; set; }
        public PageFilter PageFilter { get; set; }
    }
}
