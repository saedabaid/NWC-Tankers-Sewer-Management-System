using System;

namespace NWC.DTO.Models
{
    public class ContractTermsViolationsLogsDTO
    {
        public long Id { get; set; }
        public long TermViolationId { get; set; }
        public Nullable<long> ContractTermId { get; set; }
        public string ViolationTicketNumber { get; set; }
        public string ViolationLocation { get; set; }
        public Nullable<System.DateTime> IncidentTime { get; set; }
        public Nullable<decimal> TotalPenalty { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public Nullable<System.DateTime> PaymentDueDate { get; set; }
        public Nullable<System.DateTime> PaymentStatusDate { get; set; }
        public string ViolationDescription { get; set; }
        public Nullable<int> TermUnitNoOfDays { get; set; }
        public bool AddVehicleToBlacklist { get; set; }
        public bool IsDeleted { get; set; }
        public string ContractTermCode { get; set; }
        public string ContractTermDescription { get; set; }
        public string ContractTermName { get; set; }
        public Nullable<long> ContractID { get; set; }
        public string ContractCode { get; set; }
        public Nullable<System.Guid> ContractSubId { get; set; }
        public Nullable<long> CategoryId { get; set; }
        public string CategoryAr { get; set; }
        public string CategoryEn { get; set; }
        public Nullable<System.Guid> StationID { get; set; }
        public string StationName { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public string PaymentStatusAr { get; set; }
        public string PaymentStatusEn { get; set; }
        public Nullable<int> PaymentStatusEnumID { get; set; }
        public Nullable<System.Guid> VehicleId { get; set; }
        public string VehicleCode { get; set; }
        public string VehiclePlateNo { get; set; }
        public Nullable<System.Guid> DriverId { get; set; }
        public string DriverName { get; set; }
        public string DriverMobileNumber { get; set; }
        public string DriverCode { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusNameEn { get; set; }
        public Nullable<int> CancelReasonId { get; set; }
        public string CancelReasonAr { get; set; }
        public string CancelReasonEn { get; set; }
        public string AttachementsDocumentsIds { get; set; }
        public string AttachementsDocumentsNames { get; set; }

        public string CreatedByName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }



    }
    public class ContractTermsApprovalViolationsLogsDTO
    {
        public long Id { get; set; }
        public Boolean IsDeleted { get; set; }

        public Nullable<long> ContractTermId { get; set; }
        public string ViolationTicketNumber { get; set; }
        public string ViolationLocation { get; set; }
        public Nullable<System.DateTime> IncidentTime { get; set; }
        public Nullable<decimal> TotalPenalty { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }

        public string ViolationDescription { get; set; }
        public string ContractTermCode { get; set; }
        public string ContractTermDescription { get; set; }
        public string ContractTermName { get; set; }
        public Nullable<long> ContractID { get; set; }
        public string ContractCode { get; set; }
        public Nullable<int> PaymentStatusId { get; set; }
        public string PaymentStatusAr { get; set; }
        public string PaymentStatusEn { get; set; }
        public Nullable<System.Guid> VehicleId { get; set; }
        public string VehicleCode { get; set; }
        public string VehiclePlateNo { get; set; }   
        public string CreatedByName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public int LevelID { get; set; }

        public string DecisionType { get; set; }

    }

}
