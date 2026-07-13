using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{

    public  class UserServicePermission
    {
        public System.Guid StaffID { get; set; }
        public bool IsDeleted { get; set; }
        public string TypeAr { get; set; }
        public string TypeEn { get; set; }
        public System.Guid SubID { get; set; }
        public string IntegrationId { get; set; }
    }
}
