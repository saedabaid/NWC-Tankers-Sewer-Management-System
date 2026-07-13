using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Common
{
    [ServiceContract]
    public class DescriptiveResponse<T> : DescriptiveObject
    {
        [DataMember]
        public T Value { get; set; }

        [OperationContract]
        public static DescriptiveResponse<T> Success(T value)
        {
            return new DescriptiveResponse<T>
            {
                Value = value,
                ResponseCode = 200,
            };
        }
        [OperationContract]
        public static DescriptiveResponse<T> Success(string customDescription, T value)
        {
            return new DescriptiveResponse<T>
            {
                Value = value,
                ResponseCode = 200,
                ResponseDescription = customDescription
            };
        }
        [OperationContract]
        public static new DescriptiveResponse<T> Error(ErrorStatus status = ErrorStatus.UNEXPECTED_ERROR)
        {
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,
                ErrorMetadata = status,
                ErrorDescription = status.ToString()
            };
        }
     

        [OperationContract]
        public static new DescriptiveResponse<T> Error(string customDescription, ErrorStatus metadata = ErrorStatus.CUSTOM)
        {
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,
                ResponseCode = ((int)metadata),
                ErrorDescription = customDescription
            };
        }

        [OperationContract]
        public DescriptiveResponse<TTarget> ToGeneric<TTarget>(TTarget newValue = default(TTarget))
        {
            return new DescriptiveResponse<TTarget>
            {
                Value = newValue,
                IsErrorState = this.IsErrorState,
                ErrorDescription = this.ErrorDescription,
                ErrorMetadata = this.ErrorMetadata
            };
        }

        [OperationContract]
        public static DescriptiveResponse<T> Try(Func<DescriptiveResponse<T>> function)
        {
            DescriptiveResponse<T> response = null;
            try
            {
                response = function.Invoke();
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
            return response;
        }

        [OperationContract]
        public static DescriptiveResponse<T> Error(IEnumerable<String> errors)
        {
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,
                Errors = errors.ToList()
            };
        }
    }
}
