using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models.TMS
{
    public class StaffListDTO
    {
        public Guid Id { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string Allocation { get; set; }
        public string StaffRole { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? VehicleReceivingDate { get; set; }
        public DateTime? OwnershipDate { get; set; }
    }
}
