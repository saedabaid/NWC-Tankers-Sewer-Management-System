using Newtonsoft.Json;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.HayatUpdateWorkOrderStatus.Managers;
using NWC.HayatUpdateWorkOrderStatus.UpdateWONotificationServiceRef;
using NWC_CCB_Integration.DTO.Logger;
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

namespace NWC.HayatUpdateWorkOrderStatus
{
    public partial class HayatUpdateWorkOrderStatus : ServiceBase
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
        private int holdIntervalMinutes
        {
            get
            {
                int _holdInterval;
                int.TryParse(ConfigurationManager.AppSettings["HoldIntervalMinutes"] != null ?
                    ConfigurationManager.AppSettings["HoldIntervalMinutes"] : string.Empty, out _holdInterval);

                return _holdInterval;
            }
        }

        private int ordersCount { get; set; }
        private bool isFinished { get; set; }
        #endregion

        public HayatUpdateWorkOrderStatus()
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

                    serviceTimer = new Timer(new TimerCallback(SystemAutoAssign), null, tsInterval, tsInterval);
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
                    serviceTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    serviceTimer.Dispose();
                    serviceTimer = null;

                    this.ordersCount = 0;
                    this.isFinished = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Stop Timer : " + ex.Message.ToString());
            }
        }

        public void SystemAutoAssign(object state)
        {
            if (this.ordersCount == 0 && this.isFinished)
            {
                try
                {
                    this.isFinished = false;

                    var hayatManager = new HayatManager(new NWCContext());
                    var userDTO = hayatManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var resultWorkOrders = new DescriptiveResponse<SearchResult<HayatWorkOrderLogDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        var searchCriteria = new HayatWorkOrderLogsSC()
                        {
                            StatusIDs = new List<int>() { (int)HayatWorkOrderLogStatusEnum.Pending, (int)HayatWorkOrderLogStatusEnum.Failed },
                            Take = this.take
                        };

                        resultWorkOrders = hayatManager.GetHayatWorkOrderLogs(this.queryAPI_URL, userDTO.Value.Token, searchCriteria, this.retrials, this.holdIntervalMinutes).Result;

                        if (resultWorkOrders.Value != null && resultWorkOrders.Value.Result != null)
                        {
                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Hayat Logs: Total Not Sent Count: {0}", resultWorkOrders.Value.TotalCount)));

                            var woGroups = resultWorkOrders.Value.Result.GroupBy(x => x.OrderNumber);
                            this.ordersCount = woGroups.Count();

                            foreach (var groupItem in woGroups)
                            {
                                foreach (var wo in groupItem)
                                {
                                    try
                                    {
                                        if (wo.Retrials > 0 && wo.Retrials >= this.retrials)
                                            break;

                                        var hayatServiceClient = new TMSNwcUpdateWONotification_pttClient();

                                        var hayatRequest = JsonConvert.DeserializeObject<TMSWONotificationRequest>(wo.HayatRequest);

                                        // Log
                                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Calling update service request: {0}", JsonConvert.SerializeObject(hayatRequest))));

                                        var result = Task.Run(async () => await hayatServiceClient.updateWorkOrserStatusAsync(hayatRequest)).ConfigureAwait(false);
                                        var updateResponse = result.GetAwaiter().GetResult().TMSWONotificationResponse;

                                        // Log
                                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Calling update service response: {0}", JsonConvert.SerializeObject(updateResponse))));

                                        wo.Retrials = wo.Retrials.HasValue ? (wo.Retrials.Value + 1) : 1;
                                        wo.HayatResponse = JsonConvert.SerializeObject(updateResponse);

                                        if (updateResponse != null && updateResponse.status.ToUpper() == "OK")
                                        {
                                            wo.StatusID = (int)HayatWorkOrderLogStatusEnum.Success;

                                            hayatManager.UpdateHayatWorkOrderLog(this.commandAPI_URL, userDTO.Value.Token, wo);
                                        }
                                        else
                                        {
                                            wo.StatusID = (int)HayatWorkOrderLogStatusEnum.Failed;

                                            hayatManager.UpdateHayatWorkOrderLog(this.commandAPI_URL, userDTO.Value.Token, wo);

                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LoggerManager.LogMsg(c => c.Log(ex));

                                        wo.StatusID = (int)HayatWorkOrderLogStatusEnum.Failed;
                                        hayatManager.UpdateHayatWorkOrderLog(this.commandAPI_URL, userDTO.Value.Token, wo);

                                        break;
                                    }
                                }

                                this.ordersCount--;
                            }

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Hayat Logs: Total Not Sent Count: 0")));
                        }

                        this.isFinished = true;
                    }
                    else
                        LoggerManager.LogMsg(c => c.TrackingMsg("Authenticate User Fail"));

                    this.isFinished = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.ordersCount = 0;
                    this.isFinished = true;
                }
            }
        }
    }
}
