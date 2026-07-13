using NWC.DAL.NWCEntities;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using System.Linq;

namespace NWC.DTO.Wrapper
{
    public static class CustomerWrapper
    {

        #region NWC Portal
        public static NWC_Customer WrapToCustomerAndAccounts(this CustomerDTO input)
        {
            if (input == null) return null;

            var customer = new NWC_Customer
            {
                ID = input.ID,
                IntegrationId = input.IntegrationId.Trim(),
                Code = !string.IsNullOrEmpty(input.Code) ? input.Code.Trim() : input.IDNumber.Trim(),
                FullName = input.FullName.Trim(),
                IDTypeID = input.IDTypeID,
                IDNumber = input.IDNumber.Trim(),
                Email = input.Email.Trim(),
                Mobile = input.Mobile.Trim(),
                LandlineNumber = input.LandlineNumber
            };

            if (input.customerAccounts != null && input.customerAccounts.Any())
            {
                foreach (var item in input.customerAccounts)
                {
                    //var newLocation = WrapToCustomerLocation(item);
                    var newAccount = WrapToCustomerAccountAndLocations(item);
                    customer.NWC_CustomerAccount.Add(newAccount);
                }
            }

            return customer;
        }

        public static NWC_CustomerAccount WrapToCustomerAccountAndLocations(this CustomerAccountDTO input)
        {
            if (input == null) return null;

            var customerAccount = new NWC_CustomerAccount
            {
                ServiceTypeId = input.ServiceTypeId,
                AccountId_Integration = input.AccountId_Integration,
                IsDeleted = false
            };

            customerAccount.NWC_CustomerLocation = new NWC_CustomerLocation
            {
                //IntegrationId = input.IntegrationId,
                ID = input.ID,
                CustomerID = input.CustomerId,
                Code = !string.IsNullOrEmpty(input.CL_Code) ? input.CL_Code.Trim() : string.Empty,
                ZoneID = input.CL_ZoneID,
                ClassID = input.CL_ClassID,
                PriorityID = input.CL_PriorityID,
                CategoryID = input.CL_CategoryID,
                StatusID = input.CL_StatusID,
                //Latitude = input.Latitude,
                //Longitude = input.Longitude,
                Address = input.CL_Address
            };

            return customerAccount;
        }

        public static CustomerAccountDTO WrapToCustomerAccountDTO(this vw_NWC_CustomerAccountZone input)
        {
            if (input == null) return null;

            var customerLocation = new CustomerAccountDTO
            {
                ID = input.Id,
                CustomerId = input.CustomerId,
                CustomerLocationId = input.CustomerLocationId,
                ServiceTypeId = input.ServiceTypeId,
                AccountId_Integration = input.AccountId_Integration,
                SoqyaBalance = input.SoqyaBalance,
                ServiceTypeAr = input.ServiceTypeAr,
                ServiceTypeEn = input.ServiceTypeEn,

                CL_Code = input.CustomerLocationCode,
                CL_PriorityID = input.CustomerLocationPriorityID,
                CL_CategoryID = input.CustomerLocationCategoryID,
                CL_Address = input.CustomerLocationAddress,
                CL_StatusID = input.CustomerLocationStatusId,
                CL_ClassID = input.CustomerLocationClassId,
                CL_ClassAr = input.CustomerLocationClassAr,
                CL_ClassEn = input.CustomerLocationClassEn,
                CL_ZoneID = input.CustomerLocationZoneId,
                CL_ZoneName = input.ZoneName
            };

            return customerLocation;
        }

        #endregion



        #region Integration

        #region Customer        
        public static NWC_Customer WrapToCustomer(this CustomerDTO input)
        {
            if (input == null) return null;

            var customer = new NWC_Customer
            {
                ID = input.ID,
                IntegrationId = input.IntegrationId.Trim(),
                Code = !string.IsNullOrEmpty(input.Code) ? input.Code.Trim() : input.IDNumber.Trim(),
                FullName = input.FullName.Trim(),
                IDTypeID = input.IDTypeID,
                IDNumber = input.IDNumber.Trim(),
                Email = input.Email.Trim(),
                Mobile = input.Mobile.Trim(),
                LandlineNumber = input.LandlineNumber
            };

            //if (input.customerAccounts != null && input.customerAccounts.Any())
            //{
            //    foreach (var item in input.customerAccounts)
            //    {
            //        //var newLocation = WrapToCustomerLocation(item);
            //        var newAccount = WrapToCustomerAccount(item);
            //        customer.NWC_CustomerAccount.Add(newAccount);
            //    }
            //}

            return customer;
        }

        public static CustomerDTO WrapToCustomerDTO(this NWC_Customer input)
        {
            if (input == null) return null;

            var customer = new CustomerDTO
            {
                ID = input.ID,
                Code = input.Code?.Trim(),
                FullName = input.FullName?.Trim(),
                IDTypeID = input.IDTypeID,
                IDNumber = input.IDNumber?.Trim(),
                Email = input.Email?.Trim(),
                Mobile = input.Mobile?.Trim(),
                LandlineNumber = input.LandlineNumber,
                IntegrationId = input.IntegrationId?.Trim()
            };

            return customer;
        }
        #endregion

        #region Customer Location
        public static NWC_CustomerLocation WrapToCustomerLocation(this CustomerLocationDTO input)
        {
            if (input == null) return null;

            var customerLocation = new NWC_CustomerLocation
            {
                IntegrationId = input.IntegrationId,
                ID = input.ID,
                //CustomerID = input.CustomerID,
                Code = !string.IsNullOrEmpty(input.Code) ? input.Code.Trim() : input.IntegrationId,
                ZoneID = input.ZoneID,
                ClassID = input.ClassID,
                PriorityID = input.PriorityID,
                CategoryID = input.CategoryID,
                StatusID = input.StatusID,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                Address = input.Address,
                City = input.City,
                Province = input.Province,
                Village = input.Village,
                Center = input.Center
            };

            return customerLocation;
        }

        public static CustomerLocationDTO WrapToCustomerLocationDTO(this NWC_CustomerLocation input)
        {
            if (input == null) return null;

            var customerLocation = new CustomerLocationDTO
            {
                ID = input.ID,
                //CustomerID = input.CustomerID,
                Code = input.Code.Trim(),
                ZoneID = input.ZoneID,
                ClassID = input.ClassID,
                PriorityID = input.PriorityID,
                CategoryID = input.CategoryID,
                StatusID = input.StatusID,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                Address = input.Address,
                IntegrationId = input.IntegrationId
            };

            return customerLocation;
        }
        #endregion

        #region Customer Account
        public static NWC_CustomerAccount WrapToCustomerAccount(this CustomerAccountDTO input)
        {
            if (input == null) return null;

            var customerAccount = new NWC_CustomerAccount
            {
                ID = input.ID,
                CustomerId = input.CustomerId,
                CustomerLocationId = input.CustomerLocationId,
                ServiceTypeId = input.ServiceTypeId,
                AccountId_Integration = input.AccountId_Integration,
                EligibleStartDate = input.EligibleStartDate != System.DateTime.MinValue ? input.EligibleStartDate : DateTimeHelper.GetDateTimeNow(),
                EligibleEndDate = input.EligibleEndDate != System.DateTime.MinValue ? input.EligibleEndDate : DateTimeHelper.GetDateTimeNow(),
                Note = input.Note,
                SoqyaBalance = input.SoqyaBalance
            };

            return customerAccount;
        }

        public static CustomerAccountDTO WrapToCustomerAccountDTO(this NWC_CustomerAccount input)
        {
            if (input == null) return null;

            var customerLocation = new CustomerAccountDTO
            {
                ID = input.ID,
                CustomerId = input.CustomerId,
                CustomerLocationId = input.CustomerLocationId,
                ServiceTypeId = input.ServiceTypeId,
                AccountId_Integration = input.AccountId_Integration
            };

            return customerLocation;
        }
        #endregion

        #endregion

    }
}
