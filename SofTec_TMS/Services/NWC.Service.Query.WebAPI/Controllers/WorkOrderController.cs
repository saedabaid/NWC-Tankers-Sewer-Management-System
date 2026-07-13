using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Models;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using NWC.DTO.Common;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.BLL.Services;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/WorkOrder")]
    public class WorkOrderController : ApiControllerBase
    {
        private IWorkOrderService _WorkOrderServiceService;

        public WorkOrderController()
        {
            _WorkOrderServiceService = new WorkOrderService(loggedInService);
        }

        [Route("Search")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> Search([FromBody] WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.SearchWorkOrders(searchCriteria));
        }

        [Route("CalculateWorkOrderCost")]
        [HttpPost]
        public DescriptiveResponse<List<AvailableTankerSizesDTO>> CalculateWorkOrderCost(CostDTO costDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<AvailableTankerSizesDTO>>
                .Try(() => _WorkOrderServiceService.CalculateWorkOrderCost(costDTO));
        }



        [Route("GetWorkOrdersExceededQuota")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetWorkOrdersExceededQuota([FromBody] WorkOrderSearchCriteriaDTO searchCriteria, [FromUri] int retrials, [FromUri] int holdInterval)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.GetWorkOrdersExceededQuota(searchCriteria, retrials, holdInterval));
        }

        [Route("GetNotAssignedWorkOrders")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetNotAssignedWorkOrders([FromBody] WorkOrderSearchCriteriaDTO searchCriteria, [FromUri] int retrials, [FromUri] int holdInterval)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.GetNotAssignedWorkOrders(searchCriteria, retrials, holdInterval));
        }

        [Route("GetWorkOrdersReadyToAutoCancel")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetWorkOrdersReadyToAutoCancel([FromBody] WorkOrderSearchCriteriaDTO searchCriteria, [FromUri] int retrials, [FromUri] int holdInterval)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.GetWorkOrdersReadyToAutoCancel(searchCriteria, retrials, holdInterval));
        }

        

        [Route("GetDriverWorkOrders")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDriverWorkOrders([FromBody] WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.GetDriverWorkOrders(searchCriteria));
        }

        [Route("GetOrderBasicDetails")]
        [HttpGet]
        public DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetails(long orderId)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetOrderBasicDetails(orderId);
        }

        [Route("GetDriverWorkOrderDetails")]
        [HttpGet]
        public DescriptiveResponse<WorkOrderDTO> GetDriverWorkOrderDetails(long orderId)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetDriverWorkOrderDetails(orderId);
        }

        [Route("GetOrderBasicDetailsByOrderNumber")]
        [HttpGet]
        public DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetailsByOrderNumber(string orderNumber)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetOrderBasicDetailsByOrderNumber(orderNumber);
        }

        [Route("GetDriverWODetailsByNumber")]
        [HttpGet]
        public DescriptiveResponse<WorkOrderDTO> GetDriverWODetailsByNumber(string orderNumber)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetDriverWODetailsByNumber(orderNumber);
        }

        [Route("GetWorkOrderComments")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<WorkOrderCommentDTO>> GetWorkOrderComments(long orderId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<WorkOrderCommentDTO>>
                .Try(() => _WorkOrderServiceService.GetWorkOrderComments(orderId));
        }

        [Route("GetWorkOrderComplaints")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<WorkOrderComplaintDTO>> GetWorkOrderComplaints(long orderId)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetWorkOrderComplaints(orderId);
        }

        [Route("GetWorkOrderStatusLogs")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<WorkOrderStatusLogDTO>> GetWorkOrderStatusLogs(long workOrderId)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetWorkOrderStatusLogs(workOrderId);
        }

        [Route("GetWorkOrderChangeLogs")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<WorkOrderChangeLogDTO>> GetWorkOrderChangeLogs(long workOrderId)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetWorkOrderChangeLogs(workOrderId);
        }

        [Route("GetWorkOrderAccessory")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<AccessoryDTO>> GetWorkOrderAccessory(long workOrderId)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetWorkOrderAccessory(workOrderId);
        }

        [Route("GetWorkOrderPayments")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<WorkOrderTransactionDTO>> GetWorkOrderPayments(long workOrderID)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetWorkOrderPayments(workOrderID);
        }

        [Route("GetAssignableWorkOrders")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetAssignableWorkOrders([FromBody] WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetAssignableWorkOrders(searchCriteria);
        }

        [Route("GetDailyOrderSummaryReport")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<DailyOrderSummaryDTO>> GetDailyOrderSummaryReport([FromBody] DailyOrderReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<DailyOrderSummaryDTO>>
                .Try(() => _WorkOrderServiceService.GetDailyOrderSummaryReport(searchCriteria));
        }

        [Route("GetDailyOrderDetailsReport")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDailyOrderDetailsReport([FromBody] WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.GetDailyOrderDetailsReport(searchCriteria));
        }

        [Route("SearchDeferredWorkOrders")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<DeferredOrderDTO>> SearchDeferredWorkOrders([FromBody] DeferredOrderSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<DeferredOrderDTO>>
                .Try(() => _WorkOrderServiceService.SearchDeferredWorkOrders(searchCriteria));
        }

        [Route("IsCustomerBlacklisted")]
        [HttpGet]
        public DescriptiveResponse<Boolean> IsCustomerBlacklisted(long accountID)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.IsCustomerBlacklisted(accountID);
        }

        [Route("IsCustomerExceededQuota")]
        [HttpGet]
        public DescriptiveResponse<Boolean> IsCustomerExceededQuota(Guid stationID, long customerID)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.IsCustomerExceededQuota(stationID, customerID);
        }

        [Route("GetEstimatedDeliveryTimeByMinute")]
        [HttpGet]
        public DescriptiveResponse<Double> GetEstimatedDeliveryTimeByMinute( int zoneID, Guid stationID,int range )
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetEstimatedDeliveryTimeByMinute(zoneID,stationID,range);
        }

        [Route("IsZoneWithoutTankers")]
        [HttpGet]
        public DescriptiveResponse<Boolean> IsZoneWithoutTankers(long customerAccountID)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.IsZoneWithoutTankers(customerAccountID);
        }

        [Route("GetNoOfOrdersForThisMonth")]
        [HttpGet]
        public DescriptiveResponse<int> GetNoOfOrdersForThisMonth(long customerAccountID)
        {
            OnActionExecuting();

            return DescriptiveResponse<int>.Try(() =>
                _WorkOrderServiceService.GetNoOfOrdersForThisMonth(customerAccountID)
            );
        }

        [Route("GetHayatWorkOrderLogs")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<HayatWorkOrderLogDTO>> GetHayatWorkOrderLogs([FromBody] HayatWorkOrderLogsSC searchCriteria, int retrials, int holdInterval)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<HayatWorkOrderLogDTO>>
                .Try(() => _WorkOrderServiceService.GetHayatWorkOrderLogs(searchCriteria, retrials, holdInterval));
        }


        [Route("IsZoneWithoutTankersByZoneInt")]
        [HttpGet]
        public DescriptiveResponse<Boolean> IsZoneWithoutTankersByZoneInt(string zoneIntegrationID)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.IsZoneWithoutTankersByZoneInt(zoneIntegrationID);
        }


        [Route("GetSewerWorkOrders")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetSewerWorkOrders([FromBody] WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.GetSewerWorkOrders(searchCriteria));
        }

        [Route("SearchSewer")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> SearchSewerWorkOrders([FromBody] WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.SearchSewerWorkOrders(searchCriteria));
        }

        [Route("GetSewerOrderBasicDetails")]
        [HttpGet]
        public DescriptiveResponse<WorkOrderDTO> GetSewerOrderBasicDetails(long orderId)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetSewerOrderBasicDetails(orderId);
        }

        [Route("GetDriverSewerWorkOrders")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDriverSewerWorkOrders([FromBody] WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return _WorkOrderServiceService.GetDriverSewerWorkOrders(searchCriteria);
        }

        [Route("GetSewerNewWorkOrdersWithZoneDetails")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>> GetSewerNewWorkOrdersWithZoneDetails()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>
                .Try(() => _WorkOrderServiceService.GetSewerNewWorkOrdersWithZoneDetails());
        }

        [Route("GetSewerPreAssignWorkOrdersReadyToAutoCancel")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<WorkOrderDTO>> GetSewerPreAssignWorkOrdersReadyToAutoCancel([FromUri] int? retrials, [FromUri] int? holdInterval)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<WorkOrderDTO>>
                .Try(() => _WorkOrderServiceService.GetSewerPreAssignWorkOrdersReadyToAutoCancel(retrials, holdInterval));
        }

        [Route("GetDriverWorkOrdersPerTime")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>> GetDriverWorkOrdersPerTime([FromBody] WorkOrderSearchCriteriaDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>>
                .Try(() => _WorkOrderServiceService.GetDriverWorkOrdersPerTime(dto));
        }
    }
}