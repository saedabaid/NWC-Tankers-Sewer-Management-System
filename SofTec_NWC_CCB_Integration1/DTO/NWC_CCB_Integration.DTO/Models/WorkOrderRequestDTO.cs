using System;
using System.Collections.Generic;

namespace NWC_CCB_Integration.DTO.Models
{
    public class WorkOrderRequestDTO
    {
        public WorkOrderRequestDTO()
        {
            TankerAccessoriesCode = new List<string>();
        }

        public string OrderNumber { get; set; } //100
        public int OrderQuantity { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ScheduledDeliveryTime { get; set; }
        public DateTime CreationTime { get; set; }
        public string AccountID { get; set; }
        public string CustomerCode { get; set; } //50
        public string PersonName { get; set; } //max
        public string PersonMobile { get; set; } //500
        public string IDNumber { get; set; } //10
        public string IDTypeID { get; set; }
        public string Email { get; set; } //50
        public string ContactName { get; set; } //max
        public string ContactMobile { get; set; } //500
        public string PremiseID { get; set; }
        public string PremiseCode { get; set; } //10
        public string ClassID { get; set; }
        public string PriorityID { get; set; }
        public string CategoryID { get; set; }
        public string CustomerLocStatusID { get; set; }
        public string ServiceTypeCode { get; set; }
        public List<string> TankerAccessoriesCode { get; set; } //ask
        public string PremiseCoordinates { get; set; } //ask 100
        public string Comment { get; set; } //500
        public string ConfirmationCode { get; set; } //50

        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }

        public string SourceApplication { get; set; }
        public string CISDivision { get; set; }
        public string TransactionID { get; set; }

        public long ZoneID { get; set; }
    }
}
