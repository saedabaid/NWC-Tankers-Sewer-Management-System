using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class SoqyaCustomerBalanceDTO
    {
        public CustomerDTO Customer { get; set; }
        public CustomerLocationDTO CustomerLocation { get; set; }
        public CustomerAccountDTO Account { get; set; }
    }
}
