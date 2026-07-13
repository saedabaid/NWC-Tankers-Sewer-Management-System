using NWC.DAL.NWCEntities;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NWC.DTO.Wrapper
{
    public static class TransporterWrapper
    {
        private static bool LanguageIsEnglish
        {
            get => Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
        }
        private static Func<Transporter_Staff, bool> DriverPredicate = (transporterStaff) => transporterStaff?.Staff1?.StaffRoles?.category == 6;

        public static TransporterDTO WrapToTransporterDTO(this Transporter input)
        {
            if (input == null)
                return null;
            var drivers = input.Transporter_Staff.Where(DriverPredicate)?.Select(d => d.Staff1);
            var allocation = new List<string> { input.Branch1?.name, input.Branch1?.Branch2?.name };
            var dto = new TransporterDTO
            {
                Id = input.ID,
                LicenseExpiryDate = input.licenseExpiryDate,
                InsuranceStartDate = input.insuranceStartDate,
                Code = !string.IsNullOrEmpty(input.code) ? input.code : input.ID.ToString(),
                PlateNo = input.plateNo,
                ChassisNo = input.chassisNo,
                Group = "Group",
                Allocation = string.Join("، ", allocation.ToArray()),
                TransporterType = input.transporterType,
                TransporterTypeName = LanguageIsEnglish ? input.TransporterType1?.name : input.TransporterType1?.NameAr,
                Status = input.status,
                StatusName = LanguageIsEnglish ? input.TransporterStatus?.Name : input.TransporterStatus?.NameAr,
                DeviceCode = input.deviceCode,
                SIMCardNo = input.SIMCardNo,
                Drivers = drivers.Select(d => d?.ID).Distinct(),
                DriverNames = string.Join("، ", drivers.Select(d => d != null ? d.FirstName + " " + d.MiddleName + " " + d.LastName : "").Distinct().ToArray()),
                Staff = input.Transporter_Staff.Select(x => new StaffListDTO
                {
                    Id = x.Staff1.ID,
                    StaffName = x.Staff1.FirstName + " " + x.Staff1.MiddleName + " " + x.Staff1.LastName,
                    MobileNumber = x.Staff1.mobileNumber,
                    StaffRole = x.Staff1.StaffRoleName,
                    StaffCode = x.Staff1.code,
                    VehicleReceivingDate = x.VehicleReceivingDate,
                    OwnershipDate = x.OwnershipDate
                }),
                Image = input.image,
                ProductionYear = input.TransporterProductionYear1?.ID,
                ProductionYearName = input.TransporterProductionYear1?.name,
                Branch = input.Branch1?.Branch2?.Id,
                BranchName = input.Branch1?.Branch2?.name,
                SubBranch = input.Branch1?.Id,
                SubBranchName = input.Branch1?.name,
                Landmark = input.landmark,
                LandmarkName = input.Landmark1?.name,
                Model = input.TransporterManufacturer1?.ID,
                ModelName = LanguageIsEnglish ? input.TransporterManufacturer1?.name : input.TransporterManufacturer1?.NameAr,
                Brand = input.TransporterBrand1 != null ? input.TransporterBrand1.ID : Guid.Empty,

                BrandName = input.TransporterBrand1 != null && LanguageIsEnglish ? input.TransporterBrand1?.name : input.TransporterBrand1?.NameAr,
                CurrentMileage = input.currentMileage,
                InsuranceNo = input.insuranceNo,
                Capacity = input.Capacity
            };
            return dto;
        }
        public static VehicleTypeDTO WrapToTransporterTypeDTO(this TransporterType input)
        {
            if (input == null)
                return null;
            var dto = new VehicleTypeDTO
            {
                ID = input.ID,
                Description = input.descr,
                Name = input.name,
                Category = input.Category,
                Icon = input.icon,
            };
            return dto;

        }
        public static Transporter WrapDtoToTransporter(this TransporterDTO input) => input.WrapDtoToTransporter(new Transporter());

        public static Transporter WrapDtoToTransporter(this TransporterDTO input, Transporter entity)
        {
            if (input == null)
                return null;
            entity.ID = input.Id.HasValue ? input.Id.Value : Guid.NewGuid();
            entity.code = !string.IsNullOrEmpty(input.Code) ? input.Code : entity.ID.ToString();
            entity.plateNo = input.PlateNo;
            entity.chassisNo = input.ChassisNo;
            entity.transporterType = input.TransporterType;
            entity.transporterTypeName = input.TransporterTypeName;
            entity.status = input.Status;
            entity.deviceCode = input.DeviceCode;
            entity.SIMCardNo = input.SIMCardNo;
            entity.transporterProductionYear = input.ProductionYear;
            entity.branch = input.SubBranch;
            entity.landmark = input.Landmark;
            entity.Capacity = input.Capacity;
            entity.transporterBrand = input.Brand;
            entity.transporterManufacturer = input.Model;
            entity.currentMileage = input.CurrentMileage;
            return entity;
        }

        public static Transporter WrapExcelDtoToTransporter(this TransporterExcelDTO input, Transporter entity)
        {
            if (input == null)
                return null;
            entity.ID = Guid.NewGuid();
            entity.code = !string.IsNullOrEmpty(input.Code) ? input.Code : entity.ID.ToString();
            entity.plateNo = input.PlateNo;
            entity.chassisNo = input.ChassisNo;
            entity.transporterTypeName = input.TransporterType;
            entity.deviceCode = input.DeviceCode;
            entity.SIMCardNo = input.SIMCardNo;
            entity.color = input.Color;
            entity.engineNo = input.EngineNumber;
            entity.licenseNo = input.LicenseNumber;
            entity.insuredBy = input.InsuredBy;
            entity.insuranceNo = input.InsuranceNumber;
            entity.SupplierName = input.Supplier;
            entity.providerName = input.Provider;
            try
            { entity.status = int.Parse(input.Status); }
            catch { }
            try
            { entity.Capacity = int.Parse(input.Capacity); }
            catch { }
            try
            { entity.literPerKm = decimal.Parse(input.FuelLitres); }
            catch { }
            try
            { entity.maxSpeed = double.Parse(input.SpeedLimit); }
            catch { }
            try
            { entity.CapacityUnit = int.Parse(input.CapacityUnit); }
            catch { }
            try
            { entity.TransporterLicenseType = long.Parse(input.LicenseType); }
            catch { }
            try
            { entity.currentMileage = double.Parse(input.CurrentMileage); }
            catch { }
            try
            { entity.HourRate = double.Parse(input.HourRate); }
            catch { }
            try
            { entity.insuranceStartDate = DateTime.Parse(input.InsuranceStartDate); }
            catch { }
            try
            { entity.insuranceEndDate = DateTime.Parse(input.InsuranceEndDate); }
            catch { }
            try
            { entity.FuelCost = decimal.Parse(input.FuelCost); }
            catch { }
            try
            { entity.TempSensorMaxValue = decimal.Parse(input.TemperatureSensorMaxValue); }
            catch { }
            try
            { entity.TempSensorMinValue = decimal.Parse(input.TemperatureSensorMinValue); }
            catch { }
            try
            { entity.licenseExpiryDate = DateTime.Parse(input.LicenseExpiryDate); }
            catch { }
            try
            { entity.ServiceTypeID = int.Parse(input.ServiceTypeID); }
            catch { }
            try
            { entity.NextExaminationDate = DateTime.Parse(input.NextExaminationDate); }
            catch { }
            return entity;
        }

        public static VehiclePerformanceDTO WrapToVehiclePerformanceDTO(this sp_NWC_Report_VehiclePerformance_Result input)
        {
            if (input == null)
                return null;
            var dto = new VehiclePerformanceDTO
            {
                Code = input.Code,
                PlateNumber = input.PlateNumber,
                Status = input.Status,
                StatusName = LanguageIsEnglish ? input.EN_StatusName : input.AR_StatusName,
                DriverNames = input.DriverNames,
                BranchId = input.BranchId,
                BranchName = input.BranchName,
                SubBranchId = input.SubBranchId,
                SubBranchName = input.SubBranchName,
                StationId = input.StationId,
                StationName = input.StationName,
                Capacity = input.Capacity,
                TotalExitTripsCount = input.TotalExitTripsCount,
                TotalDeliveredOrdersCount = input.TotalDeliveredOrdersCount,
                ResidentialDeliveredOrdersCount = input.ResidentialDeliveredOrdersCount,
                CommercialDeliveredOrdersCount = input.CommercialDeliveredOrdersCount,
                ViolationsCount = input.ViolationsCount,
                PaidViolationsCount = input.PaidViolationsCount,
                UnPaidViolationsCount = input.UnPaidViolationsCount,
                PartiallyPaidViolationsCount = input.PartiallyPaidViolationsCount,
                ClosedByCodeOrdersCount = input.ClosedByCodeOrdersCount,
                ClosedByCodeOrdersPercentage = input.ClosedByCodeOrdersPercentage
            };
            return dto;
        }
    }
}
