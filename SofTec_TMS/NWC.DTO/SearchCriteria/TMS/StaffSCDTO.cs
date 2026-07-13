using NWC.DTO.Common;
using System;

namespace NWC.DTO.SearchCriteria.TMS
{
    public class StaffSCDTO
    {
        public PageFilter PageFilter { get; set; }
        public bool IsDeleted { get; set; }
        public string searchKeyword { get; set; }
        public string[] RoleId { get; set; }
        public string[] branchId { get; set; }
        public System.Nullable<Guid>[] subBranchId { get; set; }
        public string[] stationId { get; set; }
    }
}