using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class AvailableTankerSizesDTO
    {
        public int TankerSize { get; set; }
        public decimal? TankerPrice { get; set; }
        public DateTime TankerDeliveryTime { get; set; }
        public Guid StationID { get; set; }
        public bool ShowPrice { get; set; }
    }
}
