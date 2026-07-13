using NWC.DTO.Common;
using NWC.DTO.Enums;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class WorkOrderSearchCriteriaDTO
    {
        public Filters<string> FilterModel { get; set; }

        #region Advanced
        //public Guid StaffID { get; set; }
        //public Guid SubscriberID { get; set; }

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
        public List<int> CategoryIDs { get; set; }
        public int? SearchType { get; set; }
        public string CustomerIdNumber { get; set; }
        public string CustomerMobile { get; set; }

        public enum DateToSearch { RequestDate = 1, ScheduleDate = 2, LastStatusModificationDate = 3, }
        public DateToSearch? DatePeriod { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }

        public Guid VehicleID { get; set; }
        public bool excelFlage { get; set; }

        public double CancelAfterHours { get; set; }

        public List<int> TanckerCapacityAddIds{ get; set; } 
        public long Price{ get; set; } 
        public Operator _operator{ get; set; }

        public bool? IsZoneWithoutTankers { get; set; }

        public List<string> SourceApps { get; set; }

        #endregion

        public Nullable<bool> IsVirtualStation { get; set; }
    }
}
