using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class SoqyaScheduleDTO
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public int Quantity { get; set; }
        public System.DateTime ScheduleDate { get; set; }
        public string TimeSlotFrom { get; set; }
        public string TimeSlotTo { get; set; }
        public int? SoqyaBalance { get; set; }

        public long CustomerAccountId { get; set; }
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
        public System.Guid StationId { get; set; }
        public string StationName { get; set; }
        public Nullable<long> WorkOrderId { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<int> LastStatusID { get; set; }
        public string LastStatusNameEn { get; set; }
        public string LastStatusNameAr { get; set; }
        public string StatusColor { get; set; }

        public List<int> MonthYearAddIds { get; set; }
        public List<int> ScheculeDayListAdd { get; set; }

        public int ExcelSheetRowId { get; set; }
        public String ExcelValidation { get; set; }
        public string AccountId { get; set; }
        public bool? ExcelExceedQuantity { get; set; }
    }
}
