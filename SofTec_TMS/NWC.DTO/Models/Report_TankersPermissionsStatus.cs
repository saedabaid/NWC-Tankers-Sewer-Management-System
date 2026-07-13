using System;

namespace NWC.DTO.Models
{
    public class Report_TankersPermissionsStatus
    {
        public System.Guid ID { get; set; }
        public string Code { get; set; }
        public string PlateNo { get; set; }
        public string licenseNo { get; set; }
        public System.Guid StationID { get; set; }
        public string StationName { get; set; }
        public System.Guid CityID { get; set; }
        public string CityName { get; set; }
        public Nullable<System.DateTime> licenseExpiryDate { get; set; }
        public Nullable<System.DateTime> PermissionExpiryDate { get; set; }

        public string PermissionStatus { get; set; }
        public string LicenseStatus { get; set; }

    }
}
