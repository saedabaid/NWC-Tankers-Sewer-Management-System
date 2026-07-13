using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class ContractTermsViolationsDTO
    {
        public long Id { get; set; }
        public long ContractTermId { get; set; }
        public string ViolationTicketNumber { get; set; }
        public string ViolationLocation { get; set; }
        public System.DateTime IncidentTime { get; set; }
        public decimal TotalPenalty { get; set; }
        public System.DateTime IssueDate { get; set; }
        public System.DateTime? PaymentDueDate { get; set; }
        public Nullable<System.DateTime> PaymentStatusDate { get; set; }
        public string ViolationDescription { get; set; }
        public bool IsDeleted { get; set; }
        public string ContractTermCode { get; set; }
        public string ContractTermDescription { get; set; }
        public string ContractTermName { get; set; }
        public long ContractID { get; set; }
        public string ContractCode { get; set; }
        public Nullable<long> CategoryId { get; set; }
        public string CategoryAr { get; set; }
        public string CategoryEn { get; set; }
        public Nullable<System.Guid> StationID { get; set; }
        public string StationName { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentStatusAr { get; set; }
        public string PaymentStatusEn { get; set; }
        //public int PaymentStatusEnumID { get; set; }
        public Nullable<System.Guid> VehicleId { get; set; }
        //public string VehicleCodePlateNo { get; set; }
        public string VehicleCode { get; set; }
        public string VehiclePlateNo { get; set; }
        public Nullable<System.Guid> DriverId { get; set; }
        public string DriverName { get; set; }
        //public string DriverFirstName { get; set; }
        //public string DriverMiddleName { get; set; }
        //public string DriverLastName { get; set; }
        public string DriverMobileNumber { get; set; }
        public string DriverCode { get; set; }

        public int? TermUnitNoOfDays { get; set; }
        public bool AddVehicleToBlacklist { get; set; }

        public Nullable<int> StatusId { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusNameEn { get; set; }
        public Nullable<int> CancelReasonId { get; set; }
        public string CancelReasonAr { get; set; }
        public string CancelReasonEn { get; set; }

        public IEnumerable<AttachmentDTO> ViolationAttachments { get; set; }
        public bool Show { get; set; }

        public bool IsFinalDecision { get; set; }

        public Nullable<int> CurrentApprovalStatus { get; set; }

    }
    
}
