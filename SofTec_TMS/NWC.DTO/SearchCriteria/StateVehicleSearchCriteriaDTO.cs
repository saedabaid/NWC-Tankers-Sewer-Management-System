using NWC.DTO.Common;
using NWC.DTO.Enums;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class StateVehicleSearchCriteriaDTO
    {
        public List<int> StatusIDList;

        public PageFilter PageFilter;
        public string VechilePlateNumberOrCode { get; set; }
        public string Driver { get; set; }
        public string DriverCode { get; set; }

        public int? ServiceTypeID { get; set; } = (int)ServiceTypeEnum.Ashyab;
        public List<Guid> VehicleIDs { get; set; }
        public List<Guid> DriverIDs { get; set; }

    }
}
