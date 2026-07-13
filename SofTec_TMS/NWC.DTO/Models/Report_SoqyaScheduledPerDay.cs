using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class Report_SoqyaScheduledPerDay
    {
        public string StationName { get; set; }
        public DateTime? DayOfMonth { get; set; }
        public int TotalCounts { get; set; }
        public int SumQuantities { get; set; }
        public List<CapacitySum> CapacityList { get; set; }

        public class CapacitySum
        {
            public int Quantity { get; set; }
            public int? SchedulesCount { get; set; }
        }

    }



}
