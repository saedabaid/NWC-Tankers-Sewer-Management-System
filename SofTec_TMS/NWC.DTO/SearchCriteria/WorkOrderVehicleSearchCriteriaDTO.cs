using NWC.DTO.Common;
using NWC.DTO.Enums;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class WorkOrderVehicleSearchCriteriaDTO
    {
        public WorkOrderVehicleSearchCriteriaDTO()
        {
            WorkOrderStatusIDs = new List<int>();
        }

        public Boolean? IsDeassigned { get; set; }
        public Boolean? IsAssigned { get; set; }
        public PageFilter PageFilter { get; set; }
        public string WorkOrderNumber { get; set; }
        public string VechilePlateNumberOrCode { get; set; }

        public int ServiceTypeID { get; set; } = (int)ServiceTypeEnum.Ashyab;
        public string Driver { get; set; }
        public string DriverCode { get; set; }
        public List<int> WorkOrderStatusIDs { get; set; }
        public List<Guid> VehicleIDs { get; set; }
        public List<Guid> DriverIDs { get; set; }
    }
}
