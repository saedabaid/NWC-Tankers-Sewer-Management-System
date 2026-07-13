using Infrastructure;
using NWC.BL.Denormalizer.Converters;
using NWC.DAL.NWCEntities;
using NWC.DTO;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Wrapper;
using NWC.Localization.ExceptionLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer.Denormalizers
{
    public class StateWorkOrderVehicle : IStateDenormalizer
    {
        public NWCContext Context { get; private set; }

        public List<Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>> StateFunctions;

        #region Constructors
        public StateWorkOrderVehicle() : this(null)
        {

        }

        internal StateWorkOrderVehicle(NWCContext ctx)
        {
            if (ctx == null)
                Context = new NWCContext();
            else
                Context = ctx;

            StateFunctions = new List<Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>>();

            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyAssignWorkOrderEvent, ApplyAssignWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyDeassignWorkOrderEvent, ApplyDeassignWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyOutForDeliveryWorkOrderEvent, ApplyOutForDeliveryWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyCancelledWorkOrderEvent, ApplyCancelledWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyOnHoldWorkOrder, ApplyOnHoldWorkOrder));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyWOVArrivedStationEvent, ApplyWOVArrivedStationEvent));
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
                var eventWorkOrder = e.NWC_EventWorkOrder.FirstOrDefault();
                var eventWorkOrderVehicle = e.NWC_EventWorkOrderVehicle.FirstOrDefault();

                //Validate Vehicle Status Workflow
                if (ValidateVehicleStatusWorkflow((int)WorkOrderVehicleStatusEnum.Assigned, eventWorkOrderVehicle.StatusID))
                {
                    result.Value = false;
                    return result;
                }

                //Add StateWorkOrderVehicle
                var newOrderVehicleState = new NWC_StateWorkOrderVehicle()
                {
                    SubID = eventWorkOrderVehicle.SubID, 
                    WorkflowID = eventWorkOrderVehicle.WorkflowID,
                    WorkOrderID = eventWorkOrder.ParentWorkOrderID.Value,
                    WorkOrderNumber = eventWorkOrder.OrderNumber,
                    WorkOrderTime = eventWorkOrder.ScheduledDeliveryTime,
                    VehicleID = eventWorkOrderVehicle.VehicleID,
                    DriverID = eventWorkOrderVehicle.DriverID,
                    LastStatusID = (int)WorkOrderVehicleStatusEnum.Assigned,
                    LastStatusTime = DateTime.Now,
                    LastStatusBy = eventWorkOrderVehicle.CreatedBy,
                    IsAssigned = true,
                    AssignTime = DateTime.Now,
                    AssignedByID = e.UserID,
                    IsInService = false,
                    IsDeassigned = false
                };

                Context.NWC_StateWorkOrderVehicle.Add(newOrderVehicleState);

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrderVehicle.FirstOrDefault();

                var stateWorkOrderVehicle = Context.NWC_StateWorkOrderVehicle.FirstOrDefault(x => x.VehicleID == eventWorkOrderVehicle.VehicleID);

                if (stateWorkOrderVehicle == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (ValidateVehicleStatusWorkflow(stateWorkOrderVehicle.LastStatusID.Value, (int)WorkOrderVehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Update StateWorkOrderVehicle
                stateWorkOrderVehicle.LastStatusID = (int)WorkOrderVehicleStatusEnum.Available;
                stateWorkOrderVehicle.LastStatusTime = DateTime.Now;
                stateWorkOrderVehicle.LastStatusBy = eventWorkOrderVehicle.CreatedBy;
                stateWorkOrderVehicle.IsAssigned = false;
                stateWorkOrderVehicle.IsInService = false;
                stateWorkOrderVehicle.IsDeassigned = true;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrderVehicle.FirstOrDefault();
                
                var stateWorkOrderVehicle = Context.NWC_StateWorkOrderVehicle.FirstOrDefault(x => x.VehicleID == eventWorkOrderVehicle.VehicleID);

                if (stateWorkOrderVehicle == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (ValidateVehicleStatusWorkflow(stateWorkOrderVehicle.LastStatusID.Value, (int)WorkOrderVehicleStatusEnum.InService))
                {
                    result.Value = false;
                    return result;
                }

                //Update StateWorkOrderVehicle
                stateWorkOrderVehicle.LastStatusID = (int)WorkOrderVehicleStatusEnum.InService;
                stateWorkOrderVehicle.LastStatusTime = DateTime.Now;
                stateWorkOrderVehicle.LastStatusBy = eventWorkOrderVehicle.CreatedBy;
                stateWorkOrderVehicle.IsAssigned = false;
                stateWorkOrderVehicle.IsInService = true;
                stateWorkOrderVehicle.IsDeassigned = false;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrderVehicle.FirstOrDefault();

                var stateWorkOrderVehicle = Context.NWC_StateWorkOrderVehicle.FirstOrDefault(x => x.VehicleID == eventWorkOrderVehicle.VehicleID);

                if (stateWorkOrderVehicle == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (ValidateVehicleStatusWorkflow(stateWorkOrderVehicle.LastStatusID.Value, (int)WorkOrderStatusEnum.Delivered))
                {
                    result.Value = false;
                    return result;
                }

                //Update StateWorkOrderVehicle
                stateWorkOrderVehicle.LastStatusID = (int)WorkOrderStatusEnum.Delivered;
                stateWorkOrderVehicle.LastStatusTime = DateTime.Now;
                stateWorkOrderVehicle.LastStatusBy = eventWorkOrderVehicle.CreatedBy;
                stateWorkOrderVehicle.IsAssigned = false;
                stateWorkOrderVehicle.IsInService = false;
                stateWorkOrderVehicle.IsDeassigned = false;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrderVehicle.FirstOrDefault();

                var stateWorkOrderVehicle = Context.NWC_StateWorkOrderVehicle.FirstOrDefault(x => x.VehicleID == eventWorkOrderVehicle.VehicleID);

                if (stateWorkOrderVehicle == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (ValidateVehicleStatusWorkflow(stateWorkOrderVehicle.LastStatusID.Value, (int)WorkOrderVehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Update StateWorkOrderVehicle
                stateWorkOrderVehicle.LastStatusID = (int)WorkOrderVehicleStatusEnum.Available;
                stateWorkOrderVehicle.LastStatusTime = DateTime.Now;
                stateWorkOrderVehicle.LastStatusBy = eventWorkOrderVehicle.CreatedBy;
                stateWorkOrderVehicle.LastStatusComment = eventWorkOrderVehicle.StatusComment;
                stateWorkOrderVehicle.IsAssigned = false;
                stateWorkOrderVehicle.IsInService = false;
                stateWorkOrderVehicle.IsDeassigned = false;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrderVehicle.FirstOrDefault();

                var stateWorkOrderVehicle = Context.NWC_StateWorkOrderVehicle.FirstOrDefault(x => x.VehicleID == eventWorkOrderVehicle.VehicleID);

                if (stateWorkOrderVehicle == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (ValidateVehicleStatusWorkflow(stateWorkOrderVehicle.LastStatusID.Value, (int)WorkOrderVehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                var statusReason = eventWorkOrderVehicle.TransporterStatus.NWC_StatusReason.FirstOrDefault(x => x.StatusID == (int)WorkOrderVehicleStatusEnum.Available);

                //Update StateWorkOrderVehicle
                stateWorkOrderVehicle.LastStatusID = (int)WorkOrderVehicleStatusEnum.Available;
                stateWorkOrderVehicle.LastStatusTime = DateTime.Now;
                stateWorkOrderVehicle.LastStatusBy = eventWorkOrderVehicle.CreatedBy;
                stateWorkOrderVehicle.LastStatusReason = statusReason != null ? statusReason.ReasonAr : string.Empty;
                stateWorkOrderVehicle.LastStatusComment = eventWorkOrderVehicle.StatusComment;
                stateWorkOrderVehicle.IsAssigned = false;
                stateWorkOrderVehicle.IsInService = false;
                stateWorkOrderVehicle.IsDeassigned = false;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrderVehicle.FirstOrDefault();

                var stateWorkOrderVehicle = Context.NWC_StateWorkOrderVehicle.FirstOrDefault(x => x.VehicleID == eventWorkOrderVehicle.VehicleID);

                if (stateWorkOrderVehicle == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (ValidateVehicleStatusWorkflow(stateWorkOrderVehicle.LastStatusID.Value, (int)WorkOrderVehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Update StateWorkOrderVehicle
                stateWorkOrderVehicle.LastStatusID = (int)WorkOrderVehicleStatusEnum.Available;
                stateWorkOrderVehicle.LastStatusTime = DateTime.Now;
                stateWorkOrderVehicle.LastStatusBy = eventWorkOrderVehicle.CreatedBy;
                stateWorkOrderVehicle.IsAssigned = false;
                stateWorkOrderVehicle.IsInService = false;
                stateWorkOrderVehicle.IsDeassigned = false;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        #region Helper
        private bool ValidateWorkOrderStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var workOrderStatus = Context.NWC_WorkOrderStatus.FirstOrDefault(x => x.ID == currentStatusID);

            return workOrderStatus.NextStatusIDs != null && !workOrderStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }

        private bool ValidateVehicleStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var vehicleStatus = Context.TransporterStatus.FirstOrDefault(x => x.ID == currentStatusID);

            return vehicleStatus.NextStatusIDs != null && !vehicleStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }
        #endregion
    }
}
