using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class ProfileDTO
    {
        public Guid ID { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string MobileNumber { get; set; }
        public byte[] ProfileImage { get; set; }
    }
}
