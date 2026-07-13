using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Output
{
    public class TankerAccessorize
    {
        [DataMember]
        public string AccessorizeId { get; set; }
        [DataMember]
        public float AccessorizePrice { get; set; }
    }
}
