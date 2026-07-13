using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class RootLogger : ILogger
    {
        private List<ILogger> allLoggers = new List<ILogger>();

        public RootLogger(LoggerType multipleLoggerType)
        {
            
            if (multipleLoggerType.HasFlag(LoggerType.FileLogger))
            {
                allLoggers.Add(FileLogger.Instance);
            }
            if (multipleLoggerType.HasFlag(LoggerType.VisualStudioOutputWindowLogger))
            {
                allLoggers.Add(VSOutputLogger.Instance);
            }            
        }

        public bool AutoCommit
        {
            get
            {
                return allLoggers.All(c => c.AutoCommit == true);
            }
            set
            {
                allLoggers.ForEach(lg => lg.AutoCommit = value);
            }
        }

        //public bool IsTrackingLogEnabled
        //{
        //    get
        //    {
        //        return allLoggers.All(c => c.IsTrackingLogEnabled);
        //    }
        //    set
        //    {
        //        allLoggers.ForEach(lg => lg.IsTrackingLogEnabled = value);
        //    }
        //}

        public void TrackingMsg(string msg)
        {
            allLoggers.ForEach(lg => lg.TrackingMsg(msg));
        }

        public void TrackingMsg(string msg, params object[] args)
        {
            allLoggers.ForEach(lg => lg.TrackingMsg(msg, args));
        }

        public void Log(string msg)
        {
            allLoggers.ForEach(lg => lg.Log(msg));
        }

        public void Log(string msg, params object[] args)
        {
            allLoggers.ForEach(lg => lg.Log(msg, args));
        }

        public void Log(string[] msgs)
        {
            allLoggers.ForEach(c => c.Log(msgs));
        }

        public void Log(Exception e)
        {
            allLoggers.ForEach(lg => lg.Log(e));
        }

        public void LogAndThrow(Exception e)
        {
            allLoggers.ForEach(lg => lg.LogAndThrow(e));
        }

        public void Commit()
        {
            allLoggers.ForEach(lg => lg.Commit());
        }

        public void Dispose()
        {
            allLoggers.ForEach(lg => lg.Dispose());
        }
    }
}