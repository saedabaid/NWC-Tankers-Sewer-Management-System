using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Common
{
    public class ContractPriceDTO
    {
        public string StationName { get; set; }
        public long ContractID { get; set; }
        public string ServiceTypeName { get; set; }
        public string CustomerLocationClassName { get; set; }
        public decimal PriceCharge { get; set; }
        public System.Guid StationID { get; set; }
        public long ContractPriceID { get; set; }
    }
}
