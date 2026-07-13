using System;

namespace NWC.DTO.Models
{
    public class VehicleLogsDTO
    {
        public long ID { get; set; }
        public int ActionLogTypeID { get; set; }
        public string LogTypeAr { get; set; }
        public string LogTypeEn { get; set; }
        public System.Guid VehicleID { get; set; }
        public string VehicleCode { get; set; }
        public string VehiclePlateNo { get; set; }
        public Nullable<System.Guid> VehicleStationId { get; set; }
        public string VehicleStationName { get; set; }
        public Nullable<int> VehicleCapacity { get; set; }
        public Nullable<int> VehicleServiceTypeID { get; set; }
        public string VehicleServiceTypeEN { get; set; }
        public string VehicleServiceTypeAR { get; set; }
        public Nullable<System.Guid> DriverID { get; set; }
        public string DriverName { get; set; }
        public string DriverMobile { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public int StatusID { get; set; }
        public Nullable<long> WorkOrderID { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerClassesAr { get; set; }
        public string CustomerClassesEn { get; set; }

    }
}
