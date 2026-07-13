using System.Configuration;

namespace Logger.configHandler
{
    public static class LoggerConfig
    {
        static LoggerConfig()
        {
            Config = ConfigurationManager.GetSection("loggerConfig") as LoggerConfiguration;
        }
        public static LoggerConfiguration Config { get; private set; }
    }
}
