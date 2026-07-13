using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NWC.Service.Authentication.WebAPI.Models
{
    public class StaffPermissionDTO
    {
        public Guid StaffPermissionId { get; set; }
        public string ModuleUniqueName { get; set; }
        public string PageUniqueName { get; set; }
    }
}