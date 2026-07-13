using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class WorkOrderStatusLogDTO
    {
        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string StatusReason { get; set; }
        [DataMember]
        public DateTime ChangedTime { get; set; }
        [DataMember]
        public Guid ChangedBy { get; set; }
        [DataMember]
        public string ChangedByName { get; set; }
        public int ActionLogTypeID { get; set; }

    }
}
