using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class ContractAccessoryDTO
    {
        public long ID { get; set; }
        public long ContractID { get; set; }
        public Guid StationID { get; set; }
        public int AccessoryID { get; set; }
        public decimal Charge { get; set; }
        public string StationName { get; set; }
        public string AccessoryName { get; set; }

        public List<Guid> StationIDs { get; set; }
        public List<int> AccessoryIDs { get; set; }
    }
}
