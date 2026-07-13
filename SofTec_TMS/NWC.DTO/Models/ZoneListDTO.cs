using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class ZoneListDTO
    {
        //vw_NWC_ContractStations
        public long Id { get; set; }
        public System.Guid CityID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public System.Guid SubID { get; set; }
        public string CityName { get; set; }
        public string MainStationName { get; set; }
        public string MainStationCode { get; set; }
        public Nullable<System.Guid> MainStationId { get; set; }
        public Nullable<int> RestrictedZoneVehicleTypes { get; set; }
        public Nullable<int> CountOfBackupStations { get; set; }
    }
}
