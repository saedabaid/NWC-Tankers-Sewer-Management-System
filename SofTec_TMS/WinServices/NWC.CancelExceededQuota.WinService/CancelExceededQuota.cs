using NWC.CancelExceededQuota.WinService.Managers;
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

namespace NWC.CancelExceededQuota.WinService
{
    public partial class CancelExceededQuota : ServiceBase
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
        private int maxQuotaHours
        {
            get
            {
                int _maxQuotaHours;
                int.TryParse(ConfigurationManager.AppSettings["MaxQuotaHours"] != null ?
                    ConfigurationManager.AppSettings["MaxQuotaHours"] : string.Empty, out _maxQuotaHours);

                return _maxQuotaHours;
            }
        }
        private int statusReasonID
        {
            get
            {
                int _statusReasonID;
                int.TryParse(ConfigurationManager.AppSettings["StatusReasonID"] != null ?
                    ConfigurationManager.AppSettings["StatusReasonID"] : string.Empty, out _statusReasonID);

                return _statusReasonID;
            }
        }

        private string AllowedCustomerClasses
        {
            get
            {
                return ConfigurationManager.AppSettings["AllowedCustomerClasses"] != null ?
                    ConfigurationManager.AppSettings["AllowedCustomerClasses"] : string.Empty;
            }
        }

        private int citiesCount { get; set; }
        private int ordersCount { get; set; }
        private bool isFinished { get; set; }
        #endregion

        public CancelExceededQuota()
        {
            InitializeComponent();
        }

        #region Events
        protected override void OnStart(string[] args)
        {
            try
            {
                Task.Delay(10000);

                if (serviceTimer == null)
                {
                    this.isFinished = true;

                    TimeSpan tsInterval = new TimeSpan(0, 0, this.timerIntervalSecs);

                    serviceTimer = new Timer(new TimerCallback(SystemCancelExceededQuota), null, tsInterval, tsInterval);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Start Timer : " + ex.Message.ToString());
            }
        }

        public void SystemCancelExceededQuota(object state)
        {
            if (this.ordersCount == 0 && this.citiesCount == 0 && this.isFinished)
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------CancelExceededQuota - Started----------------------------"));

                try
                {
                    this.isFinished = false;

                    var workOrderManager = new WorkOrderManager(new NWCContext());
                    var userDTO = workOrderManager.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var permittedCitiesResults = new DescriptiveResponse<IEnumerable<CityDTO>>();
                    var workOrdersResults = new DescriptiveResponse<SearchResult<WorkOrderDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        permittedCitiesResults = workOrderManager.GetPermittedCities(this.queryAPI_URL, userDTO.Value.Token).Result;

                        if (permittedCitiesResults.Value != null)
                        {
                            this.citiesCount = permittedCitiesResults.Value.Count();

                            foreach (var city in permittedCitiesResults.Value)
                            {
                                var custClasses = new List<int>();

                                if (!string.IsNullOrEmpty(this.AllowedCustomerClasses))
                                {
                                    foreach (var cla in AllowedCustomerClasses.Split(','))
                                    {
                                        custClasses.Add(int.Parse(cla));
                                    }
                                }

                                var woFilter = new Filters<string>();
                                woFilter.PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 };

                                var searchCriteria = new WorkOrderSearchCriteriaDTO()
                                {
                                    StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.New, (int)WorkOrderStatusEnum.Assigned, (int)WorkOrderStatusEnum.Onhold,
                                                                    (int)WorkOrderStatusEnum.Arrived, (int)WorkOrderStatusEnum.Out_For_Delivery, (int)WorkOrderStatusEnum.Delivered },
                                    DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate,
                                    DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddHours(city.TankerQuotaNo > 0 ? -city.TankerQuotaNo : -this.maxQuotaHours),
                                    DateTimeTo = DateTimeHelper.GetDateTimeNow().AddDays(1),
                                    CityIDs = new List<Guid>() { city.ID },
                                    ClassIDs = custClasses,
                                    FilterModel = woFilter,
                                    ServiceTypeIDs = new List<int>() { (int)ServiceTypeEnum.Ashyab, (int)ServiceTypeEnum.Soqya }
                                };

                                workOrdersResults = workOrderManager.GetWorkOrders(this.queryAPI_URL, userDTO.Value.Token, searchCriteria, this.retrials, this.holdInterval).Result;

                                if (workOrdersResults.Value != null && workOrdersResults.Value.Result != null)
                                {
                                    var workOrders = workOrdersResults.Value.Result;

                                    var results = from p in workOrders
                                                  group p by p.CustomerID into g
                                                  select new { CustomerName = g.Key, WorkOrders = g };

                                    var woListToCancel = new List<WorkOrderDTO>();

                                    foreach (var woGroup in results)
                                    {
                                        if (woGroup.WorkOrders.Count() > 1)
                                        {
                                            var firstWorkOrder = new WorkOrderDTO();
                                            firstWorkOrder = woGroup.WorkOrders.OrderBy(x => x.ScheduledDeliveryTime).Where(x => x.LastStatusID == (int)WorkOrderStatusEnum.Delivered).FirstOrDefault();

                                            if (firstWorkOrder == null)
                                                firstWorkOrder = woGroup.WorkOrders.OrderBy(x => x.ScheduledDeliveryTime).FirstOrDefault();

                                            foreach (var wo in woGroup.WorkOrders)
                                            {
                                                if (wo != firstWorkOrder && !woListToCancel.Contains(wo))
                                                {
                                                    woListToCancel.Add(wo);
                                                }
                                            }
                                        }
                                    }

                                    this.ordersCount = woListToCancel.Count;

                                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CancelExceededQuota: Total WorkOrder Needs To Cancel Count: {0}", this.ordersCount)));

                                    foreach (var wo in woListToCancel)
                                    {
                                        if (new int[] { (int)WorkOrderStatusEnum.New, (int)WorkOrderStatusEnum.Assigned, (int)WorkOrderStatusEnum.Onhold }.Contains(wo.LastStatusID.Value))
                                        {
                                            var cancelResult = workOrderManager.CancelWorkOrder(this.commandAPI_URL, userDTO.Value.Token, wo.WorkOrderID, this.statusReasonID, "").Result;

                                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CancelExceededQuota: WorkOrderID: {0} Cancelled: {1}", wo.WorkOrderID, cancelResult.Value)));

                                            if (cancelResult.Value)
                                            {
                                            }
                                        }

                                        this.ordersCount--;
                                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CancelExceededQuota: Total WorkOrder Needs To Cancel Count: {0}", this.ordersCount)));
                                    }

                                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CancelExceededQuota: Total WorkOrder Needs To Cancel Count: {0}", this.ordersCount)));
                                }

                                this.isFinished = true;

                                citiesCount--;
                            }
                        }
                    }

                    LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------CancelExceededQuota - Finished----------------------------"));

                    this.isFinished = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.citiesCount = 0;
                    this.ordersCount = 0;
                    this.isFinished = true;
                }
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
        #endregion
    }
}
