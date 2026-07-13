using Infrastructure;
using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class TransporterSC
    {
        public PageFilter PageFilter { get; set; }
        public enum OrderByExepression { ContarctorName = 1, Code = 2, CommericalID = 3, MOI = 4, TaxNumber = 5, ContactPerson = 6 }
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }

        public string PlateNoOrCode { get; set; }

        public List<Guid> BrandIDs { get; set; }
        public List<string> ModelIDs { get; set; }
        public List<Guid> YearIDs { get; set; }
        public List<Guid> VehicleTypeIDs { get; set; }
        public List<string> GroupIDs { get; set; }
        public List<int> StatusIDs { get; set; }
        public List<Guid> BranchIDs { get; set; }
        public List<Guid> SubBranchIDs { get; set; }
        public List<Guid> LandmarkIDs { get; set; }

        public DateTime? NextExaminationDate { get; set; }
        public DateTime? LicenceExpirationDate { get; set; }
        public DateTime? InsuranceEndDate { get; set; }
        public DateTime? EntranceDate { get; set; }
        public string ChassisNo { get; set; }
        public string SIMCardNo { get; set; }
        public string DeviceCode { get; set; }

        public bool ExcelFlage { get; set; }
    }
}
