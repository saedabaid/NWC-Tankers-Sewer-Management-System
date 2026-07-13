using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class EventWorkOrderDTO
    {
        public EventWorkOrderDTO()
        {
            Accessories = new List<AccessoryDTO>();
        }

        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public Guid CreatedBy { get; set; }
        [DataMember]
        public DateTime ScheduledDeliveryTime { get; set; }
        [DataMember]
        public int StatusID { get; set; }
        [DataMember]
        public int StatusReasonID { get; set; }
        [DataMember]
        public string StatusComment { get; set; }
        [DataMember]
        public DateTime? StatusTime { get; set; }
        [DataMember]
        public int OrderQuantity { get; set; }
        [DataMember]
        public long CustomerLocationID { get; set; }
        [DataMember]
        public int ServiceTypeID { get; set; }
        [DataMember]
        public long? CustomerAccountId { get; set; }
        [DataMember]
        public Guid StationID { get; set; }
        [DataMember]
        public string ConfirmationCode { get; set; }
        [DataMember]
        public bool IsPaid { get; set; }
        [DataMember]
        public List<AccessoryDTO> Accessories { get; set; }
        public string RecieverName { get; set; }
        public string RecieverMobile { get; set; }
        public string Comments { get; set; }

        [DataMember]
        public Guid VehicleID { get; set; }
        [DataMember]
        public Guid? DriverID { get; set; }

        [DataMember]
        public int VehicleStatusID { get; set; }

        [DataMember]
        public double? VehicleLatitude { get; set; }
        [DataMember]
        public double? VehicleLongitude { get; set; }

        public string SourceApplication { get; set; }
        public string CISDivision { get; set; }
        public string TransactionID { get; set; }

        public int BC_NoOfOrders { get; set; }
        public int BC_HoldIntervalMin { get; set; }
        public DateTime BC_StartingTime { get; set; }
        public int? CategoryID { get; set; }

        public int? PriorityID { get; set; }

    }
}
