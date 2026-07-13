using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class NotificationDTO
    {
        [DataMember]
        public long ID { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public long NotifyingRecordId { get; set; }
        [DataMember]
        public string NotificationType { get; set; }
        [DataMember]
        public long CreatedBy { get; set; }
        [DataMember]
        public string CreatedName { get; set; }
        [DataMember]
        public bool IsNotified { get; set; }
        [DataMember]
        public int Criticality { get; set; }
        [DataMember]
        public DateTime? NotificationDateTime { get; set; }
        [DataMember]
        public long? OrderId { get; set; }
        [DataMember]
        public string NotificationConditions { get; set; }
    }
}
