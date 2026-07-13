using System;

namespace NWC.DTO.Models
{
    public class ContractTermsViolationsInVoiceDTO
    {
        public long Id { get; set; }
        public string ViolationNo { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<decimal> Value { get; set; }
        public Nullable<decimal> VAT { get; set; }
        public Nullable<decimal> ValueWithVAT { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public long ViolationId { get; set; }
        public string ContractTermName { get; set; }
        public Nullable<System.Guid> StationID { get; set; }
        public string StationName { get; set; }
        public Nullable<System.Guid> VehicleId { get; set; }
        public string VehicleCode { get; set; }
        public string VehiclePlateNo { get; set; }
        public Nullable<System.Guid> DriverId { get; set; }
        public string DriverName { get; set; }
        //public string DriverFirstName { get; set; }
        //public string DriverMiddleName { get; set; }
        //public string DriverLastName { get; set; }
        public string DriverMobileNumber { get; set; }
        public string DriverCode { get; set; }
        public string CreatedByName { get; set; }
    }
}
