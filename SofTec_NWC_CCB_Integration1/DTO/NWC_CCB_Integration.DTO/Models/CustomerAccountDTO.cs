using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class CustomerAccountDTO
    {
        public long ID { get; set; }
        public long CustomerId { get; set; }
        public long CustomerLocationId { get; set; }
        public int ServiceTypeId { get; set; }
        public string AccountId_Integration { get; set; }
        public int? SoqyaBalance { get; set; }
        public DateTime EligibleStartDate { get; set; }
        public DateTime EligibleEndDate { get; set; }
        public string Note { get; set; }
    }
}
