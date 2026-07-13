using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class AvailableTankerSizesDTO
    {
        public int TankerSize { get; set; }
        public decimal? TankerPrice { get; set; }
        public DateTime TankerDeliveryTime { get; set; }
        public List<TankerAccessorizeDTO> TankerAccessorizeDTOs { get; set; }
    }
}
