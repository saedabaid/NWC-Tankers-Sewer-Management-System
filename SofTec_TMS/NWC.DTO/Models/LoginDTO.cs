using NWC.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class LoginDTO
    {
        public string Token { get; set; }
        public LoginStatus Status { get; set; }
        public MembershipContext Context { get; set; }
    }
}
