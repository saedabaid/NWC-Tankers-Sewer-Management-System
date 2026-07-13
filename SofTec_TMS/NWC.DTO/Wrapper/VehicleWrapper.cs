using NWC.DAL.NWCEntities;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class VehicleWrapper
    {
        public static VehicleNWCSettingsDTO WrapToVehicleNWCSettingsDTO(this vw_NWC_VehicleNWCSettings input)
        {
            var vehicleAccessoryIDs = new List<int>();
            if (input.ListofAccessory != null && input.ListofAccessory.Any())
            {
                var accessoryIDs = input.ListofAccessory.Split(',');
                foreach (var strID in accessoryIDs)
                {
                    int id;
                    if (int.TryParse(strID, out id))
                        vehicleAccessoryIDs.Add(id);
                }
            }

            var VehicleClassIDs = new List<int>();
            if (input.ListofClass != null && input.ListofClass.Any())
            {
                var classIDs = input.ListofClass.Split(',');
                foreach (var strID in classIDs)
                {
                    int id;
                    if (int.TryParse(strID, out id))
                        VehicleClassIDs.Add(id);
                }
            }            

            return new VehicleNWCSettingsDTO()
            {
                VehicleID = input.Id,
                SubID = input.SubID,
                Status = input.status.Value,
                AccessoryIDs = vehicleAccessoryIDs, 
                CustLocationClassIDs = VehicleClassIDs,
                Code = input.Code,
                ServiceTypeID= input.ServiceTypeID,
                StationID = input.StationID,
                StationName = input.name,
                CodePlateNumber = $"{input.Code} | {input.plateNo}",
                Capacity = input.Capacity,
                transporterTypeNameEn = input.transporterTypeName,
                transporterTypeNameAr = input.transporterTypeNameAr,
                transporterTypeId = input.transporterType
            };
        }

        public static AvailableTankerSizesDTO WrapToAvailableTankerSizesDTO(this vw_NWC_AvailableTankerSizes input)
        {
            return new AvailableTankerSizesDTO()
            {
                TankerSize = input.Capacity.HasValue ? input.Capacity.Value : 0,
                StationID = input.Landmark.Value, 
                TankerPrice = 0, 
                ShowPrice = input.ShowPrice.HasValue ? input.ShowPrice.Value : false
            };
        }

        public static VehicleLogsDTO WrapToVehicleLogsDTO(this vw_NWC_Report_VehicleLog input)
        {
            return new VehicleLogsDTO
            {
                ID = input.ID,
                ActionLogTypeID = input.ActionLogTypeID,
                LogTypeAr = input.LogTypeAr,
                LogTypeEn = input.LogTypeEn,
                VehicleID = input.VehicleID,
                VehicleCode = input.VehicleCode,
                VehiclePlateNo = input.VehiclePlateNo,
                VehicleStationId = input.VehicleStationId,
                VehicleStationName = input.VehicleStationName,
                VehicleCapacity = input.VehicleCapacity,
                VehicleServiceTypeID = input.VehicleServiceTypeID,
                VehicleServiceTypeEN = input.VehicleServiceTypeEN,
                VehicleServiceTypeAR = input.VehicleServiceTypeAR,
                DriverID = input.DriverID,
                DriverName = input.DriverName,
                DriverMobile = input.DriverMobile,
                CreateTime = input.CreateTime,
                StatusID = input.StatusID,
                WorkOrderID = input.WorkOrderID,
                OrderNumber = input.OrderNumber,
                CustomerClassesAr = input.CustomerClassesAr,
                CustomerClassesEn = input.CustomerClassesEn
            };
        }


        public static VehicleDataDTO WrapToVehicleDataDTO(this vw_NWC_Report_VehicleData input)
        {
            var to = new VehicleDataDTO
            {
                VehicleId = input.VehicleId,
                Code = input.Code,
                PlateNo = input.PlateNo,
                Capacity = input.Capacity,
                PermissionExpiryDate = input.PermissionExpiryDate,
                chassisNo = input.chassisNo,
                licenseExpiryDate = input.licenseExpiryDate,
                CityId = input.CityId,
                CityName = input.CityName,
                AreaId = input.AreaId,
                AreaName = input.AreaName,
                StationId = input.StationId,
                StationName = input.StationName,
                DriverId = input.DriverId,
                DriverMobile = input.DriverMobile,
                DriverCode = input.DriverCode,
                DriverPersonalId = input.DriverPersonalId,
                StatusId = input.StatusId,
                StatusName = input.StatusName,
                StatusNameAr = input.StatusNameAr,
                StatusColorCode = input.StatusColorCode,
                ManufacturerId = input.ManufacturerId,
                ManufacturerName = input.ManufacturerName,
                ManufacturerNameAr = input.ManufacturerNameAr,
                BrandId = input.BrandId,
                BrandName = input.BrandName,
                BrandNameAr = input.BrandNameAr,
                ProductionYearId = input.ProductionYearId,
                ProductionYearName = input.ProductionYearName,
                TypeId = input.TypeId,
                TypeName = input.TypeName,
                TypeNameAr = input.TypeNameAr,
                ServiceTypeID = input.ServiceTypeID,
                ServiceTypeEN = input.ServiceTypeEN,
                ServiceTypeAR = input.ServiceTypeAR
            };

            to.DriverName = $"{input.DriverFirstName ?? string.Empty} {input.DriverMiddleName ?? string.Empty} {input.DriverLastName ?? string.Empty}";

            if (!string.IsNullOrEmpty(input.ClassesArEn))
            {
                var classes = input.ClassesArEn.Split('_');
                to.ClassesAr = classes[0] ?? string.Empty;
                to.ClassesEn = classes[1] ?? string.Empty;
            }

            return to;
        }

    }
}
