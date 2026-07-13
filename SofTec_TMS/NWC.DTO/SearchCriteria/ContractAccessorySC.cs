using NWC.DTO.Common;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.SearchCriteria
{
    public class ContractAccessorySC
    {
        public long ContractID { set; get; }
        public PageFilter PageFilter { get; set; }
        public List<int> AccessoryIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
    }
}
