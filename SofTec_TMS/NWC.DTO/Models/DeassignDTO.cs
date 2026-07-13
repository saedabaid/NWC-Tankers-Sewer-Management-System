using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class DeassignDTO
    {
        public DispatchWorkOrderDTO request { set; get; }
        public EventWorkOrderDTO eventWorkOrderDTO { set; get; }
    }
}
