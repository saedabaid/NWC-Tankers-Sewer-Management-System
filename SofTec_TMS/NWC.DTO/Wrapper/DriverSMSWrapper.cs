using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Models;

namespace NWC.DTO.Wrapper
{
    public static class DriverSMSWrapper
    {
        public static DriverSMSDTO WrapDriverSMS(this NWC_DriverSMS input)
        {
            if (input == null) return null;

            var dto = new DriverSMSDTO()
            {
                ID = input.ID,
                OrderNumber = input.OrderNumber,
                VehicleID = input.VehicleID,
                DriverID = input.DriverID,
                DriverMobileNo = input.DriverMobileNo,
                SMSText = input.SMSText,
                CreatedTime = input.CreatedTime,
                StatusID = input.StatusID,
                SentTime = input.SentTime,
                FailureMessage = input.FailureMessage
            };

            return dto;
        }
    }
}
