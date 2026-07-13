using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NWC.DAL.NWCEntities;

namespace NWC.DTO.Models
{
    public class PagesDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid ModuleID { get; set; }
        public List<PagesVM> Pages { get; set; }
    }

    public class PagesVM
    {
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public System.Guid ModuleID { get; set; }
        public string UniqueName { get; set; }
        public string Path { get; set; }
        public bool IsGPS { get; set; }
        public bool IsCarRental { get; set; }
        public bool IsMaintenance { get; set; }
        public System.Guid status { get; set; }
        public bool exist { get; set; }

    }

    public class RoleVM
    {
        public System.Guid ModuleID { get; set; }
        public string ModuleName { get; set; }
        public System.Guid Name { get; set; }
        public System.Guid status { get; set; }//ASPNET ROLE ID
        public System.Guid PageID { get; set; }
        public bool exist { get; set; }

    }
    public class Body
    {
        public List<RoleVM> userDetails { get; set; }
        public StaffRoleDTO roleModel { get; set; }
    }
    public class StaffRoleDTO
    {
        public Nullable<System.Guid> ID { get; set; }
        public string roleName { get; set; }
        public short staffRoleCategoryID { get; set; }
        public string descr { get; set; }
        public Nullable<System.Guid> DefaultPageId { get; set; }
        public Nullable<bool> isDefault { get; set; }
    }

}
