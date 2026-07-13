using System;

namespace NWC.Localization.ExceptionLogger
{
    public interface IExceptionLogger
    {
        void LogException(Exception e);
        void LogException(Exception e, params object[] parameters);
        void LogInformation(string information);
    }
}