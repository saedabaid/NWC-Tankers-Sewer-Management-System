using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class DashboardSC
    {
        //public DashboardSC()
        //{
        //    StatusIDs = new List<int>();
        //    AreaIDs = new List<int>();
        //    CityIDs = new List<int>();
        //    StationIDs = new List<int>();
        //}

        public List<int> StatusIDs { get; set; }
        public List<Guid> AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }

        public int ServiceTypeID { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string ClassName { get; set; }
    }
}
