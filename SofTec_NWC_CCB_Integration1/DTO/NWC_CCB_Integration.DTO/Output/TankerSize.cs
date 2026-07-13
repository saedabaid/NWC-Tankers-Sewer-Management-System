using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Output
{
    public class TankerSize
    {
        public TankerSize()
        {
            TankerAccessorizes = new List<TankerAccessorize>();
        }

        [DataMember]
        public int TankerSizeValue { get; set; }
        [DataMember]
        public float TankerPrice { get; set; }
        [DataMember]
        public DateTime TankerDeliveryDDTM { get; set; }
        [DataMember]
        public List<TankerAccessorize> TankerAccessorizes { get; set; }
    }
}
