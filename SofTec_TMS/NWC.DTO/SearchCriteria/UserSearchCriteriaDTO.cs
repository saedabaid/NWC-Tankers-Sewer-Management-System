using Infrastructure;
using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class UserSearchCriteriaDTO
    {
        public enum OrderByExepression { Name = 1, Email = 2}
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public bool IsDeleted { get; set; }
        public PageFilter PageFilter { get; set; }
        public string Name { get; set; }
    }
}
