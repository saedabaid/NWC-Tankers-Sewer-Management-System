using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class DriverSMSDTO
    {
        public long ID { get; set; }
        public string OrderNumber { get; set; }
        public Guid VehicleID { get; set; }
        public Guid DriverID { get; set; }
        public string DriverMobileNo { get; set; }
        public string SMSText { get; set; }
        public DateTime CreatedTime { get; set; }
        public int StatusID { get; set; }
        public DateTime? SentTime { get; set; }
        public long CustomerLocationID { get; set; }
        public long CustomerID { get; set; }
        public string FailureMessage { get; set; }
    }
}
