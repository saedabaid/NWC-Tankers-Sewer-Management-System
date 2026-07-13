using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
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

    public class SewerConfirmedWorkOrderDTO
    {
        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public Guid VehicleID { get; set; }
        [DataMember]
        public Guid DriverID { get; set; }
    }

    public class SewerCompletedWorkOrderDTO
    {
        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public Guid VehicleID { get; set; }
        [DataMember]
        public Guid DriverID { get; set; }
    }
}
