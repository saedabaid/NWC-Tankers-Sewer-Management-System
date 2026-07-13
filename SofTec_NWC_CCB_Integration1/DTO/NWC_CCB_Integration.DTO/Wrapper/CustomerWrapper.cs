using NWC_CCB_Integration.DTO.Models;
using NWC_CCB_Integration.DTO.Models.Soqya;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Wrapper
{
    public static class CustomerWrapper
    {
        public static SoqyaCustomerBalanceDTO WrapToSoqyaCustomerBalance(this Schema dto, long zoneID, int serviceTypeID, int priorityID, int categoryID, int statusID)
        {
            var coordinatesArr = dto.Input.Premise.XYCOORDINATESGF.Split(' ');
            double lat;
            double lng;
            double.TryParse(coordinatesArr[1], out lat);
            double.TryParse(coordinatesArr[0], out lng);

            int balance;
            int.TryParse(dto.Input.SoqyaService.ELIGIBLECOMSP.Trim(), out balance);

            DateTime startDate = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dto.Input.SoqyaService.STARTDATE.Trim()))
            {
                var startDateParams = dto.Input.SoqyaService.STARTDATE.Split(' ');
                var sdParams = startDateParams[0].Split('-');
                var stParams = startDateParams.Length == 2 ? startDateParams[1].Split(':') : new string[3] { "0", "0", "0" };

                startDate = new DateTime(int.Parse(sdParams[2]), int.Parse(sdParams[1]), int.Parse(sdParams[0]), int.Parse(stParams[0]), int.Parse(stParams[1]), int.Parse(stParams[2].Split('.')[0]));
            }

            DateTime endDate = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dto.Input.SoqyaService.ENDDATE.Trim()))
            {
                var endDateParams = dto.Input.SoqyaService.ENDDATE.Split(' ');
                var edParams = endDateParams[0].Split('-');
                var etParams = endDateParams.Length == 2 ? endDateParams[1].Split(':') : new string[3] { "0", "0", "0" };

                endDate = new DateTime(int.Parse(edParams[2]), int.Parse(edParams[1]), int.Parse(edParams[0]), int.Parse(etParams[0]), int.Parse(etParams[1]), int.Parse(etParams[2].Split('.')[0]));
            }

            return new SoqyaCustomerBalanceDTO()
            {
                Customer = new CustomerDTO()
                {
                    IntegrationId = dto.Input.Account.ACCOUNTID.Trim(), 
                    Code = dto.Input.Person.PERSONID.Trim(),
                    IDNumber = dto.Input.Person.PERSONIDVALUE.Trim(),
                    IntegrationId_IDType = dto.Input.Person.PERSONIDTYPE.Trim(), 
                    FullName = dto.Input.Person.PERSONPRIMARYNAME.Trim(),
                    Mobile = dto.Input.Person.MOBILENUMBER.Trim(), 
                    Email = "mail123@mailTest123.com"
                }, 
                CustomerLocation = new CustomerLocationDTO()
                {
                    Latitude = lat, 
                    Longitude = lng, 
                    ZoneID = zoneID, 
                    IntegrationId = dto.Input.Premise.PREMISID.Trim(), 
                    IntegartionId_Class = dto.Input.Account.CUSTOMERCLASS.Trim(), 
                    PriorityID = priorityID, 
                    CategoryID = categoryID, 
                    StatusID = statusID, 
                    Center = dto.Input.Premise.CENTER, 
                    City = dto.Input.Premise.CITY, 
                    Province = dto.Input.Premise.PROVINCE,
                    Village = dto.Input.Premise.VILLAGE
                },
                Account = new CustomerAccountDTO()
                {
                    AccountId_Integration = dto.Input.Account.ACCOUNTID,
                    ServiceTypeId = serviceTypeID,
                    SoqyaBalance = balance / 1000,  //convert from litre to m3
                    EligibleStartDate = startDate, 
                    EligibleEndDate = endDate, 
                    Note = dto.Input.SoqyaService.NOTE
                }
            };
        }
    }
}
