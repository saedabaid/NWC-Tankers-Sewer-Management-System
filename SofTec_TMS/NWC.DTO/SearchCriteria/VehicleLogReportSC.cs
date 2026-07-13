using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class VehicleLogReportSC
    {
        public PageFilter PageFilter { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<int> ServiceTypeIDs { get; set; }
        public DateTime DateTimeFrom { get; set; }
        public DateTime DateTimeTo { get; set; }
        
        public int? LogType { get; set; }
        public string OrderNumber { get; set; }
        public string Vehicle { get; set; }
        public string Driver { get; set; }

        public bool ExcelFlage { get; set; }

    }
}
