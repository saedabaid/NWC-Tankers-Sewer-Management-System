using NWC.DAL.NWCEntities;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class ReportWrapper
    {
        public static Report_TankersPermissionsStatus WrapReport_OrderPerZone(this vw_NWC_Report_TankersPermissionsStatus from)
        {
            if (from == null) return null;

            var to = new Report_TankersPermissionsStatus
            {
                Code = from.Code,
                PlateNo = from.PlateNo,
                licenseNo = from.licenseNo,
                StationID = from.StationID,
                StationName = from.StationName,
                CityID = from.CityID,
                CityName = from.CityName,
                licenseExpiryDate = from.licenseExpiryDate,
                PermissionExpiryDate = from.PermissionExpiryDate
            };

            if (from.licenseExpiryDate.HasValue)
            {
                to.LicenseStatus = from.licenseExpiryDate.Value >= DateTime.Today ? CustomConstants.Valid : CustomConstants.Expired;
            }
            if (from.PermissionExpiryDate.HasValue)
            {
                to.PermissionStatus = from.PermissionExpiryDate.Value >= DateTime.Today ? CustomConstants .Valid: CustomConstants.Expired;
            }
            return to;
        }

    }
}
