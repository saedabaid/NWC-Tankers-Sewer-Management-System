using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.User
{
    public class StaffPermissionDTO
    {
        //public Guid StaffPermissionId { get; set; }
        public string ModuleUniqueName { get; set; }
        public string PageUniqueName { get; set; }
        public string RoleName { get; set; }
    }
}
