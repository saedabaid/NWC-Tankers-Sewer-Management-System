using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class StationSettingsSC : SearchDto
    {
        public List<Guid> AreaIds { get; set; }
        public List<Guid> CityIds { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> CustomerClass { get; set; }
        public Nullable<int> ServiceType { get; set; }
    }
}
