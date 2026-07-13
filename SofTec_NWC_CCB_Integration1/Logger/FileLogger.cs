using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger.configHandler;

namespace Logger
{
    internal sealed class FileLogger : ILogger
    {
        #region Singlton Implementation

        private StreamWriter logWriter = null;
        private StreamWriter trackingWriter = null;
        private static readonly FileLogger _instance = new FileLogger();
        private bool isLogWriterConfigured = false;
        private bool isTrackingWriterConfigured = false;

        static FileLogger()
        {
        }

        private DateTime dtLog;
        private string formatLog;
        private int paddingLog;
        private DateTime dtTracking;
        private string formatTracking;
        private int paddingTracking;
        private FileLogger()
        {
            try
            {
                var dt = DateTime.Now.Date;
                InitializeWriter(true, dt);
                InitializeWriter(false, dt);
            }
            catch (Exception ex)
            {
                if (isLogWriterConfigured)
                {
                    logWriter.Close();
                    logWriter.Dispose();
                    isLogWriterConfigured = false;
                }

                if (isTrackingWriterConfigured)
                {
                    trackingWriter.Close();
                    trackingWriter.Dispose();
                    isTrackingWriterConfigured = false;
                }
            }
        }

        public static FileLogger Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion Singlton Implementation

        #region Interface Implementation

        private bool _autoCommit;
        public bool AutoCommit
        {
            get
            {
                return _autoCommit;
            }
            set
            {
                _autoCommit = value;
                if (isLogWriterConfigured)
                {
                    logWriter.AutoFlush = value;
                }
                if (isTrackingWriterConfigured)
                {
                    trackingWriter.AutoFlush = value;
                }
            }
        }

        public void TrackingMsg(string msg)
        {
            if (isTrackingWriterConfigured)
            {
                WriteTheStringToWriter(trackingWriter, msg, false);
            }
        }

        public void TrackingMsg(string msg, params object[] args)
        {
            if (isTrackingWriterConfigured)
            {
                WriteTheStringToWriter(trackingWriter, string.Format(msg, args), false);
            }
        }

        public void Log(string msg)
        {
            if (isLogWriterConfigured)
            {
                WriteTheStringToWriter(logWriter, msg, true);
            }
        }

        public void Log(string msg, params object[] args)
        {
            if (isLogWriterConfigured)
            {
                WriteTheStringToWriter(logWriter, string.Format(msg, args), true);
            }
        }

        public void Log(string[] msgs)
        {
            if (isLogWriterConfigured)
            {
                WriteTheStringToWriter(msgs);
            }
        }

        public void Log(Exception e)
        {
            if (isLogWriterConfigured)
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
                WriteTheStringToWriter(msgs.ToArray());
            }
        }

        public void LogAndThrow(Exception e)
        {
            Log(e);
            throw e;
        }

        public void Commit()
        {
            if (isLogWriterConfigured)
            {
                logWriter.Flush();
            }
            if (isTrackingWriterConfigured)
            {
                trackingWriter.Flush();
            }
        }

        #endregion Interface Implementation

        #region private method.       
        private void InitializeWriter(bool isLog, DateTime dateTime)
        {
            string path = string.Empty;
            if (isLog)
            {
                dtLog = dateTime;
                formatLog = LoggerConfig.Config.LogFile.TimeFormat ?? "G";
                paddingLog = dtLog.ToString(formatLog).Length + 4;
                path = LoggerConfig.Config.LogFile.Enabled ? LoggerConfig.Config.LogFile.Path : string.Empty;
            }
            else
            {
                dtTracking = dateTime;
                formatTracking = LoggerConfig.Config.TraceFile.TimeFormat ?? "G";
                paddingTracking = dtTracking.ToString(formatTracking).Length + 4;
                path = LoggerConfig.Config.TraceFile.Enabled ? LoggerConfig.Config.TraceFile.Path : string.Empty;
            }
            if (!string.IsNullOrEmpty(path))
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string fileName = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);
                fileName = string.Format("{0}_{1}{2}", fileName, dateTime.ToString("yyyyMMdd"), ext);

                if (isLog)
                {
                    logWriter = new StreamWriter(Path.Combine(dir, fileName), true);
                    isLogWriterConfigured = true;
                }
                else
                {
                    trackingWriter = new StreamWriter(Path.Combine(dir, fileName), true);
                    isTrackingWriterConfigured = true;
                }
            }
        }
        private void WriteTheStringToWriter(string[] strArray)
        {
            foreach (var str in strArray)
            {
                WriteTheStringToWriter(logWriter, str, true);
            }
        }

        private static object writerLock = new object();
        private void WriteTheStringToWriter(StreamWriter writer, string msg, bool isLog)
        {
            lock (writerLock)
            {
                try
                {
                    writer.WriteLine(string.Format("{0}{1}", DateTime.Now.ToString(isLog ? formatLog : formatTracking).PadRight(isLog ? paddingLog : paddingTracking), msg));
                    var dt = DateTime.Now.Date;
                    if (!Equals(dt, isLog ? dtLog : dtTracking))
                    {
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                        InitializeWriter(isLog, dt);
                        if (isLog)
                        {
                            logWriter.AutoFlush = writer.AutoFlush;
                            writer = logWriter;
                        }
                        else
                        {
                            trackingWriter.AutoFlush = writer.AutoFlush;
                            writer = trackingWriter;
                        }
                    }
                }
                catch (Exception e)
                {
                    //not to consider.
                }
            }
        }

        #endregion private method.

        public void Dispose()
        {
            if (logWriter != null)
            {
                isLogWriterConfigured = false;
                logWriter.Close();
                logWriter.Dispose();
            }
            if (trackingWriter != null)
            {
                isTrackingWriterConfigured = false;
                trackingWriter.Close();
                trackingWriter.Dispose();
            }
        }
    }
}