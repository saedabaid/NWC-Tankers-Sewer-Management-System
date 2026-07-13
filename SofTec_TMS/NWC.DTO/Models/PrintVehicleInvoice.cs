using System;

namespace NWC.DTO.Models
{
    public class PrintVehicleInvoice
    {
        public System.Guid VehicleId { get; set; }
        public string VehicleCode { get; set; }
        public string VehiclePlateNo { get; set; }
        public Nullable<int> VehicleCapacity { get; set; }
        public Nullable<System.Guid> StationId { get; set; }
        public string StationName { get; set; }
        public string ContractCode { get; set; }
        public long ContractorID { get; set; }
        public string ContractorCode { get; set; }
        public string ContractorFullName { get; set; }
        public string ContractorTaxNumber { get; set; }
        public string ContractorAddress { get; set; }
        public int CustomerLocationClassID { get; set; }

        public decimal TotalCost { get; set; }
        public decimal NetCost { get; set; }
        public decimal VatValue { get; set; }

     

        //public string SubContractorFullName { get; set; }
        //public string SubContTaxNumber { get; set; }
        //public string SubContCompanyAddress { get; set; }
    }
}
