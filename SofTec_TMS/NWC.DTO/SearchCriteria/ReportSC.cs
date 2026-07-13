using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class ReportSC
    {
        public PageFilter PageFilter { get; set; }

        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<long> ZoneIDs { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }


        public bool ExcelFlage { get; set; }
    }
}
