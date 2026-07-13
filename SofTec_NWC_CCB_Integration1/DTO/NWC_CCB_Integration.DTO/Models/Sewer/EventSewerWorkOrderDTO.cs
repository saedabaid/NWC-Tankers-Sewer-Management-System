using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
   public class EventSewerWorkOrderDTO
    {
        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public int StatusName { get; set; }
        [DataMember]
        public Guid DriverID { get; set; }
        [DataMember]
        public string ConfirmationCode { get; set; }
        [DataMember]
        public int? ReasonID { get; set; }
    }
}
