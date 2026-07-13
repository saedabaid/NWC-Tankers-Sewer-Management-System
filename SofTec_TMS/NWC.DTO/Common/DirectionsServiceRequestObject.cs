using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Common
{
    public class DirectionsServiceRequestObject
    {
        public string source { get; set; }
        public string destination { get; set; }
        public string waypoints { get; set; }
        public string mode { get; set; }
        public string language { get; set; }
    }
}
