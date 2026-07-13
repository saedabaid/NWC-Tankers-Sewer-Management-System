using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
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


        public string ServiceTypeAr { get; set; }
        public string ServiceTypeEn { get; set; }

        public string CL_Code { get; set; }
        public int CL_PriorityID { get; set; }
        public int CL_CategoryID { get; set; }
        public string CL_Address { get; set; }
        public int CL_StatusID { get; set; }
        public int CL_ClassID { get; set; }
        public string CL_ClassAr { get; set; }
        public string CL_ClassEn { get; set; }
        public long CL_ZoneID { get; set; }
        public string CL_ZoneName { get; set; }

        public bool IsDeleted { get; set; }
    }
}
