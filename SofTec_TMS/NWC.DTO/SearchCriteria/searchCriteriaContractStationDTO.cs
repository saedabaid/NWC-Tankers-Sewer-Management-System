using NWC.DTO.Common;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.SearchCriteria
{
    public class searchCriteriaContractStationDTO
    {
        public PageFilter PageFilter { get; set; }
        public ContractStationDTO ContractStationDTO { get; set; }
        
    }
}
