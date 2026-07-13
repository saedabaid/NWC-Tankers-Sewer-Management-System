using NWC.DTO.Enums;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NWC.DTO.Common
{
    public class DescriptiveResponse<T> : DescriptiveObject
    {
        public T Value { get; set; }
   

        public static DescriptiveResponse<T> Success(T value)
        {
            return new DescriptiveResponse<T>
            {
                Value = value,
                ResponseCode = 200
            };
        }
        public static DescriptiveResponse<T> Success(string customDescription, T value)
        {
            return new DescriptiveResponse<T>
            {
                Value = value,
                ResponseCode = 200,
                ResponseDescription = customDescription
            };
        }
        public static new DescriptiveResponse<T> Error(ErrorStatus status = ErrorStatus.UNEXPECTED_ERROR)
        {
             
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,               
                ErrorMetadata = status,
                ErrorDescription = status.ToString(),
                ResponseCode = status
            };
        }
        public static new DescriptiveResponse<T> Error(string customDescription, ErrorStatus status = ErrorStatus.CUSTOM)
        {
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,
                ErrorMetadata = status,
                ResponseCode = status,
                ErrorDescription = customDescription
            };
        }

        public static DescriptiveResponse<T> Error(IEnumerable<String> errors)
        {
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,
                Errors = errors.ToList()
            };
        }

        public DescriptiveResponse<TTarget> ToGeneric<TTarget>(TTarget newValue = default(TTarget))
        {
            return new DescriptiveResponse<TTarget>
            {
                Value = newValue,
                IsErrorState = this.IsErrorState,
                ErrorDescription = this.ErrorDescription,
                ErrorMetadata = this.ErrorMetadata,
                ResponseCode=this.ResponseCode,
                ResponseDescription = this.ResponseDescription

            };
        }

        public static DescriptiveResponse<T> Try(Func<DescriptiveResponse<T>> function)
        {
            DescriptiveResponse<T> response = null;
            try
            {
                response = function.Invoke();
            }
            //catch (DbEntityValidationException ex)
            //{
            //    foreach (var errors in ex.EntityValidationErrors)
            //    {
            //        foreach (var validationError in errors.ValidationErrors)
            //        {
            //            // get the error message 
            //            string errorMessage = validationError.ErrorMessage;
            //        }
            //    }
            //}
            catch (Exception ex)
            {
                var m = ex.StackTrace.IndexOf('(');
                var mm = m > 0 ? $"{ex.StackTrace.Substring(0, m)}: " : string.Empty;
                LoggerManager.LogMsg(c => c.Log(ex, mm));
                return Error(ex.Message + " - " + ex.InnerException?.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
            return response;
        }
    }
}
