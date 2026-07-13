using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class ContractViolationSC
    {
        public PageFilter PageFilter { get; set; }

        public long? Id { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<long> CategoryIds { get; set; }
        public List<int> PaymentIds { get; set; }
        public List<int> StatusIds { get; set; }
        public List<int> CancelReasonIds { get; set; }


        public string ViolationTicketNumber { get; set; }
        public string Contract { get; set; }
        public string Term { get; set; }
        public string Vehicle { get; set; }
        public string Driver { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? ViolationDateFrom { get; set; }
        public DateTime? ViolationDateTo { get; set; }

        public bool ExcelFlage { get; set; }

    }
    public class ContractApprovalViolation
    {
        public PageFilter PageFilter { get; set; }
        public Guid? LandmarkID { get; set; }

    }
}
