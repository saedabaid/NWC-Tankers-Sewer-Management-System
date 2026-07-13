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
    public class WorkOrderResponse
    {
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int ResponseCode { get; set; }
        [DataMember]
        public string ResponseDescription { get; set; }
        [DataMember]
        public int? ErrorCode { get; set; }
        [DataMember]
        public string ErrorDescription { get; set; }
    }
}
