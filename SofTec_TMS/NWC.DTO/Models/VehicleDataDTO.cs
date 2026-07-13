using System;

namespace NWC.DTO.Models
{
    public class VehicleDataDTO
    {
        public System.Guid VehicleId { get; set; }
        public string Code { get; set; }
        public string PlateNo { get; set; }
        public Nullable<int> Capacity { get; set; }
        public Nullable<System.DateTime> PermissionExpiryDate { get; set; }
        public string chassisNo { get; set; }
        public Nullable<System.DateTime> licenseExpiryDate { get; set; }
        public Nullable<System.Guid> CityId { get; set; }
        public string CityName { get; set; }
        public Nullable<System.Guid> AreaId { get; set; }
        public string AreaName { get; set; }
        public Nullable<System.Guid> StationId { get; set; }
        public string StationName { get; set; }
        public Nullable<System.Guid> DriverId { get; set; }
        public string DriverName { get; set; }
        public string DriverMobile { get; set; }
        public string DriverCode { get; set; }
        public string DriverPersonalId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusColorCode { get; set; }
        public Nullable<System.Guid> ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerNameAr { get; set; }
        public Nullable<System.Guid> BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandNameAr { get; set; }
        public Nullable<System.Guid> ProductionYearId { get; set; }
        public string ProductionYearName { get; set; }
        public Nullable<System.Guid> TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeNameAr { get; set; }
        public Nullable<int> ServiceTypeID { get; set; }
        public string ServiceTypeEN { get; set; }
        public string ServiceTypeAR { get; set; }
        public string ClassesAr { get; set; }
        public string ClassesEn { get; set; }
    }
}
