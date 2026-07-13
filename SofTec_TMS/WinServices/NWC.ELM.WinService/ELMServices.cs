using NLog;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.Sewer.WinService.Managers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;

namespace NWC.Sewer.WinService
{
    public class ELMServices
    {
        #region prop
        private readonly bool ElmServiceEnabled;


        private System.Timers.Timer _timer;
        private Thread LockoutUserThread;
        private readonly int ELMRetryCounter;


        private string _token;
        private Guid subid;

        #endregion prop

        #region ctor
        private string commandAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["CommandAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["CommandAPI_URL"] : string.Empty;
            }
        }
        private string authenticationAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthenticationAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["AuthenticationAPI_URL"] : string.Empty;
            }
        }
        private string username
        {
            get
            {
                return ConfigurationManager.AppSettings["UserName"] != null ?
                    ConfigurationManager.AppSettings["UserName"] : string.Empty;
            }
        }
        private string password
        {
            get
            {
                return ConfigurationManager.AppSettings["Password"] != null ?
                    ConfigurationManager.AppSettings["Password"] : string.Empty;
            }
        }
        public void setToken()
        {
            RegisterLog(LogLevel.Info, string.Format("***** start to generate token "));
            var loginDto = new AuthenticationManager().AuthenticateUser(authenticationAPI_URL, username, password);
            _token = loginDto.Value != null && !string.IsNullOrEmpty(loginDto.Value.Token) ? loginDto.Value.Token : null;
            subid = loginDto.Value.Context.Account.SubID;
            RegisterLog(LogLevel.Info, string.Format("***** Refresh token{0}", _token));

        }
        public ELMServices()
        {
            try
            {
                setToken();
                LogManager.GetLogger("NWC.ELM.Services").Log(LogLevel.Error, "const : token : " + _token);

                if (_token != null)
                {
                    this.ELMRetryCounter = KeyConfig.ELMRetryCounter;

                    //To set job interval in Minutes
                    var jobInterval = KeyConfig.WindowsServiceTimerIntervalMinutes;
                    _timer = new System.Timers.Timer(1000 * 60 * jobInterval) { AutoReset = true };
                    _timer.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
                    this.ElmServiceEnabled = KeyConfig.ElmServiceEnabled;

                    RegisterLog(LogLevel.Info, "***** NWC.ELM.WinService Started At " + DateTime.Now);
                }
                else
                {
                    RegisterLog(LogLevel.Info, "***** NWC.ELM.WinService Login failed At " + DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error At  :" + DateTime.Now + " - " + ex.Message);
            }
        }

        #endregion ctor

        #region Timer Event and Setting

        public void Start()
        {
            _timer.Start();
            ServiceTimer_Tick(null, null);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        #region Setting

        public void RegisterLog(LogLevel level, string message)
        {
            LogManager.GetLogger("NWC.ELM.Services").Log(level, message);
            Console.WriteLine(level.ToString() + message);
        }

        public void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            RegisterLog(LogLevel.Info, "***** Timer Tick Started At " + DateTime.Now);
            SaveTrankerMethod();
            //if (ElmServiceEnabled && (LockoutUserThread == null || !LockoutUserThread.IsAlive))
            //{
            //    LockoutUserThread = new Thread(new ThreadStart(SaveTrankerMethod));
            //    LockoutUserThread.Start();
            //}
            RegisterLog(LogLevel.Info, "***** Timer Tick End At " + DateTime.Now);

        }

        #endregion Setting


        public void SaveTrankerMethod()
        {
            try
            {
                if(_token == null||string.IsNullOrEmpty(_token))
                {
                    setToken();
                }
                var workOrderManager = new TankerManager(_token);
                var resultWorkOrders = workOrderManager.SaveTankerToTransporter(commandAPI_URL, _token, subid, ELMRetryCounter);
                int code = (int)resultWorkOrders.ResponseCode;
                RegisterLog(LogLevel.Info, "ResponseCode " + code);
                if (resultWorkOrders != null && !resultWorkOrders.IsErrorState && resultWorkOrders.Value)
                    RegisterLog(LogLevel.Info, "***** Tankers Inserted To Transporter.");

                else if (code == (int)ErrorStatus.InvalidUserOrPass)
                    setToken();
                else
                    RegisterLog(LogLevel.Info, string.Format("***** No Tankers Inserted. {0} - {1}", resultWorkOrders.ResponseDescription, resultWorkOrders.ErrorDescription));
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error :" + ex.Message);
            }
            RegisterLog(LogLevel.Info, "|--- Ended At " + DateTime.Now + " -- Call SaveTrankerMethod");


        }
        #endregion Timer Event and Setting
    }
}
