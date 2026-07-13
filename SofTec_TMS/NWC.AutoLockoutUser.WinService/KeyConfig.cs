using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.AutoLockoutUser.WinService
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

        public static bool LockoutUserEnable
        {
            get
            {
                var result = ConfigurationManager.AppSettings["LockoutUserEnable"];

                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Boolean.Parse(result);
            }
        }
        #endregion
    }
}
