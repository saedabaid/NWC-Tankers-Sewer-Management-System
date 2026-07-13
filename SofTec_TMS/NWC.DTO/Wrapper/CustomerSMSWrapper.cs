using NWC.DAL.NWCEntities;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class CustomerSMSWrapper
    {
        public static CustomerSMSDTO WrapCustomerSMS(this NWC_CustomerSMS input)
        {
            if (input == null) return null;

            var dto = new CustomerSMSDTO()
            {
                ID = input.ID,
                OrderNumber = input.OrderNumber,
                VehicleID = input.VehicleID,
                DriverID = input.DriverID,
                DriverMobileNo = input.DriverMobileNo,
                CustomerMobileNo = input.CustomerMobileNo, 
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