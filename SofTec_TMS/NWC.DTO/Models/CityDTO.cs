using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class CityDTO
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TankerQuotaNo { get; set; }
        public int AutoCancelationNewOrdersHours { get; set; }
        public int AutoCancelationOnHoldOrdersHours { get; set; }
    }
}
