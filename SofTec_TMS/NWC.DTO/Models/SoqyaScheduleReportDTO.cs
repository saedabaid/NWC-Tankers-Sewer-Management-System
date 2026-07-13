using System;

namespace NWC.DTO.Models
{
    public class SoqyaScheduleReportDTO
    {
        public long CustomerAccountId { get; set; }
        public string AccountId_Integration { get; set; }
        public Nullable<int> SoqyaBalance { get; set; }
        public long CustomerId { get; set; }
        public long CustomerLocationId { get; set; }
        public string CustomerName { get; set; }
        public int CustomerIdTypeId { get; set; }
        public string CustomerIdNumber { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerLocationAddress { get; set; }
        public long ZoneID { get; set; }
        public string ZoneName { get; set; }
        public System.Guid CityID { get; set; }
        public string CityName { get; set; }
        public Nullable<int> CancelledSum { get; set; }
        public Nullable<int> DeliveredSum { get; set; }
        public Nullable<int> NotDeliveredSum { get; set; }
        public Nullable<int> ScheduledSum { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> Month { get; set; }

        //public string YearMonth { get; set; }
        public int? RemainingQty { get; set; }
    }
}
