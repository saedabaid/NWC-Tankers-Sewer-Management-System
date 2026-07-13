using System;

namespace NWC.DTO.Models
{
    public class vw_NWC_ContractTermsDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string ContractTermName{ get; set; }
        public string Description { get; set; }
        public Nullable<long> TermsCategoryID { get; set; }
        public long ContractID { get; set; }
        public Nullable<System.Guid> StationID { get; set; }
        public string stationName { get; set; }
        public string Category { get; set; }
        public string ContractTermCode { get; set; }
        public string stationCode { get; set; }

        public Nullable<decimal> TotalValue { get; set; }
        public Nullable<int> TotalValueUnitId { get; set; }
        public string TotalValueUnitName { get; set; }

    }
}
