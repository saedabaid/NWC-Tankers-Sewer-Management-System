using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class TransporterExcelDTO
    {
        public string PlateNo { get; set; }
        public string Branch { get; set; }
        public string SubBranch { get; set; }
        public string Code { get; set; }
        public string ChassisNo { get; set; }
        public string Landmark { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string ProductionYear { get; set; }
        public string Color { get; set; }
        public string TransporterType { get; set; }
        public string FuelLitres { get; set; }
        public string SpeedLimit { get; set; }
        public string Capacity { get; set; }
        public string CapacityUnit { get; set; }
        public string LicenseType { get; set; }
        public string DriverCode { get; set; }
        public string CurrentMileage { get; set; }
        public string HourRate { get; set; }
        public string EngineNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string InsuredBy { get; set; }
        public string InsuranceNumber { get; set; }
        public string InsuranceStartDate { get; set; }
        public string InsuranceEndDate { get; set; }
        public string Supplier { get; set; }
        public string FuelCost { get; set; }
        public string TemperatureSensorMaxValue { get; set; }
        public string TemperatureSensorMinValue { get; set; }
        public string DeviceCode { get; set; }
        public string SIMCardNo { get; set; }
        public string Provider { get; set; }
        public string ProviderName { get; set; }
        public string LicenseExpiryDate { get; set; }
        public string ServiceTypeID { get; set; }
        public string Status { get; set; }
        public string NextExaminationDate { get; set; }
        public long? ExcelSheetRowId { get; set; }
        public string ExcelValidation { get; set; }
    }
}
