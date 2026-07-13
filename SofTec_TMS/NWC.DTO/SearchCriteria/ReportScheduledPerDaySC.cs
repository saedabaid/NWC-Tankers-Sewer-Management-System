using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class ReportScheduledPerDaySC
    {
        //public PageFilter PageFilter { get; set; }

        //public Guid? AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }

        public DateTime? SelectedDate { get; set; }


        //public bool ExcelFlage { get; set; }

    }
}
