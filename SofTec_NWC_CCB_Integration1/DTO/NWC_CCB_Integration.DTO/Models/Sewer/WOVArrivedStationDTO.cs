using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class WOVArrivedStationDTO
    {
        [DataMember]
        public string VehicleID { get; set; }
        [DataMember]
        public long DriverID { get; set; }
        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public string ConfirmationCode { get; set; }
        [DataMember]
        public bool IsPaid { get; set; }

        [DataMember]
        public double? VehicleLatitude { get; set; }
        [DataMember]
        public double? VehicleLongitude { get; set; }
        [DataMember]
        public int VehicleCustomerClassId { get; set; }

        public List<int> VehicleCustomerLocationClassesIds { get; set; }

    }

}
