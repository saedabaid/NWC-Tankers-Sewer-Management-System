using NWC_CCB_Integration.DTO.Constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.ExceptionLogger
{
    public static class ExceptionManager
    {
        public static IExceptionLogger GetExceptionLogger()
        {
            var exceptionStorage = ConfigurationManager.AppSettings[ExceptionHandlerConstants.ExceptionAppSettingKey];
            var logsPath = ConfigurationManager.AppSettings[ExceptionHandlerConstants.LogsPathKey];

            if (exceptionStorage == ExceptionHandlerConstants.TextFileStorageKey)
            {
                return TextExceptionLogger.GetInstance;
            }
            else if (exceptionStorage == ExceptionHandlerConstants.DatabaseStorageKey)
            {
                return new DatabaseExceptionLogger();
            }
            else
            {
                return TextExceptionLogger.GetInstance;
            }
        }
    }
}
