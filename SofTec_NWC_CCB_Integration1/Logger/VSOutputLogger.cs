using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public sealed class VSOutputLogger : ILogger
    {
        #region Singlton Implementation

        private static readonly VSOutputLogger _instance = new VSOutputLogger();

        static VSOutputLogger()
        {
        }

        private VSOutputLogger()
        {
        }

        public static VSOutputLogger Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion Singlton Implementation

        // not in use.
        private bool autoCommit = false;

        #region Interface Implementation

        public bool AutoCommit
        {
            get
            {
                return autoCommit;
            }
            set
            {
                autoCommit = value;
            }
        }

        public bool IsTrackingLogEnabled { get; set; }

        public void TrackingMsg(string msg)
        {
            Debugger.Log(0, "Tracking Msg", DateTime.Now.ToString("G").PadRight(25) + msg + Environment.NewLine);
        }

        public void TrackingMsg(string msg, params object[] args)
        {
            TrackingMsg(string.Format(msg, args));
        }
        public void Log(string msg)
        {
            Debugger.Log(0, "Log", DateTime.Now.ToString("G").PadRight(25) + msg + Environment.NewLine);
        }

        public void Log(string msg, params object[] args)
        {
            Log(string.Format(msg, args));
        }

        public void Log(string[] msgs)
        {
            foreach (var msg in msgs)
            {
                Log(msg);
            }
        }

        public void Log(Exception e)
        {
            List<string> msgs = new List<string>();
            msgs.Add("Exception - ");
            int tabCount = 0;
            while (e != null)
            {
                tabCount++;
                msgs.Add("\t".Repeat(tabCount) + " - " + e.Message);
                e = e.InnerException;
            }
            Log(msgs.ToArray());
        }

        public void LogAndThrow(Exception e)
        {
            Log(e);
            throw e;
        }

        public void Commit()
        {
            //throw new NotImplementedException();
        }

        #endregion Interface Implementation

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}