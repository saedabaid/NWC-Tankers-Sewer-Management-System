using NWC.DTO.Common;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class HayatWorkOrderLogsSC
    {
        public List<int> StatusIDs { get; set; }

        public int Take { get; set; }
    }
}
