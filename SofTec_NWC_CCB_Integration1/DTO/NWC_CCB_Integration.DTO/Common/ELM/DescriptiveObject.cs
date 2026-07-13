using NWC_CCB_Integration.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Common.ELM
{
    [DataContract]

    public class DescriptiveObject
    {
        [IgnoreDataMember]
        public string CoreValue { get; set; }

        [IgnoreDataMember]
        public bool IsErrorState { get; set; }

        [DataMember]
        public string ErrorDescription { get; set; }

        [IgnoreDataMember]
        public object ErrorMetadata { get; set; }

        [IgnoreDataMember]
        public List<string> Errors { get; set; }
        [DataMember]
        public string ResponseDescription { get; set; }

        [DataMember]
        public int ResponseCode { get; set; }

        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? TimeStamp = null;


        [OperationContract]
        public static DescriptiveObject Error(ErrorStatus status)
        {
            return new DescriptiveObject
            {
                Status = "OK",
                ResponseCode = (int)status,
                ResponseDescription = status.ToString(),
                ErrorCode = null,
                ErrorDescription = null
            };
        }


        [OperationContract]
        public static DescriptiveObject Error(string customDescription, ErrorStatus metadata = ErrorStatus.CUSTOM)
        {
            return new DescriptiveObject
            {
                Status = "OK",
                ResponseCode = ((int)metadata),
                ResponseDescription = customDescription,
                ErrorCode = null,
                ErrorDescription = null
            };
        }
    }

}
