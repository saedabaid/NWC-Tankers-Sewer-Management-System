using System;

namespace NWC.DTO.Models
{
    public class DeviceMeterDTO
    {
        public long ID { get; set; }
        public string ConnectorTubeNumber { get; set; }
        public string MeterSerialNumber { get; set; }
        public string ManholeNumber { get; set; }
        public bool? IsScadaAutoReading { get; set; }
        public int ServiceTypeID { get; set; }
        
        //station
        public Guid StationID { get; set; }
        public string StationName { get; set; }
        public string StationCode { get; set; }

    }
}
