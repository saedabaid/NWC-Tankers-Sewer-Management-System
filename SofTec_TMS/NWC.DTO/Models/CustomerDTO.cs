using System.Collections.Generic;

namespace NWC.DTO.Models
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
        public List<CustomerAccountDTO> customerAccounts { get; set; }
    }
}
