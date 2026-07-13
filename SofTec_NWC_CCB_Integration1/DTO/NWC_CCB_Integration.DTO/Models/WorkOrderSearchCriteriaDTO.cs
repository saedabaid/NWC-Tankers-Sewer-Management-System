using NWC_CCB_Integration.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class WorkOrderSearchCriteriaDTO
    {
        public Filters<string> FilterModel { get; set; }

        #region Advanced
        public List<long> CustomerIDs { get; set; }
        public List<int> ClassIDs { get; set; }
        public List<int> PriorityIDs { get; set; }
        public List<int> ServiceTypeIDs { get; set; }
        public List<Guid> AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<long> ZoneIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
        public List<int> StatusIDs { get; set; }
        public List<Guid> VehicleIDs { get; set; }
        public List<Guid> DriverIDs { get; set; }

        public enum DateToSearch { RequestDate = 1, ScheduleDate = 2, LastStatusModificationDate = 3, }
        public DateToSearch? DatePeriod { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }

        public Guid VehicleID { get; set; }
        public bool excelFlage { get; set; }
        #endregion
    }
}
