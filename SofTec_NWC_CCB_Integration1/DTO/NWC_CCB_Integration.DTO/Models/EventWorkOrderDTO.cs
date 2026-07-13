using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
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
        public Guid StationID { get; set; }
        [DataMember]
        public string ConfirmationCode { get; set; }
        [DataMember]
        public bool IsPaid { get; set; }
        [DataMember]
        public List<AccessoryDTO> Accessories { get; set; }
        [DataMember]
        public string RecieverName { get; set; }
        [DataMember]
        public string RecieverMobile { get; set; }
        [DataMember]
        public string Comments { get; set; }

        [DataMember]
        public Guid VehicleID { get; set; }
        [DataMember]
        public Guid? DriverID { get; set; }

        [DataMember]
        public double? VehicleLatitude { get; set; }
        [DataMember]
        public double? VehicleLongitude { get; set; }

        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string CancelReasonCode { get; set; }
        [DataMember]
        public string FailReasonCode { get; set; }

        public string SourceApplication { get; set; }
        public string CISDivision { get; set; }
        public string TransactionID { get; set; }

        public long ZoneID { get; set; }

        public long? CustomerAccountId { get; set; }
    }
}
