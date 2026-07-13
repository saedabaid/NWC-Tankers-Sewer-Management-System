using NWC.Localization.Constants;
using System.Configuration;

namespace NWC.Localization.ExceptionLogger
{
    public static class ExceptionManager
    {
        public static IExceptionLogger GetExceptionLogger()
        {
            var exceptionStorage = ConfigurationManager.AppSettings[ExceptionHandlerConstants.ExceptionAppSettingKey];
            var logsPath = ConfigurationManager.AppSettings[ExceptionHandlerConstants.LogsPathKey];

            if (exceptionStorage == ExceptionHandlerConstants.TextFileStorageKey)
            {
                return new TextExceptionLogger(logsPath);
            }
            else if (exceptionStorage == ExceptionHandlerConstants.DatabaseStorageKey)
            {
                return new DatabaseExceptionLogger();
            }
            else
            {
                return new TextExceptionLogger(logsPath);
            }
        }
    }
}
