using Hangfire;
using Microsoft.Extensions.Configuration;
using NWC.BLL.Interfaces;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWC.Hangfire.Jobs
{
    public class RecurringJobs
    {
        private int take
        {
            get
            {
                int _take;
                int.TryParse(Configuration["Take"] != null ? Configuration["Take"] : string.Empty, out _take);

                return _take > 0 ? _take : 10;
            }
        }
        private int timerIntervalSecs
        {
            get
            {
                int _interval;
                int.TryParse(Configuration["TimerIntervalSecs"] != null ? Configuration["TimerIntervalSecs"] : string.Empty, out _interval);

                return _interval > 0 ? _interval : 10;
            }
        }
        private int retrials
        {
            get
            {
                int _retrials;
                int.TryParse(Configuration["Retrials"] != null ? Configuration["Retrials"] : string.Empty, out _retrials);

                return _retrials;
            }
        }
        private int holdIntervalMinutes
        {
            get
            {
                int _holdInterval;
                int.TryParse(Configuration["HoldIntervalMinutes"] != null ? Configuration["HoldIntervalMinutes"] : string.Empty, out _holdInterval);

                return _holdInterval;
            }
        }
        private int dateFromInHours
        {
            get
            {
                int _dateFromInHours;
                int.TryParse(Configuration["DateFromInHours"] != null ? Configuration["DateFromInHours"] : string.Empty, out _dateFromInHours);

                return _dateFromInHours > 0 ? _dateFromInHours : 72;
            }
        }

        private int ordersCount { get; set; }
        private bool isFinished { get; set; } = true;

        private readonly IRecurringJobManager _recurringJobManager;
        private IConfiguration Configuration { get; }
        private readonly IWorkOrderService _workOrderService;
        private readonly IWorkOrderVehicleService _workOrderVehicleService;

        public RecurringJobs(IRecurringJobManager recurringJobManager
            , IConfiguration configuration
            , IWorkOrderService workOrderService
            , IWorkOrderVehicleService workOrderVehicleService)
        {
            _recurringJobManager = recurringJobManager;
            Configuration = configuration;
            _workOrderService = workOrderService;
            _workOrderVehicleService = workOrderVehicleService;
        }

        public void Register()
        {
            _recurringJobManager.AddOrUpdate("auto-assign", () => AutoAssign(), Configuration["AutoAssignCronSchedule"]);
        }

        public void AutoAssign()
        {
            if (ordersCount == 0 && isFinished)
            {
                try
                {
                    isFinished = false;

                    var resultWorkOrders = new DescriptiveResponse<SearchResult<WorkOrderDTO>>();

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

                    resultWorkOrders = _workOrderService.GetNotAssignedWorkOrders(searchCriteria, this.retrials, this.holdIntervalMinutes);

                    if (resultWorkOrders.Value != null && resultWorkOrders.Value.Result != null)
                    {
                        ordersCount = resultWorkOrders.Value.TotalCount;

                        foreach (var wo in resultWorkOrders.Value.Result)
                        {
                            var filter = new WorkOderFilter() { OrderId = wo.WorkOrderID, PageFilter = new PageFilter() { PageIndex = 0, PageSize = 2 } };

                            var vehicleResult = _workOrderVehicleService.GetAssignableVehicles(filter);

                            if (!vehicleResult.IsErrorState && vehicleResult.Value != null && vehicleResult.Value.Result != null && vehicleResult.Value.Result.Any())
                            {
                                foreach (var veh in vehicleResult.Value.Result)
                                {
                                    var dto = new DispatchWorkOrderDTO()
                                    {
                                        EventWorkOrderDTO = new EventWorkOrderDTO() { WorkOrderID = wo.WorkOrderID },
                                        EventWorkOrderVehicleDTO = new EventWorkOrderVehicleDTO() { VehicleID = veh.VehicleID, DriverID = veh.DriverID }
                                    };

                                    var result = _workOrderService.AssignWorkOrder(dto);

                                    if (result.IsErrorState || result.Value == null || !result.Value.Any())
                                    {
                                        var updateResult = _workOrderService.UpdateWorkOrderAssignRetrials(wo.WorkOrderID, this.holdIntervalMinutes);
                                    }
                                }
                            }
                            else
                            {
                                var updateResult = _workOrderService.UpdateWorkOrderAssignRetrials(wo.WorkOrderID, this.holdIntervalMinutes);
                            }
                            ordersCount--;
                        }
                        isFinished = true;
                    }
                    isFinished = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ordersCount = 0;
                    isFinished = true;
                }
            }
        }
    }
}
