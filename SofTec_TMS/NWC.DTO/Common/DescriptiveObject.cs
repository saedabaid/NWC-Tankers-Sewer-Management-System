using NWC.DTO.Enums;
using System.Collections.Generic;

namespace NWC.DTO.Common
{
    public class DescriptiveObject
    {
        public string CoreValue { get; set; }

        public bool IsErrorState { get; set; }

        public string ErrorDescription { get; set; }
        public string ResponseDescription { get; set; }

        public object ErrorMetadata { get; set; }

        public object ResponseCode { get; set; }

        public List<string> Errors { get; set; }

        public static DescriptiveObject Error(ErrorStatus status)
        {
            return new DescriptiveObject
            {
                IsErrorState = true,
                ErrorMetadata = status,
                ErrorDescription = status.ToString()
            };
        }

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
