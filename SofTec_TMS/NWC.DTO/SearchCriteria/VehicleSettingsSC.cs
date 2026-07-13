using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class VehicleSettingsSC
    {
        public PageFilter PageFilter { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<Guid> VehicleIDs { get; set; }
        public List<Guid> VehicleTypeIDs { get; set; }

    }
}
