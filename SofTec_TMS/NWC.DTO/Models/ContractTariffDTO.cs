using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class ContractTariffDTO
    {
        public long ID { get; set; }
        public long ContractID { get; set; }

        public System.Guid CityID { get; set; }
        public string CityName { get; set; }
        public Guid StationID { get; set; }
        public string StationName { get; set; }
        public long? ZoneID { get; set; }
        public string ZoneName { get; set; }
        public int CustomerLocationClassID { get; set; }
        public string CustomerClassName { get; set; }
        public int ServiceTypeID { get; set; }
        public string ServiceTypeName { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal CubicMeterCharge { get; set; }
        public decimal DistanceCharge { get; set; }
        public decimal AfterFirstKM { get; set; }
        public int? TanckerCapacityId { get; set; }
        public long? DateFromHijri { get; set; }
        public long? DateToHijri { get; set; }
        public long ExcelSheetRowId { get; set; }
        public string ExcelValidation { get; set; }


        // properties for add only
        public IEnumerable<Guid> StationsAddIds { get; set; }
        public IEnumerable<long> ZoneAddIds { get; set; }
        public IEnumerable<int> CustomerLocationClassAddIds { get; set; }
        public IEnumerable<int> ServiceTypeAddIds { get; set; }
        public IEnumerable<int> TanckerCapacityAddIds { get; set; }
        //-----------------------------------------------------------
    }
}
