using System;
using System.Configuration;

namespace NWC.Sewer.WinService
{
    public static class KeyConfig
    {
        #region Windows Service Setting
        public static Double WindowsServiceTimerIntervalMinutes
        {
            get
            {
                var result = ConfigurationManager.AppSettings["WindowsServiceTimerIntervalMinutes"];

                if (String.IsNullOrEmpty(result))
                {
                    return 1;
                }

                return Double.Parse(result);
            }
        }


        public static int ELMRetryCounter
        {
            get
            {
                var result = ConfigurationManager.AppSettings["ELMRetryCounter"];

                if (String.IsNullOrEmpty(result))
                {
                    return 1;
                }

                return Int32.Parse(result);
            }
        }

        #endregion

        #region Methode Enable and Disable
        public static bool ElmServiceEnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["ElmServiceEnabled"];
                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Convert.ToBoolean(result);
            }
        }

        #endregion
    }
}
