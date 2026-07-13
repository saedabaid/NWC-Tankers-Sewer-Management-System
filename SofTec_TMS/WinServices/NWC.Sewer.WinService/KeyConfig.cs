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
        #endregion

        #region   SewerSettingValues
        public static int SewerPreAssignRetryCounter
        {
            get
            {
                var result = ConfigurationManager.AppSettings["SewerPreAssignRetryCounter"];

                if (String.IsNullOrEmpty(result))
                {
                    return 1;
                }

                return Int32.Parse(result);
            }
        }
        public static int PushTimePeriodToCancelOrderInMinutes
        {
            get
            {
                var result = ConfigurationManager.AppSettings["PushTimePeriodToCancelOrderInMinutes"];

                if (String.IsNullOrEmpty(result))
                {
                    return 1;
                }

                return Int32.Parse(result);
            }
        }
        public static int DeAssignTimePeriodInMinutes
        {
            get
            {
                var result = ConfigurationManager.AppSettings["DeAssignTimePeriodInMinutes"];

                if (String.IsNullOrEmpty(result))
                {
                    return 10;
                }

                return Int32.Parse(result);
            }
        }

        public static int ExitVehicleInMinutes
        {
            get
            {
                var result = ConfigurationManager.AppSettings["ExitVehicleInMinutes"];

                if (String.IsNullOrEmpty(result))
                {
                    return 30;
                }

                return Int32.Parse(result);
            }
        }
        #endregion

        #region Methode Enable and Disable
        public static bool NewOrderEnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["NewOrderEnabled"];
                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Convert.ToBoolean(result);
            }
        }
        public static bool AssignOrderEnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["AssignOrderEnabled"];
                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Convert.ToBoolean(result);
            }
        }
        public static bool DeAssignOrderEnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["DeAssignOrderEnabled"];
                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Convert.ToBoolean(result);
            }
        }
        public static bool CancelOrderBasedOnAssignRetryCounterEnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["CancelOrderBasedOnAssignRetryCounterEnabled"];
                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Convert.ToBoolean(result);
            }
        }
        public static bool CancelOrderBasedOnPushTimeEnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["CancelOrderBasedOnPushTimeEnabled"];
                if (String.IsNullOrEmpty(result))
                {
                    return false;
                }

                return Convert.ToBoolean(result);
            }
        }

        public static bool ExitVehicleEnabled
        {
            get
            {
                var result = ConfigurationManager.AppSettings["ExitVehicleEnabled"];
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
