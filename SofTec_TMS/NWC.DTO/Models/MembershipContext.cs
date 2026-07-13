using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class MembershipContext
    {
        public IPrincipal Principal { get; set; }
        public AccountDTO Account { get; set; }
        public MobileAccountDTO MobileAccount { get; set; }

        public bool IsValid()
        {
            return Principal != null;
        }
    }
}
