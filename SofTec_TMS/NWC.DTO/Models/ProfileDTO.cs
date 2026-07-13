using System;

namespace NWC.DTO.Models
{
    public class ProfileDTO
    {
        public DateTime licenseExpiryDate;
        public int licenseStatus;
        public string personalID;
        public string staffDTO;

        public Guid ID { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string MobileNumber { get; set; }
        public byte[] ProfileImage { get; set; }
        public Guid? Branch { get; set; }
        public string SubLogo { get; set; }
        public string StaffRoleName { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public Guid UserID { get; set; }
        public int RemaininglicenseExpiry { get; set; }
        public string StationName { get; set; }
        public string TankerNumber { get; set; }
    }
}
