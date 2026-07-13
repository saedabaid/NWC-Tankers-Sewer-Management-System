using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class MeterReadingDTO
    {
        public long ID { get; set; }
        public long DeviceMeterID { get; set; }
        public decimal MeterReading { get; set; }
        public DateTime ReadingTime { get; set; }
        public string ReadingComment { get; set; }
    }
}
