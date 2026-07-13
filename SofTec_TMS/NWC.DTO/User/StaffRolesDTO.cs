using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.User
{
    public class StaffRolesDTO
    {
        public Nullable<System.Guid> ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Nullable<System.Guid> PageId { get; set; }
        public Nullable<bool> isDefault { get; set; }
    }
}
