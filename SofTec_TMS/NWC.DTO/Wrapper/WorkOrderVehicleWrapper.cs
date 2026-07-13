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
    public static class WorkOrderVehicleWrapper
    {
        public static StateWorkOrderVehicleDTO WrapToWorkOrderVehicleDTO(this vw_NWC_WorkOrderVehicle WOV)
        {
            var result = new StateWorkOrderVehicleDTO()
            {
                VehicleID = WOV.VehicleID,
                VehicleStatusId = WOV.VehicleStatusID,
                // VehicleStatusNameAr = WOV.VehicleStatusNameAr,
                //DriverName = WOV.DriverName,
                DriverMobileNumber = WOV.DriverMobileNumber,
                DriverCode = WOV.DriverCode,
                IsAssigned = WOV.VehicleStatusID == (int)VehicleStatusEnum.Assigned,
                IsInService = WOV.VehicleStatusID == (int)VehicleStatusEnum.InService,
                VehicleCode = WOV.TransporterCode,
                //VehicleCodePlateNo = $"{WOV.TransporterCode} | {WOV.TransporterPlateNo}",
                TotalCost = WOV.TotalCost,
                LastStatusTime = WOV.orderLastStatusTime,
                WorkOrderNumber = WOV.OrderNumber,
                WorkOrderID = WOV.WorkOrderID,
                LastStatusID = WOV.LastStatusID ,
                VehicleStatusColorCode = WOV.VehicleStatusColorCode,
                OrderCreateTime = WOV.orderCreateTime,
                Capacity = WOV.Capacity,
                CityId = WOV.CityId,
                CitySettings_ShowInvoice = WOV.CitySettings_ShowInvoice,
                CitySettings_ShowCustomerClassEntryGate = WOV.CitySettings_ShowCustomerClassEntryGate,
                PermitNumber = WOV.PermitNumber,
                PermitStatus = WOV.PermitStatus,
                IsHold = WOV.IsHold,
                Expirationdate = WOV.Expirationdate,
                //VehicleCustomerLocationClassId = WOV.VehicleCustomerLocationClassId
            };

            result.DriverName = $"{WOV.DriverFirstName ?? string.Empty} {WOV.DriverMiddleName ?? string.Empty} {WOV.DriverLastName ?? string.Empty}";
            result.VehicleCodePlateNo = $"{WOV.TransporterCode ?? string.Empty} | {WOV.TransporterPlateNo ?? string.Empty}";
            result.VehicleStatusName = LanguageIsEnglish ? WOV.VehicleStatusName : WOV.VehicleStatusNameAr;

            result.VehicleCustomerLocationClassesIds = Utilities.ConvertToList(WOV.VehicleCustomerLocationClassesIds);

            return result;
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
