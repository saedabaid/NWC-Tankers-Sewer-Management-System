using Infrastructure;
using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class ZoneSearchCriteriaDTO
    {
        public enum OrderByExepression { ZoneName = 1, ZoneCode = 2, City = 3 }
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public string NameOrCode { get; set; }
        public bool IsDeleted { get; set; }
        public PageFilter PageFilter { get; set; }
        public List<Guid> AreaIDs { get; set; }
        public List<Guid>  CityIDs { get; set; }
        public List<Guid>  StationIDs { get; set; }
       

    }
}
