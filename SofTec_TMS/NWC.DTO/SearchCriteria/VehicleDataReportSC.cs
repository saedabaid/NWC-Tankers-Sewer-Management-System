using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class VehicleDataReportSC
    {
        public PageFilter PageFilter { get; set; }
        public List<Guid> AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<int> ServiceTypeIDs { get; set; }
        public List<int> StatusIDs { get; set; }


        public string ClassName { get; set; }
        //public DateTime DateTimeFrom { get; set; }
        //public DateTime DateTimeTo { get; set; }


        public string Vehicle { get; set; }
        public string Driver { get; set; }

        public bool ExcelFlage { get; set; }
    }
}
