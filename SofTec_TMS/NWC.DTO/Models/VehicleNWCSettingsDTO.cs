using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class VehicleNWCSettingsDTO
    {
        public VehicleNWCSettingsDTO()
        {
            this.AccessoryIDs = new List<int>();
            this.CustLocationClassIDs = new List<int>();
        }

        public Guid VehicleID { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public Guid SubID { get; set; }
        public int? ServiceTypeID { get; set; }
        public System.Guid? StationID { get; set; }
        public string StationName { get; set; }
        public string CodePlateNumber { get; set; }

        public Nullable<int> Capacity { get; set; }
        public Nullable<System.Guid> transporterTypeId { get; set; }
        public string transporterTypeNameEn { get; set; }
        public string transporterTypeNameAr { get; set; }


        public List<int> AccessoryIDs { get; set; }
        public List<int> CustLocationClassIDs { get; set; }
    }
}
