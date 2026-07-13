using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class AccountDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Guid StaffID { get; set; }
        public Guid SubID { get; set; }
    }
}
