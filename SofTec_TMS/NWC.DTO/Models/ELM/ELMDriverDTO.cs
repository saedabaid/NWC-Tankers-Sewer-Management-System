using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models.ELM
{
    public class ELMDriverDTO
    {
        public string DriverFullNameAR { get; set; }   //plateNo_Characters
        public string DriverIDType { get; set; }   //plateNo_Characters
        public string DriverIDValue { get; set; }   //plateNo_Characters
        public string DriverMobileNumber { get; set; }   //plateNo_Characters
        public string DriverDrivingLicenseNumber { get; set; }   //plateNo_Characters
        public string DriverDrivingLicenseExpiryDate { get; set; }   //plateNo_Characters
    }
}
