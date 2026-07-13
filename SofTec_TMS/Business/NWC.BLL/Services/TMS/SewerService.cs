using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace NWC.BLL.Services
{


    public class SewerService : ISewerService
    {
        #region ctor
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<NWC_Event> _eventRepository;
        private readonly IRepository<NWC_StateWorkOrder> _stateWorkOrderRepository;
        private readonly IRepository<Transporter> _transporterRepository;
        private readonly IRepository<NWC_VehicleLog> _vehicleLog;
        private readonly IRepository<Transporter> _transporter;
        private readonly IRepository<vw_NWC_StateVehicle> _StateVehicleViewRepository;
        private readonly ILoggedInUserService _loggedInUser;

        public SewerService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._eventRepository = new Repository<NWC_Event>(ctx);
            this._stateWorkOrderRepository = new Repository<NWC_StateWorkOrder>(ctx);
            this._StateVehicleViewRepository = new Repository<vw_NWC_StateVehicle>(ctx);
            this._vehicleLog = new Repository<NWC_VehicleLog>(ctx);
            this._transporter = new Repository<Transporter>(ctx);
            this._transporterRepository = new Repository<Transporter>(ctx);
            this._unitofWork = new UnitofWork(ctx);

        }
        #endregion

        #region Not Used
        //public DescriptiveResponse<bool> SewerVehicleArrivedStation(Guid vehicleId)
        //{
        //    try
        //    {
        //        var arrivedId = SWVehicleArrived(vehicleId);

        //        return DescriptiveResponse<bool>.Success(true);

        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerManager.LogMsg(c => c.Log(ex, "SewerVehicleArrivedStation => WOVehicleArrivedStation: "));
        //        return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
        //    }
        //}

        //public DescriptiveResponse<bool> SewerVehicleArrivedStation(WOVArrivedStationDTO dto)
        //{
        //    try
        //    {
        //        var arrivedId = SWVehicleArrived(Guid.Parse(dto.VehicleID));

        //        return DescriptiveResponse<bool>.Success(true);

        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerManager.LogMsg(c => c.Log(ex, "SewerVehicleArrivedStation => WOVehicleArrivedStation: "));
        //        return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
        //    }
        //}
        //public DescriptiveResponse<bool> SWVehicleArrived(Guid vehicleId)//WOVArrivedStationDTO dto)
        //{
        //    try
        //    {
        //        if (vehicleId == null)
        //        {
        //            throw new ArgumentNullException("WorkOrderId can not be null or equal 0.");
        //        }
        //        //var stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.AssignedVehicleID == vehicleId);

        //        var transporter = this._transporterRepository.GetQuery().FirstOrDefault(x => x.ID == vehicleId);
        //        transporter.status = (int)VehicleStatusEnum.Dumping;
        //        transporter.LastModificationDate = DateTime.Now;
        //        #region Not Used
        //        //Validate Vehicle Status Workflow
        //        //if (transporterStatus == null || transporterStatus.Value != (int)VehicleStatusEnum.SWInTheWayToStation)
        //        //{
        //        //    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
        //        //}

        //        //var e = new NWC_Event()
        //        //{
        //        //    EventTypeID = (int)EventTypeEnum.SW_Vehicle_Dumping,
        //        //    EventTime = DateTimeHelper.GetDateTimeNow(),
        //        //    UserID = _loggedInUser.LoggedInUser.StaffId,
        //        //    SubID = this._loggedInUser.LoggedInUser.SubscriberId
        //        //};

        //        //var eventWorkOrder = new NWC_EventWorkOrder()
        //        //{
        //        //    CreateTime = DateTimeHelper.GetDateTimeNow(),
        //        //    RequestTime = DateTimeHelper.GetDateTimeNow(),
        //        //    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
        //        //    ParentWorkOrderID = stateWorkOrder.WorkOrderId,
        //        //    OrderNumber = stateWorkOrder.OrderNumber,
        //        //    OrderQuantity = stateWorkOrder.OrderQuantity,
        //        //    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
        //        //    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
        //        //    ServiceTypeID = stateWorkOrder.ServiceTypeID,
        //        //    CustomerAccountId = stateWorkOrder.CustomerAccountId,
        //        //    StationID = stateWorkOrder.AssignedStationID,
        //        //    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
        //        //    StatusID = stateWorkOrder.LastStatusID,
        //        //    StatusTime = DateTimeHelper.GetDateTimeNow(),
        //        //    VehicleStatusID = (int)VehicleStatusEnum.Dumping,
        //        //    VehicleID = stateWorkOrder.AssignedVehicleID,
        //        //    DriverID = stateWorkOrder.AssignedDriverID,
        //        //    //VehicleLatitude = dto.VehicleLatitude,
        //        //    //VehicleLongitude = dto.VehicleLongitude,
        //        //    //VehicleCustomerClassId = dto.VehicleCustomerClassId
        //        //    //VehicleCustomerClassesIds = Utilities.ConvertToString(dto.VehicleCustomerLocationClassesIds)
        //        //}; 
        //        #endregion

        //        using (_unitofWork)
        //        {
        //            this._transporterRepository.Update(transporter);
        //        }

        //        return DescriptiveResponse<bool>.Success(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerManager.LogMsg(c => c.Log(ex, "SewerWorkOrderService => WOVehicleArrived: "));
        //        return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
        //    }
        //} 
        #endregion

        #region Updated
        public DescriptiveResponse<List<long>> SewerVehicleArrivedStation(Guid vehicleId)
        {
            try
            {
                var arrivedId = SWVehicleArrived(vehicleId);

                var ids = new List<long>();
                if (arrivedId.Value != null && arrivedId.Value.Any())
                    ids.AddRange(arrivedId.Value);
                return DescriptiveResponse<List<long>>.Success(ids);

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "SewerVehicleArrivedStation => WOVehicleArrivedStation: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> SewerVehicleArrivedStation(WOVArrivedStationDTO dto)
        {
            try
            {
                var arrivedId = SWVehicleArrived(Guid.Parse(dto.VehicleID));

                var ids = new List<long>();
                if (arrivedId.Value != null && arrivedId.Value.Any())
                    ids.AddRange(arrivedId.Value);
                return DescriptiveResponse<List<long>>.Success(ids);

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "SewerVehicleArrivedStation => WOVehicleArrivedStation: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> ArriveSewerVehicleWithOutOrderToStation(Guid vehicleID)
        {
            try
            {
                var vehicle = this._transporter.FindById(vehicleID);
                if (vehicle != null && vehicle.status == (int)VehicleStatusEnum.Parking)
                {
                    using (_unitofWork)
                    {
                        vehicle.status = (int)VehicleStatusEnum.Dumping;
                        vehicle.LastModificationDate = DateTime.Now;
                        #endregion
                    }
                    #region logs
                    using (_unitofWork)
                    {
                        var driverId = this._StateVehicleViewRepository.GetQuery()
                                       .Where(x => x.isDeleted != true
                                                   && x.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                                   && x.VehicleID == vehicleID)
                                       .Select(s => s.DriverId).FirstOrDefault();
                        this._vehicleLog.Add(new NWC_VehicleLog()
                        {
                            VehicleID = vehicle.ID,
                            DriverID = driverId,
                            StatusID = (int)VehicleStatusEnum.Available,
                            CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                            CreateTime = DateTimeHelper.GetDateTimeNow(),
                            ActionLogTypeID = (int)VehicleActionLogTypeEnum.Entry,
                        });
                    }
                    #endregion
                }
                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => ArriveVehicleToStation: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> SWVehicleArrived(Guid vehicleId)
        {
            try
            {
                if (vehicleId == null)
                {
                    throw new ArgumentNullException("WorkOrderId can not be null or equal 0.");
                }
                var stateWorkOrder = this._stateWorkOrderRepository.GetQuery()
                    .FirstOrDefault(x => x.AssignedVehicleID == vehicleId && x.IsAssigned==true);// && x.LastStatusID == (int)WorkOrderStatusEnum.Out_For_Delivery);


                //TODO : Need to check
                #region Used for manual
                //if (stateWorkOrder == null || stateWorkOrder.LastStatusID != (int)WorkOrderStatusEnum.Delivered)
                //{
                //    var transporter = this._transporterRepository.GetQuery().FirstOrDefault(x => x.ID == vehicleId);
                //    transporter.status = (int)VehicleStatusEnum.Dumping;
                //    transporter.LastModificationDate = DateTime.Now;

                //    using (_unitofWork)
                //    {
                //        this._transporterRepository.Update(transporter);
                //    }


                //    return DescriptiveResponse<List<long>>.Success(new List<long> { 0 });
                //} 
                #endregion


                var transporterStatus = this._transporterRepository.GetQuery()
                    .Where(x => x.ID == vehicleId)
                    .Select(s => s.status).FirstOrDefault();

                //Validate Vehicle Status Workflow
                if (transporterStatus == null)
                {
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.SW_Vehicle_Dumping,
                    EventTime = DateTimeHelper.GetDateTimeNow(),
                    UserID = _loggedInUser.LoggedInUser.StaffId,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId
                };

                var eventWorkOrder = new NWC_EventWorkOrder()
                {
                    CreateTime = DateTimeHelper.GetDateTimeNow(),
                    RequestTime = DateTimeHelper.GetDateTimeNow(),
                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                    ParentWorkOrderID = stateWorkOrder.WorkOrderId,
                    OrderNumber = stateWorkOrder.OrderNumber,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    StationID = stateWorkOrder.AssignedStationID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = stateWorkOrder.LastStatusID,
                    StatusTime = DateTimeHelper.GetDateTimeNow(),
                    VehicleStatusID = (int)VehicleStatusEnum.Dumping,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    //VehicleLatitude = dto.VehicleLatitude,
                    //VehicleLongitude = dto.VehicleLongitude,
                    //VehicleCustomerClassId = dto.VehicleCustomerClassId
                    //VehicleCustomerClassesIds = Utilities.ConvertToString(dto.VehicleCustomerLocationClassesIds)
                };

                using (_unitofWork)
                {
                    e.NWC_EventWorkOrder.Add(eventWorkOrder);

                    this._eventRepository.Add(e);
                }

                var ids = new List<long>();
                ids.Add(e.ID);

                return DescriptiveResponse<List<long>>.Success(ids);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "SewerWorkOrderService => WOVehicleArrived: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        public DescriptiveResponse<Boolean> OutForWork(Guid vehicleID)
        {
            try
            {
                if (vehicleID == null)
                {
                    throw new ArgumentNullException("WorkOrderId can not be null or equal 0.");
                }

                using (_unitofWork)
                {
                    var vehicle = this._transporterRepository.FindById(vehicleID);

                    if (vehicle != null && vehicle.status == (int)VehicleStatusEnum.Dumping)
                    {
                        vehicle.status = (int)VehicleStatusEnum.Parking;
                        vehicle.LastModificationDate = DateTime.Now;

                        var driverId = this._StateVehicleViewRepository.GetQuery()
                            .Where(x => x.isDeleted != true
                                        && x.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                        && x.VehicleID == vehicleID)
                            .Select(s => s.DriverId).FirstOrDefault();

                        this._vehicleLog.Add(new NWC_VehicleLog()
                        {
                            VehicleID = vehicle.ID,
                            DriverID = driverId,
                            StatusID = (int)VehicleStatusEnum.Parking,
                            CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                            CreateTime = DateTimeHelper.GetDateTimeNow(),
                            ActionLogTypeID = (int)VehicleActionLogTypeEnum.Exist
                        });
                    }
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => OutForParking: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
    }
}
