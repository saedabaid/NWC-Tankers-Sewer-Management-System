using System;

namespace NWC.DTO.Models
{
    public class DailyOrderSummaryDTO
    {
        public int ServiceTypeID { get; set; }
        public string ServiceTypeName { get; set; }
        public Guid? StationID { get; set; }
        public string StationName { get; set; }
        public string StationCode { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalSum { get; set; }
        public int? FailedToDeliverCount { get; set; }
        public int? FailedToDeliverSum { get; set; }
        public int? DeliveredCount { get; set; }
        public int? DeliveredSum { get; set; }
        public int? CancelledCount { get; set; }
        public int? CancelledSum { get; set; }
        public DateTime? CreateDate { get; set; }

    }
}
