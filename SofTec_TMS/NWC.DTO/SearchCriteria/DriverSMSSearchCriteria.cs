using NWC.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.SearchCriteria
{
    public class DriverSMSSearchCriteria
    {
        public PageFilter PageFilter { get; set; }
        public List<int> StatusIDs { get; set; }
    }
}
