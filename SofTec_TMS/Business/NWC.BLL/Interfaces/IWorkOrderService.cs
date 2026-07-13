using Infrastructure;
using NWC.DTO.Models;
using System;
using NWC.DTO.SearchCriteria;
using System.Collections.Generic;
using NWC.DTO.Common;

namespace NWC.BLL.Interfaces
{
    public interface IWorkOrderService
    {
        DescriptiveResponse<List<long>> CreateWorkOrder(EventWorkOrderDTO dto, out EventWorkOrderDTO outDto);
        DescriptiveResponse<List<long>> BulkCreateWorkOrder(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> UpdateWorkOrder(EventWorkOrderDTO dto);
        DescriptiveResponse<Boolean> UpdateWorkOrderAssignRetrials(long workOrderID, int holdInterval);

        DescriptiveResponse<bool> CreateSignalRNotification(long workOrderID);
        DescriptiveResponse<Boolean> UpdateWorkOrderCancelRetrials(long workOrderID, int holdInterval);
        DescriptiveResponse<List<long>> AssignWorkOrder(DispatchWorkOrderDTO dto);

        DescriptiveResponse<List<long>> SewerConfirmWorkOrder(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> SewerCompleteWorkOrder(EventWorkOrderDTO dto);

        DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>> GetDriverWorkOrdersPerTime(WorkOrderSearchCriteriaDTO searchCriteria);

        DescriptiveResponse<List<long>> DeassignWorkOrder(DispatchWorkOrderDTO dto);
        DescriptiveResponse<List<long>> PreAssignWorkOrder(DispatchWorkOrderDTO dto);
        DescriptiveResponse<List<long>> OutForDeliveryWorkOrder(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> ArrivedWorkOrder(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> UpdateWorkOrderPayment(WOVArrivedStationDTO dto);
        DescriptiveResponse<List<long>> WOVehicleArrivedStation(WOVArrivedStationDTO dto);
        DescriptiveResponse<List<long>> DeliveredWorkOrder(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> CancelWorkOrder(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> FailedToDeliver(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> OnHold(EventWorkOrderDTO dto);
        DescriptiveResponse<List<long>> NotAssigned(EventWorkOrderDTO dto);
        DescriptiveResponse<Boolean> AddComment(WorkOrderCommentDTO WorkOrderComment);
        DescriptiveResponse<Boolean> DeleteComment(int id);
        DescriptiveResponse<Boolean> CheckCanUpdateDeferredOrder(DeferredOrderDTO deferredOrder);
        DescriptiveResponse<Boolean> UpdateDeferredOrder(DeferredOrderDTO deferredOrder);
        DescriptiveResponse<Boolean> CancelDeferredOrder(int deferredOrderId);
        DispatchWorkOrderDTO GetMatchedWorkOrderToAssign(Guid vehicleID);
        DescriptiveResponse<Boolean> IsCustomerExceededQuota(Guid stationID, long customerID);
        DescriptiveResponse<Boolean> IsZoneWithoutTankers(long customerAccountID);

        DescriptiveResponse<Boolean> IsCustomerBlacklisted(long accountID);

        DescriptiveResponse<SearchResult<WorkOrderDTO>> SearchWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> GetWorkOrdersExceededQuota(WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> GetNotAssignedWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> GetWorkOrdersReadyToAutoCancel(WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval);
        DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>> GetSewerNewWorkOrdersWithZoneDetails();
        DescriptiveResponse<List<long>> CancelSewerWorkOrdersReadyToCancel(int? retrials, int? holdInterval);
        DescriptiveResponse<List<long>> DeAssignSewerWorkOrdersAfterTimeout(int period);
        DescriptiveResponse<List<bool>> ExitSewerVehicleAfterTimeout(int period);

        DescriptiveResponse<Double> GetEstimatedDeliveryTimeByMinute(int ZoneID, Guid StationId,int range);
        DescriptiveResponse<IEnumerable<WorkOrderDTO>> GetSewerPreAssignWorkOrdersReadyToAutoCancel(int? retrials, int? holdInterval);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDriverWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria);

         DescriptiveResponse<SearchResult<WorkOrderDTO>> GetSewerWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria);

        DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetails(long orderId);
        DescriptiveResponse<WorkOrderDTO> GetDriverWorkOrderDetails(long orderId);
        DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetailsByOrderNumber(string orderNumber);

        DescriptiveResponse<WorkOrderDTO> GetDriverWODetailsByNumber(string orderNumber);

        DescriptiveResponse<IEnumerable<WorkOrderCommentDTO>> GetWorkOrderComments(long OrderId);
        DescriptiveResponse<IEnumerable<WorkOrderComplaintDTO>> GetWorkOrderComplaints(long OrderId);
        DescriptiveResponse<IEnumerable<WorkOrderStatusLogDTO>> GetWorkOrderStatusLogs(long workOrderId);
        DescriptiveResponse<IEnumerable<WorkOrderChangeLogDTO>> GetWorkOrderChangeLogs(long workOrderId);
        DescriptiveResponse<IEnumerable<AccessoryDTO>> GetWorkOrderAccessory(long workOrderId);
        DescriptiveResponse<IEnumerable<WorkOrderTransactionDTO>> GetWorkOrderPayments(long workOrderId);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> GetAssignableWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<SearchResult<DailyOrderSummaryDTO>> GetDailyOrderSummaryReport(DailyOrderReportSC searchCriteria);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDailyOrderDetailsReport(WorkOrderSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<SearchResult<DeferredOrderDTO>> SearchDeferredWorkOrders(DeferredOrderSC searchCriteria);

        DescriptiveResponse<int> GetNoOfOrdersForThisMonth(long customerAccount);

        DescriptiveResponse<SearchResult<HayatWorkOrderLogDTO>> GetHayatWorkOrderLogs(HayatWorkOrderLogsSC searchCriteria, int retrials, int holdInterval);
        DescriptiveResponse<Boolean> UpdateHayatWorkOrderLog(HayatWorkOrderLogDTO dto);


        DescriptiveResponse<Boolean> IsZoneWithoutTankersByZoneInt(string zoneIntegrationID);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> SearchSewerWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<WorkOrderDTO> GetSewerOrderBasicDetails(long orderId);
        DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDriverSewerWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria);

         DescriptiveResponse<List<AvailableTankerSizesDTO>> CalculateWorkOrderCost(CostDTO costDTO);


    }
}
