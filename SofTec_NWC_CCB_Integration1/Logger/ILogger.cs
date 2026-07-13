using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public interface ILogger : IDisposable
    {
        bool AutoCommit { get; set; }

        //bool IsTrackingLogEnabled { get; set; }

        void TrackingMsg(string msg);

        void TrackingMsg(string msg, params object[] args);

        void Log(string msg);

        void Log(string msg, params object[] args);

        void Log(string[] msgs);

        void Log(Exception e);

        void LogAndThrow(Exception e);

        void Commit();
    }
}