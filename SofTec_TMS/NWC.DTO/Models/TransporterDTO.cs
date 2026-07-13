using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class TransporterDTO
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public string PlateNo { get; set; }
        public string ChassisNo { get; set; }
        public string Group { get; set; }
        public string Allocation { get; set; }
        public Guid? TransporterType { get; set; }
        public string TransporterTypeName { get; set; }
        public IEnumerable<Guid?> Drivers { get; set; }
        public string DriverNames { get; set; }
        public IEnumerable<StaffListDTO> Staff { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public string DeviceCode { get; set; }
        public string SIMCardNo { get; set; }
        public string Image { get; set; }
        public Guid? ProductionYear { get; set; }
        public string ProductionYearName { get; set; }
        public Guid? Branch { get; set; }
        public string BranchName { get; set; }
        public Guid? SubBranch { get; set; }
        public string SubBranchName { get; set; }
        public Guid? Landmark { get; set; }
        public string LandmarkName { get; set; }
        public Guid? Model { get; set; }
        public string ModelName { get; set; }
        public Guid? Brand { get; set; }
        public string BrandName { get; set; }
        public double? CurrentMileage { get; set; }
        public string InsuranceNo { get; set; }
        public int? Capacity { get; set; }
        public Nullable<System.DateTime> LicenseExpiryDate { get; set; }
        public Nullable<System.DateTime> InsuranceStartDate { get; set; }

    }

}
