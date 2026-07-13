using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class StationNWCSettingsDTO
    {
        public StationNWCSettingsDTO()
        {
            StationServiceIds = new List<int>();
            CustomerClassIds = new List<int>();
        }

        public Guid? StationId { get; set; }
        public string StationName { get; set; }
        public Guid AreaId { get; set; }
        public string AreaName { get; set; }
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public List<int> StationServiceIds { get; set; }
        public string StationServiceNames { get; set; }
        public List<int> CustomerClassIds { get; set; }
        public string CustomerClassNames { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; }
        public bool IsVirtual { get; set; }
        public List<LookUpDTO<int>> AllCapacities { get; set; }
        public List<LookUpDTO<int>> SelectedCapacities { get; set; }
    }
}
