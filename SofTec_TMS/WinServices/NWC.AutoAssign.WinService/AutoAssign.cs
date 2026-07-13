using NWC.AutoAssign.WinService.Managers;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
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
using System.Timers;
namespace NWC.AutoAssign.WinService
{
    public partial class AutoAssign : ServiceBase
    {
        //private System.Threading.Timer serviceTimer;
        private System.Timers.Timer timmer;
        Thread AutoAssignThread;
        bool ForceStop = false;
   
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

        private int dateFromInHours
        {
            get
            {
                int _dateFromInHours;
                int.TryParse(ConfigurationManager.AppSettings["DateFromInHours"] != null ?
                    ConfigurationManager.AppSettings["DateFromInHours"] : string.Empty, out _dateFromInHours);

                return _dateFromInHours > 0 ? _dateFromInHours : 72;
            }
        }

        private int ordersCount { get; set; }
        private bool isFinished { get; set; }
        #endregion

        #region ctor
        public AutoAssign()
        {
            InitializeComponent();
            double interval = (1000 * timerIntervalSecs);
            timmer = new System.Timers.Timer(interval);
            timmer.Elapsed += new ElapsedEventHandler(ServiceTimmer_Tick);
        }
        #endregion

        #region Thread
        private void ServiceTimmer_Tick(object sender, ElapsedEventArgs args)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg("Service Start At:"+DateTime.Now));
            if (AutoAssignThread == null || !AutoAssignThread.IsAlive)
            {
                AutoAssignThread = new Thread(new ThreadStart(AutoAssignExecute));
                AutoAssignThread.Start();
            }
        }
        protected override void OnStart(string[] args)
        {
            timmer.Start();
            ServiceTimmer_Tick(null, null);
        }
        protected override void OnStop()
        {
            try
            {
                timmer.Stop();
                ForceStop = true;
                LoggerManager.LogMsg(c => c.TrackingMsg("Stopping Timer"));
                this.ordersCount = 0;
                this.isFinished = true;

                //if (serviceTimer != null)
                //{
                //    LoggerManager.LogMsg(c => c.TrackingMsg("Stopping Timer"));

                //    serviceTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                //    serviceTimer.Dispose();
                //    serviceTimer = null;

                //    this.ordersCount = 0;
                //    this.isFinished = true;
                //}
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log("Error - Stop Timer : " + ex.Message.ToString()));
            }
        }

        // This used only for debug
        public void Start()
        {
            timmer.Start();
            ServiceTimmer_Tick(null, null);
        } 
        #endregion

        #region This code not used
        //This code not used
        private void Excecute()
        {
            //try
            //{
            //    Task.Delay(10000);

            //    if (serviceTimer == null)
            //    {
            //        LoggerManager.LogMsg(c => c.TrackingMsg("Starting Timer"));

            //        this.isFinished = true;

            //        TimeSpan tsInterval = new TimeSpan(0, 0, this.timerIntervalSecs);

            //        serviceTimer = new Timer(new TimerCallback(SystemAutoAssign), null, tsInterval, tsInterval);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LoggerManager.LogMsg(c => c.Log("Error - Start Timer : " + ex.Message.ToString()));
            //}
        }
        public void SystemAutoAssign(object state)
        {
            if (this.ordersCount == 0 && this.isFinished)
            {
                try
                {
                    this.isFinished = false;

                    var workOrderManager = new WorkOrderManager(new NWCContext());
                    var userDTO = workOrderManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var resultWorkOrders = new DescriptiveResponse<SearchResult<WorkOrderDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("AutoAssign: User Authenticated xx"));
                        LoggerManager.LogMsg(c => c.TrackingMsg("0"));
                        var woFilter = new Filters<string>();
                        woFilter.PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 };
                        LoggerManager.LogMsg(c => c.TrackingMsg("0.5"));
                        var searchCriteria = new WorkOrderSearchCriteriaDTO()
                        {
                            StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.New },
                            DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate,
                            DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddHours(-this.dateFromInHours),
                            DateTimeTo = DateTimeHelper.GetDateTimeNow(),
                            FilterModel = woFilter,
                            ServiceTypeIDs = new List<int>() { (int)ServiceTypeEnum.Ashyab, (int)ServiceTypeEnum.Soqya }
                        };
                        LoggerManager.LogMsg(c => c.TrackingMsg("1"));
                        resultWorkOrders = workOrderManager.GetNotAssignedWorkOrders(this.queryAPI_URL, userDTO.Value.Token, searchCriteria, this.retrials, this.holdIntervalMinutes).Result;
                        LoggerManager.LogMsg(c => c.TrackingMsg("2"));
                        if (resultWorkOrders.Value != null && resultWorkOrders.Value.Result != null)
                        {
                            this.ordersCount = resultWorkOrders.Value.TotalCount;

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Total Unassigned Count: {0}", resultWorkOrders.Value.TotalCount)));

                            foreach (var wo in resultWorkOrders.Value.Result)
                            {
                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: OrderNumber: {0}", wo.OrderNumber)));

                                var filter = new WorkOderFilter() { OrderId = wo.WorkOrderID, PageFilter = new PageFilter() { PageIndex = 0, PageSize = 2 } };

                                var vehicleResult = workOrderManager.GetAssignableVehicles(this.queryAPI_URL, userDTO.Value.Token, filter);

                                if (!vehicleResult.IsErrorState && vehicleResult.Value != null && vehicleResult.Value.Result != null && vehicleResult.Value.Result.Any())
                                {
                                    foreach (var veh in vehicleResult.Value.Result)
                                    {
                                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: OrderNumber: {0}, AssignableVehicle: {1}",
                                            wo.OrderNumber, veh.VehicleID)));

                                        //var vehicle = vehicleResult.Value.Result.FirstOrDefault();

                                        var dto = new DispatchWorkOrderDTO()
                                        {
                                            EventWorkOrderDTO = new EventWorkOrderDTO() { WorkOrderID = wo.WorkOrderID },
                                            EventWorkOrderVehicleDTO = new EventWorkOrderVehicleDTO() { VehicleID = veh.VehicleID, DriverID = veh.DriverID }
                                        };

                                        var result = workOrderManager.AssignedWorkOrders(this.commandAPI_URL, userDTO.Value.Token, dto).Result;

                                        if (!result.IsErrorState && result.Value)
                                        {
                                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Assign Success => WorkOrderId: {0}, VehicleID: {1}", wo.WorkOrderID, veh.VehicleID)));
                                            break;
                                        }
                                        else
                                        {
                                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Assign Fail => WorkOrderId: {0}, VehicleID: {1}, Error Description: {2}",
                                                            wo.WorkOrderID, veh.VehicleID, result.ErrorDescription)));

                                            var updateResult = workOrderManager.UpdateWorkOrderRetrials(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.holdIntervalMinutes).Result;
                                        }
                                    }
                                }
                                else
                                {
                                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: OrderNumber: {0}, <No-AssignableVehicle>", wo.OrderNumber)));

                                    var updateResult = workOrderManager.UpdateWorkOrderRetrials(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.holdIntervalMinutes).Result;
                                }

                                this.ordersCount--;
                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Total Unassigned Count: {0}", this.ordersCount)));
                            }

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Total Unassigned Count: 0")));
                        }
                        LoggerManager.LogMsg(c => c.TrackingMsg("3"));
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: GetNotAssignedWorkOrders results NULL")));

                        this.isFinished = true;
                    }
                    else
                        LoggerManager.LogMsg(c => c.TrackingMsg("AutoAssign: Authenticate User Fail"));

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
        #endregion

        public void AutoAssignExecute()
        {
            if (this.ordersCount == 0)
            {
                try
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("==> AutoAssign Service Starting At : "+DateTime.Now));
                    var workOrderManager = new WorkOrderManager(new NWCContext());
                    var userDTO = workOrderManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var resultWorkOrders = new DescriptiveResponse<SearchResult<WorkOrderDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("AutoAssign: User Authenticated"));
               
                        var woFilter = new Filters<string>();
                        woFilter.PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 };
                   
                        var searchCriteria = new WorkOrderSearchCriteriaDTO()
                        {
                            StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.New },
                            DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate,
                            DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddHours(-this.dateFromInHours),
                            DateTimeTo = DateTimeHelper.GetDateTimeNow(),
                            FilterModel = woFilter,
                            ServiceTypeIDs = new List<int>() { (int)ServiceTypeEnum.Ashyab, (int)ServiceTypeEnum.Soqya }
                        };
          
                        resultWorkOrders = workOrderManager.GetNotAssignedWorkOrders(this.queryAPI_URL, userDTO.Value.Token, searchCriteria, this.retrials, this.holdIntervalMinutes).Result;
            
                        if (resultWorkOrders.Value != null && resultWorkOrders.Value.Result != null)
                        {
                            this.ordersCount = resultWorkOrders.Value.TotalCount;

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Total Unassigned Count: {0}", resultWorkOrders.Value.TotalCount)));

                            foreach (var wo in resultWorkOrders.Value.Result)
                            {
                                if (ForceStop) break;
                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: OrderNumber: {0}", wo.OrderNumber)));

                                var filter = new WorkOderFilter() { OrderId = wo.WorkOrderID, PageFilter = new PageFilter() { PageIndex = 0, PageSize = 2 } };

                                var vehicleResult = workOrderManager.GetAssignableVehicles(this.queryAPI_URL, userDTO.Value.Token, filter);

                                if (!vehicleResult.IsErrorState && vehicleResult.Value != null && vehicleResult.Value.Result != null && vehicleResult.Value.Result.Any())
                                {
                                    foreach (var veh in vehicleResult.Value.Result)
                                    {
                                        if (ForceStop) break;
                                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: OrderNumber: {0}, AssignableVehicle: {1}",
                                            wo.OrderNumber, veh.VehicleID)));

                                        //var vehicle = vehicleResult.Value.Result.FirstOrDefault();

                                        var dto = new DispatchWorkOrderDTO()
                                        {
                                            EventWorkOrderDTO = new EventWorkOrderDTO() { WorkOrderID = wo.WorkOrderID },
                                            EventWorkOrderVehicleDTO = new EventWorkOrderVehicleDTO() { VehicleID = veh.VehicleID, DriverID = veh.DriverID }
                                        };

                                        var result = workOrderManager.AssignedWorkOrders(this.commandAPI_URL, userDTO.Value.Token, dto).Result;

                                        if (!result.IsErrorState && result.Value)
                                        {
                                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Assign Success => WorkOrderId: {0}, VehicleID: {1}", wo.WorkOrderID, veh.VehicleID)));
                                            break;
                                        }
                                        else
                                        {
                                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Assign Fail => WorkOrderId: {0}, VehicleID: {1}, Error Description: {2}",
                                                            wo.WorkOrderID, veh.VehicleID, result.ErrorDescription)));

                                            var updateResult = workOrderManager.UpdateWorkOrderRetrials(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.holdIntervalMinutes).Result;
                                        }
                                    }
                                }
                                else
                                {
                                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: OrderNumber: {0}, <No-AssignableVehicle>", wo.OrderNumber)));

                                    var updateResult = workOrderManager.UpdateWorkOrderRetrials(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.holdIntervalMinutes).Result;
                                }

                                this.ordersCount--;
                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Total Unassigned Count: {0}", this.ordersCount)));
                            }

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: Total Unassigned Count: 0")));
                        }
               
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoAssign: GetNotAssignedWorkOrders results NULL")));

                        this.isFinished = true;
                    }
                    else
                        LoggerManager.LogMsg(c => c.TrackingMsg("AutoAssign: Authenticate User Fail"));
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.ordersCount = 0;
                    //this.isFinished = true;
                    LoggerManager.LogMsg(c => c.TrackingMsg("==> AutoAssign Service Ending At : " + DateTime.Now));
                    LoggerManager.LogMsg(c => c.TrackingMsg("======================================================"));
                }
            }
        }
    }
}