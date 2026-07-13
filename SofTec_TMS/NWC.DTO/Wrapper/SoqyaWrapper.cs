using NWC.DAL.NWCEntities;
using NWC.DTO.Constants;
using NWC.DTO.Models;
using System.Threading;

namespace NWC.DTO.Wrapper
{
    public static class SoqyaWrapper
    {
        public static SoqyaScheduleDTO WrapToSoqyaScheduleDTO(this vw_NWC_SoqyaSchedule input)
        {
            if (input == null) return null;

            var to = new SoqyaScheduleDTO()
            {
                Id = input.Id,
                Quantity = input.Quantity,
                ScheduleDate = input.ScheduleDate,
                AccountId = input.AccountId_Integration, 
                //CustomerBalanceId = input.CustomerBalanceId,
                SoqyaBalance = input.SoqyaBalance,

                CustomerAccountId = input.CustomerAccountId,
                CustomerId = input.CustomerId,
                CustomerLocationId = input.CustomerLocationId,
                CustomerName = input.CustomerName,
                CustomerIdTypeId = input.CustomerIdTypeId,
                CustomerIdNumber = input.CustomerIdNumber,
                CustomerMobile = input.CustomerMobile,
                CustomerLocationAddress = input.CustomerLocationAddress,
                ZoneID = input.ZoneID,
                ZoneName = input.ZoneName,
                CityID = input.CityID,
                CityName = input.CityName,
                //StationId = input.StationId,
                //StationName = input.StationName,
                WorkOrderId = input.WorkOrderId,
                OrderNumber = input.OrderNumber,
                LastStatusID = input.LastStatusID,
                LastStatusNameEn = input.LastStatusNameEn,
                LastStatusNameAr = input.LastStatusNameAr,
                StatusColor = input.StatusColor
                
            };

            //to.BalanceYearMonth = Utilities.YearMonthLongToString(input.BalanceYearMonth);
            to.TimeSlotFrom = input.TimeSlotFrom.HasValue ? input.TimeSlotFrom.Value.ToString(@"hh\:mm") : string.Empty;
            to.TimeSlotTo = input.TimeSlotTo.HasValue ? input.TimeSlotTo.Value.ToString(@"hh\:mm") : string.Empty;
            //to.LastStatusName = LanguageIsEnglish ? input.LastStatusNameEn : input.LastStatusNameAr;

            return to;
        }

        public static SoqyaScheduleReportDTO WrapToSoqyaScheduleReportDTO(this vw_NWC_Report_SoqyaSchedule input)
        {
            var to = new SoqyaScheduleReportDTO
            {
                CustomerAccountId= input.CustomerAccountId,
                AccountId_Integration = input.AccountId_Integration,
                SoqyaBalance = input.SoqyaBalance,
                CustomerId = input.CustomerId,
                CustomerLocationId = input.CustomerLocationId,
                CustomerName = input.CustomerName,
                CustomerIdTypeId = input.CustomerIdTypeId,
                CustomerIdNumber = input.CustomerIdNumber,
                CustomerMobile = input.CustomerMobile,
                CustomerLocationAddress = input.CustomerLocationAddress,
                ZoneID = input.ZoneID,
                ZoneName = input.ZoneName,
                CityID = input.CityID,
                CityName = input.CityName,
                CancelledSum = input.CancelledSum,
                DeliveredSum = input.DeliveredSum,
                NotDeliveredSum = input.NotDeliveredSum,
                ScheduledSum = input.ScheduledSum,
                Year = input.Year,
                Month = input.Month
                //YearMonth = input.YearMonth
            };

            if (to.SoqyaBalance.HasValue)
            {
                to.RemainingQty = to.SoqyaBalance - (to.DeliveredSum ?? 0) - (to.ScheduledSum ?? 0); 
            }

            return to;
        }

        //public static NWC_SoqyaSchedules WrapToSoqyaSchedule(this SoqyaScheduleDTO input)
        //{
        //    if (input == null) return null;

        //    var to = new NWC_SoqyaSchedules
        //    {
        //        CustomerBalanceId = input.CustomerBalanceId,
        //        ScheduleDate = input.ScheduleDate,
        //        Quantity = input.Quantity
        //    };

        //    if (!string.IsNullOrEmpty(input.TimeSlotFrom))
        //    {
        //        to.TimeSlotFrom =  TimeSpan.Parse(input.TimeSlotFrom);
        //    }
        //    if (!string.IsNullOrEmpty(input.TimeSlotTo))
        //    {
        //        to.TimeSlotTo = TimeSpan.Parse(input.TimeSlotTo);
        //    }

        //    return to;
        //}



        #region Helper
        private static bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
        #endregion

    }
}
