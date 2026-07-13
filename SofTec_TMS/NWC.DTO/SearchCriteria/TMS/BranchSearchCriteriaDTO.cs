using System;

namespace NWC.DTO.SearchCriteria
{
    public class BranchSearchCriteriaDTO : SearchDto
    {
        public Guid? ParentBranchId { get; set; }
        public bool? IsSubBranch { get; set; }
    }
}
