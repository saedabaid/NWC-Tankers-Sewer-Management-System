using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class ZonePriceListDTO
    {
        public string Zone { get; set; }
        public string Station { get; set; }
        public decimal PriceCharge { get; set; }
        public string CustomerClassName { get; set; }
        public System.Guid StationID { get; set; }
    }
}
