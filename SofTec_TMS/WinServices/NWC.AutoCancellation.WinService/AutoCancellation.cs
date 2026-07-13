using NWC.AutoCancellation.WinService.Managers;
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

namespace NWC.AutoCancellation.WinService
{
    public partial class AutoCancellation : ServiceBase
    {
        private Timer serviceTimer_New;
        private Timer serviceTimer_OnHold;
        private Timer serviceTimer_Restricted;

        #region Properties
        private string authenticationAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthenticationAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["AuthenticationAPI_URL"] : string.Empty;
            }
        }
        private string ZoneWithoutTankers_Enabled
        {
            get
            {
                return ConfigurationManager.AppSettings["ZoneWithoutTankers_Enabled"] != null ?
                    ConfigurationManager.AppSettings["ZoneWithoutTankers_Enabled"] : "TRUE";
            }
        }
        private string OnHold_Enabled
        {
            get
            {
                return ConfigurationManager.AppSettings["OnHold_Enabled"] != null ?
                    ConfigurationManager.AppSettings["OnHold_Enabled"] : "TRUE";
            }
        }
        private string New_Enabled
        {
            get
            {
                return ConfigurationManager.AppSettings["New_Enabled"] != null ?
                    ConfigurationManager.AppSettings["New_Enabled"] : "TRUE";
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
        private double cancelAfterHours
        {
            get
            {
                double cancelAfterHours;
                double.TryParse(ConfigurationManager.AppSettings["CancelAfterHours"] != null ?
                    ConfigurationManager.AppSettings["CancelAfterHours"] : string.Empty, out cancelAfterHours);

                return cancelAfterHours > 0 ? cancelAfterHours : 6;
            }
        }

        private int newOrderCancelationReasonID
        {
            get
            {
                int _reasonID;
                int.TryParse(ConfigurationManager.AppSettings["NewOrderCancelationReasonID"] != null ?
                    ConfigurationManager.AppSettings["NewOrderCancelationReasonID"] : string.Empty, out _reasonID);

                return _reasonID > 0 ? _reasonID : 1;
            }
        }
        private int zoneWithoutTankersReasonID
        {
            get
            {
                int _reasonID;
                int.TryParse(ConfigurationManager.AppSettings["ZoneWithoutTankersReasonID"] != null ?
                    ConfigurationManager.AppSettings["ZoneWithoutTankersReasonID"] : string.Empty, out _reasonID);

                return _reasonID > 0 ? _reasonID : 1;
            }
        }

        private string AllowedCustomerClasses_config
        {
            get
            {
                return ConfigurationManager.AppSettings["AllowedCustomerClasses"] != null ?
                    ConfigurationManager.AppSettings["AllowedCustomerClasses"] : string.Empty;
            }
        }

        private int ordersCount_New { get; set; }
        private int ordersCount_OnHold { get; set; }
        private int ordersCount_Restricted { get; set; }

        private bool isFinished_New { get; set; }
        private bool isFinished_OnHold { get; set; }
        public bool isFinished_Restricted { get; set; }
        #endregion

        public AutoCancellation()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Task.Delay(10000);

                TimeSpan tsInterval = new TimeSpan(0, 0, this.timerIntervalSecs);

                if (serviceTimer_New == null && New_Enabled.ToUpper() == "TRUE")
                {
                    this.isFinished_New = true;

                    serviceTimer_New = new Timer(new TimerCallback(SystemAutoCancellation_New), null, tsInterval, tsInterval);
                }

                if (serviceTimer_OnHold == null && OnHold_Enabled.ToUpper() == "TRUE")
                {
                    this.isFinished_OnHold = true;

                    serviceTimer_OnHold = new Timer(new TimerCallback(SystemAutoCancellation_OnHold), null, tsInterval, tsInterval);
                }

                if (serviceTimer_Restricted == null && ZoneWithoutTankers_Enabled.ToUpper()=="TRUE")
                {
                    this.isFinished_Restricted = true;

                    serviceTimer_Restricted = new Timer(new TimerCallback(SystemAutoCancellation_Restricted), null, tsInterval, tsInterval);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Start Timer : " + ex.Message.ToString());
                LoggerManager.LogMsg(c => c.Log(ex));
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (serviceTimer_New != null)
                {
                    Debug.WriteLine("Stopping Timer New");
                    serviceTimer_New.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    serviceTimer_New.Dispose();
                    serviceTimer_New = null;

                    this.ordersCount_New = 0;
                    this.isFinished_New = true;
                }

                if (serviceTimer_OnHold != null)
                {
                    Debug.WriteLine("Stopping Timer OnHold");
                    serviceTimer_OnHold.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    serviceTimer_OnHold.Dispose();
                    serviceTimer_OnHold = null;
                    
                    this.ordersCount_OnHold = 0;
                    this.isFinished_OnHold = true;
                }

                if (serviceTimer_Restricted != null)
                {
                    Debug.WriteLine("Stopping Timer Restricted");
                    serviceTimer_Restricted.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    serviceTimer_Restricted.Dispose();
                    serviceTimer_Restricted = null;

                    this.ordersCount_Restricted = 0;
                    this.isFinished_Restricted = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Stop Timer : " + ex.Message.ToString());
            }
        }

        public void SystemAutoCancellation_New(object state)
        {
            if (this.ordersCount_New == 0 && this.isFinished_New)
            {
                try
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("------------------- AutoCancellation - Cancelled New Orders - Started -------------------"));

                    this.isFinished_New = false;

                    var workOrderManager = new WorkOrderManager(new NWCContext());
                    var userDTO = workOrderManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var permittedCitiesResults = new DescriptiveResponse<IEnumerable<CityDTO>>();
                    var resultWorkOrders = new DescriptiveResponse<SearchResult<WorkOrderDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        permittedCitiesResults = workOrderManager.GetPermittedCities(this.queryAPI_URL, userDTO.Value.Token).Result;

                        if (permittedCitiesResults.Value != null)
                        {
                            int citiesCount = permittedCitiesResults.Value.Count();

                            foreach (var city in permittedCitiesResults.Value)
                            {
                                if (city.AutoCancelationNewOrdersHours > 0)
                                {
                                    var woFilter = new Filters<string>();
                                    woFilter.PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 };

                                    var searchCriteria = new WorkOrderSearchCriteriaDTO()
                                    {
                                        CancelAfterHours = city.AutoCancelationNewOrdersHours,
                                        StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.New },
                                        DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate,
                                        DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddHours(-720), // before 30 days
                                        DateTimeTo = DateTimeHelper.GetDateTimeNow().AddHours(720),
                                        CityIDs = new List<Guid>() { city.ID },
                                        FilterModel = woFilter,// after 30 days,
                                        ServiceTypeIDs = new List<int>() { (int)ServiceTypeEnum.Ashyab, (int)ServiceTypeEnum.Soqya}
                                        
                                    };

                                    resultWorkOrders = workOrderManager.GetWorkOrdersReadyToAutoCancel(this.queryAPI_URL, userDTO.Value.Token, searchCriteria, this.retrials, this.holdInterval);

                                    if (resultWorkOrders.Value != null && resultWorkOrders.Value.Result != null)
                                    {
                                        this.ordersCount_New = resultWorkOrders.Value.Result.Count;

                                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancellation: New Orders to Cancel Count: {0}", resultWorkOrders.Value.TotalCount)));

                                        foreach (var wo in resultWorkOrders.Value.Result)
                                        {
                                            var dto = new EventWorkOrderDTO()
                                            {
                                                WorkOrderID = wo.WorkOrderID,
                                                StatusTime = DateTimeHelper.GetDateTimeNow(),
                                                StatusReasonID = this.newOrderCancelationReasonID,
                                                StatusComment = "Cancelled by system"
                                            };

                                            var result = workOrderManager.CancelWorkOrder(this.commandAPI_URL, userDTO.Value.Token, dto);

                                            if (!result.IsErrorState && result.Value)
                                            {
                                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancelation: Cancel => WorkOrderId: {0}", wo.WorkOrderID)));
                                            }
                                            else
                                            {
                                                workOrderManager.UpdateWorkOrderRetrials(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.holdInterval);
                                            }

                                            this.ordersCount_New = this.ordersCount_New - 1;
                                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancelation: Total New Orders to Cancel Count: {0}", this.ordersCount_New)));
                                        }
                                    }

                                    this.isFinished_New = true;
                                }
                            }
                        }
                    }

                    this.isFinished_New = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.ordersCount_New = 0;
                    this.isFinished_New = true;
                }
            }
        }

        public void SystemAutoCancellation_OnHold(object state)
        {
            if (this.ordersCount_OnHold == 0 && this.isFinished_OnHold)
            {
                try
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("------------------- AutoCancellation - Cancelled OnHold Orders - Started -------------------"));

                    this.isFinished_OnHold = false;

                    var workOrderManager = new WorkOrderManager(new NWCContext());
                    var userDTO = workOrderManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var permittedCitiesResults = new DescriptiveResponse<IEnumerable<CityDTO>>();
                    var resultWorkOrders = new DescriptiveResponse<SearchResult<WorkOrderDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        permittedCitiesResults = workOrderManager.GetPermittedCities(this.queryAPI_URL, userDTO.Value.Token).Result;

                        if (permittedCitiesResults.Value != null)
                        {
                            int citiesCount = permittedCitiesResults.Value.Count();

                            foreach (var city in permittedCitiesResults.Value)
                            {
                                if (city.AutoCancelationOnHoldOrdersHours > 0)
                                {
                                    var woFilter = new Filters<string>();
                                    woFilter.PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 };

                                    var searchCriteria = new WorkOrderSearchCriteriaDTO()
                                    {
                                        CancelAfterHours = city.AutoCancelationOnHoldOrdersHours,
                                        StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.Onhold },
                                        DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate,
                                        DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddHours(-720), // before 30 days
                                        DateTimeTo = DateTimeHelper.GetDateTimeNow().AddHours(720),
                                        CityIDs = new List<Guid>() { city.ID },
                                        FilterModel = woFilter,// after 30 days
                                        ServiceTypeIDs = new List<int>() { (int)ServiceTypeEnum.Ashyab, (int)ServiceTypeEnum.Soqya }
                                    };

                                    resultWorkOrders = workOrderManager.GetWorkOrdersReadyToAutoCancel(this.queryAPI_URL, userDTO.Value.Token, searchCriteria, this.retrials, this.holdInterval);

                                    if (resultWorkOrders.Value != null && resultWorkOrders.Value.Result != null)
                                    {
                                        this.ordersCount_OnHold = resultWorkOrders.Value.Result.Count;

                                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancellation: OnHold Orders to Cancel Count: {0}", resultWorkOrders.Value.TotalCount)));

                                        foreach (var wo in resultWorkOrders.Value.Result)
                                        {
                                            var dto = new EventWorkOrderDTO()
                                            {
                                                WorkOrderID = wo.WorkOrderID,
                                                StatusTime = DateTimeHelper.GetDateTimeNow(),
                                                StatusReasonID = this.newOrderCancelationReasonID,
                                                StatusComment = "Cancelled by system"
                                            };

                                            var result = workOrderManager.CancelWorkOrder(this.commandAPI_URL, userDTO.Value.Token, dto);

                                            if (!result.IsErrorState && result.Value)
                                            {
                                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancelation: Cancel => WorkOrderId: {0}", wo.WorkOrderID)));
                                            }
                                            else
                                            {
                                                workOrderManager.UpdateWorkOrderRetrials(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.holdInterval);
                                            }

                                            this.ordersCount_OnHold = this.ordersCount_OnHold - 1;
                                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancelation: Total OnHold Orders to Cancel Count: {0}", this.ordersCount_OnHold)));
                                        }
                                    }

                                    this.isFinished_OnHold = true;
                                }
                            }
                        }
                    }

                    this.isFinished_OnHold = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.ordersCount_OnHold = 0;
                    this.isFinished_OnHold = true;
                }
            }
        }

        public void SystemAutoCancellation_Restricted(object state)
        {
            if (this.ordersCount_Restricted == 0 && this.isFinished_Restricted)
            {
                try
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("------------------- AutoCancellation - RestrictedZone - Started -------------------"));

                    this.isFinished_Restricted = false;

                    var workOrderManager = new WorkOrderManager(new NWCContext());
                    var userDTO = workOrderManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var resultWorkOrders = new DescriptiveResponse<SearchResult<WorkOrderDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        var custClasses = new List<int>();

                        if (!string.IsNullOrEmpty(this.AllowedCustomerClasses_config))
                        {
                            foreach (var cla in AllowedCustomerClasses_config.Split(','))
                            {
                                custClasses.Add(int.Parse(cla));
                            }
                        }

                        var woFilter = new Filters<string>();
                        woFilter.PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 };

                        var searchCriteria = new WorkOrderSearchCriteriaDTO()
                        {
                            //CancelAfterHours = this.cancelAfterHours,
                            StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.New, (int)WorkOrderStatusEnum.Assigned, (int)WorkOrderStatusEnum.Out_For_Delivery,
                                (int)WorkOrderStatusEnum.Arrived, (int)WorkOrderStatusEnum.Failed_To_Deliver, (int)WorkOrderStatusEnum.Onhold },
                            DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate,
                            //DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddHours(-720), // before 30 days
                            //DateTimeTo = DateTimeHelper.GetDateTimeNow().AddHours(720),
                            FilterModel = woFilter, // after 30 days
                            IsZoneWithoutTankers = true,
                            ClassIDs = custClasses,
                            ServiceTypeIDs = new List<int>() { (int)ServiceTypeEnum.Ashyab, (int)ServiceTypeEnum.Soqya }
                        };

                        resultWorkOrders = workOrderManager.GetWorkOrdersReadyToAutoCancel(this.queryAPI_URL, userDTO.Value.Token, searchCriteria, this.retrials, this.holdInterval);

                        if (resultWorkOrders.Value != null && resultWorkOrders.Value.Result != null)
                        {
                            this.ordersCount_Restricted = resultWorkOrders.Value.Result.Count;

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancellation - RestrictedZone: Orders to Cancel Count: {0}", resultWorkOrders.Value.TotalCount)));

                            foreach (var wo in resultWorkOrders.Value.Result)
                            {
                                var dto = new EventWorkOrderDTO()
                                {
                                    WorkOrderID = wo.WorkOrderID,
                                    StatusTime = DateTimeHelper.GetDateTimeNow(),
                                    StatusReasonID = this.zoneWithoutTankersReasonID,
                                    StatusComment = "Cancelled by system"
                                };

                                var result = workOrderManager.CancelWorkOrder(this.commandAPI_URL, userDTO.Value.Token, dto);

                                if (!result.IsErrorState && result.Value)
                                {
                                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancelation - RestrictedZone: Cancel => WorkOrderId: {0}", wo.WorkOrderID)));
                                }
                                else
                                {
                                    workOrderManager.UpdateWorkOrderRetrials(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.holdInterval);
                                }

                                this.ordersCount_Restricted = this.ordersCount_Restricted - 1;
                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AutoCancelation - RestrictedZone: Total Orders to Cancel Count: {0}", this.ordersCount_Restricted)));
                            }
                        }

                        this.isFinished_Restricted = true;
                    }

                    this.isFinished_Restricted = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.ordersCount_Restricted = 0;
                    this.isFinished_Restricted = true;
                }
            }
        }
    }
}
