using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class VehiclePerformanceReportSC
    {
        public PageFilter PageFilter { get; set; }

        public List<Guid> AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<int> ServiceTypeIDs { get; set; }

        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }
        public string VehicleCode { get; set; }

        public bool ExcelFlage { get; set; }
    }
}
