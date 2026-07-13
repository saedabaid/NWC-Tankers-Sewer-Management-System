using NWC.DAL.NWCEntities;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class StateVehicleWrapper
    {
        public static StateVehicleDTO WrapToStateVehicleDTO(this vw_NWC_StateVehicle WOV)
        {
            var to = new StateVehicleDTO()
            {
                VehicleID = WOV.VehicleID,
                //VehicleCodePlateNo = WOV.VehicleCodePlateNo,
                VehicleStatusId = WOV.VehicleStatusId,
                VehicleStatusName = WOV.VehicleStatusName,
                VehicleStatusNameAr = WOV.VehicleStatusNameAr,
                DriverID = WOV.DriverId,
                //DriverName = WOV.DriverName,
                DriverMobileNumber = WOV.DriverMobile,
                DriverCode = WOV.DriverCode,
                IsAvailable = WOV.VehicleStatusId == (int)VehicleStatusEnum.Available,
                IsOutOfService = WOV.VehicleStatusId == (int)VehicleStatusEnum.OutOfService,
                IsBlacklisted = WOV.VehicleStatusId == (int)VehicleStatusEnum.Blacklisted,
                IsParking = WOV.VehicleStatusId == (int)VehicleStatusEnum.Parking,
                VehicleStatusColorCode = WOV.VehicleStatusColorCode,
                Capacity = WOV.Capacity,
                PermitNumber=WOV.PermitNumber,
                PermitStatus=WOV.PermitStatus,
                IsHold = WOV.IsHold,
                Expirationdate = WOV.Expirationdate,
                CitySettings_ShowInvoice = WOV.CitySettingsShowInvoice,
                CitySettings_ShowCustomerClassEntryGate = WOV.CitySettings_ShowCustomerClassEntryGate,
                //VehicleCustomerLocationClassId = WOV.VehicleCustomerLocationClassId
            };

            to.DriverName = $"{WOV.DriverFirstName ?? string.Empty} {WOV.DriverMiddleName ?? string.Empty} {WOV.DriverLastName ?? string.Empty}";
            to.VehicleCodePlateNo = $"{WOV.VehicleCode} | {WOV.VehiclePlateNo}";

            to.VehicleCustomerLocationClassesIds = Utilities.ConvertToList(WOV.VehicleCustomerLocationClassesIds);

            return to;
        }

        public static StateVehicleDTO WrapToStateVehicleDTO(this sp_NWC_GetAssignableVehicles_Result WOV)
        {
            return new StateVehicleDTO()
            {
                VehicleID = WOV.VehicleID,
                VehicleCodePlateNo = WOV.VehicleCodePlateNo,
                VehicleStatusId = WOV.VehicleStatusId,
                VehicleStatusName = WOV.VehicleStatusName,
                VehicleStatusNameAr = WOV.VehicleStatusNameAr,
                DriverID = WOV.DriverID,
                DriverName = WOV.DriverName,
                DriverMobileNumber = WOV.DriverMobileNumber,
                DriverCode = WOV.DriverCode,
                IsAvailable = WOV.VehicleStatusId == (int)VehicleStatusEnum.Available,
                IsOutOfService = WOV.VehicleStatusId == (int)VehicleStatusEnum.OutOfService,
                IsBlacklisted = WOV.VehicleStatusId == (int)VehicleStatusEnum.Blacklisted,
                IsParking = WOV.VehicleStatusId == (int)VehicleStatusEnum.Parking,
                Capacity = WOV.Capacity,
                ZoneID = WOV.ZoneID
            };
        }

        public static PrintCustomerInvoice WrapToPrintCustomerInvoiceDTO(this vw_NWC_PrintCustomerInvoice PCI)
        {
            return new PrintCustomerInvoice()
            {
                CustomerName = PCI.CustomerName,
                CustomerMobile = PCI.CustomerMobile,
                CustomerAddress = PCI.CustomerAddress,
                OrderNumber = PCI.OrderNumber,
                OrderQuantity = PCI.OrderQuantity,
                TotalCost = PCI.TotalCost,
                ScheduledDeliveryTime = PCI.ScheduledDeliveryTime,
                EncryptedConfirmationCode = PCI.EncryptedConfirmationCode,
                ZoneName = PCI.NWC_ZoneName,
                StationName = PCI.StationName,
                ContractorFullName = PCI.ContractorFullName,
                TaxNumber = PCI.TaxNumber,
                CompanyAddress = PCI.CompanyAddress,
                assignedStation = PCI.assignedStationID,
                plateNo = PCI.plateNo,
                VehicleID = PCI.VehicleID,
                NetCost = PCI.NetCost,
                InvoiceNo = PCI.InvoiceNo,
                TransporterCode = PCI.TransporterCode,
                Category = LanguageIsEnglish ? PCI.CategoryEn : PCI.CategoryAr,
                CategoryName = LanguageIsEnglish ? PCI.CategoryEn : PCI.CategoryAr,
                CategoryID = PCI.CategoryID
                //SubContractorFullName = PCI.SubContractorFullName,
                //SubContTaxNumber = PCI.SubContTaxNumber,
                //SubContCompanyAddress = PCI.SubContCompanyAddress
            };
        }

        public static PrintDriverInvoice WrapToPrintDriverInvoiceDTO(this vw_NWC_PrintDriverInvoice PDI)
        {
            return new PrintDriverInvoice()
            {
                CustomerName = PDI.CustomerName,
                CustomerMobile = PDI.CustomerMobile,
                CustomerAddress = PDI.CustomerAddress,
                CustomerCode = PDI.CustomerCode,
                OrderNumber = PDI.OrderNumber,
                OrderQuantity = PDI.OrderQuantity,
                TotalCost = PDI.TotalCost,
                ScheduledDeliveryTime = PDI.ScheduledDeliveryTime,
                EncryptedConfirmationCode = PDI.EncryptedConfirmationCode,
                ZoneName = PDI.NWC_ZoneName,
                ContractorFullName = PDI.ContractorFullName,
                TaxNumber = PDI.TaxNumber,
                CompanyAddress = PDI.CompanyAddress,
                StationName = PDI.StationName,
                CustomerLocationClassName = LanguageIsEnglish ? PDI.CustomerLocationClassNameEn : PDI.CustomerLocationClassNameAr,
                plateNo = PDI.plateNo,
                VehicleID = PDI.VehicleID,
                NetCost = PDI.NetCost ,
                InvoiceNo = PDI.InvoiceNo,
                TransporterCode = PDI.TransporterCode,
                Category = LanguageIsEnglish ? PDI.CategoryEn : PDI.CategoryAr,
                CategoryName = LanguageIsEnglish ? PDI.CategoryEn : PDI.CategoryAr,
                CategoryID = PDI.CategoryID,
                //SubContractorFullName = PDI.SubContractorFullName,
                //SubContTaxNumber = PDI.SubContTaxNumber,
                //SubContCompanyAddress = PDI.SubContCompanyAddress
            };
        }

        public static PrintVehicleInvoice WrapToPrintVehicleInvoiceDTO(this vw_NWC_PrintVehicleInvoice from)
        {
            return new PrintVehicleInvoice()
            {
                VehicleId = from.VehicleId,
                VehicleCode = from.VehicleCode,
                VehiclePlateNo = from.VehiclePlateNo,
                VehicleCapacity = from.VehicleCapacity,
                StationId = from.StationId,
                StationName =  from.StationName,
                ContractCode = from.ContractCode,
                ContractorID = from.ContractorID,
                ContractorCode = from.ContractorCode,
                ContractorFullName = from.ContractorFullName,
                ContractorTaxNumber = from.ContractorTaxNumber,
                ContractorAddress = from.ContractorAddress,
                CustomerLocationClassID = from.CustomerLocationClassID,
                //SubContractorFullName = from.SubContractorFullName,
                //SubContTaxNumber = from.SubContTaxNumber,
                //SubContCompanyAddress = from.SubContCompanyAddress
            };
        }


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
