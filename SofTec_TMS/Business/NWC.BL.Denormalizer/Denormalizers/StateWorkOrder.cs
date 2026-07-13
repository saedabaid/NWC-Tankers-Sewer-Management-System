using Infrastructure;
using Newtonsoft.Json;
using NWC.BL.Denormalizer.Converters;
using NWC.BL.Denormalizer.CoreBusiness;
using NWC.BL.Denormalizer.OutServices;
using NWC.BL.Denormalizer.UpdateWONotificationServiceRef;
using NWC.DAL.NWCEntities;
using NWC.DTO;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer.Denormalizers
{
    public class StateWorkOrder : IStateDenormalizer
    {
        public NWCContext Context { get; private set; }

        public List<Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>> StateFunctions;

        #region Constructors
        public StateWorkOrder() : this(null)
        {

        }

        internal StateWorkOrder(NWCContext ctx)
        {
            if (ctx == null)
                Context = new NWCContext();
            else
                Context = ctx;

            StateFunctions = new List<Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>>();

            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyCreateOrderEvent, ApplyCreateOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyUpdateOrderEvent, ApplyUpdateOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyUpdateOrderPaymentEvent, ApplyUpdateOrderPaymentEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyAssignWorkOrderEvent, ApplyAssignWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyDeassignWorkOrderEvent, ApplyDeassignWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyOutForDeliveryWorkOrderEvent, ApplyOutForDeliveryWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyArrivedWorkOrderEvent, ApplyArrivedWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyDeliveredWorkOrderEvent, ApplyDeliveredWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyCancelledWorkOrderEvent, ApplyCancelledWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyFailedToDeliverWorkOrder, ApplyFailedToDeliverWorkOrder));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyOnHoldWorkOrder, ApplyOnHoldWorkOrder));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyNotAssigned, ApplyNotAssigned));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyWOVArrivedStationEvent, ApplyWOVArrivedStationEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerConfirmEvent, ApplySewerConfirmEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerCompleteEvent, ApplySewerCompleteEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerAssignWorkOrderEvent, ApplySewerAssignWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerCancelledWorkOrderEvent, ApplySewerCancelledWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerDumpingEvent, ApplySewerDumpingEvent));

        }
        #endregion

        public DescriptiveResponse<Boolean> ApplyEvent(long eventID, DescriptiveResponse<Boolean> result)
        {
            var e = Context.NWC_Event.FirstOrDefault(o => o.ID == eventID);

            var newState =
                StateFunctions.
                Where(a => a.Item1(e)).
                Select(a => a.Item2(e, result)).
                FirstOrDefault();

            return result;
        }

        #region Sewer
        private bool CanApplySewerAssignWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.SW_Assign)
                return true;

            return false;
        }
        private DescriptiveResponse<Boolean> ApplySewerAssignWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------ApplySewerAssignWorkOrderEvent - Denormalizer - Start----------------------------------------"));

                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Assigned))
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerAssignWorkOrderEvent - Denormalizer - WorkOrder Status Workflow Fail - OrderNumber: {stateWorkOrder.OrderNumber} - WorkOrderStatusID: {stateWorkOrder.LastStatusID}"));

                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusBy = e.UserID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.IsAssigned = true;
                stateWorkOrder.AssignedVehicleID = eventWorkOrder.VehicleID;
                stateWorkOrder.AssignedDriverID = eventWorkOrder.DriverID;
                stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Assign,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerAssignWorkOrderEvent - WorkOrder Denormalizer - Assign Successfully - OrderNumber: {stateWorkOrder.OrderNumber} - TransporterID: {stateWorkOrder.AssignedVehicleID} - TransporterStatus: {stateWorkOrder.VehicleStatusID}"));

                result.Value = true;

                #region Calling Driver SMS service
                var driverSMSDTO = new DriverSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    VehicleID = stateWorkOrder.AssignedVehicleID.Value,
                    DriverID = stateWorkOrder.AssignedDriverID.Value,
                    //CustomerMobile = stateWorkOrder.RecieverMobile,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId
                };

                SendDriverSMS(driverSMSDTO);
                #endregion

                #region Calling Customer SMS service
                var customerSMSDTO = new CustomerSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    VehicleID = stateWorkOrder.AssignedVehicleID.Value,
                    DriverID = stateWorkOrder.AssignedDriverID.Value,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId
                };

                SendCustomerSMS(customerSMSDTO,SMSType.OutForDelivery,(int)ServiceTypeEnum.SewageRemoval);
                #endregion

                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item5, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID);
                }

                LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------ApplySewerAssignWorkOrderEvent - Denormalizer - End----------------------------------------"));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplySewerAssignWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        //========================================
        private bool CanApplySewerConfirmEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.SW_Confirm)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplySewerConfirmEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);
                //Validate WorkOrder Status Workflow
                //TODO : need to check
                if (stateWorkOrder == null || stateWorkOrder.LastStatusID != (int)WorkOrderStatusEnum.Assigned)
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusBy = e.UserID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_OutForDelivery,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;

                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item6, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyOutForDeliveryWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }


        //========================================

        private bool CanApplySewerCompleteEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.SW_Complete)
                return true;

            return false;
        }
   
        private DescriptiveResponse<Boolean> ApplySewerCompleteEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);
                //Validate WorkOrder Status Workflow
                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusBy = e.UserID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Delivered,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;

                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item4, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID);
                }


                var customerSMSDTO = new CustomerSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    //VehicleID = newOrderState.AssignedVehicleID.Value,
                    //DriverID = newOrderState.AssignedDriverID.Value,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId,
                    ConfirmationCode = eventWorkOrder.ConfirmationCode
                };

                SendCustomerSMS(customerSMSDTO, SMSType.Delivered, (int)ServiceTypeEnum.SewageRemoval);
            }

            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyOutForDeliveryWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        //========================================
        private bool CanApplySewerCancelledWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.SW_Cancelled)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplySewerCancelledWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                //if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Cancelled))
                //{
                //    result.Value = false;
                //    return result;
                //}

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                var statusReason = Context.NWC_StatusReason.FirstOrDefault(x => x.ID == eventWorkOrder.StatusReasonID);

                //Updating StateWorkOrder
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusReason = statusReason != null ? statusReason.ReasonAr : string.Empty;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;

                stateWorkOrder.IsAssigned = false;
                stateWorkOrder.AssignedVehicleID = null;
                stateWorkOrder.AssignedDriverID = null;
                if (eventWorkOrder.VehicleStatusID.HasValue)
                {
                    stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID.GetValueOrDefault();
                }

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Cancelled,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude,
                    StatusReasonId = eventWorkOrder.StatusReasonID
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;

                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item8, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID, stateWorkOrder.LastStatusComment, reasonIntegrationId: statusReason.IntegrationId);
                }
                var customerSMSDTO = new CustomerSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    //VehicleID = newOrderState.AssignedVehicleID.Value,
                    //DriverID = newOrderState.AssignedDriverID.Value,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId,
                    ConfirmationCode = eventWorkOrder.ConfirmationCode
                };

                SendCustomerSMS(customerSMSDTO, SMSType.Cancel, (int)ServiceTypeEnum.SewageRemoval);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplySewerCancelledWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }
        #endregion


        private bool CanApplyCreateOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_Create)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyCreateOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                decimal distance = 0M;
                var customerAccount = Context.NWC_CustomerAccount.Where(x => x.ID == eventWorkOrder.CustomerAccountId).FirstOrDefault();
                //var customerLocation = Context.NWC_CustomerLocation.Where(x => x.ID == eventWorkOrder.CustomerLocationID).FirstOrDefault();

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("customerAccount: {0}", eventWorkOrder.CustomerAccountId)));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("customerLocation: {0}", customerAccount.ID)));

                var zoneStation = Context.NWC_ZoneStations.Where(x => x.StationID == eventWorkOrder.StationID && x.ZoneID == customerAccount.NWC_CustomerLocation.ZoneID).FirstOrDefault();
                distance = (zoneStation != null && zoneStation.Distance.HasValue) ? zoneStation.Distance.Value : 0M;

                LoggerManager.LogMsg(c => c.TrackingMsg("Denormalizer---------------------------------------------------------------------------------------------------------------------------"));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("OrderNumber: {0}", eventWorkOrder.OrderNumber)));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("workOrderID: {0}", eventWorkOrder.EventOrderID)));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("scheduledDeliveryTime: {0}", eventWorkOrder.ScheduledDeliveryTime.Value)));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("customerLocationClassID: {0}", customerAccount.NWC_CustomerLocation.NWC_CustomerLocationClass.ID)));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("serviceTypeID: {0}", customerAccount.ServiceTypeId)));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("zoneStation: {0}", zoneStation.StationID)));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("orderQuantity: {0}", eventWorkOrder.OrderQuantity.Value)));

                decimal netCost = WorkOrderCost.CalculateWorkOrderCost(eventWorkOrder.EventOrderID, eventWorkOrder.ScheduledDeliveryTime.Value, customerAccount.NWC_CustomerLocation.NWC_CustomerLocationClass.ID,
                    customerAccount.ServiceTypeId, zoneStation, eventWorkOrder.OrderQuantity.Value);

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Net Cost: {0}", netCost)));

                //n cost + 5% VAT tax
                decimal vat = WorkOrderCost.CalculateVat(netCost);
                decimal totalCost = netCost > -1 ? (netCost + vat) : -1;

                string encryptedCinfermationCode = GetEncryptedConfirmationCode(eventWorkOrder.ConfirmationCode);
                string invoiceNumber = GetInvoiceNumber();

                var newOrderState = new NWC_StateWorkOrder()
                {
                    WorkOrderId = eventWorkOrder.EventOrderID,
                    OrderNumber = eventWorkOrder.OrderNumber,
                    CreateTime = e.EventTime,
                    CreatedBy = e.UserID,
                    LastStatusID = (int)WorkOrderStatusEnum.New,
                    LastStatusTime = eventWorkOrder.StatusTime,
                    LastStatusBy = eventWorkOrder.CreatedBy,
                    LastModifiedTime = eventWorkOrder.CreateTime,
                    LastModifiedBy = eventWorkOrder.CreatedBy,
                    OrderQuantity = eventWorkOrder.OrderQuantity.Value,
                    ScheduledDeliveryTime = eventWorkOrder.ScheduledDeliveryTime.Value,
                    RequestTime = eventWorkOrder.RequestTime,
                    //CustomerLocationID = eventWorkOrder.CustomerLocationID.Value,
                    ServiceTypeID = eventWorkOrder.ServiceTypeID == null ? 1 : eventWorkOrder.ServiceTypeID,
                    CustomerAccountId = eventWorkOrder.CustomerAccountId,
                    NetCost = netCost,
                    TotalCost = totalCost,
                    AssignedStationID = eventWorkOrder.StationID.Value,
                    IsAssigned = false,
                    IsDeleted = false,
                    SubID = eventWorkOrder.SubID,
                    ConfirmationCode = eventWorkOrder.ConfirmationCode,
                    EncryptedConfirmationCode = encryptedCinfermationCode,
                    Distance = distance,
                    AccessoriesAr = string.Join(", ", eventWorkOrder.NWC_WorkOrderAccessory.Select(x => x.NWC_Accessory.NameAr).ToArray()),
                    AccessoriesEn = string.Join(", ", eventWorkOrder.NWC_WorkOrderAccessory.Select(x => x.NWC_Accessory.NameEn).ToArray()),
                    RecieverName = eventWorkOrder.RecieverName,
                    RecieverMobile = eventWorkOrder.RecieverMobile,
                    Comments = eventWorkOrder.Comments,
                    InvoiceNo = invoiceNumber,
                    InvoiceStatusID = (int)WorkOrderInvoiceStatusEnum.Un_Paid,
                    AssignRetrialTime = DateTimeHelper.GetDateTimeNow(),
                    AssignRetrials = 0,
                    CancelRetrialTime = DateTimeHelper.GetDateTimeNow(),
                    CancelRetrials = 0,
                    SourceApplication = eventWorkOrder.SourceApplication,
                    CISDivision = eventWorkOrder.CISDivision,
                    TransactionID = eventWorkOrder.TransactionID,
                    CategoryID = eventWorkOrder.CategoryID,
                    PriorityID=eventWorkOrder.PriorityID
                };

                Context.NWC_StateWorkOrder.Add(newOrderState);

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Create,
                    WorkOrderId = newOrderState.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = newOrderState.LastStatusID,
                    StatusTime = newOrderState.LastStatusTime,
                    StatusComment = newOrderState.LastStatusComment,
                    OrderQuantity = newOrderState.OrderQuantity,
                    //CustomerLocationID = newOrderState.CustomerLocationID,
                    ServiceTypeID = newOrderState.ServiceTypeID,
                    CustomerAccountId = newOrderState.CustomerAccountId,
                    NetCost = newOrderState.NetCost,
                    TotalCost = newOrderState.TotalCost,
                    Distance = newOrderState.Distance,
                    AccessoriesAr = newOrderState.AccessoriesAr,
                    AccessoriesEn = newOrderState.AccessoriesEn,
                    RecieverName = newOrderState.RecieverName,
                    RecieverMobile = newOrderState.RecieverMobile,
                    Comments = newOrderState.Comments,
                    StationID = newOrderState.AssignedStationID,
                    ScheduledDeliveryTime = newOrderState.ScheduledDeliveryTime,
                    SourceApplication = eventWorkOrder.SourceApplication,
                    CISDivision = eventWorkOrder.CISDivision,
                    TransactionID = eventWorkOrder.TransactionID,
                    CategoryID = eventWorkOrder.CategoryID
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                // Update schedule if service type Soqya
                if (customerAccount.ServiceTypeId == 2)
                {
                    var date = eventWorkOrder.ScheduledDeliveryTime.Value.Date;
                    var soqyaSchedule = Context.NWC_SoqyaSchedules.Where(x => x.CustomerAccountId == eventWorkOrder.CustomerAccountId &&
                                            x.ScheduleDate.Year == date.Year && x.ScheduleDate.Month == date.Month && x.ScheduleDate.Day == date.Day).FirstOrDefault();

                    if (soqyaSchedule != null)
                        soqyaSchedule.WorkOrderId = newOrderState.WorkOrderId;
                }

                Context.SaveChanges();

                //Add Comment
                if (totalCost < 0)
                    AddWorkOrderComment(eventWorkOrder.EventOrderID, e.UserID, "Failed to calculate order price");

                #region Calling Customer SMS service
                if (!string.IsNullOrEmpty(newOrderState.SourceApplication) && newOrderState.SourceApplication == "TMS")
                {
                    var customerSMSDTO = new CustomerSMSDTO
                    {
                        OrderNumber = newOrderState.OrderNumber,
                        //VehicleID = newOrderState.AssignedVehicleID.Value,
                        //DriverID = newOrderState.AssignedDriverID.Value,
                        CustomerLocationID = newOrderState.NWC_CustomerAccount.CustomerLocationId,
                        CustomerID = newOrderState.NWC_CustomerAccount.CustomerId,
                        ConfirmationCode = newOrderState.ConfirmationCode
                    };

                    SendCustomerSMS(customerSMSDTO,SMSType.Create, (int)newOrderState.ServiceTypeID);
                }
                #endregion

                result.Value = true;
                LoggerManager.LogMsg(c => c.TrackingMsg("Create WorkOrder: Success"));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyCreateOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyUpdateOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_Update)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyUpdateOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();
                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                decimal distance = 0M;
                var customerAccount = Context.NWC_CustomerAccount
                   .Where(x => x.ID == eventWorkOrder.CustomerAccountId).FirstOrDefault();

                var zoneStation = Context.NWC_ZoneStations.Where(x => x.StationID == eventWorkOrder.StationID && x.ZoneID == customerAccount.NWC_CustomerLocation.ZoneID).FirstOrDefault();
                distance = (zoneStation != null && zoneStation.Distance.HasValue) ? zoneStation.Distance.Value : 0M;

                decimal netCost = -1;

                if (zoneStation != null)
                    netCost = WorkOrderCost.CalculateWorkOrderCost(stateWorkOrder.WorkOrderId, eventWorkOrder.ScheduledDeliveryTime.Value, customerAccount.NWC_CustomerLocation.NWC_CustomerLocationClass.ID,
                                                                    customerAccount.ServiceTypeId, zoneStation, eventWorkOrder.OrderQuantity.Value);

                //n cost + 5% VAT tax
                decimal vat = WorkOrderCost.CalculateVat(netCost);
                decimal totalCost = netCost > -1 ? (netCost + vat) : -1;

                var woAccessories = Context.NWC_WorkOrderAccessory.Where(x => x.WorkOrderID == stateWorkOrder.WorkOrderId).ToList();

                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = eventWorkOrder.CreatedBy;
                stateWorkOrder.OrderQuantity = eventWorkOrder.OrderQuantity.Value;
                stateWorkOrder.ScheduledDeliveryTime = eventWorkOrder.ScheduledDeliveryTime.Value;
                //stateWorkOrder.CustomerLocationID = eventWorkOrder.CustomerLocationID.Value;
                //stateWorkOrder.ServiceTypeID = eventWorkOrder.ServiceTypeID.Value;
                stateWorkOrder.CustomerAccountId = eventWorkOrder.CustomerAccountId;
                stateWorkOrder.NetCost = netCost;
                stateWorkOrder.TotalCost = totalCost;
                stateWorkOrder.Distance = distance;
                stateWorkOrder.AccessoriesAr = woAccessories != null && woAccessories.Any() ? string.Join(", ", woAccessories.Select(x => x.NWC_Accessory.NameAr).ToArray()) : string.Empty;
                stateWorkOrder.AccessoriesEn = woAccessories != null && woAccessories.Any() ? string.Join(", ", woAccessories.Select(x => x.NWC_Accessory.NameEn).ToArray()) : string.Empty;
                stateWorkOrder.AssignedStationID = eventWorkOrder.StationID.Value;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Update,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    StationID = stateWorkOrder.AssignedStationID,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                //Add Comment
                if (totalCost < 0)
                    AddWorkOrderComment(stateWorkOrder.WorkOrderId, e.UserID, "Failed to calculate order price");

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyUpdateOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyUpdateOrderPaymentEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_UpdatePaymentStatus)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyUpdateOrderPaymentEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();
                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = eventWorkOrder.CreatedBy;
                stateWorkOrder.OrderQuantity = eventWorkOrder.OrderQuantity.Value;
                stateWorkOrder.ScheduledDeliveryTime = eventWorkOrder.ScheduledDeliveryTime.Value;
                stateWorkOrder.RequestTime = eventWorkOrder.RequestTime;
                //stateWorkOrder.CustomerLocationID = eventWorkOrder.CustomerLocationID.Value;
                //stateWorkOrder.ServiceTypeID = eventWorkOrder.ServiceTypeID.Value;
                stateWorkOrder.CustomerAccountId = eventWorkOrder.CustomerAccountId;
                stateWorkOrder.AssignedStationID = eventWorkOrder.StationID.Value;
                stateWorkOrder.AccessoriesAr = string.Join(", ", eventWorkOrder.NWC_WorkOrderAccessory.Select(x => x.NWC_Accessory.NameAr).ToArray());
                stateWorkOrder.AccessoriesEn = string.Join(", ", eventWorkOrder.NWC_WorkOrderAccessory.Select(x => x.NWC_Accessory.NameEn).ToArray());
                stateWorkOrder.InvoiceStatusID = stateWorkOrder.TotalCost > 0 ? (int)WorkOrderInvoiceStatusEnum.Paid : (int)WorkOrderInvoiceStatusEnum.Un_Paid;

                var woTrans = Context.NWC_WorkOrderTransaction.FirstOrDefault(x => x.ID == eventWorkOrder.ParentWorkOrderID.Value);

                if (woTrans == null)
                {
                    var workOrderPayment = new NWC_WorkOrderTransaction()
                    {
                        ID = eventWorkOrder.ParentWorkOrderID.Value,
                        CreatedBy = e.UserID,
                        Amount = stateWorkOrder.TotalCost,
                        TransactionDate = DateTimeHelper.GetDateTimeNow(),
                        IsDeleted = false
                    };

                    Context.NWC_WorkOrderTransaction.Add(workOrderPayment);
                }
                else
                {
                    woTrans.CreatedBy = e.UserID;
                    woTrans.Amount = stateWorkOrder.TotalCost;
                    woTrans.TransactionDate = DateTimeHelper.GetDateTimeNow();
                }

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyUpdateOrderPaymentEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyAssignWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WO_Vehicle_Assign)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyAssignWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------AssignWorkOrder - Denormalizer - Start----------------------------------------"));

                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Assigned))
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Denormalizer - WorkOrder Status Workflow Fail - OrderNumber: {stateWorkOrder.OrderNumber} - WorkOrderStatusID: {stateWorkOrder.LastStatusID}"));

                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusBy = e.UserID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.IsAssigned = true;
                stateWorkOrder.AssignedVehicleID = eventWorkOrder.VehicleID;
                stateWorkOrder.AssignedDriverID = eventWorkOrder.DriverID;
                stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Assign,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - WorkOrder Denormalizer - Assign Successfully - OrderNumber: {stateWorkOrder.OrderNumber} - TransporterID: {stateWorkOrder.AssignedVehicleID} - TransporterStatus: {stateWorkOrder.VehicleStatusID}"));

                result.Value = true;

                #region Calling Driver SMS service
                var driverSMSDTO = new DriverSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    VehicleID = stateWorkOrder.AssignedVehicleID.Value,
                    DriverID = stateWorkOrder.AssignedDriverID.Value,
                    //CustomerMobile = stateWorkOrder.RecieverMobile,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId
                };
                SendDriverSMS(driverSMSDTO);
        
        
                #endregion

                #region Calling Customer SMS service
                var customerSMSDTO = new CustomerSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    VehicleID = stateWorkOrder.AssignedVehicleID.Value,
                    DriverID = stateWorkOrder.AssignedDriverID.Value,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId
                };
                if (stateWorkOrder.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval)
                {
                    SendCustomerSMS(customerSMSDTO);
                }
                else
                {
                    SendCustomerSMS(customerSMSDTO,SMSType.OutForDelivery,(int)ServiceTypeEnum.SewageRemoval);
                }
     
                #endregion

                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item5, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID, 
                        createdBy, currentStatusID, newStatusID);
                }

                LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------AssignWorkOrder - Denormalizer - End----------------------------------------"));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyAssignWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyOutForDeliveryWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_OutForDelivery)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyOutForDeliveryWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Out_For_Delivery))
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusBy = e.UserID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_OutForDelivery,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;
                if(stateWorkOrder.ServiceTypeID==null || stateWorkOrder.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval)
                {
                    #region Calling Customer SMS service
                    var customerSMSDTO = new CustomerSMSDTO
                    {
                        OrderNumber = stateWorkOrder.OrderNumber,
                        VehicleID = stateWorkOrder.AssignedVehicleID.Value,
                        DriverID = stateWorkOrder.AssignedDriverID.Value,
                        CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                        CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId,
                        CustomerAccount = stateWorkOrder.NWC_CustomerAccount.AccountId_Integration
                    };

                    SendCustomerSMS(customerSMSDTO, SMSType.OutForDelivery, (int)stateWorkOrder.ServiceTypeID);
                    #endregion
                }


                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item6, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyOutForDeliveryWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyArrivedWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_Arrived)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyArrivedWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Arrived))
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusBy = e.UserID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Arrived,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;

                #region Calling Customer SMS service
                var customerSMSDTO = new CustomerSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    VehicleID = stateWorkOrder.AssignedVehicleID.Value,
                    DriverID = stateWorkOrder.AssignedDriverID.Value,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId,
                    CustomerAccount = stateWorkOrder.NWC_CustomerAccount.AccountId_Integration
                };
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("StateWorkOrder => Before sending SMS :{0}", JsonConvert.SerializeObject(customerSMSDTO) )));
                SendCustomerSMS(customerSMSDTO, SMSType.ArrivedToCustomer, (int)stateWorkOrder.ServiceTypeID);
                #endregion
                LoggerManager.LogMsg(c => c.TrackingMsg("StateWorkOrder => after sending SMS :"));
                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item7, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID, stateWorkOrder.LastStatusComment);
                }
                LoggerManager.LogMsg(c => c.TrackingMsg("StateWorkOrder => after Calling update service :"));
            }
          
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyArrivedWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyDeliveredWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_Delivered)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyDeliveredWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Delivered))
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusBy = e.UserID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.Comments = eventWorkOrder.Comments;
                 stateWorkOrder.ClosedByCode = eventWorkOrder.ConfirmationCode != null && eventWorkOrder.ConfirmationCode.Length > 0  ;
                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Delivered,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;

                #region Calling Customer SMS service
                //TODO : Virtual Station
                var customerSMSDTO = new CustomerSMSDTO
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    VehicleID = stateWorkOrder.AssignedVehicleID.HasValue ? stateWorkOrder.AssignedVehicleID.Value : Guid.Empty,

                    DriverID = stateWorkOrder.AssignedDriverID.HasValue ? stateWorkOrder.AssignedDriverID.Value : Guid.Empty,
                    CustomerLocationID = stateWorkOrder.NWC_CustomerAccount.CustomerLocationId,
                    CustomerID = stateWorkOrder.NWC_CustomerAccount.CustomerId,
                    CustomerAccount = stateWorkOrder.NWC_CustomerAccount?.AccountId_Integration
                };

                
                #endregion
                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item4, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID, stateWorkOrder.LastStatusComment);
                }
                else if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() == "TMS")
                {
                    SendCustomerSMS(customerSMSDTO, SMSType.Delivered,(int)stateWorkOrder.ServiceTypeID);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyDeliveredWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
               result.Value = false;
            }

            return result;
        }

        private bool CanApplyCancelledWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_Cancelled)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyCancelledWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Cancelled))
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                var statusReason = Context.NWC_StatusReason.FirstOrDefault(x => x.ID == eventWorkOrder.StatusReasonID);

                //Updating StateWorkOrder
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusReason = statusReason != null ? statusReason.ReasonAr : string.Empty;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;

                stateWorkOrder.IsAssigned = false;
                stateWorkOrder.AssignedVehicleID = null;
                stateWorkOrder.AssignedDriverID = null;
                if (eventWorkOrder.VehicleStatusID.HasValue)
                {
                    stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID.GetValueOrDefault();
                }

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Cancelled,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude,
                    StatusReasonId = eventWorkOrder.StatusReasonID
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;



                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item8, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID, stateWorkOrder.LastStatusComment, reasonIntegrationId: statusReason.IntegrationId);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyCancelledWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyDeassignWorkOrderEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WO_Vehicle_Deassign)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyDeassignWorkOrderEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                var statusReason = Context.NWC_DeassignReason.FirstOrDefault(x => x.ID == eventWorkOrder.StatusReasonID);

                //Updating StateWorkOrder
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.LastStatusReason = statusReason != null ? statusReason.ReasonAr : string.Empty;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;
                stateWorkOrder.IsAssigned = false;
                stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID;
                stateWorkOrder.AssignedVehicleID = null;
                //stateWorkOrder.AssignedDriverID = null;
                //stateWorkOrder.VehicleStatusID = null;

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_Deassign,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    DeassignReasonID = statusReason.ID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyDeassignWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyFailedToDeliverWorkOrder(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_FailedToDeliver)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyFailedToDeliverWorkOrder(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Failed_To_Deliver))
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                var statusReason = Context.NWC_StatusReason.FirstOrDefault(x => x.ID == eventWorkOrder.StatusReasonID);

                //Updating StateWorkOrder
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusReason = statusReason != null ? statusReason.ReasonAr : string.Empty;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;
                if(stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    stateWorkOrder.IsAssigned = false;
                }

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_FailedToDeliver,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude,
                    StatusReasonId = eventWorkOrder.StatusReasonID
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;

                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item3, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID, stateWorkOrder.LastStatusComment, reasonIntegrationId: statusReason.IntegrationId);
                }


            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyFailedToDeliverWorkOrder: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyOnHoldWorkOrder(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_OnHold)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyOnHoldWorkOrder(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Onhold))
                {
                    result.Value = false;
                    return result;
                }

                int currentStatusID = stateWorkOrder.LastStatusID;
                int newStatusID = eventWorkOrder.StatusID;

                //Updating StateWorkOrder
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;

                stateWorkOrder.IsAssigned = false;
                stateWorkOrder.AssignedVehicleID = null;
                stateWorkOrder.AssignedDriverID = null;
                if (eventWorkOrder.VehicleStatusID.HasValue)
                {
                    stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID.GetValueOrDefault();
                }

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_OnHold,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID.HasValue ? stateWorkOrder.AssignedVehicleID : (Guid?)null,
                    DriverID = stateWorkOrder.AssignedDriverID.HasValue ? stateWorkOrder.AssignedDriverID : (Guid?)null,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;

                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item2, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy, currentStatusID, newStatusID);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyOnHoldWorkOrder: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyNotAssigned(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WorkOrder_NotAssigned)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyNotAssigned(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.New))
                {
                    result.Value = false;
                    return result;
                }

                //Updating StateWorkOrder
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.LastStatusID = eventWorkOrder.StatusID;
                stateWorkOrder.LastStatusTime = eventWorkOrder.StatusTime;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;

                stateWorkOrder.IsAssigned = false;
                stateWorkOrder.AssignedVehicleID = null;
                stateWorkOrder.AssignedDriverID = null;
                if (eventWorkOrder.VehicleStatusID.HasValue)
                {
                    stateWorkOrder.VehicleStatusID = eventWorkOrder.VehicleStatusID.GetValueOrDefault();
                }

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WorkOrder_NotAssigned,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = stateWorkOrder.LastStatusTime,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = stateWorkOrder.VehicleStatusID
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;
                // Calling update service
                if (!string.IsNullOrEmpty(stateWorkOrder.SourceApplication) &&
                    stateWorkOrder.SourceApplication.ToUpper() != "TMS")
                {
                    Guid createdBy = eventWorkOrder.CreatedBy;

                    ConsumingUpdateService.CallUpdateWONotificationService(stateWorkOrder.OrderNumber, TMSWONotificationRequestOrderStatus.Item1, stateWorkOrder.CISDivision, stateWorkOrder.TransactionID,
                        createdBy,(int) WorkOrderStatusEnum.Onhold, (int)WorkOrderStatusEnum.New);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyNotAssigned: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        private bool CanApplyWOVArrivedStationEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.WO_Vehicle_ArrivedToStation)
                return true;

            return false;
        }

        private DescriptiveResponse<Boolean> ApplyWOVArrivedStationEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Updating StateWorkOrder
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                stateWorkOrder.LastModifiedBy = e.UserID;
                stateWorkOrder.LastStatusComment = eventWorkOrder.StatusComment;
                stateWorkOrder.IsAssigned = false;
                stateWorkOrder.VehicleStatusID = null;
                //stateWorkOrder.AssignedVehicleID = null;
                //stateWorkOrder.AssignedDriverID = null;

                try
                {
                    CloseAllPreviousOrders(stateWorkOrder);
                    LoggerManager.LogMsg(c => c.TrackingMsg("Success to close all previous orders"));
                }
                catch
                {
                    LoggerManager.LogMsg(c => c.Log("Failed to close all previous orders"));
                }

                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WO_Vehicle_ArrivedToStation,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = eventWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };

                Context.NWC_WorkOrderLog.Add(woLog);

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => ApplyWOVArrivedStationEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }
        private bool CanApplySewerDumpingEvent(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.SW_Vehicle_Dumping)
                return true;

            return false;
        }
        private DescriptiveResponse<Boolean> ApplySewerDumpingEvent(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();

                var stateWorkOrder = Context.NWC_StateWorkOrder.FirstOrDefault(x => x.WorkOrderId == eventWorkOrder.ParentWorkOrderID);

                if (stateWorkOrder == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate WorkOrder Status Workflow
                if (stateWorkOrder.LastStatusID != (int)WorkOrderStatusEnum.Delivered)
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                stateWorkOrder.IsAssigned = false;
                stateWorkOrder.LastModifiedTime = eventWorkOrder.CreateTime;
                //Add WorkOrder Log
                var woLog = new NWC_WorkOrderLog()
                {
                    ActionLogTypeID = (int)ActionLogTypeEnum.WO_Vehicle_ArrivedToStation,
                    WorkOrderId = stateWorkOrder.WorkOrderId,
                    CreateTime = eventWorkOrder.CreateTime,
                    CreatedBy = eventWorkOrder.CreatedBy,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusComment = stateWorkOrder.LastStatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    NetCost = stateWorkOrder.NetCost,
                    TotalCost = stateWorkOrder.TotalCost,
                    Distance = stateWorkOrder.Distance,
                    AccessoriesAr = stateWorkOrder.AccessoriesAr,
                    AccessoriesEn = stateWorkOrder.AccessoriesEn,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleStatusID = eventWorkOrder.VehicleStatusID,
                    VehicleLatitude = eventWorkOrder.VehicleLatitude,
                    VehicleLongitude = eventWorkOrder.VehicleLongitude
                };
                Context.NWC_WorkOrderLog.Add(woLog);
                Context.SaveChanges();
                LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerDumpingEvent - Work order Denormalizer - Assign Successfully - OrderNumber: {stateWorkOrder.OrderNumber} "));
                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplySewerDumpingEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        #region Helper
        private void CloseAllPreviousOrders(NWC_StateWorkOrder stateWorkOrder)
        {
            if (stateWorkOrder.AssignedVehicleID != null)
            {
                var CompletedOrdersIDs = new List<int>() { (int)WorkOrderStatusEnum.Delivered, (int)WorkOrderStatusEnum.Failed_To_Deliver };
                var PreviousOrders = Context.NWC_StateWorkOrder.Where(x => x.AssignedVehicleID == stateWorkOrder.AssignedVehicleID && x.IsAssigned == true && CompletedOrdersIDs.Contains(x.LastStatusID));
                foreach (var item in PreviousOrders)
                {
                    item.IsAssigned = false;
                }
            }
        }
        private void AddWorkOrderComment(long workOrderID, Guid createdBy, string comment)
        {
            try
            {
                var woComment = new NWC_WorkOrderComment()
                {
                    WorkOrderID = workOrderID,
                    Comment = "Failed to calculate order price",
                    CreatedBy = createdBy,
                    CreatedTime = DateTimeHelper.GetDateTimeNow(),
                    IsDeleted = false
                };

                Context.NWC_WorkOrderComment.Add(woComment);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => AddWorkOrderComment: "));
            }
        }

        private bool IsValidWorkOrderStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var workOrderStatus = Context.NWC_WorkOrderStatus.FirstOrDefault(x => x.ID == currentStatusID);

            return workOrderStatus.NextStatusIDs != null && workOrderStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }

        private bool ValidateVehicleStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var vehicleStatus = Context.TransporterStatus.FirstOrDefault(x => x.ID == currentStatusID);

            return vehicleStatus.NextStatusIDs != null && !vehicleStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }

        private string GetEncryptedConfirmationCode(string code)
        {
            return code.Replace(code.Substring(0), string.Format("****{0}", code.Substring(4)));
        }

        private string GetInvoiceNumber()
        {
            using (var cc = new NWCContext())
            {
                var seq = cc.sp_NWC_SeqNextValue_OrderInvoice();
                var next = seq.Single().Value;
                return next.ToString("D8");
            }

            //Random r = new Random();
            //string digits7 = Math.Round(r.NextDouble() * 1e+7, 0).ToString(CultureInfo.InvariantCulture).PadLeft(7, '0');

            //return digits7;
        }

        private void SendDriverSMS(DriverSMSDTO dto)
        {
            try
            {
                LoggerManager.LogMsg(c => c.TrackingMsg($"Start Inserting Driver SMS: OrderNo:{dto.OrderNumber}, VehicleId:{dto.VehicleID}, DriverId:{dto.DriverID}, CustomerLocationID:{dto.CustomerLocationID}"));

                var EnableDriverSMS = ConfigurationManager.AppSettings["EnableDriverSMS"];
                if (string.IsNullOrEmpty(EnableDriverSMS) || EnableDriverSMS.ToLower() != "true")
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"Configuration Driver SMS stop- EnableDriverSMS:{EnableDriverSMS}"));
                    return;
                }

                var driverMobile = this.Context.Staff.FirstOrDefault(s => s.ID == dto.DriverID).mobileNumber;
                var customerLoc = this.Context.NWC_CustomerLocation.FirstOrDefault(s => s.ID == dto.CustomerLocationID);

                var locationUrl = string.Empty;
                var customerMobile = string.Empty;

                #region location URL & tiny URL
                locationUrl = $"https://www.google.com/maps/search/?api=1&query={customerLoc.Latitude},{customerLoc.Longitude}";

                var EnableTinyURL = ConfigurationManager.AppSettings["EnableTinyURL"];
                if (EnableTinyURL.ToLower() == "true")
                {
                    locationUrl = Utilities.MakeTinyUrl(locationUrl);
                }
                #endregion

                Match match = Regex.Match(driverMobile, @"9665[\d]{8}");

                #region customer Mobile
                customerMobile = this.Context.NWC_Customer.FirstOrDefault(s => s.ID == dto.CustomerID).Mobile;
                if (!customerMobile.StartsWith("0"))
                {
                    customerMobile = "0" + customerMobile;
                }
                #endregion

                //var smsText = $"تم تخصيص رقم الطلب 2142958875 ويمكن التواصل على جوال العميل0554560660 + موقع العميل الموقع";
                var smsText = $"تم تخصيص رقم الطلب {dto.OrderNumber} ويمكن التواصل على جوال العميل {customerMobile} موقع العميل {locationUrl}";

                //**********************************************************************************************************
                LoggerManager.LogMsg(c => c.TrackingMsg($"SMS: {smsText}"));

                var newMSG = new NWC_DriverSMS
                {
                    OrderNumber = dto.OrderNumber,
                    VehicleID = dto.VehicleID,
                    DriverID = dto.DriverID,
                    DriverMobileNo = driverMobile,
                    SMSText = smsText,
                    CreatedTime = DateTimeHelper.GetDateTimeNow(),
                    StatusID = match.Success && driverMobile.Length == 12 ? 1 : 5,
                };

                this.Context.NWC_DriverSMS.Add(newMSG);

                this.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => SendDriverSMS: "));
            }
        }

        private void SendCustomerSMS(CustomerSMSDTO dto)
        {
            try
            {
                LoggerManager.LogMsg(c => c.TrackingMsg($"Start Inserting Customer SMS: OrderNo:{dto.OrderNumber}, VehicleId:{dto.VehicleID}, DriverId:{dto.DriverID}, CustomerLocationID:{dto.CustomerLocationID}"));

                var enableCustomerSMS = ConfigurationManager.AppSettings["EnableCustomerSMS"];
                if (string.IsNullOrEmpty(enableCustomerSMS) || enableCustomerSMS.ToLower() != "true")
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"Configuration Customer SMS Stop- EnableCustomerSMS:{enableCustomerSMS}"));
                    return;
                }

                var driver = this.Context.Staff.FirstOrDefault(s => s.ID == dto.DriverID);
                var driverMobile = driver.mobileNumber;
                var customDriverMobile = $"0{driver.mobileNumber.Substring(3, (driver.mobileNumber.Length - 3))}";
                var driverName = $"{driver.FirstName} {driver.MiddleName} {driver.LastName}";

                var customerLoc = this.Context.NWC_CustomerLocation.FirstOrDefault(s => s.ID == dto.CustomerLocationID);

                var customer = this.Context.NWC_Customer.FirstOrDefault(s => s.ID == dto.CustomerID);
                var customerMobile = "966" + customer.Mobile;

                Match match = Regex.Match(customerMobile, @"9665[\d]{8}");
            
                //var smsText = $"عميلنا العزيز: نفيدكم بتخصيص ناقلة مياه لطلبكم رقم 1233333669 مع السائق الاسم سلجيب  كومار رقم الجوال 966570533305";
                var smsText = $"عميلنا العزيز: نفيدكم بتخصيص ناقلة مياه لطلبكم رقم {dto.OrderNumber} مع السائق الاسم {driverName} رقم الجوال {customDriverMobile}";

                if (string.IsNullOrEmpty(driverMobile) || driverMobile.Length < 8)
                    smsText = $"عميلنا العزيز: نفيدكم بتخصيص ناقلة مياه لطلبكم رقم {dto.OrderNumber}";

                LoggerManager.LogMsg(c => c.TrackingMsg($"SMS: {smsText}"));

                var newMSG = new NWC_CustomerSMS
                {
                    OrderNumber = dto.OrderNumber,
                    VehicleID = dto.VehicleID,
                    DriverID = dto.DriverID,
                    DriverMobileNo = driverMobile,
                    CustomerMobileNo = customerMobile,
                    SMSText = smsText,
                    CreatedTime = DateTimeHelper.GetDateTimeNow(),
                    StatusID = match.Success && customerMobile.Length == 12 ? 1 : 5
                };

                this.Context.NWC_CustomerSMS.Add(newMSG);

                this.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => SendCustomerSMS: "));
            }
        }

        private void SendCustomerSMS(CustomerSMSDTO dto,SMSType type,int ServieType)
        {
            try
            {
                var enableCustomerSMS = ConfigurationManager.AppSettings["EnableCustomerSMS"];
                if (string.IsNullOrEmpty(enableCustomerSMS) || enableCustomerSMS.ToLower() != "true")
                {
                    return;
                }

      
                var customerMobile = this.Context.NWC_Customer
                    .Where(s => s.ID == dto.CustomerID).Select(s => s.Mobile).FirstOrDefault();

                if (!string.IsNullOrEmpty(customerMobile))
                    customerMobile = "966" + customerMobile;
             
                Match match = Regex.Match(customerMobile, @"9665[\d]{8}");
                var smsText = "";
                if (ServieType != 3)
                {
                    switch (type)
                    {
                        case SMSType.Create:
                            smsText = $"عميلنا العزيز: يوجد لديكم طلب صهريج مياه برقم {dto.OrderNumber} وسيتم التواصل معكم لتاكيد الطلب ويمكنكم استخدام الرمز السرى {dto.ConfirmationCode} لتأكيد استلام الصهريج";
                            break;
                        case SMSType.OutForDelivery:

                            smsText = string.Format("عميلنا العزيز: نفيدكم بخروج ناقلة مياه لطلبكم رقم {0} {1} ", dto.OrderNumber, dto.CustomerAccount.Length > 0 && !IsVirtualAccount(dto.CustomerAccount) ? $"للحساب رقم {dto.CustomerAccount}" : "");
                            break;
                        case SMSType.ArrivedToCustomer:
                            smsText = string.Format("عميلنا العزيز: نفيدكم بوصول ناقلة مياه لطلبكم رقم {0} {1} ", dto.OrderNumber, dto.CustomerAccount.Length > 0 && !IsVirtualAccount(dto.CustomerAccount) ? $"للحساب رقم {dto.CustomerAccount}" : "");
                            break;
                        case SMSType.Delivered:
                            smsText = string.Format("عميلنا العزيز: نسعد لخدمتك, ونود إبلاغك أنه قد تمت معالجة طلب خدمة ناقلة المياه {0} {1} ", dto.OrderNumber, dto.CustomerAccount.Length > 0 && !IsVirtualAccount(dto.CustomerAccount) ? $"للحساب رقم {dto.CustomerAccount}" : "");
                            break;
                        default:
                            break;
                    }
                } 
                else
                {
                    switch (type)
                    {
                        case SMSType.Create:
                            smsText =string.Format("عميلنا العزيز: نسعد بخدمتك ونفيدك بإنشاء طلب صهريج بيئتي رقم {0} وبإمكانك إستخدام رمز {1} لتأكيد وصول الصهريج. ولمتابعة الطلب ،الرجاء زيارة القنوات الرقمية", dto.OrderNumber, dto.ConfirmationCode);
                            break;
                        case SMSType.OutForDelivery:
                            var driver = this.Context.Staff.FirstOrDefault(s => s.ID == dto.DriverID);
                            var driverMobile = driver.mobileNumber;
                            var driverName = $"{driver.FirstName} {driver.MiddleName} {driver.LastName}";
                            smsText = string.Format("{2} عميلنا العزيز: نفيدك بتوجه صهريج بيئتي إلى عقاركم للطلب رقم {0} إسم السائق {1} رقم الجوال", dto.OrderNumber, driverName, driverMobile);
                            break;
                        case SMSType.ArrivedToCustomer:
                            smsText = string.Format("عميلنا العزيز: نفيدك بوصول صهريج بيئتي إلى عقاركم للطلب رقم {0}", dto.OrderNumber);
                            break;
                        case SMSType.Delivered:
                            smsText = string.Format("عميلنا العزيز: نفيدك بمعالجة طلب صهريج بيئتي رقم {0} ", dto.OrderNumber);
                            break;
                        case SMSType.Cancel:
                            smsText = string.Format("عميلنا العزيز: نفيدك بإلغاء طلب صهريج بيئتي رقم {0} ", dto.OrderNumber);
                            break;
                        default:
                            break;
                    }
                }
    
                //var smsText = $"عميلنا العزيز: يوجد لديكم طلب صهريج مياه برقم (8968513725) وسيتم التواصل معكم لتاكيد الطلب ويمكنكم استخدام الرمز السرى 1626 لتأكيد استلام الصهريج";
              

                LoggerManager.LogMsg(c => c.TrackingMsg($"SMS: {smsText}"));

                var newMSG = new NWC_CustomerSMS
                {
                    OrderNumber = dto.OrderNumber,
                    VehicleID = Guid.Empty,
                    DriverID = Guid.Empty,
                    DriverMobileNo = string.Empty,
                    CustomerMobileNo = customerMobile,
                    SMSText = smsText,
                    CreatedTime = DateTimeHelper.GetDateTimeNow(),
                    StatusID = match.Success && customerMobile.Length == 12 ? 1 : 5
                };

                this.Context.NWC_CustomerSMS.Add(newMSG);

                this.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateWorkOrder => SendCustomerSMS_Create: "));
            }
        }


        #endregion


        enum SMSType
        {
            Create=1,
            OutForDelivery=2,
            ArrivedToCustomer=3,
            Delivered = 4,
            Cancel = 5
        }

        #region "Virtual Accounts"
        List<string> VirtualAccounts = new List<string> {
        "4092108306",
        "5205633694",
        "1129102548",
        "6134686578",
        "1209701884",
        "5020201902",
        "2539890842",
        "6954045351",
        "7384769384",
        "8769332338",
        "2244520838",
        "7255627274",
        "0768683208",
        "4982867252",
        "0953444060",
        "3118991018",
        "9385645698"};
        private bool IsVirtualAccount(string AccountNum)
        {
            return VirtualAccounts.Contains(AccountNum);
        }
        #endregion

    }
}
