using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class PermitListSC
    {
        public PageFilter PageFilter { get; set; }
        public List<Guid> AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
        public string DriverID { get; set; }
        public string DriverMobile { get; set; }
        public string TankerCode { get; set; }
        public Nullable<int> PermitStatus { get; set; }
        public DateTime? ExpirationdateFrom { get; set; }
        public DateTime? ExpirationdateTo { get; set; }
        public Boolean IsHold { get; set; }
       
        public DateTime? Expirationdate { get; set; }
    }
}
