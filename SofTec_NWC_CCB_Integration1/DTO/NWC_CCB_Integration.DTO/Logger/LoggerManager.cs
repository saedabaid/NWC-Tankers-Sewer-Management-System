using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Logger
{
    public static class LoggerManager
    {
        public static ILogger logger = new RootLogger(LoggerType.FileLogger) { AutoCommit = true };
        
        public static void LogMsg(Action<ILogger> action)
        {
            if (logger != null)
            {
                action.Invoke(logger);
            }
        }
    }
}
