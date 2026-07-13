using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models.TMS
{
    public class EditTransporterStatusDTO
    {
        public Guid TransporterID { get; set; }
        public int VehicleStatusID { get; set; }
    }
}
