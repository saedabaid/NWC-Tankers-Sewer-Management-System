using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class CustomerDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string IntegrationId_IDType { get; set; }
        public int IDTypeID { get; set; }
        public string IDNumber { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string LandlineNumber { get; set; }
        public string IntegrationId { get; set; }
    }
}
