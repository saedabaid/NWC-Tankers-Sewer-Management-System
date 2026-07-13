using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{

    public class VehicleDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string PlateNo { get; set; }
        public string ChassisNo { get; set; }
        public DateTime InsuranceStartDate { get; set; }
        public DateTime InsuranceEndDate { get; set; }
        public string OwnerIDValue { get; set; }
        public string OwnerFullNameAR { get; set; }
        public DateTime LicenseExpiryDate { get; set; }

    }

}
