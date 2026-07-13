using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class SewerWorkOrderDTO
    {
        public long WorkOrderId { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public int OrderQuantity { get; set; }
        public System.DateTime RequestTime { get; set; }
        public Nullable<int> LastStatusID { get; set; }
        public Nullable<long> CustomerLocationID { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public Nullable<decimal> NetCost { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> InvoiceStatusID { get; set; }
        public Nullable<bool> IsAssigned { get; set; }
        public Nullable<System.Guid> AssignedVehicleID { get; set; }
        public Nullable<System.Guid> AssignedDriverID { get; set; }
        public string ConfirmationCode { get; set; }
        public string EncryptedConfirmationCode { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public System.Guid SubID { get; set; }
        public string RecieverName { get; set; }
        public string RecieverMobile { get; set; }
        public string Comments { get; set; }
        public Nullable<int> Retrials { get; set; }
        public string SourceApplication { get; set; }
        public Nullable<long> CustomerAccountId { get; set; }
    }
}
