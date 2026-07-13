using System;

namespace NWC.DTO.Models
{
    public class Report_OrderPerZone
    {
        public Nullable<int> TotalNoOfOrders { get; set; }
        //public long ZoneID { get; set; }
        public string ZoneName { get; set; }
        //public Nullable<System.Guid> CityID { get; set; }
        public string CityName { get; set; }
        public string StationName { get; set; }
        //public System.Guid StationID { get; set; }
        public int TotalQuantity { get; set; }
        public double? AvgCreateToDeliveredTime { get; set; }
        public double? AvgCreateToOutTime { get; set; }
        public double? AvgOutToDeliveredTime { get; set; }
    }
}
