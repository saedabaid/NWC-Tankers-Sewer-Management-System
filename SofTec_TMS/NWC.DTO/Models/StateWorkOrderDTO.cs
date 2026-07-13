using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class StateWorkOrderDTO
    {
        [DataMember]
        public long WorkOrderID { get; set; }
        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public DateTime? CreateTime { get; set; }
        [DataMember]
        public Guid? CreatedBy { get; set; }
        [DataMember]
        public DateTime? LastModifiedTime { get; set; }
        [DataMember]
        public Guid? LastModifiedBy { get; set; }
        [DataMember]
        public int LastStatusID { get; set; }
        [DataMember]
        public Guid? LastStatusBy { get; set; }
        [DataMember]
        public DateTime? LastStatusTime { get; set; }
        [DataMember]
        public Int64 OrderQuantity { get; set; }
        [DataMember]
        public DateTime? ScheduledDeliveryTime { get; set; }
        [DataMember]
        public DateTime? RequestTime { get; set; }
        [DataMember]
        public long CustomerLocationID { get; set; }
        [DataMember]
        public int ServiceTypeID { get; set; }
        [DataMember]
        public decimal TotalCost { get; set; }
        [DataMember]
        public bool? IsPaid { get; set; }
        [DataMember]
        public DateTime? PaymentTime { get; set; }
        [DataMember]
        public int? PaymentTypeID { get; set; }
        [DataMember]
        public string PaymentComment { get; set; }
        [DataMember]
        public bool? IsAssigned { get; set; }
        [DataMember]
        public Guid? AssignedVehicleID { get; set; }
        [DataMember]
        public Guid? AssignedDriverID { get; set; }
        [DataMember]
        public Guid? AssignedStationID { get; set; }
        [DataMember]
        public string Accessories { get; set; }
        [DataMember]
        public string ConfirmationCode { get; set; }
        [DataMember]
        public string EncryptedConfirmationCode { get; set; }
        [DataMember]
        public bool? IsDeleted { get; set; }
    }
}
