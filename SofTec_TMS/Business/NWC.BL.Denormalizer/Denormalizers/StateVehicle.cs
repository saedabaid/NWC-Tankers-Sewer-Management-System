using Infrastructure;
using NWC.BL.Denormalizer.Converters;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer.Denormalizers
{
    public class StateVehicle : IStateDenormalizer
    {
        public NWCContext Context { get; private set; }

        public List<Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>> StateFunctions;

        #region Constructors
        public StateVehicle() : this(null)
        {

        }

        internal StateVehicle(NWCContext ctx)
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
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyNotAssigned, ApplyNotAssigned));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyOnHoldWorkOrder, ApplyOnHoldWorkOrder));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyArrivedWorkOrderEvent, ApplyArrivedWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyDeliveredWorkOrderEvent, ApplyDeliveredWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyWOVArrivedStationEvent, ApplyWOVArrivedStationEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplyFailedToDeliverWorkOrder, ApplyFailedToDeliverWorkOrder));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerAssignWorkOrderEvent, ApplySewerAssignWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerConfirmEvent, ApplySewerConfirmEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerCompleteEvent, ApplySewerCompleteEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerCancelledWorkOrderEvent, ApplySewerCancelledWorkOrderEvent));
            StateFunctions.Add(new Tuple<Func<NWC_Event, bool>, Func<NWC_Event, DescriptiveResponse<Boolean>, DescriptiveResponse<Boolean>>>(CanApplySewerDumpingEvent, ApplySewerDumpingEvent));
        }
        #endregion

        public DescriptiveResponse<Boolean> ApplyEvent(long eventID, DescriptiveResponse<Boolean> retult)
        {
            var e = Context.NWC_Event.FirstOrDefault(o => o.ID == eventID);

            var newState =
                StateFunctions.
                Where(a => a.Item1(e)).
                Select(a => a.Item2(e, retult)).
                FirstOrDefault();

            return retult;
        }


        #region Sewer
        //=========================================
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null || transporter.status == null || transporter.status.Value != (int)VehicleStatusEnum.Parking)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerAssignWorkOrderEvent - Denormalizer - Transporter Status not in Parking State - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerAssignWorkOrderEvent - Vehicle Denormalizer - Assign Successfully - OrderNumber: {eventWorkOrderVehicle.OrderNumber} - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplySewerAssignWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }


        //=========================================

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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null || transporter.status == null || transporter.status.Value != (int)VehicleStatusEnum.Assigned)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerConfirmEvent - Denormalizer - Transporter Status not in assign state - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                    result.Value = false;
                    return result;
                }

                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerConfirmEvent - Vehicle Denormalizer - Assign Successfully - OrderNumber: {eventWorkOrderVehicle.OrderNumber} - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplySewerConfirmEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        //==========================================
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null || transporter.status ==null)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"ApplyAssignWorkOrderEvent - Denormalizer - Transporter Status not Available - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Vehicle Denormalizer - Assign Successfully - OrderNumber: {eventWorkOrderVehicle.OrderNumber} - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyAssignWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        //==========================================
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null || transporter.status.Value != (int)VehicleStatusEnum.Assigned)
                {
                    result.Value = false;
                    return result;
                }
              
                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyCancelledSewerWorkOrderEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }

        //=========================================
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null || transporter.status == null )
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerDumpingEvent - Denormalizer - Transporter Status not valid State - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                LoggerManager.LogMsg(c => c.TrackingMsg($"ApplySewerDumpingEvent - Vehicle Denormalizer - Assign Successfully - OrderNumber: {eventWorkOrderVehicle.OrderNumber} - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

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



        #endregion


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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null || transporter.status.Value != (int)VehicleStatusEnum.Available)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Denormalizer - Transporter Status not Available - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Assigned))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Vehicle Denormalizer - Assign Successfully - OrderNumber: {eventWorkOrderVehicle.OrderNumber} - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyAssignWorkOrderEvent: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyDeassignWorkOrderEvent: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.InService))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.NWC_VehicleLog.Add(new NWC_VehicleLog()
                {
                    VehicleID = transporter.ID,
                    StatusID = transporter.status.Value,
                    DriverID = eventWorkOrderVehicle.DriverID,
                    CreatedBy = eventWorkOrderVehicle.CreatedBy,
                    CreateTime = eventWorkOrderVehicle.CreateTime,
                    ActionLogTypeID = (int)VehicleActionLogTypeEnum.Exist,
                    WorkOrderID = eventWorkOrderVehicle.ParentWorkOrderID
                });

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyOutForDeliveryWorkOrderEvent: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyCancelledWorkOrderEvent: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyNotAssigned: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyOnHoldWorkOrder: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Available))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                #region update vehicle Location Class
                var canChangeCustomerClass =
                    this.Context.NWC_BranchSetting
                        .Where(s => s.BranchID == transporter.branch)
                        .Select(s => s.ShowCustomerClassEntryGate)
                        .FirstOrDefault();

                if (canChangeCustomerClass.GetValueOrDefault() && !string.IsNullOrEmpty(eventWorkOrderVehicle.VehicleCustomerClassesIds))
                {
                    var vehicleClasses = this.Context.NWC_VehicleCustomerLocationClass
                                        .Where(s => s.VehicleID == eventWorkOrderVehicle.VehicleID);

                    var customerClassesIds = Utilities.ConvertToList(eventWorkOrderVehicle.VehicleCustomerClassesIds);
                    if (vehicleClasses.Any())
                    {
                        foreach (var item in vehicleClasses)
                        {
                            if (!customerClassesIds.Any(a => a == item.CustomerLocationClassID))
                            {
                                this.Context.NWC_VehicleCustomerLocationClass.Remove(item);
                            }
                        }
                    }

                    if (customerClassesIds.Any())
                    {
                        foreach (var item in customerClassesIds)
                        {

                            if (!vehicleClasses.Any(a => a.CustomerLocationClassID == item))
                            {
                                var newVehicleClass = new NWC_VehicleCustomerLocationClass
                                {
                                    VehicleID = eventWorkOrderVehicle.VehicleID.Value,
                                    CustomerLocationClassID = item
                                };
                                this.Context.NWC_VehicleCustomerLocationClass.Add(newVehicleClass);

                                //this._VehicleCustomerLocationClass.Delete(item);
                            }
                        }
                    }


                    //var flagClassExist = false;
                    //if (vehicleClasses.Any())
                    //{
                    //    foreach (var item in vehicleClasses)
                    //    {
                    //        if (item.CustomerLocationClassID == eventWorkOrderVehicle.VehicleCustomerClassId)
                    //        {
                    //            flagClassExist = true;
                    //        }
                    //        else
                    //        {
                    //            this.Context.NWC_VehicleCustomerLocationClass.Remove(item);
                    //        }
                    //    }
                    //}

                    //if (!flagClassExist)
                    //{
                    //    var newVehicleClass = new NWC_VehicleCustomerLocationClass
                    //    {
                    //        VehicleID = eventWorkOrderVehicle.VehicleID.Value,
                    //        CustomerLocationClassID = eventWorkOrderVehicle.VehicleCustomerClassId.Value
                    //    };
                    //    this.Context.NWC_VehicleCustomerLocationClass.Add(newVehicleClass);
                    //}
                }
                #endregion

                Context.SaveChanges();

                #region get classes for logs
                var classes = this.Context.NWC_VehicleCustomerLocationClass
                                                        .Where(s => s.VehicleID == eventWorkOrderVehicle.VehicleID)
                                                        .Select(s => s.NWC_CustomerLocationClass).Distinct().ToList();
                var classesAr = string.Empty;
                var classesEn = string.Empty;
                if (classes.Any())
                {
                    classesAr = string.Join(", ", classes.Select(s => s.NameAr));
                    classesEn = string.Join(", ", classes.Select(s => s.NameEn));
                } 
                #endregion

                Context.NWC_VehicleLog.Add(new NWC_VehicleLog()
                {
                    VehicleID = transporter.ID,
                    StatusID = transporter.status.Value,
                    DriverID = eventWorkOrderVehicle.DriverID,
                    CreatedBy = eventWorkOrderVehicle.CreatedBy,
                    CreateTime = eventWorkOrderVehicle.CreateTime,
                    ActionLogTypeID = (int)VehicleActionLogTypeEnum.Entry,
                    WorkOrderID = eventWorkOrderVehicle.ParentWorkOrderID,
                    CustomerClassesAr = classesAr,
                    CustomerClassesEn = classesEn
                });

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyWOVArrivedStationEvent: "));
                result.ErrorDescription = ex.Message;
                result.IsErrorState = true;
                result.Value = false;
            }

            return result;
        }


        private bool CanApplyArriveSewerVehicleToStation(NWC_Event arg)
        {
            if (arg.EventTypeID == (int)EventTypeEnum.SW_Vehicle_GoToStation)
                return true;

            return false;
        }

 
        
        private DescriptiveResponse<Boolean> ApplyArriveSewerVehicleToStation(NWC_Event e, DescriptiveResponse<Boolean> result)
        {
            try
            {
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }
                var statuses = new List<VehicleStatusEnum> { VehicleStatusEnum.InService, VehicleStatusEnum.ArrivedToCustomer, VehicleStatusEnum.Delivered };
                //Validate Vehicle Status Workflow
                if (statuses.Select(x=>(int?) x).Contains(transporter.status))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyOnHoldWorkOrder: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();
                if(eventWorkOrderVehicle.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval)
                {
                    result.Value = true;
                    return result;
                }
                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (transporter.status.Value != (int)VehicleStatusEnum.Assigned && transporter.status.Value != (int)VehicleStatusEnum.InService && transporter.status.Value != (int)VehicleStatusEnum.ArrivedToCustomer)
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyFailedToDeliverWorkOrder: "));
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.ArrivedToCustomer))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyArrivedWorkOrder: "));
                //Temp Area
                result.Value = true;
                //=========
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
                var eventWorkOrderVehicle = e.NWC_EventWorkOrder.FirstOrDefault();

                var transporter = Context.Transporter.FirstOrDefault(x => x.ID == eventWorkOrderVehicle.VehicleID);

                if (transporter == null)
                {
                    result.Value = false;
                    return result;
                }

                //Validate Vehicle Status Workflow
                if (!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Delivered))
                {
                    result.Value = false;
                    return result;
                }

                //Updating Transporter
                transporter.status = eventWorkOrderVehicle.VehicleStatusID;
                transporter.LastModificationDate = eventWorkOrderVehicle.CreateTime;

                Context.SaveChanges();

                result.Value = true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StateVehicle => ApplyDeliveredWorkOrder: "));
                //Temp Area
                result.Value = true;
                //=========
            }

            return result;
        }
        #region Helper
        private bool ValidateWorkOrderStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var workOrderStatus = Context.NWC_WorkOrderStatus.FirstOrDefault(x => x.ID == currentStatusID);

            return workOrderStatus.NextStatusIDs != null && !workOrderStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }

        private bool IsValidVehicleStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var vehicleStatus = Context.TransporterStatus.FirstOrDefault(x => x.ID == currentStatusID);

            return vehicleStatus.NextStatusIDs != null && vehicleStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }
        #endregion
    }
}
