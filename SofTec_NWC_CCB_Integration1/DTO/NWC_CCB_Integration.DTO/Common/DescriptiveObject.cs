using NWC_CCB_Integration.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Common
{
    public class DescriptiveObject
    {
        [DataMember]
        public string CoreValue { get; set; }

        [DataMember]
        public bool IsErrorState { get; set; }

        [DataMember]
        public string ErrorDescription { get; set; }

        [DataMember]
        public object ErrorMetadata { get; set; }

        [DataMember]
        public List<string> Errors { get; set; }
        [DataMember]
        public string ResponseDescription { get; set; }

        [DataMember]
        public int ResponseCode { get; set; }
        [OperationContract]
        public static DescriptiveObject Error(ErrorStatus status)
        {
            return new DescriptiveObject
            {
                IsErrorState = true,
                ErrorMetadata = status,
                ErrorDescription = status.ToString()
            };
        }


        [OperationContract]
        public static DescriptiveObject Error(string customDescription, ErrorStatus metadata = ErrorStatus.CUSTOM)
        {
            return new DescriptiveObject
            {
                IsErrorState = true,
                ErrorMetadata = metadata,
                ErrorDescription = customDescription
            };
        }
    }
}
