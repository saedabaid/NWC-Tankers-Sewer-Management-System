using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class VehiclePerformanceDTO
    {
        public string Code { get; set; }
        public string PlateNumber { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? Capacity { get; set; }
        public Guid? StationId { get; set; }
        public string StationName { get; set; }
        public Guid? SubBranchId { get; set; }
        public string SubBranchName { get; set; }
        public Guid? BranchId { get; set; }
        public string BranchName { get; set; }
        public string DriverNames { get; set; }
        public int? TotalExitTripsCount { get; set; }
        public int? TotalDeliveredOrdersCount { get; set; }
        public int? ResidentialDeliveredOrdersCount { get; set; }
        public int? CommercialDeliveredOrdersCount { get; set; }
        public int? ViolationsCount { get; set; }
        public int? PaidViolationsCount { get; set; }
        public int? UnPaidViolationsCount { get; set; }
        public int? PartiallyPaidViolationsCount { get; set; }
        public int ClosedByCodeOrdersCount { get; set; }
        public decimal ClosedByCodeOrdersPercentage { get; set; }
    }
}
