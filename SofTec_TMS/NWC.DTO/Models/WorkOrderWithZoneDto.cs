using System;

namespace NWC.DTO.Models
{
    public class WorkOrderWithZoneDto
    {
        public long WorkOrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? RequestTime { get; set; }
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public double ZoneLongitude  { get; set; }
        public double ZoneLatitude  { get; set; }
        public int? LastStatusID { get; set; }
        public Guid? LastStatusBy { get; set; }
    }
}
