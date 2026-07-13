using NWC.DTO.Enums;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NWC.DTO.Common.ELM
{
    public class DescriptiveResponse<T> : DescriptiveObject
    {
        public T Result { get; set; }
   

        public static DescriptiveResponse<T> Success(T value)
        {
            return new DescriptiveResponse<T>
            {
                Result = value,
                ResponseCode = 200,
                Status="OK"
            };
        }
        public static DescriptiveResponse<T> Success(string customDescription, T value)
        {
            return new DescriptiveResponse<T>
            {
                Result = value,
                ResponseCode = 200,
                ResponseDescription = customDescription,
                Status = "OK"
            };
        }
        public static new DescriptiveResponse<T> Error(ErrorStatus status = ErrorStatus.UNEXPECTED_ERROR)
        {
             
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,
                ResponseCode = status
            };
        }
        public static new DescriptiveResponse<T> Error(string customDescription, ErrorStatus status = ErrorStatus.CUSTOM)
        {
            return new DescriptiveResponse<T>
            {
                IsErrorState = true,
                ResponseCode = status,
                Status = "OK",
                ResponseDescription = customDescription
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
                Result = newValue,
                IsErrorState = this.IsErrorState,
                ResponseCode = this.ResponseCode,
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
