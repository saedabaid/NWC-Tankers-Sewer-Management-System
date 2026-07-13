using System.Configuration;

namespace Logger.configHandler
{
    public class LoggerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("logFile")]
        public CustomElements<string> LogFile
        {
            get { return (CustomElements<string>)this["logFile"]; }
            set { this["logFile"] = value; }
        }

        [ConfigurationProperty("traceFile")]
        public CustomElements<string> TraceFile
        {
            get { return (CustomElements<string>)this["traceFile"]; }
            set { this["traceFile"] = value; }
        }
    }

    public class CustomElements<T> : ConfigurationElement
    {
        [ConfigurationProperty("path")]
        public T Path
        {
            get { return (T)this["path"]; }
            set { this["path"] = value; }
        }        

        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }

        [ConfigurationProperty("timeFormat")]
        public string TimeFormat
        {
            get { return (string)this["timeFormat"]; }
            set { this["timeFormat"] = value; }
        }
        public override string ToString()
        {
            return this.Path.ToString();
        }
    }
}
