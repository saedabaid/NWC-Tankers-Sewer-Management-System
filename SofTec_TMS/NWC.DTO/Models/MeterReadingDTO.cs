using System;

namespace NWC.DTO.Models
{
    public class MeterReadingDTO
    {
        public long ID { get; set; }
        public long DeviceMeterID { get; set; }
        public decimal MeterReading { get; set; }
        public DateTime ReadingTime { get; set; }
        public string ReadingComment { get; set; }
        
        //Device meter
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
