using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class DispatchWorkOrderDTO
    {
        public DispatchWorkOrderDTO()
        {

        }

        [DataMember]
        public EventWorkOrderDTO EventWorkOrderDTO { get; set; }
        [DataMember]
        public EventWorkOrderVehicleDTO EventWorkOrderVehicleDTO { get; set; }
    }
}
