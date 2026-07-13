using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class SewerConfirmedWorkOrderDTO
    {
        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public Guid VehicleID { get; set; }
        [DataMember]
        public Guid DriverID { get; set; }
    }
}
