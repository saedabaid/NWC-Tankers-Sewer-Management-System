using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class UserListDTO
    {
        //vw_NWC_ContractStations
        public System.Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string MobilePIN { get; set; }
        public string Comment { get; set; }
        public Nullable<bool> IsLockedOut { get; set; }

    }
}
