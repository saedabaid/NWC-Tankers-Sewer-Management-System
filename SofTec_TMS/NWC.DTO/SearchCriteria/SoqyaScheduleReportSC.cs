using NWC.DTO.Common;
using NWC.DTO.Enums;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class SoqyaScheduleReportSC
    {
        public PageFilter PageFilter { get; set; }
        //public Guid? AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<long> ZoneIDs { get; set; }

        //public List<int> MonthYearList { get; set; }
        public long? CustomerId { get; set; }
        public List<long> CustomerAccountIDs { get; set; }
        public List<ScheduleStatusEnum?> ScheduleStatus { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Operator B_Operator { get; set; }
        public long Balance { get; set; }

        public Operator NotScheduled_Operator { get; set; }
        public long NotScheduledQty { get; set; }

        public bool ExcelFlage { get; set; }



        public enum ScheduleStatusEnum
        {
            FullScheduled = 1,
            PartiallyScheduled = 2,
            NotScheduled = 3,
        }
    }

}
