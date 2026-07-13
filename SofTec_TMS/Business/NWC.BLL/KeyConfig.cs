using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL
{
    public static class KeyConfig
    {
        public static bool IsSignalREnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["isSignalREnabled"];

                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Boolean.Parse(result);
            }
        }

        public static string SignalRApiUrl
        {
            get
            {
                var result = ConfigurationManager.AppSettings["signalRApiUrl"];
                if (String.IsNullOrEmpty(result))
                {
                    return string.Empty;
                }
                return result;
            }
        }
    }
}
