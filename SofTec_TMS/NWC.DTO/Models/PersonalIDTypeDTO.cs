using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class PersonalIDTypeDTO
    {
        // NWC_PersonalIDType
        public int ID { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public System.Guid SubID { get; set; }
    }
}
