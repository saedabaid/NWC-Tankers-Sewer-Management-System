using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public enum ApplyOption
    {
        AllResults = 1,
        CurrentPage = 2,
        SelectedOnly = 3,
    }

    public class VehicleNWCSettingsBulkUpdateDTO
    {
        public ApplyOption ApplyOption { get; set; }
        public VehicleSettingsSC VehicleSettingsSCModel { get; set; }
        public List<Guid> ApplyVehicleIds { get; set; }

        public int? ServiceTypeID { get; set; }
        public int? Capacity { get; set; }
        public Guid? StationID { get; set; }
        public List<int> AccessoryIDs { get; set; }
        public List<int> CustLocationClassIDs { get; set; }

    }

}
