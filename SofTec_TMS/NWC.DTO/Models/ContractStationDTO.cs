using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
     public class ContractStationDTO
    {
        public List<Guid> AreaIDs { get; set; }
        public List<Guid> CityIDs { get; set; }
        public List<Guid> StationIDs { get; set; }
        public ContactPersonDTO ContactPerson { get; set; }
        public long ContractID { set; get; }
        public long ContractStationID { get; set; }
        public int index { get; set; }
    }
}
