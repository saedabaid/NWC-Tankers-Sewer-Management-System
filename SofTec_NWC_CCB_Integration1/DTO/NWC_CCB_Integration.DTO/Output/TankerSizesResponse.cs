using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Output
{
    [ServiceContract]
    public class TankerSizesResponse
    {
        public TankerSizesResponse()
        {
            TankerSizes = new List<TankerSize>();
        }

        [DataMember]
        public List<TankerSize> TankerSizes { get; set; }
    }
}
