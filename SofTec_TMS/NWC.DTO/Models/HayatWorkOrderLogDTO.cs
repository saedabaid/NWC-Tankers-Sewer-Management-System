using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class HayatWorkOrderLogDTO
    {
        public long ID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreatedBy { get; set; }
        public int CurrentStatus { get; set; }
        public int NewStatus { get; set; }
        public string HayatRequest { get; set; }
        public string HayatResponse { get; set; }
        public int StatusID { get; set; }
        public int? Retrials { get; set; }
        public DateTime? RetrialTime { get; set; }
    }
}
