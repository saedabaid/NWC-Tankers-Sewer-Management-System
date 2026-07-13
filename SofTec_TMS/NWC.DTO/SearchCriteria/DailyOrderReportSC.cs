using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class DailyOrderReportSC
    {
        public PageFilter PageFilter { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<int> ServiceTypeIDs { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool ExcelFlage { get; set; }

    }
}
