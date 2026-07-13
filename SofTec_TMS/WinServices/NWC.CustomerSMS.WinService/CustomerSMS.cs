using NWC.DAL.NWCEntities;
using NWC.CustomerSMS.WinService.Managers;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NWC.DTO.Helpers;
using NWC_CCB_Integration.DTO.Logger;

namespace NWC.CustomerSMS.WinService
{
    public partial class CustomerSMS : ServiceBase
    {
        private Timer serviceTimer;

        #region Properties
        private string authenticationAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthenticationAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["AuthenticationAPI_URL"] : string.Empty;
            }
        }
        private string commandAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["CommandAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["CommandAPI_URL"] : string.Empty;
            }
        }
        private string queryAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["QueryAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["QueryAPI_URL"] : string.Empty;
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

        private int take
        {
            get
            {
                int _take;
                int.TryParse(ConfigurationManager.AppSettings["Take"] != null ?
                    ConfigurationManager.AppSettings["Take"] : string.Empty, out _take);

                return _take > 0 ? _take : 10;
            }
        }
        private int timerIntervalSecs
        {
            get
            {
                int _interval;
                int.TryParse(ConfigurationManager.AppSettings["TimerIntervalSecs"] != null ?
                    ConfigurationManager.AppSettings["TimerIntervalSecs"] : string.Empty, out _interval);

                return _interval > 0 ? _interval : 10;
            }
        }
        private int retrials
        {
            get
            {
                int _retrials;
                int.TryParse(ConfigurationManager.AppSettings["Retrials"] != null ?
                    ConfigurationManager.AppSettings["Retrials"] : string.Empty, out _retrials);

                return _retrials;
            }
        }
        private int holdInterval
        {
            get
            {
                int _holdInterval;
                int.TryParse(ConfigurationManager.AppSettings["HoldInterval"] != null ?
                    ConfigurationManager.AppSettings["HoldInterval"] : string.Empty, out _holdInterval);

                return _holdInterval;
            }
        }
        private string smsLang
        {
            get
            {
                return ConfigurationManager.AppSettings["SMSLang"] != null ?
                    ConfigurationManager.AppSettings["SMSLang"] : string.Empty;
            }
        }
        private string eventSource
        {
            get
            {
                return ConfigurationManager.AppSettings["EventSource"] != null ?
                    ConfigurationManager.AppSettings["EventSource"] : string.Empty;
            }
        }
        private string eventPriority
        {
            get
            {
                return ConfigurationManager.AppSettings["EventPriority"] != null ?
                    ConfigurationManager.AppSettings["EventPriority"] : string.Empty;
            }
        }
        private int smsType
        {
            get
            {
                int _smsType;
                int.TryParse(ConfigurationManager.AppSettings["SMSType"] != null ?
                    ConfigurationManager.AppSettings["SMSType"] : string.Empty, out _smsType);

                return _smsType;
            }
        }

        private double delayInMin
        {
            get
            {
                double _delayInMin;
                double.TryParse(ConfigurationManager.AppSettings["DelayInMin"] != null ?
                    ConfigurationManager.AppSettings["DelayInMin"] : string.Empty, out _delayInMin);

                return _delayInMin > 0 ? _delayInMin : 0;
            }
        }

        private int smsCount { get; set; }
        private bool isFinished { get; set; }
        #endregion

        public CustomerSMS()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Task.Delay(10000);

                if (serviceTimer == null)
                {
                    this.isFinished = true;

                    TimeSpan tsInterval = new TimeSpan(0, 0, this.timerIntervalSecs);

                    serviceTimer = new Timer(new TimerCallback(SendCustomerSMS), null, tsInterval, tsInterval);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Start Timer : " + ex.Message.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (serviceTimer != null)
                {
                    Debug.WriteLine("Stopping Timer");
                    serviceTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    serviceTimer.Dispose();
                    serviceTimer = null;

                    this.smsCount = 0;
                    this.isFinished = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Stop Timer : " + ex.Message.ToString());
            }
        }

        public void SendCustomerSMS(object state)
        {
            if (this.smsCount == 0 && this.isFinished)
            {
                try
                {
                    this.isFinished = false;

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg("---------------------------------------------------------------------------------------------------------------------------"));
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format(">>>>>>>>>>>>>>>>> Start CustomerSMS >>>>>>>>>>>>>>>>>")));

                    var smsManager = new SMSManager(new NWCContext());
                    var userDTO = smsManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var resultDriverSMSs = new DescriptiveResponse<SearchResult<CustomerSMSDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        var searchCriteria = new CustomerSMSSearchCriteria()
                        {
                            DelayInMin = this.delayInMin, 
                            StatusIDs = new List<int>() { 1 },
                            PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 }
                        };

                        resultDriverSMSs = smsManager.GetCustomerSMSs(this.queryAPI_URL, userDTO.Value.Token, searchCriteria).Result;

                        if (resultDriverSMSs.Value != null && resultDriverSMSs.Value.Result != null)
                        {
                            this.smsCount = resultDriverSMSs.Value.TotalCount;

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CustomerSMS: Total SMS Count: {0}", this.smsCount)));

                            foreach (var sms in resultDriverSMSs.Value.Result)
                            {
                                var isSentSMS = smsManager.SendCustomerSMS(sms.ID, sms.CustomerMobileNo, sms.OrderNumber, sms.SMSText, this.smsLang, this.smsType, this.eventSource, this.eventPriority);

                                if (isSentSMS)
                                {
                                    var result = smsManager.UpdateSuccessDriverSMS(this.commandAPI_URL, userDTO.Value.Token, sms.ID).Result;
                                }
                                else
                                {
                                    var result = smsManager.UpdateFailDriverSMS(this.commandAPI_URL, userDTO.Value.Token, sms.ID).Result;
                                }

                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CustomerSMS: ID: {0}, Order Number: {1}, SMS Is Sent: {2}", sms.ID, sms.OrderNumber, isSentSMS)));

                                this.smsCount--;

                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CustomerSMS: Total SMS Count: {0}", this.smsCount)));
                            }
                        }
                        else
                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CustomerSMS: Total SMS Count: 0")));

                        this.isFinished = true;
                    }

                    this.isFinished = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.smsCount = 0;
                    this.isFinished = true;
                }
            }
        }
    }
}
