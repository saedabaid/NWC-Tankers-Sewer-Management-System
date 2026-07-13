using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class ReportTankersPermissionsStatusSC
    {
        public PageFilter PageFilter { get; set; }

        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
        public String Tanker { get; set; }
        
        
        public ExpiryStatusEnum? PermissionStatus { get; set; }
        public ExpiryStatusEnum? LicenseStatus { get; set; }

        public DateTime? PermissionExpiryDateFrom { get; set; }
        public DateTime? PermissionExpiryDateTo { get; set; }
        public DateTime? LicenseExpiryDateFrom { get; set; }
        public DateTime? LicenseExpiryDateTo { get; set; }

        public bool ExcelFlage { get; set; }


        public enum ExpiryStatusEnum
        {
            Valid = 1,
            Expiry = 2
        }
    }

    

}
