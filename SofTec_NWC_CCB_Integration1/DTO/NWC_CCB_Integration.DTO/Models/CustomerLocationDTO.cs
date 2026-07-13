using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class CustomerLocationDTO
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public string Code { get; set; }
        public string IntegartionId_Zone { get; set; }
        public long ZoneID { get; set; }
        public string IntegartionId_Class { get; set; }
        public int ClassID { get; set; }
        public string IntegartionId_Priority { get; set; }
        public int PriorityID { get; set; }
        public string IntegartionId_Category { get; set; }
        public int CategoryID { get; set; }
        public string IntegartionId_Status { get; set; }
        public int StatusID { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Address { get; set; }
        public string IntegrationId { get; set; }

        public string Center { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Village { get; set; }
    }
}
