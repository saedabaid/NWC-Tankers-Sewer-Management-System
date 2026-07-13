using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class EventWorkOrderVehicleDTO
    {
        public EventWorkOrderVehicleDTO()
        {

        }


        [DataMember]
        public Guid WorkflowID { get; set; }
        [DataMember]
        public Guid VehicleID { get; set; }
        [DataMember]
        public Guid CreatedBy { get; set; }
        [DataMember]
        public Guid? DriverID { get; set; }
        [DataMember]
        public int StatusReasonID { get; set; }
        [DataMember]
        public string StatusComment { get; set; }
        public int StatusID { get; set; }
    }
}
