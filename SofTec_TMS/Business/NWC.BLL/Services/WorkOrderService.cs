using Infrastructure;
using LinqKit;
using Newtonsoft.Json;
using NWC.BL.Denormalizer.CoreBusiness;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NWC.BLL.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private static bool CallAccountBlacklistService_config
        {
            get
            {
                var myConfig = ConfigurationManager.AppSettings["CallAccountBlacklistService"];
                if (!string.IsNullOrEmpty(myConfig) && myConfig.ToLower() == "true")
                {
                    return true;
                }
                return false;
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

        #region Properties
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<NWC_Event> _eventRepository;
        private readonly IRepository<NWC_EventWorkOrder> _eventWorkOrderRepository;
        private readonly IRepository<NWC_StateWorkOrder> _stateWorkOrderRepository;
        private readonly IRepository<vw_NWC_WorkOrderList> _workOrderListRepository;
        private readonly IRepository<vw_NWC_WorkOrderBasicDetails> _orderBasicDetailsRepository;
        private readonly IRepository<NWC_WorkOrderComment> _workOrderComment;
        private readonly IRepository<NWC_WorkOrderComplaint> _workOrderComplaint;
        private readonly IRepository<NWC_Contract> _contractRepository;
        private readonly IRepository<NWC_Accessory> _accessory;
        private readonly IRepository<NWC_WorkOrderAccessory> _workOrderAccessory;
        private readonly IRepository<sp_NWC_GetAssignableWorkOrders_Result> _sp_NWC_GetAssignableWorkOrders;
        private readonly IRepository<NWC_WorkOrderTransaction> _workOrderTransaction;
        private readonly IRepository<NWC_ZoneStations> _zoneStations;
        private readonly IRepository<NWC_CustomerLocation> _customerLocation;
        private readonly IRepository<NWC_CustomerAccount> _customerAccount;
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IRepository<vw_NWC_ChangesLogsLocationData> _changesLogsLocationData;
        private readonly IRepository<vw_NWC_WorkOrderLogs> _vw_NWC_WorkOrderLogs;
        private readonly IRepository<NWC_ContractStations> _contractStationsRepository;
        private readonly IRepository<NWC_ContractTariff> _contractTariffRepository;
        private readonly IRepository<NWC_ContractAccessory> _contractAccessoryRepository;
        private readonly IRepository<NWC_CustomerLocation> _customerLocationRepository;
        private readonly IRepository<Transporter> _transporterRepository;
        private readonly IRepository<vw_NWC_Report_DailyOrderSummary> _DailyOrderSummaryRepository;
        private readonly IRepository<vw_NWC_Report_DailyOrderDetails> _DailyOrderDetaileRepository;
        private readonly IRepository<NWC_DeferredWorkOrder> _DeferredWorkOrderRepository;
        private readonly IRepository<Transporter> _transporter;
        private readonly IRepository<NWC_VehicleCustomerLocationClass> _vehicleClassRepository;
        private readonly IRepository<NWC_RestrictedZoneVehicleType> _restrictedZoneVehicleTypeRep;
        private readonly IRepository<NWC_WorkOrderStatus> _workOrderStatusRep;
        private readonly IRepository<TransporterStatus> _transporterStatusRep;
        private readonly IRepository<NWC_Zone> _zoneRepository;
        private readonly IRepository<vw_NWC_CustomerAccountCity> _vw_NWC_CustomerAccountCityRep;
        private readonly IRepository<Branch> _BranchRepository;
        private readonly IRepository<Landmark> _landmarkRepository;
        private readonly IRepository<NWC_Hayat_OrderStatusLog> _hayatOrderStatusLogRepository;
        private readonly IRepository<Transporter_Staff> _transporterStaffRepository;
        private readonly ISignalRService _signalRService;
        private readonly ISewerService _SewerService;
        private readonly bool isSignalREnabled = true;
        #endregion

        #region Constructors        

        public WorkOrderService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            _signalRService = new SignalRService(loggedInUser);
            this._unitofWork = new UnitofWork(ctx);

            this._eventRepository = new Repository<NWC_Event>(ctx);
            this._eventWorkOrderRepository = new Repository<NWC_EventWorkOrder>(ctx);
            this._stateWorkOrderRepository = new Repository<NWC_StateWorkOrder>(ctx);
            this._workOrderListRepository = new Repository<vw_NWC_WorkOrderList>(ctx);
            this._orderBasicDetailsRepository = new Repository<vw_NWC_WorkOrderBasicDetails>(ctx);
            this._workOrderComment = new Repository<NWC_WorkOrderComment>(ctx);
            this._workOrderComplaint = new Repository<NWC_WorkOrderComplaint>(ctx);
            this._contractRepository = new Repository<NWC_Contract>(ctx);
            this._accessory = new Repository<NWC_Accessory>(ctx);
            this._workOrderAccessory = new Repository<NWC_WorkOrderAccessory>(ctx);
            this._zoneStations = new Repository<NWC_ZoneStations>(ctx);
            this._customerLocation = new Repository<NWC_CustomerLocation>(ctx);
            this._workOrderTransaction = new Repository<NWC_WorkOrderTransaction>(ctx);
            this._sp_NWC_GetAssignableWorkOrders = new Repository<sp_NWC_GetAssignableWorkOrders_Result>(ctx);
            this._changesLogsLocationData = new Repository<vw_NWC_ChangesLogsLocationData>(ctx);
            this._vw_NWC_WorkOrderLogs = new Repository<vw_NWC_WorkOrderLogs>(ctx);
            this._contractStationsRepository = new Repository<NWC_ContractStations>(ctx);
            this._contractTariffRepository = new Repository<NWC_ContractTariff>(ctx);
            this._contractAccessoryRepository = new Repository<NWC_ContractAccessory>(ctx);
            this._customerLocationRepository = new Repository<NWC_CustomerLocation>(ctx);

            this._DailyOrderSummaryRepository = new Repository<vw_NWC_Report_DailyOrderSummary>(ctx);
            this._DailyOrderDetaileRepository = new Repository<vw_NWC_Report_DailyOrderDetails>(ctx);
            this._DeferredWorkOrderRepository = new Repository<NWC_DeferredWorkOrder>(ctx);

            this._vehicleClassRepository = new Repository<NWC_VehicleCustomerLocationClass>(ctx);
            this._restrictedZoneVehicleTypeRep = new Repository<NWC_RestrictedZoneVehicleType>(ctx);
            this._workOrderStatusRep = new Repository<NWC_WorkOrderStatus>(ctx);
            this._transporterStatusRep = new Repository<TransporterStatus>(ctx);
            this._customerAccount = new Repository<NWC_CustomerAccount>(ctx);
            this._zoneRepository = new Repository<NWC_Zone>(ctx);
            this._vw_NWC_CustomerAccountCityRep = new Repository<vw_NWC_CustomerAccountCity>(ctx);
            this._BranchRepository = new Repository<Branch>(ctx);
            this._landmarkRepository = new Repository<Landmark>(ctx);
            this._hayatOrderStatusLogRepository = new Repository<NWC_Hayat_OrderStatusLog>(ctx);

            this._transporter = new Repository<Transporter>(ctx);
            this._transporterRepository = new Repository<Transporter>(ctx);
            this._transporterStaffRepository = new Repository<Transporter_Staff>(ctx);
            this._SewerService =  new SewerService(loggedInUser); ;
        }
        #endregion

        #region Command
        public DescriptiveResponse<List<long>> CreateWorkOrder(EventWorkOrderDTO dto, out EventWorkOrderDTO outDto)
        {
            outDto = new EventWorkOrderDTO();

            try
            {
                #region Log
                LoggerManager.LogMsg(c => c.TrackingMsg("---------------------------------------------------------------------------------------------------------------------------"));
                LoggerManager.LogMsg(c => c.TrackingMsg("CreateWorkOrder"));
                LoggerManager.LogMsg(c => c.TrackingMsg(JsonConvert.SerializeObject(dto)));
                #endregion

                #region prep
                if (string.IsNullOrEmpty(dto.OrderNumber))
                {
                    dto.OrderNumber = GenerateWorkOrderNumber();
                    if (dto.ScheduledDeliveryTime == DateTime.MinValue)
                    {
                        dto.ScheduledDeliveryTime = DateTimeHelper.GetDateTimeNow();
                    }
                    dto.ConfirmationCode = GetConfirmationCode(4);
                }

                if (string.IsNullOrEmpty(dto.ConfirmationCode))
                {
                    dto.ConfirmationCode = GetConfirmationCode(7);
                }
                #endregion

                #region Validations
                //var ZoneWithoutTanker = this._customerAccount.GetQuery().Where(x => x.ID == dto.CustomerAccountId)
                //                .Select(a => a.NWC_CustomerLocation.NWC_Zone.ZoneWithoutTanker)
                //                .FirstOrDefault();
                //if (ZoneWithoutTanker == true)
                //{
                //    List<string> resultError = new List<string>();
                //    resultError.Add(ValidationMessagesKeys.CustomerWithSelectedLocationNotAllowed);
                //    return DescriptiveResponse<List<long>>.Error(resultError);
                //}
                var validator = new WorkOrderValidator(ValidationMode.Create, this._loggedInUser, this._eventWorkOrderRepository, this._contractRepository, this._contractStationsRepository,
                    this._contractTariffRepository, this._contractAccessoryRepository, this._customerLocationRepository, this._customerAccount, this._stateWorkOrderRepository);

                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Not-Valid - Errors: {0}", JsonConvert.SerializeObject(results.Errors.Select(s => s.ErrorMessage)))));

                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<List<long>>.Error(failures);
                }
                #endregion

                LoggerManager.LogMsg(c => c.TrackingMsg("Valid"));

                #region prep.
                decimal distance = 0M;
                var customerLocation = this._customerAccount.GetQuery()
                    .Where(s => s.ID == dto.CustomerAccountId && !s.IsDeleted).FirstOrDefault().NWC_CustomerLocation;//.FindById(dto.CustomerLocationID);
                if (customerLocation != null)
                {
                    var zoneStation = this._zoneStations.GetQuery().Where(x => x.StationID == dto.StationID && x.ZoneID == customerLocation.ZoneID).FirstOrDefault();
                    distance = (zoneStation != null && zoneStation.Distance.HasValue) ? zoneStation.Distance.Value : 0M;
                }

                var accIDs = dto.Accessories != null ? dto.Accessories.Select(y => y.ID).ToList() : new List<int>();

                #endregion

                #region Wappers
                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_Create,
                    EventTime = DateTimeHelper.GetDateTimeNow(),
                    UserID = _loggedInUser.LoggedInUser.StaffId,
                    SubID = _loggedInUser.LoggedInUser.SubscriberId
                };

                var eventWorkOrder = new NWC_EventWorkOrder()
                {
                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.New,
                    StatusTime = DateTimeHelper.GetDateTimeNow(),
                    CreateTime = DateTimeHelper.GetDateTimeNow(),
                    ParentWorkOrderID = (long?)null,
                    OrderNumber = dto.OrderNumber,
                    OrderQuantity = dto.OrderQuantity,
                    RequestTime = DateTimeHelper.GetDateTimeNow(),
                    ScheduledDeliveryTime = dto.ScheduledDeliveryTime,
                    //CustomerLocationID = dto.CustomerLocationID,
                    ServiceTypeID = dto.ServiceTypeID == 0 ? 1 : dto.ServiceTypeID,            //Specify to Ashyab only
                    CustomerAccountId = dto.CustomerAccountId,
                    StationID = dto.StationID,
                    ConfirmationCode = dto.ConfirmationCode,
                    Distance = distance,
                    AccessoriesAr = string.Join(", ", this._accessory.GetQuery().Where(x => accIDs.Contains(x.ID)).Select(x => x.NameAr).ToArray()),
                    AccessoriesEn = string.Join(", ", this._accessory.GetQuery().Where(x => accIDs.Contains(x.ID)).Select(x => x.NameEn).ToArray()),
                    RecieverName = dto.RecieverName,
                    RecieverMobile = dto.RecieverMobile,
                    Comments = dto.Comments,
                    SourceApplication = dto.SourceApplication,
                    CISDivision = dto.CISDivision,
                    TransactionID = dto.TransactionID,
                    CategoryID = dto.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval || dto.CategoryID!= null ? dto.CategoryID : dto.IsPaid ? 11 : 12,
                     PriorityID   = dto.PriorityID == null ? 5 : (int)dto.PriorityID
                };

                foreach (var acc in dto.Accessories)
                {
                    eventWorkOrder.NWC_WorkOrderAccessory.Add(new NWC_WorkOrderAccessory()
                    {
                        AccessoryID = acc.ID
                    });
                }

                e.NWC_EventWorkOrder.Add(eventWorkOrder);
                #endregion

                using (_unitofWork)
                {
                    this._eventRepository.Add(e);
                }

                outDto.WorkOrderID = eventWorkOrder.EventOrderID;
                outDto.OrderNumber = eventWorkOrder.OrderNumber;
                outDto.ConfirmationCode = eventWorkOrder.ConfirmationCode;

     

                var ids = new List<long>();
                ids.Add(e.ID);

                LoggerManager.LogMsg(c => c.TrackingMsg("Create WorkOrder: Partially Success"));
                return DescriptiveResponse<List<long>>.Success(ids);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Fail - Exception: {0}", JsonConvert.SerializeObject(ex))));

                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => CreateWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> CreateSignalRNotification(long workOrderID)
        {
            try
            {
                var workOrderDto = GetSewerOrderBasicDetails(workOrderID);
                if (!workOrderDto.IsErrorState && workOrderDto.Value != null)
                    _signalRService.WorkOrderCreated(workOrderDto.Value);
                return    DescriptiveResponse<bool>.Success(true);
            }
            catch
            {
                return DescriptiveResponse<bool>.Error();
            }
        
        }

        public DescriptiveResponse<List<long>> BulkCreateWorkOrder(EventWorkOrderDTO dto)
        {
            var response = new DescriptiveResponse<List<long>>();
            response.Value = new List<long>();
            response.Errors = new List<string>();

            for (int index = 0; index < dto.BC_NoOfOrders; index++)
            {
                var eventWorkOrder = new EventWorkOrderDTO();
                dto.OrderNumber = string.Empty;
                //var scheduledDeliveryTime = dto.BC_StartingTime;

                //if(index > 0)
                //    scheduledDeliveryTime = dto.BC_StartingTime.AddMinutes(index * dto.BC_HoldIntervalMin);

                dto.ScheduledDeliveryTime = dto.BC_StartingTime.AddMinutes(index * dto.BC_HoldIntervalMin);

                //Give high priority to bulk orders 
                dto.PriorityID = 1;
                var result = CreateWorkOrder(dto, out eventWorkOrder);

                if (!result.IsErrorState)
                    response.Value.AddRange(result.Value);
                else
                    response.Errors.AddRange(result.Errors);
            }

            return response;
        }

        public DescriptiveResponse<List<long>> UpdateWorkOrder(EventWorkOrderDTO dto)
        {
            try
            {
                #region Validations
                var validator = new WorkOrderValidator(ValidationMode.Update, this._loggedInUser, this._eventWorkOrderRepository, this._contractRepository, this._contractStationsRepository,
                    this._contractTariffRepository, this._contractAccessoryRepository, this._customerLocationRepository, this._customerAccount, this._stateWorkOrderRepository);

                var results = validator.Validate(dto);

                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<List<long>>.Error(failures);
                }
                #endregion

                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                decimal distance = 0M;
                var customerLocation = this._customerAccount.GetQuery()
                    .Where(s => s.ID == dto.CustomerAccountId && !s.IsDeleted).FirstOrDefault().NWC_CustomerLocation;
                //var customerLocation = this._customerLocation.FindById(dto.CustomerLocationID);

                if (customerLocation != null)
                {
                    var zoneStation = this._zoneStations.GetQuery().Where(x => x.StationID == dto.StationID && x.ZoneID == customerLocation.ZoneID).FirstOrDefault();
                    distance = (zoneStation != null && zoneStation.Distance.HasValue) ? zoneStation.Distance.Value : 0M;
                }

                var accIDs = dto.Accessories.Select(y => y.ID).ToList();

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_Update,
                    EventTime = DateTimeHelper.GetDateTimeNow(),
                    UserID = _loggedInUser.LoggedInUser.StaffId,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId
                };

                var eventWorkOrder = new NWC_EventWorkOrder()
                {
                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    CreateTime = DateTimeHelper.GetDateTimeNow(),
                    RequestTime = DateTimeHelper.GetDateTimeNow(),
                    ParentWorkOrderID = dto.WorkOrderID,
                    OrderQuantity = dto.OrderQuantity,
                    ScheduledDeliveryTime = dto.ScheduledDeliveryTime,
                    //CustomerLocationID = dto.CustomerLocationID,
                    //ServiceTypeID = dto.ServiceTypeID,
                    CustomerAccountId = dto.CustomerAccountId,
                    Distance = distance,
                    StationID = dto.StationID,
                    AccessoriesAr = string.Join(", ", this._accessory.GetQuery().Where(x => accIDs.Contains(x.ID)).Select(x => x.NameAr).ToArray()),
                    AccessoriesEn = string.Join(", ", this._accessory.GetQuery().Where(x => accIDs.Contains(x.ID)).Select(x => x.NameEn).ToArray())
                };

                using (_unitofWork)
                {
                    //Delete old work order accessories
                    var woAcc = this._workOrderAccessory.GetQuery().Where(x => x.WorkOrderID == dto.WorkOrderID).ToList();

                    foreach (var acc in woAcc)
                    {
                        this._workOrderAccessory.Delete(acc);
                    }

                    foreach (var acc in dto.Accessories)
                    {
                        this._workOrderAccessory.Add(new NWC_WorkOrderAccessory()
                        {
                            AccessoryID = acc.ID,
                            WorkOrderID = dto.WorkOrderID
                        });
                    }

                    e.NWC_EventWorkOrder.Add(eventWorkOrder);

                    this._eventRepository.Add(e);
                }

                var ids = new List<long>();
                ids.Add(e.ID);

                return DescriptiveResponse<List<long>>.Success(ids);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => UpdateWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> UpdateWorkOrderAssignRetrials(long workOrderID, int holdInterval)
        {
            try
            {
                using (_unitofWork)
                {
                    var retryTime = DateTimeHelper.GetDateTimeNow().AddMinutes(holdInterval);

                    var stateWorkOrder = this._stateWorkOrderRepository.GetQuery().Where(x => x.WorkOrderId == workOrderID).FirstOrDefault();

                    stateWorkOrder.AssignRetrials = stateWorkOrder.AssignRetrials != null ? (stateWorkOrder.AssignRetrials + 1) : 1;
                    stateWorkOrder.AssignRetrialTime = stateWorkOrder.AssignRetrialTime.HasValue ? stateWorkOrder.AssignRetrialTime.Value.AddMinutes(holdInterval) : retryTime;

                    this._stateWorkOrderRepository.Update(stateWorkOrder);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => UpdateWorkOrderAssignRetrials: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> UpdateWorkOrderCancelRetrials(long workOrderID, int holdInterval)
        {
            try
            {
                using (_unitofWork)
                {
                    var retryTime = DateTimeHelper.GetDateTimeNow().AddMinutes(holdInterval);

                    var stateWorkOrder = this._stateWorkOrderRepository.GetQuery().Where(x => x.WorkOrderId == workOrderID).FirstOrDefault();

                    stateWorkOrder.CancelRetrials = stateWorkOrder.CancelRetrials != null ? (stateWorkOrder.CancelRetrials + 1) : 1;
                    stateWorkOrder.CancelRetrialTime = stateWorkOrder.CancelRetrialTime.HasValue ? stateWorkOrder.CancelRetrialTime.Value.AddMinutes(holdInterval) : retryTime;

                    this._stateWorkOrderRepository.Update(stateWorkOrder);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => UpdateWorkOrderCancelRetrials: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        //TODO : Add Sewer buz - Assign | Arrive | Confirm | Complete
        public DescriptiveResponse<List<long>> AssignWorkOrder(DispatchWorkOrderDTO dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------AssignWorkOrder - WorkOrderService - Start----------------------------------------"));

            try
            {
                if (dto == null)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("AssignWorkOrder - DTO Is NULL"));
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.COMMIT_FAIL);
                }
                LoggerManager.LogMsg(c => c.TrackingMsg("-----------------------------------------------------"));
                LoggerManager.LogMsg(c => c.TrackingMsg("AssignWorkOrder DTO:"));
                LoggerManager.LogMsg(c => c.TrackingMsg(JsonConvert.SerializeObject(dto)));
                LoggerManager.LogMsg(c => c.TrackingMsg("-----------------------------------------------------"));

                var stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.EventWorkOrderDTO.WorkOrderID);
                LoggerManager.LogMsg(c => c.TrackingMsg("AssignWorkOrder - stateWorkOrder "));

                try
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg(JsonConvert.SerializeObject(stateWorkOrder.WrapToStateWorkOrderDTO())));
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"{ex.Message} | {ex.InnerException?.Message}"));
                }

                if (stateWorkOrder.ServiceTypeID == null || stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.Ashyab)
                {
                    #region Ashyab
                    //Validate WorkOrder Status Workflow
                    if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Assigned))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - WorkOrder Status Workflow Fail - OrderNumber: {stateWorkOrder.OrderNumber} - WorkOrderStatusID: {stateWorkOrder.LastStatusID}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    var transporter = this._transporterRepository.GetQuery().Where(x => x.ID == dto.EventWorkOrderVehicleDTO.VehicleID && x.status == (int)VehicleStatusEnum.Available).FirstOrDefault();


                    var activeWOStatusIDs = new List<int>() { 5, 6, 7 };


                    if (transporter == null || this._stateWorkOrderRepository.GetQuery().Where(x => x.AssignedVehicleID == dto.EventWorkOrderVehicleDTO.VehicleID && activeWOStatusIDs.Contains(x.LastStatusID)).Any())
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Transporter Status not Available - TransporterStatus: {transporter.status}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    LoggerManager.LogMsg(c => c.TrackingMsg("AssignWorkOrder - Transporter Status is Available"));
                    //Validate Vehicle Status Workflow
                    if (transporter == null || !IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Assigned))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Validate Vehicle Status Workflow Fail - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }
                    try
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("AssignWorkOrder - transporter"));
                        LoggerManager.LogMsg(c => c.TrackingMsg(JsonConvert.SerializeObject(transporter.WrapToTransporterDTO())));
                    }
                    catch (Exception ex)
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"{ex.Message} | {ex.InnerException?.Message}"));
                    }
                    var e = new NWC_Event()
                    {
                        EventTypeID = (int)EventTypeEnum.WO_Vehicle_Assign,
                        EventTime = DateTimeHelper.GetDateTimeNow(),
                        UserID = this._loggedInUser.LoggedInUser.StaffId,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId
                    };
                    LoggerManager.LogMsg(c => c.TrackingMsg("AssignWorkOrder - NWC_Event:"));

                    var eventWorkOrder = new NWC_EventWorkOrder()
                    {
                        OrderNumber = stateWorkOrder.OrderNumber,
                        OrderQuantity = stateWorkOrder.OrderQuantity,
                        //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                        ConfirmationCode = stateWorkOrder.ConfirmationCode,
                        ServiceTypeID = stateWorkOrder.ServiceTypeID,
                        CustomerAccountId = stateWorkOrder.CustomerAccountId,
                        CreateTime = DateTimeHelper.GetDateTimeNow(),
                        CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        RequestTime = DateTimeHelper.GetDateTimeNow(),
                        ParentWorkOrderID = dto.EventWorkOrderDTO.WorkOrderID,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                        StatusID = (int)WorkOrderStatusEnum.Assigned,
                        StatusTime = DateTimeHelper.GetDateTimeNow(),
                        VehicleStatusID = (int)VehicleStatusEnum.Assigned,
                        VehicleID = dto.EventWorkOrderVehicleDTO.VehicleID,
                        DriverID = dto.EventWorkOrderVehicleDTO.DriverID,
                        VehicleLatitude = dto.EventWorkOrderDTO.VehicleLatitude,
                        VehicleLongitude = dto.EventWorkOrderDTO.VehicleLongitude
                    };
                    LoggerManager.LogMsg(c => c.TrackingMsg("AssignWorkOrder - NWC_EventWorkOrder:"));

                    using (_unitofWork)
                    {
                        e.NWC_EventWorkOrder.Add(eventWorkOrder);

                        this._eventRepository.Add(e);
                    }

                    var ids = new List<long>();
                    ids.Add(e.ID);

                    LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Event Assign Inserted Successfully - Order Number: {eventWorkOrder.OrderNumber} - TransporterID {transporter.ID} - TransportrStatus: {transporter.status}"));
                    LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------AssignWorkOrder - WorkOrderService - End----------------------------------------"));

                    return DescriptiveResponse<List<long>>.Success(ids);
                    #endregion
                }
                else if (stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    #region Sewer
                    //Validate WorkOrder Status Workflow

                    if (stateWorkOrder.LastStatusID != (int)WorkOrderStatusEnum.New)
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignSewerWorkOrder - WorkOrder Status Workflow Fail - OrderNumber: {stateWorkOrder.OrderNumber} - WorkOrderStatusID: {stateWorkOrder.LastStatusID}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }
                    //TODO : Need to check this condition
                    if (dto.EventWorkOrderDTO.DriverID != Guid.Empty && dto.EventWorkOrderVehicleDTO.VehicleID == Guid.Empty)
                        dto.EventWorkOrderVehicleDTO.VehicleID = GetVehicleIDByDriverID(dto.EventWorkOrderDTO.DriverID.Value);



                    var transporter = this._transporterRepository.GetQuery().FirstOrDefault(x => x.ID == dto.EventWorkOrderVehicleDTO.VehicleID && x.status == (int)VehicleStatusEnum.Parking);

                    var activeWOStatusIDs = new List<int>();
                    activeWOStatusIDs.Add((int)WorkOrderStatusEnum.Out_For_Delivery);
                    activeWOStatusIDs.Add((int)WorkOrderStatusEnum.Arrived);
                    activeWOStatusIDs.Add((int)WorkOrderStatusEnum.Assigned);
                    if (transporter == null || this._stateWorkOrderRepository.GetQuery().Any(x => x.AssignedVehicleID == dto.EventWorkOrderVehicleDTO.VehicleID && activeWOStatusIDs.Contains(x.LastStatusID)))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignSewerWorkOrder - Transporter Status not Available - TransporterStatus: {transporter.status}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    //Validate Vehicle Status Workflow
                    if (transporter == null)// || !IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Assigned))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignSewerWorkOrder - Validate Vehicle Status Workflow Fail - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }


                    var e = new NWC_Event()
                    {
                        EventTypeID = (int)EventTypeEnum.SW_Assign,
                        EventTime = DateTimeHelper.GetDateTimeNow(),
                        UserID = _loggedInUser.LoggedInUser.StaffId,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId
                    };

                    var eventWorkOrder = new NWC_EventWorkOrder()
                    {
                        OrderNumber = stateWorkOrder.OrderNumber,
                        OrderQuantity = stateWorkOrder.OrderQuantity,
                        //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                        ConfirmationCode = stateWorkOrder.ConfirmationCode,
                        ServiceTypeID = stateWorkOrder.ServiceTypeID,
                        CustomerAccountId = stateWorkOrder.CustomerAccountId,
                        CreateTime = DateTimeHelper.GetDateTimeNow(),
                        CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        RequestTime = DateTimeHelper.GetDateTimeNow(),
                        ParentWorkOrderID = dto.EventWorkOrderDTO.WorkOrderID,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                        StatusID = (int)WorkOrderStatusEnum.Assigned,//
                        StatusTime = DateTimeHelper.GetDateTimeNow(),
                        VehicleStatusID = (int)VehicleStatusEnum.Assigned,
                        VehicleID = dto.EventWorkOrderVehicleDTO.VehicleID,
                        DriverID = dto.EventWorkOrderVehicleDTO.DriverID,
                        VehicleLatitude = dto.EventWorkOrderDTO.VehicleLatitude,
                        VehicleLongitude = dto.EventWorkOrderDTO.VehicleLongitude
                    };

                    using (_unitofWork)
                    {
                        e.NWC_EventWorkOrder.Add(eventWorkOrder);

                        this._eventRepository.Add(e);
                    }
                  
                    var workOrderDto = GetSewerOrderBasicDetails(stateWorkOrder.WorkOrderId);
                    if (!workOrderDto.IsErrorState && workOrderDto.Value != null)
                        _signalRService.WorkOrderAssigned(new SignalRWorkOrderEvent
                        {
                            WorkOrderId = workOrderDto.Value.WorkOrderID,
                            CityId = workOrderDto.Value.CityID
                        });

                    var ids = new List<long>();
                    ids.Add(e.ID);

                    LoggerManager.LogMsg(c => c.TrackingMsg($"ConfirmSewerWorkOrder - Event Assign Inserted Successfully - Order Number: {eventWorkOrder.OrderNumber} - TransporterID {transporter.ID} - TransportrStatus: {transporter.status}"));
                    LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------ConfirmSewerWorkOrder - WorkOrderService - End----------------------------------------"));

                    return DescriptiveResponse<List<long>>.Success(ids);
                    #endregion
                }

                throw new Exception("WorkOrderService => AssignWorkOrder : service type is not ashyab or sewer");
            }
            catch (Exception ex)
            { 
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => AssignWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
            finally
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("==============================================================================="));
            }
        }

        public DescriptiveResponse<List<long>> SewerConfirmWorkOrder(EventWorkOrderDTO dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------SewerConfirmWorkOrder - WorkOrderService - Start----------------------------------------"));

            try
            {
                if (dto == null)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("SewerConfirmWorkOrder - DTO Is NULL"));
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.COMMIT_FAIL);
                }
                if (dto.VehicleID == Guid.Empty)
                {
                    dto.VehicleID = GetVehicleIDByDriverID((Guid)dto.DriverID);
                }

                var stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                if (stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    #region Sewer
                    //Validate WorkOrder Status Workflow
                    if (stateWorkOrder.LastStatusID == (int)WorkOrderStatusEnum.Assigned && stateWorkOrder.AssignedVehicleID != dto.VehicleID)
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"SewerConfirmWorkOrder - WorkOrder Status Workflow Fail - OrderNumber: {stateWorkOrder.OrderNumber} - WorkOrderStatusID: {stateWorkOrder.LastStatusID}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    //TODO : Need to check this condition
                    if (dto.DriverID != Guid.Empty && dto.VehicleID == Guid.Empty)
                        dto.VehicleID = GetVehicleIDByDriverID((Guid)dto.DriverID);


                    var transporter = this._transporterRepository.GetQuery().FirstOrDefault(x => x.ID == dto.VehicleID && x.status == (int)VehicleStatusEnum.Assigned);
                    //Validate Vehicle Status Workflow
                    if (transporter == null)// || !IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Assigned))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Validate Vehicle Status Workflow Fail - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    var e = new NWC_Event()
                    {
                        EventTypeID = (int)EventTypeEnum.SW_Confirm,
                        EventTime = DateTimeHelper.GetDateTimeNow(),
                        UserID = _loggedInUser.LoggedInUser.StaffId,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId
                    };

                    var eventWorkOrder = new NWC_EventWorkOrder()
                    {
                        OrderNumber = stateWorkOrder.OrderNumber,
                        OrderQuantity = stateWorkOrder.OrderQuantity,
                        //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                        ConfirmationCode = stateWorkOrder.ConfirmationCode,
                        ServiceTypeID = stateWorkOrder.ServiceTypeID,
                        CustomerAccountId = stateWorkOrder.CustomerAccountId,
                        CreateTime = DateTimeHelper.GetDateTimeNow(),
                        CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        RequestTime = DateTimeHelper.GetDateTimeNow(),
                        ParentWorkOrderID = dto.WorkOrderID,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                        StatusID = (int)WorkOrderStatusEnum.Out_For_Delivery,
                        StatusTime = DateTimeHelper.GetDateTimeNow(),
                        VehicleStatusID = (int)VehicleStatusEnum.InService,
                        VehicleID = stateWorkOrder.AssignedVehicleID,//dto.EventWorkOrderVehicleDTO.VehicleID,
                        DriverID = stateWorkOrder.AssignedDriverID,//dto.EventWorkOrderVehicleDTO.DriverID,
                        //VehicleLatitude = dto.EventWorkOrderDTO.VehicleLatitude,
                        //VehicleLongitude = dto.EventWorkOrderDTO.VehicleLongitude
                    };

                    using (_unitofWork)
                    {
                        e.NWC_EventWorkOrder.Add(eventWorkOrder);

                        this._eventRepository.Add(e);
                    }


                    var workOrderDto = GetSewerOrderBasicDetails(dto.WorkOrderID);
                    if (!workOrderDto.IsErrorState && workOrderDto.Value != null)
                        _signalRService.WorkOrderConfirmed(new SignalRWorkOrderEvent
                        {
                            WorkOrderId = workOrderDto.Value.WorkOrderID,
                            CityId = workOrderDto.Value.CityID
                        });

                    var ids = new List<long>();
                    ids.Add(e.ID);

                    LoggerManager.LogMsg(c => c.TrackingMsg($"SewerConfirmWorkOrder - Event Assign Inserted Successfully - Order Number: {eventWorkOrder.OrderNumber} - TransporterID {transporter.ID} - TransportrStatus: {transporter.status}"));
                    LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------SewerConfirmWorkOrder - WorkOrderService - End----------------------------------------"));

                    return DescriptiveResponse<List<long>>.Success(ids);
                    #endregion
                }

                throw new Exception("WorkOrderService => SewerConfirmWorkOrder : service type is not ashyab or sewer");
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => SewerConfirmWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> SewerCompleteWorkOrder(EventWorkOrderDTO dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------SewerCompleteWorkOrder - SewerWorkOrderService - Start----------------------------------------"));

            try
            {
                if (dto == null)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("SewerConfirmWorkOrder - DTO Is NULL"));
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.COMMIT_FAIL);
                }
                if (dto.VehicleID == Guid.Empty)
                {
                    dto.VehicleID = GetVehicleIDByDriverID((Guid)dto.DriverID);
                }
                var stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                if (stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    #region Sewer
                    //Validate WorkOrder Status Workflow
                    if ( stateWorkOrder.AssignedVehicleID != dto.VehicleID)
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"SewerConfirmWorkOrder - WorkOrder Status Workflow Fail - OrderNumber: {stateWorkOrder.OrderNumber} - WorkOrderStatusID: {stateWorkOrder.LastStatusID}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    //TODO : Need to check this condition
                    if (dto.DriverID != Guid.Empty && dto.VehicleID == Guid.Empty)
                        dto.VehicleID = GetVehicleIDByDriverID((Guid)dto.DriverID);

                    var transporter = this._transporterRepository.GetQuery().FirstOrDefault(x => x.ID == dto.VehicleID);
                    if (transporter == null)// || !IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Assigned))
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"AssignWorkOrder - Validate Vehicle Status Workflow Fail - TransporterID: {transporter.ID} - TransporterStatus: {transporter.status}"));
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    var e = new NWC_Event()
                    {
                        EventTypeID = (int)EventTypeEnum.SW_Complete,
                        EventTime = DateTimeHelper.GetDateTimeNow(),
                        UserID = _loggedInUser.LoggedInUser.StaffId,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId
                    };

                    var eventWorkOrder = new NWC_EventWorkOrder()
                    {
                        OrderNumber = stateWorkOrder.OrderNumber,
                        OrderQuantity = stateWorkOrder.OrderQuantity,
                        //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                        ConfirmationCode = stateWorkOrder.ConfirmationCode,
                        ServiceTypeID = stateWorkOrder.ServiceTypeID,
                        CustomerAccountId = stateWorkOrder.CustomerAccountId,
                        CreateTime = DateTimeHelper.GetDateTimeNow(),
                        CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        RequestTime = DateTimeHelper.GetDateTimeNow(),
                        ParentWorkOrderID = dto.WorkOrderID,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                        StatusID = (int)WorkOrderStatusEnum.Delivered,
                        StatusTime = DateTimeHelper.GetDateTimeNow(),
                        VehicleStatusID = (int)VehicleStatusEnum.Delivered,
                        VehicleID = dto.VehicleID,
                        DriverID = dto.DriverID,
                        //VehicleLatitude = dto.EventWorkOrderDTO.VehicleLatitude,
                        //VehicleLongitude = dto.EventWorkOrderDTO.VehicleLongitude
                    };

                    using (_unitofWork)
                    {
                        e.NWC_EventWorkOrder.Add(eventWorkOrder);

                        this._eventRepository.Add(e);
                    }

                    var ids = new List<long>();
                    ids.Add(e.ID);

                    LoggerManager.LogMsg(c => c.TrackingMsg($"SewerConfirmWorkOrder - Event Assign Inserted Successfully - Order Number: {eventWorkOrder.OrderNumber} - TransporterID {transporter.ID} - TransportrStatus: {transporter.status}"));
                    LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------SewerConfirmWorkOrder - WorkOrderService - End----------------------------------------"));

                    return DescriptiveResponse<List<long>>.Success(ids);
                    #endregion
                }

                throw new Exception("WorkOrderService => SewerConfirmWorkOrder : service type is not ashyab or sewer");
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => SewerConfirmWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        //==============================================================

        public DescriptiveResponse<List<long>> DeassignWorkOrder(DispatchWorkOrderDTO dto)
        {
            var ids = new List<long>();

            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.EventWorkOrderDTO.WorkOrderID);
                if(dto.EventWorkOrderVehicleDTO==null)
                {
                    dto.EventWorkOrderVehicleDTO = dto.EventWorkOrderVehicleDTO != null && dto.EventWorkOrderDTO.DriverID != null ? dto.EventWorkOrderVehicleDTO : SetVehicleDTO((Guid)dto.EventWorkOrderDTO.DriverID);
                }
                var eventDTO = new EventWorkOrderDTO()
                {
                    WorkOrderID = dto.EventWorkOrderDTO.WorkOrderID,
                    StatusReasonID = dto.EventWorkOrderDTO.StatusReasonID,
                    StatusTime = DateTimeHelper.GetDateTimeNow(),
                    VehicleID = dto.EventWorkOrderVehicleDTO.VehicleID,
                    DriverID = dto.EventWorkOrderVehicleDTO.DriverID,
                    VehicleStatusID = dto.EventWorkOrderVehicleDTO.StatusID, //(int)VehicleStatusEnum.Available,
                    VehicleLatitude = dto.EventWorkOrderDTO.VehicleLatitude,
                    VehicleLongitude = dto.EventWorkOrderDTO.VehicleLongitude,
                    Comments = dto.EventWorkOrderDTO.StatusReasonID == 1 ? "Auto" : "Manual",
                };

                if (dto.EventWorkOrderDTO.StatusID == (int)WorkOrderStatusEnum.New || (stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval && dto.EventWorkOrderDTO.StatusID== 0))
                {
                    return NotAssigned(eventDTO);
                }
                else if (dto.EventWorkOrderDTO.StatusID == (int)WorkOrderStatusEnum.Onhold)
                {
                    return OnHold(eventDTO);
                }
                else
                {
                    return CancelWorkOrder(eventDTO);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => DeassignWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> PreAssignWorkOrder(DispatchWorkOrderDTO dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------PreAssignWorkOrder - WorkOrderService - Start----------------------------------------"));

            try
            {
                if (dto == null)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("PreAssignWorkOrder - DTO Is NULL"));
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.COMMIT_FAIL);
                }

                var stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.EventWorkOrderDTO.WorkOrderID);

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.PreAssign))
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"PreAssignWorkOrder - WorkOrder Status Workflow Fail - OrderNumber: {stateWorkOrder.OrderNumber} - WorkOrderStatusID: {stateWorkOrder.LastStatusID}"));
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_PreAssign,
                    EventTime = DateTimeHelper.GetDateTimeNow(),
                    UserID = _loggedInUser.LoggedInUser.StaffId,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId
                };

                var eventWorkOrder = new NWC_EventWorkOrder()
                {
                    OrderNumber = stateWorkOrder.OrderNumber,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ConfirmationCode = stateWorkOrder.ConfirmationCode,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    CreateTime = DateTimeHelper.GetDateTimeNow(),
                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                    RequestTime = DateTimeHelper.GetDateTimeNow(),
                    ParentWorkOrderID = dto.EventWorkOrderDTO.WorkOrderID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.PreAssign,
                    StatusTime = DateTimeHelper.GetDateTimeNow(),
                    //VehicleStatusID = (int)VehicleStatusEnum.Assigned,
                    //VehicleID = dto.EventWorkOrderVehicleDTO.VehicleID,
                    //DriverID = dto.EventWorkOrderVehicleDTO.DriverID,
                    //VehicleLatitude = dto.EventWorkOrderDTO.VehicleLatitude,
                    //VehicleLongitude = dto.EventWorkOrderDTO.VehicleLongitude
                };

                using (_unitofWork)
                {
                    e.NWC_EventWorkOrder.Add(eventWorkOrder);

                    this._eventRepository.Add(e);
                }

                var ids = new List<long>();
                ids.Add(e.ID);

                LoggerManager.LogMsg(c => c.TrackingMsg($"PreAssignWorkOrder - Event pre Assign Inserted Successfully - Order Number: {eventWorkOrder.OrderNumber}"));
                LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------PreAssignWorkOrder - WorkOrderService - End----------------------------------------"));

                return DescriptiveResponse<List<long>>.Success(ids);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => PreAssignWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> OutForDeliveryWorkOrder(EventWorkOrderDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var stateWorkOrder = new NWC_StateWorkOrder();

                if (dto.WorkOrderID > 0)
                    stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                else
                    stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.OrderNumber == dto.OrderNumber);

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Out_For_Delivery))
                {
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }

                if (stateWorkOrder.AssignedVehicleID != null && stateWorkOrder.AssignedVehicleID != Guid.Empty)
                {
                    var transporter = this._transporterRepository.GetQuery().Where(x => x.ID == stateWorkOrder.AssignedVehicleID).FirstOrDefault();

                    //Validate Vehicle Status Workflow
                    if (transporter == null || !IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.InService))
                    {
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_OutForDelivery,
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
                    StatusComment = dto.StatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    StationID = stateWorkOrder.AssignedStationID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.Out_For_Delivery,
                    StatusTime = dto.StatusTime.HasValue ? dto.StatusTime.Value : DateTimeHelper.GetDateTimeNow(),
                    VehicleStatusID = (int)VehicleStatusEnum.InService,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleLatitude = dto.VehicleLatitude,
                    VehicleLongitude = dto.VehicleLongitude
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
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => OutForDeliveryWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> WOVehicleArrivedStation(WOVArrivedStationDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                #region Validation
                var OrderConfirmationCodeRequired = ConfigurationManager.AppSettings["OrderConfirmationCodeRequired"];
                if (!string.IsNullOrEmpty(OrderConfirmationCodeRequired) && OrderConfirmationCodeRequired.ToLower() == "true")
                {
                    if (string.IsNullOrEmpty(dto.ConfirmationCode))
                    {
                        return DescriptiveResponse<List<long>>.Error(ValidationMessagesKeys.EnterConfirmationCode);
                    }
                }

                if (!string.IsNullOrEmpty(dto.ConfirmationCode))
                {
                    var stateWorkOrder_ConfirmationCode = this._stateWorkOrderRepository.GetQuery()
                        .Where(s => s.WorkOrderId == dto.WorkOrderID)
                        .Select(s => s.ConfirmationCode)
                        .FirstOrDefault();

                    if (dto.ConfirmationCode.Trim().ToLower() != stateWorkOrder_ConfirmationCode.Trim().ToLower())
                    {
                        return DescriptiveResponse<List<long>>.Error(ValidationMessagesKeys.ErrorInConfirmationCode);
                    }
                }
                #endregion

                var ids = new List<long>();

                var paymentId = UpdateWorkOrderPayment(dto);

                if (!paymentId.IsErrorState)
                {
                    var arrivedId = WOVehicleArrived(dto);

                    if (paymentId.Value != null && paymentId.Value.Any())
                        ids.AddRange(paymentId.Value);

                    if (arrivedId.Value != null && arrivedId.Value.Any())
                        ids.AddRange(arrivedId.Value);

                    return DescriptiveResponse<List<long>>.Success(ids);
                }

                return DescriptiveResponse<List<long>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => WOVehicleArrivedStation: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> UpdateWorkOrderPayment(WOVArrivedStationDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }
                LoggerManager.LogMsg(c => c.TrackingMsg("-----------------------------------------------------"));
                LoggerManager.LogMsg(c => c.TrackingMsg("UpdateWorkOrderPayment DTO:"));
                LoggerManager.LogMsg(c => c.TrackingMsg(JsonConvert.SerializeObject(dto)));
                LoggerManager.LogMsg(c => c.TrackingMsg("-----------------------------------------------------"));
                var stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_UpdatePaymentStatus,
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
                    ConfirmationCode = dto.ConfirmationCode,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId
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
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => UpdateWorkOrderPayment: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> ArrivedWorkOrder(EventWorkOrderDTO dto)
        {
            try
            {
                LoggerManager.LogMsg(c => c.Log("ArrivedWorkOrder => Start: "));
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var stateWorkOrder = new NWC_StateWorkOrder();

                if (dto.WorkOrderID > 0)
                    stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                else
                    stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.OrderNumber == dto.OrderNumber);


                LoggerManager.LogMsg(c => c.Log(string.Format("ArrivedWorkOrder => LastStatusID: {0}", stateWorkOrder.LastStatusID)));
                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Arrived))
                {
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }
                LoggerManager.LogMsg(c => c.Log(string.Format("ArrivedWorkOrder => Before Event")));
                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_Arrived,
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
                    StatusComment = dto.StatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    StationID = stateWorkOrder.AssignedStationID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.Arrived,
                    VehicleStatusID = (int)VehicleStatusEnum.ArrivedToCustomer,
                    StatusTime = dto.StatusTime.HasValue ? dto.StatusTime.Value : DateTimeHelper.GetDateTimeNow(),
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleLatitude = dto.VehicleLatitude,
                    VehicleLongitude = dto.VehicleLongitude
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
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => ArrivedWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> DeliveredWorkOrder(EventWorkOrderDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                #region Validation
                var OrderConfirmationCodeRequired = ConfigurationManager.AppSettings["OrderConfirmationCodeRequired"];
                if (!string.IsNullOrEmpty(OrderConfirmationCodeRequired) && OrderConfirmationCodeRequired.ToLower() == "true")
                {
                    if (string.IsNullOrEmpty(dto.ConfirmationCode))
                    {
                        return DescriptiveResponse<List<long>>.Error(ValidationMessagesKeys.EnterConfirmationCode);
                    }
                }

                if (!string.IsNullOrEmpty(dto.ConfirmationCode))
                {
                    var stateWorkOrder_ConfirmationCode = this._stateWorkOrderRepository.GetQuery()
                        .Where(s => s.WorkOrderId == dto.WorkOrderID)
                        .Select(s => s.ConfirmationCode)
                        .FirstOrDefault();

                    if (dto.ConfirmationCode.Trim().ToLower() != stateWorkOrder_ConfirmationCode.Trim().ToLower())
                    {
                        return DescriptiveResponse<List<long>>.Error(ValidationMessagesKeys.ErrorInConfirmationCode);
                    }
                }
                #endregion

                var stateWorkOrder = new NWC_StateWorkOrder();

                if (dto.WorkOrderID > 0)
                    stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                else
                    stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.OrderNumber == dto.OrderNumber);

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Delivered))
                {
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_Delivered,
                    EventTime = DateTimeHelper.GetDateTimeNow(),
                    UserID = _loggedInUser.LoggedInUser.StaffId,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId
                };

                var eventWorkOrder = new NWC_EventWorkOrder()
                {
                    CreateTime = DateTimeHelper.GetDateTimeNow(),
                    RequestTime = DateTimeHelper.GetDateTimeNow(),
                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                    ParentWorkOrderID = dto.WorkOrderID,
                    OrderNumber = stateWorkOrder.OrderNumber,
                    StatusComment = dto.StatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    StationID = stateWorkOrder.AssignedStationID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.Delivered,
                    StatusTime = dto.StatusTime.HasValue ? dto.StatusTime.Value : DateTimeHelper.GetDateTimeNow(),
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleLatitude = dto.VehicleLatitude,
                    VehicleLongitude = dto.VehicleLongitude,
                    ConfirmationCode = dto.ConfirmationCode,
                    VehicleStatusID = (int)VehicleStatusEnum.Parking
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
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => DeliveredWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> CancelWorkOrder(EventWorkOrderDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException("dto caCancelWorkOrder(EventWorkOrderDTO dto) : " + nameof(dto));
                }

                var stateWorkOrder = new NWC_StateWorkOrder();

                if (dto.WorkOrderID > 0)
                    stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                else
                    stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.OrderNumber == dto.OrderNumber);

                if (stateWorkOrder.ServiceTypeID == null || stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.Ashyab)
                {
                    #region Ashyab
                    //Validate WorkOrder Status Workflow
                    if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Cancelled))
                    {
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }

                    if (stateWorkOrder.AssignedVehicleID != null && stateWorkOrder.AssignedVehicleID != Guid.Empty)
                    {
                        var transporter = this._transporterRepository.GetQuery().Where(x => x.ID == stateWorkOrder.AssignedVehicleID).FirstOrDefault();

                        //Validate Vehicle Status Workflow
                        if (transporter == null || !IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Available))
                        {
                            return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                        }
                    }

                    var e = new NWC_Event()
                    {
                        EventTypeID = (int)EventTypeEnum.WorkOrder_Cancelled,
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
                        StatusReasonID = dto.StatusReasonID,
                        StatusComment = dto.StatusComment,
                        OrderQuantity = stateWorkOrder.OrderQuantity,
                        ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                        //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                        ServiceTypeID = stateWorkOrder.ServiceTypeID,
                        CustomerAccountId = stateWorkOrder.CustomerAccountId,
                        StationID = stateWorkOrder.AssignedStationID,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                        StatusID = (int)WorkOrderStatusEnum.Cancelled,
                        StatusTime = dto.StatusTime.HasValue ? dto.StatusTime.Value : DateTimeHelper.GetDateTimeNow(),
                        VehicleStatusID = dto.VehicleStatusID > 0 ? dto.VehicleStatusID : (int)VehicleStatusEnum.Available, //(int)VehicleStatusEnum.Available,
                        VehicleID = stateWorkOrder.AssignedVehicleID,
                        DriverID = stateWorkOrder.AssignedDriverID,
                        VehicleLatitude = dto.VehicleLatitude,
                        VehicleLongitude = dto.VehicleLongitude
                    };

                    e.NWC_EventWorkOrder.Add(eventWorkOrder);

                    using (_unitofWork)
                    {
                        this._eventRepository.Add(e);
                    }

                    var ids = new List<long>();
                    ids.Add(e.ID);

                    return DescriptiveResponse<List<long>>.Success(ids);
                    #endregion
                }
                else if (stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    #region Sewer
                    //Validate WorkOrder Status Workflow
                    if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Cancelled))
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);

                    var e = new NWC_Event()
                    {
                        EventTypeID = (int)EventTypeEnum.SW_Cancelled,
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
                        StatusReasonID = dto.StatusReasonID,
                        StatusComment = dto.StatusComment,
                        OrderQuantity = stateWorkOrder.OrderQuantity,
                        ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                        //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                        ServiceTypeID = stateWorkOrder.ServiceTypeID,
                        CustomerAccountId = stateWorkOrder.CustomerAccountId,
                        StationID = stateWorkOrder.AssignedStationID,
                        SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                        StatusID = (int)WorkOrderStatusEnum.Cancelled,
                        StatusTime = dto.StatusTime.HasValue ? dto.StatusTime.Value : DateTimeHelper.GetDateTimeNow(),
                        VehicleStatusID = (int)VehicleStatusEnum.Parking,
                        VehicleID = stateWorkOrder.AssignedVehicleID,
                        DriverID = stateWorkOrder.AssignedDriverID,
                        VehicleLatitude = dto.VehicleLatitude,
                        VehicleLongitude = dto.VehicleLongitude
                    };

                    e.NWC_EventWorkOrder.Add(eventWorkOrder);

                    using (_unitofWork)
                    {
                        this._eventRepository.Add(e);
                    }

                    var workOrderDto = GetSewerOrderBasicDetails(stateWorkOrder.WorkOrderId);
                    if (!workOrderDto.IsErrorState && workOrderDto.Value != null)
                        _signalRService.WorkOrderCancelled(new SignalRWorkOrderEvent
                        {
                            WorkOrderId = workOrderDto.Value.WorkOrderID,
                            CityId = workOrderDto.Value.CityID
                        });

                    var ids = new List<long>();
                    ids.Add(e.ID);

                    return DescriptiveResponse<List<long>>.Success(ids);
                    #endregion
                }
                throw new Exception("WorkOrderService => CancelSewerWorkOrder : service type is not ashyab or sewer");

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => CancelWorkOrder: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> FailedToDeliver(EventWorkOrderDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var stateWorkOrder = new NWC_StateWorkOrder();

                if (dto.WorkOrderID > 0)
                    stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                else
                    stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.OrderNumber == dto.OrderNumber);

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Failed_To_Deliver))
                {
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_FailedToDeliver,
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
                    StatusComment = dto.StatusComment,
                    StatusReasonID = dto.StatusReasonID,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    StationID = stateWorkOrder.AssignedStationID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.Failed_To_Deliver,
                    StatusTime = dto.StatusTime.HasValue ? dto.StatusTime.Value : DateTimeHelper.GetDateTimeNow(),
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleLatitude = dto.VehicleLatitude,
                    VehicleStatusID = stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval ? (int)VehicleStatusEnum.Parking : (int)VehicleStatusEnum.Available,
                    VehicleLongitude = dto.VehicleLongitude,
                    
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
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => FailedToDeliver: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> OnHold(EventWorkOrderDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var stateWorkOrder = new NWC_StateWorkOrder();

                if (dto.WorkOrderID > 0)
                    stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                else
                    stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.OrderNumber == dto.OrderNumber);

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.Onhold))
                {
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }

                if (stateWorkOrder.AssignedVehicleID != null && stateWorkOrder.AssignedVehicleID != Guid.Empty)
                {
                    var transporter = this._transporterRepository.GetQuery().Where(x => x.ID == stateWorkOrder.AssignedVehicleID).FirstOrDefault();

                    //Validate Vehicle Status Workflow
                    if (transporter == null || ((!IsValidVehicleStatusWorkflow(transporter.status.Value, (int)VehicleStatusEnum.Available) && transporter.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval)))
                    {
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_OnHold,
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
                    StatusComment = dto.StatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    StationID = stateWorkOrder.AssignedStationID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.Onhold,
                    StatusReasonID = dto.StatusReasonID,
                    StatusTime = dto.StatusTime.HasValue ? dto.StatusTime : DateTimeHelper.GetDateTimeNow(),
                    //TODO : ServiceType - Check condition
                    VehicleStatusID = dto.VehicleStatusID > 0 ? dto.VehicleStatusID : stateWorkOrder.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval ? (int)VehicleStatusEnum.Available : (int)VehicleStatusEnum.Parking,
                    VehicleID = stateWorkOrder.AssignedVehicleID != Guid.Empty ? stateWorkOrder.AssignedVehicleID : (Guid?)null,
                    DriverID = stateWorkOrder.AssignedDriverID != Guid.Empty ? stateWorkOrder.AssignedDriverID : (Guid?)null,
                    VehicleLatitude = dto.VehicleLatitude,
                    VehicleLongitude = dto.VehicleLongitude
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
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => OnHold: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> NotAssigned(EventWorkOrderDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var stateWorkOrder = new NWC_StateWorkOrder();

                if (dto.WorkOrderID > 0)
                    stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);
                else
                    stateWorkOrder = this._stateWorkOrderRepository.GetQuery().FirstOrDefault(x => x.OrderNumber == dto.OrderNumber);

                //Validate WorkOrder Status Workflow
                if (!IsValidWorkOrderStatusWorkflow(stateWorkOrder.LastStatusID, (int)WorkOrderStatusEnum.New) && stateWorkOrder.LastStatusID != (int)WorkOrderStatusEnum.Delivered)
                {
                    return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WorkOrder_NotAssigned,
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
                    StatusComment = dto.StatusComment,
                    OrderQuantity = stateWorkOrder.OrderQuantity,
                    ScheduledDeliveryTime = stateWorkOrder.ScheduledDeliveryTime,
                    //CustomerLocationID = stateWorkOrder.CustomerLocationID,
                    ServiceTypeID = stateWorkOrder.ServiceTypeID,
                    CustomerAccountId = stateWorkOrder.CustomerAccountId,
                    StationID = stateWorkOrder.AssignedStationID,
                    SubID = this._loggedInUser.LoggedInUser.SubscriberId,
                    StatusID = (int)WorkOrderStatusEnum.New,
                    StatusTime = dto.StatusTime.HasValue ? dto.StatusTime.Value : DateTimeHelper.GetDateTimeNow(),
                    //TODO : ServiceType - Check condition
                    VehicleStatusID = dto.VehicleStatusID > 0 ? dto.VehicleStatusID : (int)VehicleStatusEnum.Available, //(int)VehicleStatusEnum.Available,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleLatitude = dto.VehicleLatitude,
                    VehicleLongitude = dto.VehicleLongitude,

                };
                if (stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    eventWorkOrder.VehicleStatusID = (int)VehicleStatusEnum.Parking;
                }
                using (_unitofWork)
                {
                    e.NWC_EventWorkOrder.Add(eventWorkOrder);

                    this._eventRepository.Add(e);
                }
                if (stateWorkOrder.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    var workOrderDto = GetSewerOrderBasicDetails(stateWorkOrder.WorkOrderId);
                    if (!workOrderDto.IsErrorState && workOrderDto.Value != null)
                        workOrderDto.Value.Comments = dto.Comments;
                         _signalRService.WorkOrderDeAssigned(workOrderDto.Value);
                }


                var ids = new List<long>();
                ids.Add(e.ID);

                return DescriptiveResponse<List<long>>.Success(ids);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => NotAssigned: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> WOVehicleArrived(WOVArrivedStationDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var stateWorkOrder = this._stateWorkOrderRepository.FindById(dto.WorkOrderID);

                if (stateWorkOrder.AssignedVehicleID != null && stateWorkOrder.AssignedVehicleID != Guid.Empty)
                {
                    var transporterStatus = this._transporterRepository.GetQuery()
                        .Where(x => x.ID == stateWorkOrder.AssignedVehicleID)
                        .Select(s => s.status).FirstOrDefault();

                    //Validate Vehicle Status Workflow
                    if (transporterStatus == null || !IsValidVehicleStatusWorkflow(transporterStatus.Value, (int)VehicleStatusEnum.Available))
                    {
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                    }
                }

                var e = new NWC_Event()
                {
                    EventTypeID = (int)EventTypeEnum.WO_Vehicle_ArrivedToStation,
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
                    VehicleStatusID = (int)VehicleStatusEnum.Available,
                    VehicleID = stateWorkOrder.AssignedVehicleID,
                    DriverID = stateWorkOrder.AssignedDriverID,
                    VehicleLatitude = dto.VehicleLatitude,
                    VehicleLongitude = dto.VehicleLongitude,
                    //VehicleCustomerClassId = dto.VehicleCustomerClassId
                    VehicleCustomerClassesIds = Utilities.ConvertToString(dto.VehicleCustomerLocationClassesIds)
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
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => WOVehicleArrived: "));
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> AddComment(WorkOrderCommentDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var woComment = new NWC_WorkOrderComment()
                {
                    WorkOrderID = dto.WorkOrderID,
                    Comment = dto.Comment,
                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                    CreatedTime = DateTimeHelper.GetDateTimeNow(),
                    IsDeleted = false
                };

                using (_unitofWork)
                {
                    this._workOrderComment.Add(woComment);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => AddComment: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> DeleteComment(int id)
        {
            try
            {
                var comment = this._workOrderComment.FindById(id);

                if (comment == null)
                    return DescriptiveResponse<Boolean>.Error(ErrorStatus.NOT_FOUNT);

                using (_unitofWork)
                {
                    this._workOrderComment.Delete(comment);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => DeleteComment: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> UpdateDeferredOrder(DeferredOrderDTO deferredOrder)
        {
            try
            {
                var dbObject = this._DeferredWorkOrderRepository.GetQuery()
                                .Where(s => s.ID == deferredOrder.ID
                                && (s.StatusId == 1 || s.StatusId == 3) //Pending, Failed
                                )
                                .FirstOrDefault();

                if (dbObject == null)
                    return DescriptiveResponse<Boolean>.Error(ErrorStatus.NOT_FOUNT);


                #region Mappers
                dbObject.SERVICETYPE = deferredOrder.SERVICETYPE;
                dbObject.TANKERSIZE = deferredOrder.TANKERSIZE;
                dbObject.CUSTOMERCLASS = deferredOrder.CUSTOMERCLASS;
                dbObject.XYCOORDINATESGF = $"{deferredOrder.helper_longitude} {deferredOrder.helper_latitude}";

                dbObject.SCHEDDTTM = deferredOrder.helper_scheduleTime.ToString("dd/MM/yyyy HH:mm:ss");  //deferredOrder.SCHEDDTTM; //exclude
                dbObject.CONFIRMATIONCODE = deferredOrder.CONFIRMATIONCODE;
                dbObject.CONTACTNAME = deferredOrder.CONTACTNAME;
                dbObject.CONTACTMOBILE = deferredOrder.CONTACTMOBILE;
                dbObject.MOBILENUMBER = deferredOrder.MOBILENUMBER;
                dbObject.PERSONID = deferredOrder.PERSONID;
                dbObject.PERSONIDTYPE = deferredOrder.PERSONIDTYPE;
                dbObject.PERSONIDVALUE = deferredOrder.PERSONIDVALUE;
                dbObject.PERSONPRIMARYNAME = deferredOrder.PERSONPRIMARYNAME;
                dbObject.COMMENT = deferredOrder.COMMENT;

                #endregion

                dbObject.LastUpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                dbObject.LastUpdatedTime = DateTimeHelper.GetDateTimeNow();
                dbObject.StatusId = 2; //success

                using (_unitofWork)
                {
                    this._DeferredWorkOrderRepository.Update(dbObject);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => UpdateDeferredOrder: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> CancelDeferredOrder(int deferredOrderId)
        {
            try
            {
                var dbObject = this._DeferredWorkOrderRepository.GetQuery()
                                .Where(s => s.ID == deferredOrderId
                                && (s.StatusId == 1 || s.StatusId == 3)  //Pending, Failed
                                )
                                .FirstOrDefault();

                if (dbObject == null)
                    return DescriptiveResponse<Boolean>.Error(ErrorStatus.NOT_FOUNT);


                dbObject.LastUpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                dbObject.LastUpdatedTime = DateTimeHelper.GetDateTimeNow();
                dbObject.StatusId = 4; //CancelRequest

                using (_unitofWork)
                {
                    this._DeferredWorkOrderRepository.Update(dbObject);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => CancelDeferredOrder: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> CheckCanUpdateDeferredOrder(DeferredOrderDTO deferredOrder)
        {
            try
            {
                var dbObject = this._DeferredWorkOrderRepository.GetQuery()
                                .Where(s => s.ID == deferredOrder.ID
                                && (s.StatusId == 1 || s.StatusId == 3) //Pending, Failed
                                )
                                .Any();

                return DescriptiveResponse<Boolean>.Success(dbObject);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => CheckCanUpdateDeferredOrder: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region Query
        public DescriptiveResponse<Boolean> IsCustomerExceededQuota(Guid stationID, long customerID)
        {
            try
            {
                var station = this._landmarkRepository.FindById(stationID);
                var citySettings = this._BranchRepository.GetQuery().Where(x => x.Id == station.branchId.Value).Select(x => x.NWC_BranchSetting).FirstOrDefault();
                var tankerQuotaNo = citySettings != null ? citySettings.TankerQuotaNo : -1;

                LoggerManager.LogMsg(c => c.TrackingMsg($"IsCustomerExceededQuota - TankerQuotaNo: {tankerQuotaNo}, CustomerID: {customerID}, StationID: {station.Id}, CityID: {station.branchId.Value}"));

                if (tankerQuotaNo > 0)
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
                    woFilter.PageFilter = new PageFilter() { PageSize = 1000, PageIndex = 1 };

                    var searchCriteria = new WorkOrderSearchCriteriaDTO()
                    {
                        StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.New, (int)WorkOrderStatusEnum.Assigned, (int)WorkOrderStatusEnum.Onhold,
                                                                    (int)WorkOrderStatusEnum.Arrived, (int)WorkOrderStatusEnum.Out_For_Delivery, (int)WorkOrderStatusEnum.Delivered },
                        DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate,
                        DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddHours((tankerQuotaNo.HasValue && tankerQuotaNo.Value > 0) ? -tankerQuotaNo.Value : -24),
                        DateTimeTo = DateTimeHelper.GetDateTimeNow().AddDays(1),
                        CityIDs = new List<Guid>() { station.branchId.Value },
                        CustomerIDs = new List<long>() { customerID },
                        ClassIDs = custClasses,
                        FilterModel = woFilter
                    };

                    var workOrdersResults = SearchWorkOrders(searchCriteria);
                    var workOrders = workOrdersResults.Value.Result;

                    LoggerManager.LogMsg(c => c.TrackingMsg($"IsCustomerExceededQuota - WorkOrdersResults-TotalCount: {workOrdersResults.Value.TotalCount}"));

                    return DescriptiveResponse<Boolean>.Success(workOrdersResults.Value.TotalCount > 0);
                }

                return DescriptiveResponse<Boolean>.Success(false);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => IsCustomerExceededQuota: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> IsZoneWithoutTankers(long customerAccountID)
        {
            try
            {
                var custClasses = new List<int>();

                if (!string.IsNullOrEmpty(this.AllowedCustomerClasses_config))
                {
                    foreach (var cla in AllowedCustomerClasses_config.Split(','))
                    {
                        custClasses.Add(int.Parse(cla));
                    }
                }

                var zoneWithoutTankers = this._customerAccount.GetQuery().Where(x => x.ID == customerAccountID &&
                                custClasses.Contains(x.NWC_CustomerLocation.ClassID))
                                .Select(a => a.NWC_CustomerLocation.NWC_Zone.ZoneWithoutTanker)
                                .FirstOrDefault();

                return (zoneWithoutTankers.HasValue && zoneWithoutTankers.Value) ? DescriptiveResponse<Boolean>.Success(true) : DescriptiveResponse<Boolean>.Success(false);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => IsZoneWithoutTankers: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DispatchWorkOrderDTO GetMatchedWorkOrderToAssign(Guid vehicleID)
        {
            var workOrderToAssign = new DispatchWorkOrderDTO();
            workOrderToAssign.EventWorkOrderDTO = new EventWorkOrderDTO();

            try
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("----------------------------------------GetMatchedWorkOrderToAssign----------------------------------------"));

                if (vehicleID == null || vehicleID == Guid.Empty)
                    return workOrderToAssign;

                var vehicle = this._transporter.GetQuery().Where(x => x.ID == vehicleID && x.status == 0).FirstOrDefault();

                if (vehicle != null)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"Get available vehicle by ID - VehicleID: {vehicleID} - StationID: {vehicle.landmark.Value}"));

                    var zone = this._zoneStations.GetQuery().Where(x => x.StationID == vehicle.landmark.Value).FirstOrDefault();

                    if (zone != null)
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg($"Get zone vehicle landmark - StationID: {vehicle.landmark.Value} - ZoneID: {zone.ZoneID}"));

                        var restricted = this._restrictedZoneVehicleTypeRep.GetQuery().Where(x =>
                                            x.ZoneID == zone.ZoneID && (x.VehicleTypeID == vehicle.transporterType)).Any();

                        LoggerManager.LogMsg(c => c.TrackingMsg($"Check restricted zone vehicle type - VehicleTypeID: {vehicle.transporterType} - Is Restricted: {restricted}"));

                        if (!restricted && vehicle.Transporter_Staff.Any())
                        {
                            var vehicleCLasses = this._vehicleClassRepository.GetQuery().Where(x => x.VehicleID == vehicle.ID).Select(x => x.CustomerLocationClassID).ToList();

                            LoggerManager.LogMsg(c => c.TrackingMsg($"Get vehicle classes - Classes Count: {vehicleCLasses.Count}"));

                            #region Predicate
                            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

                            predicate = predicate.And(s => s.IsDeleted != true);
                            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

                            #region Lists Predicate
                            if (vehicleCLasses != null && vehicleCLasses.Any())
                            {
                                predicate = predicate.And(s => vehicleCLasses.Any(a => a == s.ClassID));
                            }

                            predicate = predicate.And(s => s.ServiceTypeID == vehicle.ServiceTypeID.Value);

                            predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityID.Value));
                            //predicate = predicate.And(s => s.ZoneID == zone.ZoneID);
                            predicate = predicate.And(s => s.AssignedStationID == vehicle.landmark.Value);
                            predicate = predicate.And(s => s.LastStatusID == (int)WorkOrderStatusEnum.New);

                            predicate = predicate.And(s => s.OrderQuantity == vehicle.Capacity);
                            #endregion

                            #region Date Time Predicate
                            int delayInMin = 0;
                            int.TryParse(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DelayWOToAssignInMin"]) ?
                            ConfigurationManager.AppSettings["DelayWOToAssignInMin"].ToString() : string.Empty, out delayInMin);

                            var dtFrom = DateTimeHelper.GetDateTimeNow().AddMinutes(-delayInMin);
                            predicate = predicate.And(s => s.ScheduledDeliveryTime <= dtFrom);
                            #endregion
                            #endregion

                            LoggerManager.LogMsg(c => c.TrackingMsg($"Get WorkOrder - dtFrom: {dtFrom.ToString()}"));

                            IQueryable<vw_NWC_WorkOrderList> workOrderList = null;

                            #region skip & take
                            var skip = 0;
                            var take = 1;
                            #endregion

                            workOrderList = this._workOrderListRepository.GetQuery()
                                .Where(predicate)
                                .OrderBy(s => s.ScheduledDeliveryTime)
                                .Skip(skip)
                                .Take(take);

                            if (workOrderList != null && workOrderList.Any())
                            {
                                var workOrder = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).FirstOrDefault();

                                if (workOrder != null)
                                {
                                    LoggerManager.LogMsg(c => c.TrackingMsg($"Get WorkOrder - Order Number: {workOrder.OrderNumber}"));

                                    workOrderToAssign.EventWorkOrderVehicleDTO = new EventWorkOrderVehicleDTO()
                                    {
                                        VehicleID = vehicle.ID,
                                        DriverID = vehicle.Transporter_Staff.FirstOrDefault().Staff.Value
                                    };

                                    workOrderToAssign.EventWorkOrderDTO = new EventWorkOrderDTO()
                                    {
                                        WorkOrderID = workOrder.WorkOrderID
                                    };
                                }
                                else
                                    LoggerManager.LogMsg(c => c.TrackingMsg($"Get WorkOrder - Order Number: null"));
                            }
                            else
                                LoggerManager.LogMsg(c => c.TrackingMsg($"Get WorkOrder - Order List: null or empty"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetMatchedWorkOrderToAssign: "));
            }

            return workOrderToAssign;
        }

        public DescriptiveResponse<SearchResult<WorkOrderDTO>> SearchWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            predicate = predicate.And(s => s.IsDeleted != true);
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.OrderNumber.Contains(word));
            }

            #region Lists Predicate
          
            if (searchCriteria.CustomerIDs != null && searchCriteria.CustomerIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CustomerIDs.Any(a => a == s.CustomerID));
            }
            if (searchCriteria.ClassIDs != null && searchCriteria.ClassIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ClassIDs.Any(a => a == s.ClassID));
            }
            if (searchCriteria.PriorityIDs != null && searchCriteria.PriorityIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.PriorityIDs.Any(a => a == s.PriorityID));
            }
            if (searchCriteria.TanckerCapacityAddIds != null && searchCriteria.TanckerCapacityAddIds.Any())
            {
                predicate = predicate.And(s => searchCriteria.TanckerCapacityAddIds.Any(a => a == s.OrderQuantity));
            }
            if (searchCriteria._operator > 0)
            {
                switch (searchCriteria._operator)
                {
                    case Operator.Equal:
                        predicate = predicate.And(s => searchCriteria.Price == s.TotalCost);
                        break;

                    case Operator.LessThan:
                        predicate = predicate.And(s => s.TotalCost < searchCriteria.Price);
                        break;

                    case Operator.MoreThan:
                        predicate = predicate.And(s => s.TotalCost > searchCriteria.Price);
                        break;
                }
            }

            if (searchCriteria.SourceApps != null && searchCriteria.SourceApps.Any())
            {
                predicate = predicate.And(s => searchCriteria.SourceApps.Contains(s.SourceApplication));
            }

            //permitted serviceTypes
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            }

            //permitted cities
            if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            {
                var searchList = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.CityID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityID.Value));
            }

            if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ZoneIDs.Any(a => a == s.ZoneID));
            }
            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));
            }

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
            }
            if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.AssignedVehicleID));
            }
            if (searchCriteria.DriverIDs != null && searchCriteria.DriverIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.DriverIDs.Any(a => a == s.AssignedDriverID));
            }
            if (searchCriteria.CategoryIDs != null && searchCriteria.CategoryIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CategoryIDs.Any(a => a == s.CategoryID));
            }
            #endregion

            if (!string.IsNullOrEmpty(searchCriteria.CustomerIdNumber))
            {
                var text = searchCriteria.CustomerIdNumber.Trim();
                predicate = predicate.And(s => s.CustomerIdNumber.Contains(text));
            }

            if (!string.IsNullOrEmpty(searchCriteria.CustomerMobile))
            {
                var text = searchCriteria.CustomerMobile.Trim();
                predicate = predicate.And(s => s.CustomerMobile.Contains(text));
            }

            #region Date Time Predicate
            if (searchCriteria.DateTimeFrom != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                        break;
                }
            }
            if (searchCriteria.DateTimeTo != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                        break;
                }
            }
            #endregion

            #endregion

            //test------------------------------------------------------------------------------------------------
            //var text = $"SearchOrders@LoggedInUser.Lang: {this._loggedInUser.LoggedInUser.Lang}@thread lang (SearchWorkOrders): {Thread.CurrentThread.CurrentCulture.Name}";
            //text = text.Replace("@", System.Environment.NewLine);
            //LoggerManager.LogMsg(c => c.TrackingMsg(text);
            //----------------------------------------------------------------------------------------------------

            IQueryable<vw_NWC_WorkOrderList> workOrderList = null;
            if (searchCriteria.excelFlage == true)
            {
                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime);
            }
            else
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
                {
                    skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                    take = searchCriteria.FilterModel.PageFilter.PageSize;
                }
                #endregion

                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime)
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<WorkOrderDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._workOrderListRepository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetWorkOrdersExceededQuota(WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            var retryTime = DateTimeHelper.GetDateTimeNow().AddMinutes(-holdInterval);

            predicate = predicate.And(s => s.AssignRetrials == 0 || (s.AssignRetrials < retrials) && (s.AssignRetrialTime <= retryTime));
            predicate = predicate.And(s => s.IsDeleted != true);
            predicate = predicate.And(s => s.PriorityID != 1); //Just noraml Orders
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.OrderNumber.Contains(word));
            }

            #region Lists Predicate
            if (searchCriteria.CustomerIDs != null && searchCriteria.CustomerIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CustomerIDs.Any(a => a == s.CustomerID));
            }
            if (searchCriteria.ClassIDs != null && searchCriteria.ClassIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ClassIDs.Any(a => a == s.ClassID));
            }
            if (searchCriteria.PriorityIDs != null && searchCriteria.PriorityIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.PriorityIDs.Any(a => a == s.PriorityID));
            }
            if (searchCriteria.TanckerCapacityAddIds != null && searchCriteria.TanckerCapacityAddIds.Any())
            {
                predicate = predicate.And(s => searchCriteria.TanckerCapacityAddIds.Any(a => a == s.OrderQuantity));
            }
            if (searchCriteria._operator > 0)
            {
                switch (searchCriteria._operator)
                {
                    case Operator.Equal:
                        predicate = predicate.And(s => searchCriteria.Price == s.TotalCost);
                        break;

                    case Operator.LessThan:
                        predicate = predicate.And(s => s.TotalCost < searchCriteria.Price);
                        break;

                    case Operator.MoreThan:
                        predicate = predicate.And(s => s.TotalCost > searchCriteria.Price);
                        break;
                }
            }

            //permitted serviceTypes
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            }

            //permitted cities
            if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            {
                var searchList = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.CityID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityID.Value));
            }

            if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ZoneIDs.Any(a => a == s.ZoneID));
            }
            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));
            }

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
            }
            if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.AssignedVehicleID));
            }
            if (searchCriteria.DriverIDs != null && searchCriteria.DriverIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.DriverIDs.Any(a => a == s.AssignedDriverID));
            }
            if (searchCriteria.CategoryIDs != null && searchCriteria.CategoryIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CategoryIDs.Any(a => a == s.CategoryID));
            }
            #endregion

            if (!string.IsNullOrEmpty(searchCriteria.CustomerIdNumber))
            {
                var text = searchCriteria.CustomerIdNumber.Trim();
                predicate = predicate.And(s => s.CustomerIdNumber.Contains(text));
            }

            if (!string.IsNullOrEmpty(searchCriteria.CustomerMobile))
            {
                var text = searchCriteria.CustomerMobile.Trim();
                predicate = predicate.And(s => s.CustomerMobile.Contains(text));
            }

            #region Date Time Predicate
            if (searchCriteria.DateTimeFrom != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                        break;
                }
            }
            if (searchCriteria.DateTimeTo != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                        break;
                }
            }
            #endregion

            #endregion

            //test------------------------------------------------------------------------------------------------
            //var text = $"SearchOrders@LoggedInUser.Lang: {this._loggedInUser.LoggedInUser.Lang}@thread lang (SearchWorkOrders): {Thread.CurrentThread.CurrentCulture.Name}";
            //text = text.Replace("@", System.Environment.NewLine);
            //LoggerManager.LogMsg(c => c.TrackingMsg(text);
            //----------------------------------------------------------------------------------------------------

            IQueryable<vw_NWC_WorkOrderList> workOrderList = null;
            if (searchCriteria.excelFlage == true)
            {
                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime);
            }
            else
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
                {
                    skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                    take = searchCriteria.FilterModel.PageFilter.PageSize;
                }
                #endregion

                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime)
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<WorkOrderDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._workOrderListRepository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetNotAssignedWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            var retryTime = DateTimeHelper.GetDateTimeNow().AddMinutes(-holdInterval);

            predicate = predicate.And(s => s.AssignRetrials == 0 || (s.AssignRetrials < retrials) && (s.AssignRetrialTime <= retryTime));

            predicate = predicate.And(s => s.IsDeleted != true);
            //TODO : IsVirtualStation
            predicate = predicate.And(s => s.IsVirtualStation != true);
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.OrderNumber.Contains(word));
            }

            #region Lists Predicate
            
            if (searchCriteria.CustomerIDs != null && searchCriteria.CustomerIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CustomerIDs.Any(a => a == s.CustomerID));
            }
            if (searchCriteria.ClassIDs != null && searchCriteria.ClassIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ClassIDs.Any(a => a == s.ClassID));
            }
            if (searchCriteria.PriorityIDs != null && searchCriteria.PriorityIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.PriorityIDs.Any(a => a == s.PriorityID));
            }
           
            //permitted serviceTypes
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            }

            //permitted cities
            if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            {
                var searchList = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.CityID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityID.Value));
            }

            if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ZoneIDs.Any(a => a == s.ZoneID));
            }
            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));
            }

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
            }
            if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.AssignedVehicleID));
            }
            if (searchCriteria.DriverIDs != null && searchCriteria.DriverIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.DriverIDs.Any(a => a == s.AssignedDriverID));
            }
            #endregion

            #region Date Time Predicate
            if (searchCriteria.DateTimeFrom != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                        break;
                }
            }
            if (searchCriteria.DateTimeTo != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                        break;
                }
            }
            #endregion

            #endregion

            IQueryable<vw_NWC_WorkOrderList> workOrderList = null;

            #region skip & take
            var skip = 0;
            var take = 10;
            if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
            {
                skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;

                take = searchCriteria.FilterModel.PageFilter.PageSize;
            }
            #endregion

            workOrderList = this._workOrderListRepository.GetQuery()
                .Where(predicate)
                     .OrderBy(s => s.PriorityID)
                .ThenBy(s => s.ScheduledDeliveryTime)
                .Skip(skip)
                .Take(take);

            #region response
            var result = new SearchResult<WorkOrderDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._workOrderListRepository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetWorkOrdersReadyToAutoCancel(WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

                var retryTime = DateTimeHelper.GetDateTimeNow().AddMinutes(-holdInterval);

                predicate = predicate.And(s => s.CancelRetrials == 0 || (s.CancelRetrials < retrials) && (s.CancelRetrialTime <= retryTime));
                predicate = predicate.And(s => s.PriorityID != 1);
                predicate = predicate.And(s => s.IsDeleted != true);
                predicate = predicate.And(s => s.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval);
                predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

                if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
                {
                    var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                    predicate = predicate.And(s => s.OrderNumber.Contains(word));
                }

                if (searchCriteria.IsZoneWithoutTankers.HasValue && searchCriteria.IsZoneWithoutTankers.Value)
                {
                    predicate = predicate.And(s => s.ZoneWithoutTanker.Value);
                }

                #region Lists Predicate
                if (searchCriteria.CustomerIDs != null && searchCriteria.CustomerIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.CustomerIDs.Any(a => a == s.CustomerID));
                }
                if (searchCriteria.ClassIDs != null && searchCriteria.ClassIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.ClassIDs.Any(a => a == s.ClassID));
                }
                if (searchCriteria.PriorityIDs != null && searchCriteria.PriorityIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.PriorityIDs.Any(a => a == s.PriorityID));
                }

                //permitted serviceTypes
                if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
                {
                    var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                    predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
                }

                //permitted cities
                if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
                {
                    var searchList = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs);
                    predicate = predicate.And(s => searchList.Any(a => a == s.CityID));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityID.Value));
                }

                if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.ZoneIDs.Any(a => a == s.ZoneID));
                }
                //permitted stations
                if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
                {
                    var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                    predicate = predicate.And(s => searchList.Any(a => a == s.AssignedStationID));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));
                }

                if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
                }
                if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.AssignedVehicleID));
                }
                if (searchCriteria.DriverIDs != null && searchCriteria.DriverIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.DriverIDs.Any(a => a == s.AssignedDriverID));
                }
                #endregion

                #region Date Time Predicate
                if (searchCriteria.DateTimeFrom != null)
                {
                    switch (searchCriteria.DatePeriod)
                    {
                        case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                            predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                            predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                            predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                            break;
                    }
                }
                if (searchCriteria.DateTimeTo != null)
                {
                    switch (searchCriteria.DatePeriod)
                    {
                        case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                            predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                            predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                            predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                            break;
                    }
                }
                #endregion

                DateTime dtToCancel = DateTimeHelper.GetDateTimeNow().AddHours(-searchCriteria.CancelAfterHours);
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime <= dtToCancel);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.CreateTime <= dtToCancel);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime <= dtToCancel);
                        break;
                }


                #endregion

                IQueryable<vw_NWC_WorkOrderList> workOrderList = null;
                if (searchCriteria.excelFlage == true)
                {
                    workOrderList = this._workOrderListRepository.GetQuery()
                        .Where(predicate)
                        .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime);
                }
                else
                {
                    #region skip & take
                    var skip = 0;
                    var take = 10;
                    if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
                    {
                        skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                        take = searchCriteria.FilterModel.PageFilter.PageSize;
                    }
                    #endregion

                    workOrderList = this._workOrderListRepository.GetQuery()
                        .Where(predicate)
                        .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime)
                        .Skip(skip)
                        .Take(take);
                }

                #region response
                var result = new SearchResult<WorkOrderDTO>();
                if (workOrderList != null && workOrderList.Any())
                {
                    var count = this._workOrderListRepository.GetQuery()
                        .Count(predicate);

                    result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetWorkOrdersReadyToAutoCancel: "));
                return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>> GetSewerNewWorkOrdersWithZoneDetails()
        {
            try
            {
                var stateWorkOrder = _stateWorkOrderRepository.GetQuery().Where(x => x.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval && x.LastStatusID == (int)WorkOrderStatusEnum.New && x.CustomerLocationID.HasValue);
                if (stateWorkOrder != null && stateWorkOrder.Any())
                {
                    var result = stateWorkOrder.AsEnumerable().Select(x =>
                    {
                        var zone = _customerLocation.GetQuery().FirstOrDefault(z => z.ID == x.CustomerLocationID);
                        return new WorkOrderWithZoneDto
                        {
                            WorkOrderId = x.WorkOrderId,
                            OrderNumber = x.OrderNumber,
                            RequestTime = x.RequestTime,
                            ZoneId = zone?.NWC_Zone != null ? zone.NWC_Zone.ID : 0,
                            ZoneName = zone?.NWC_Zone?.Name,
                            ZoneLongitude = zone.Longitude.GetValueOrDefault(),
                            ZoneLatitude = zone.Latitude.GetValueOrDefault(),
                            LastStatusID = x.LastStatusID,
                            LastStatusBy = x.LastStatusBy,
                        };
                    }).ToList();
                    return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>.Success(result);
                }
                return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>.Error("No sewer workorders found", ErrorStatus.NOT_FOUNT);

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetSewerNewWorkOrdersWithZoneDetails: "));
                return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> CancelSewerWorkOrdersReadyToCancel(int? retrials, int? holdInterval)
        {
            var workOrders = GetSewerPreAssignWorkOrdersReadyToAutoCancel(retrials, holdInterval);
            if (!workOrders.IsErrorState && workOrders.Value != null && workOrders.Value.Any())
            {
                var returnValue = new List<long>();
                foreach (var wo in workOrders.Value)
                {
                    var cancelled = CancelWorkOrder(new EventWorkOrderDTO()
                    {
                        WorkOrderID = wo.WorkOrderID,
                        OrderNumber = wo.OrderNumber,
                        StatusTime = DateTimeHelper.GetDateTimeNow(),
                        ServiceTypeID = (int)ServiceTypeEnum.SewageRemoval,
                        StatusReasonID = 0,
                        StatusComment = "Cancelled by system"
                    });
                    if (!cancelled.IsErrorState && cancelled.Value != null)
                        returnValue.AddRange(cancelled.Value);
                    else
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }
                return DescriptiveResponse<List<long>>.Success(returnValue);
            }
            else
            {
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.NOT_FOUNT);
            }
        }

        public DescriptiveResponse<IEnumerable<WorkOrderDTO>> GetSewerPreAssignWorkOrdersReadyToAutoCancel(int? retrials, int? holdInterval)
        {
            try
            {
                if (retrials.HasValue)
                {
                    var assignOrders = _stateWorkOrderRepository.GetQuery().Where(x => x.LastStatusID == (int)WorkOrderStatusEnum.New && x.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval).Select(a => a.WorkOrderId).ToList();

                    if (assignOrders.Count == 0)
                        return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Success(new List<WorkOrderDTO>());

                    //var orderNumbers = new List<string>();
                    var initialOrders = _eventWorkOrderRepository.GetQuery().Where(e => assignOrders.Any(a => a == e.ParentWorkOrderID) && e.StatusID == (int)WorkOrderStatusEnum.Assigned).ToList();
                    var orders = initialOrders.GroupBy(x => x.OrderNumber).Where(x => x.Count(y => y.StatusID == (int)WorkOrderStatusEnum.Assigned) >= retrials.Value).Select(x => x.Key);

                    //orderNumbers.Concat(orders);

                    var workOrders = orders.Distinct().Select(x => _stateWorkOrderRepository.GetQuery().FirstOrDefault(wo => wo.OrderNumber == x).WrapToStateWorkOrderDTO());
                    LoggerManager.LogMsg(c => c.Log("WorkOrderService => GetSewerPreAssignWorkOrdersReadyToAutoCancel: Completed "));

                    return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Success(workOrders);
                }
                var timeFrom = DateTime.Now.AddMinutes(-holdInterval.Value);
                if (holdInterval.HasValue)
                {
                    var orders = _stateWorkOrderRepository.GetQuery()
                        .Where(x =>
                        x.LastStatusID == (int)WorkOrderStatusEnum.New &&
                        x.CreateTime.Value < timeFrom  &&
                        x.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval).ToList();

                    if (orders.Count == 0)
                        return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Success(new List<WorkOrderDTO>());

                    var returnOrders = orders.Select(a => a.WrapToStateWorkOrderDTO());

                    LoggerManager.LogMsg(c => c.Log("WorkOrderService => GetSewerPreAssignWorkOrdersReadyToAutoCancel: Completed "));

                    return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Success(returnOrders);
                }

                return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Success(null);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetSewerPreAssignWorkOrdersReadyToAutoCancel: "));
                return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> DeAssignSewerWorkOrdersAfterTimeout(int period)
        {
            if (period != 0)
            {
                DateTime date = DateTime.Now.AddMinutes(-period);
                var workOrders = _stateWorkOrderRepository.GetQuery()
                    .Where(x =>
                        x.LastStatusID == (int)WorkOrderStatusEnum.Assigned
                        && x.LastModifiedTime.HasValue && x.LastModifiedTime.Value <= date
                        && x.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                    .ToList();
                var returnValue = new List<long>();
                foreach (var wo in workOrders)
                {
                    var deassigned = DeassignWorkOrder(new DispatchWorkOrderDTO()
                    {
                        EventWorkOrderDTO = new EventWorkOrderDTO
                        {
                            WorkOrderID = wo.WorkOrderId,
                            OrderNumber = wo.OrderNumber,
                            StatusTime = DateTimeHelper.GetDateTimeNow(),
                            ServiceTypeID = (int)ServiceTypeEnum.SewageRemoval,
                            StatusID = (int)WorkOrderStatusEnum.New,
                            StatusReasonID = 1,
                            StatusComment = "Sewer De-assigned by system",
                            CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        },
                        EventWorkOrderVehicleDTO = new EventWorkOrderVehicleDTO
                        {
                            VehicleID = wo.AssignedVehicleID.HasValue ? wo.AssignedVehicleID.Value : Guid.Empty,
                            DriverID = wo.AssignedDriverID,
                            StatusID = (int)WorkOrderStatusEnum.New,
                            StatusReasonID = 1,
                            StatusComment = "De-assigned by system",
                            CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        }
                    });
                    if (!deassigned.IsErrorState && deassigned.Value != null)
                        returnValue.AddRange(deassigned.Value);
                    else
                        return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
                }
                LoggerManager.LogMsg(c => c.Log("WorkOrderService => GetSewerPreAssignWorkOrdersReadyToAutoCancel: Completed "));
                return DescriptiveResponse<List<long>>.Success(returnValue);
            }
            else
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.INPUT_INVALID);
        }


        public DescriptiveResponse<List<bool>> ExitSewerVehicleAfterTimeout(int period)
        {
            if (period != 0)
            {
                DateTime date = DateTime.Now.AddMinutes(-period);
                var transporters = _transporterRepository.GetQuery()
                    .Where(x =>
                        x.status == (int)VehicleStatusEnum.Dumping
                        && x.LastModificationDate.HasValue && x.LastModificationDate.Value <= date
                        && x.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                    .ToList();
                var returnValue = new List<bool>();
                foreach (var transporter in transporters)
                {
                    var Out = _SewerService.OutForWork(transporter.ID);
                    if (!Out.IsErrorState && Out.Value)
                        returnValue.Add(Out.Value);
                    else
                        return DescriptiveResponse<List<bool>>.Error(ErrorStatus.INPUT_INVALID);
                }
                LoggerManager.LogMsg(c => c.Log("WorkOrderService => GetSewerPreAssignWorkOrdersReadyToAutoCancel: Completed "));
                return DescriptiveResponse<List<bool>>.Success(returnValue);
            }
            else
                return DescriptiveResponse<List<bool>>.Error(ErrorStatus.INPUT_INVALID);
        }
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDriverWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            predicate = predicate.And(s => s.IsDeleted != true);
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);
            predicate = predicate.And(s => this._loggedInUser.LoggedInUser.StaffId == s.AssignedDriverID);

            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.OrderNumber.Contains(word));
            }

            #region Lists Predicate
            //if (searchCriteria.CustomerIDs != null && searchCriteria.CustomerIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.CustomerIDs.Any(a => a == s.CustomerID));
            //}
            //if (searchCriteria.ClassIDs != null && searchCriteria.ClassIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.ClassIDs.Any(a => a == s.ClassID));
            //}
            //if (searchCriteria.PriorityIDs != null && searchCriteria.PriorityIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.PriorityIDs.Any(a => a == s.PriorityID));
            //}
            ////permitted serviceTypes
            //if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            //{
            //    var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
            //    predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            //}
            //else
            //{
            //    predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            //}

            //permitted cities
            //if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            //{
            //    var searchList = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs);
            //    predicate = predicate.And(s => searchList.Any(a => a == s.CityID));
            //}
            //else
            //{
            //    predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityID.Value));
            //}

            //if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.ZoneIDs.Any(a => a == s.ZoneID));
            //}
            ////permitted stations
            //if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            //{
            //    var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
            //    predicate = predicate.And(s => searchList.Any(a => a == s.AssignedStationID));
            //}
            //else
            //{
            //    predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));
            //}

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
            }
            if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.AssignedVehicleID));
            }
            #endregion

            #region Date Time Predicate
            if (searchCriteria.DateTimeFrom != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                        break;
                }
            }
            if (searchCriteria.DateTimeTo != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                        break;
                }
            }
            #endregion

            #endregion

            IQueryable<vw_NWC_WorkOrderList> workOrderList = null;
            if (searchCriteria.excelFlage == true)
            {
                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime);
            }
            else
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
                {
                    skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                    take = searchCriteria.FilterModel.PageFilter.PageSize;
                }
                #endregion

                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime)
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<WorkOrderDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._workOrderListRepository.GetQuery().Count(predicate);
                var list = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).ToList();

                foreach (var wo in list)
                {
                    var branchSetting = this._BranchRepository.FindById(wo.CityID.HasValue ? wo.CityID.Value : Guid.Empty);

                    wo.ValidateConfermationCode = (branchSetting != null &&
                                                                branchSetting.NWC_BranchSetting != null &&
                                                                branchSetting.NWC_BranchSetting.ValidateConfermationCode.HasValue) ?
                                                                branchSetting.NWC_BranchSetting.ValidateConfermationCode.Value : false;
                }

                result.Result = list;
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
            #endregion
        }


        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetSewerWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            predicate = predicate.And(s => s.IsDeleted != true);
            predicate = predicate.And(s => s.CategoryID !=null &&  s.CategoryID == (int)SewerOrderType.Chargerd);

            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.OrderNumber.Contains(word));
            }
        var transporter =    this._transporterStaffRepository.GetQuery().Where(x => x.Staff == this._loggedInUser.LoggedInUser.StaffId).Select(x=>x.Transporter1).FirstOrDefault();
            predicate = predicate.And(s => s.CategoryID != null && ((s.CategoryID == (int)SewerOrderType.Chargerd && !transporter.IsFreeSewer) || (s.CategoryID == (int)SewerOrderType.Free && transporter.IsFreeSewer)));
            if (transporter != null)
            {
                predicate = predicate.And(s => s.CityID == transporter.branch);
                predicate = predicate.And(s => s.OrderQuantity == transporter.Capacity);
            }
            else
            {
                return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.NOT_FOUNT);
            }
            
            #region Lists Predicate
            //if (searchCriteria.CustomerIDs != null && searchCriteria.CustomerIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.CustomerIDs.Any(a => a == s.CustomerID));
            //}
            //if (searchCriteria.ClassIDs != null && searchCriteria.ClassIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.ClassIDs.Any(a => a == s.ClassID));
            //}
            //if (searchCriteria.PriorityIDs != null && searchCriteria.PriorityIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.PriorityIDs.Any(a => a == s.PriorityID));
            //}
            ////permitted serviceTypes
            //if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            //{
            //    var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
            //    predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            //}
            //else
            //{
            //    predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            //}

            //permitted cities
            //if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            //{
            //    var searchList = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs);
            //    predicate = predicate.And(s => searchList.Any(a => a == s.CityID));
            //}
            //else
            //{
            //    predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityID.Value));
            //}

            //if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.ZoneIDs.Any(a => a == s.ZoneID));
            //}
            ////permitted stations
            //if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            //{
            //    var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
            //    predicate = predicate.And(s => searchList.Any(a => a == s.AssignedStationID));
            //}
            //else
            //{
            //    predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));
            //}

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
            }
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ServiceTypeIDs.Any(a => a == s.ServiceTypeID));
            }
            if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.AssignedVehicleID));
            }
            #endregion

            #region Date Time Predicate
            if (searchCriteria.DateTimeFrom != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                        break;
                }
            }
            if (searchCriteria.DateTimeTo != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                        break;
                }
            }
            #endregion

            #endregion

            IQueryable<vw_NWC_WorkOrderList> workOrderList = null;
            if (searchCriteria.excelFlage == true)
            {
                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime);
            }
            else
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
                {
                    skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                    take = searchCriteria.FilterModel.PageFilter.PageSize;
                }
            #endregion

                workOrderList = this._workOrderListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.LastStatusID).ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime)
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<WorkOrderDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._workOrderListRepository.GetQuery().Count(predicate);
                var list = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).ToList();

                result.Result = list;
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
            #endregion
        }


        public DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetails(long orderId)
        {
            try
            {
                var orderDetails = this._orderBasicDetailsRepository.GetQuery().Where(order => order.OrderId == orderId).FirstOrDefault();

                if (orderDetails != null)
                {
                    var orderDetailsDTO = orderDetails.WrapToWorkOrderDTO();

                    orderDetailsDTO.WorkOrderStatusLogs = GetWorkOrderStatusLogs(orderId).Value.ToList();

                    return DescriptiveResponse<WorkOrderDTO>.Success(orderDetailsDTO);
                }

                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.NOT_FOUNT);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetOrderBasicDetails: "));
                return DescriptiveResponse<WorkOrderDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<WorkOrderDTO> GetDriverWorkOrderDetails(long orderId)
        {
            try
            {
                var orderDetails = this._orderBasicDetailsRepository.GetQuery().Where(order => order.OrderId == orderId).FirstOrDefault();

                if (orderDetails != null)
                {
                    var orderDetailsDTO = orderDetails.WrapToWorkOrderDTO();

                    orderDetailsDTO.WorkOrderStatusLogs = GetWorkOrderStatusLogs(orderId).Value.ToList();

                    var ZONE = this._zoneRepository.FindById(orderDetailsDTO.ZoneID);
                    orderDetailsDTO.CityID = ZONE.Branch.Id;
                    if (ZONE != null)
                    {
                        var branchSetting = this._BranchRepository.GetQuery().Where(x => x.Id == ZONE.CityID).FirstOrDefault();

                        orderDetailsDTO.ValidateConfermationCode = (branchSetting != null &&
                                                                    branchSetting.NWC_BranchSetting != null &&
                                                                    branchSetting.NWC_BranchSetting.ValidateConfermationCode.HasValue) ?
                                                                    branchSetting.NWC_BranchSetting.ValidateConfermationCode.Value : false;
                    }

                    return DescriptiveResponse<WorkOrderDTO>.Success(orderDetailsDTO);
                }

                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.NOT_FOUNT);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetDriverWorkOrderDetails: "));
                return DescriptiveResponse<WorkOrderDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>> GetDriverWorkOrdersPerTime(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

                predicate = predicate.And(s => s.IsDeleted != true);
                predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);
                predicate = predicate.And(s => this._loggedInUser.LoggedInUser.StaffId == s.AssignedDriverID);

                if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
                {
                    var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                    predicate = predicate.And(s => s.OrderNumber.Contains(word));
                }

                if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
                }
                if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.AssignedVehicleID));
                }
                #endregion

                #region Date Time Predicate
                if (searchCriteria.DateTimeFrom != null)
                {
                    switch (searchCriteria.DatePeriod)
                    {
                        case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                            predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                            predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                            predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                            break;
                    }
                }
                if (searchCriteria.DateTimeTo != null)
                {
                    switch (searchCriteria.DatePeriod)
                    {
                        case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                            predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                            predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                            break;
                        case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                            predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                            break;
                    }
                }
                #endregion

                if (searchCriteria.SearchType != null)
                {
                    if (searchCriteria.SearchType == 1)
                    {
                        var DriverOrders = this._workOrderListRepository.GetQuery()
                       .Where(predicate)
                           .OrderBy(x => x.CreateTime).ToList();
                        var DriverStatistic = DriverOrders.GroupBy(s => s.CreateTime.Value.Day.ToString() + "-" + s.CreateTime.Value.Month.ToString() + "-" + s.CreateTime.Value.Year.ToString())

                               .Select(g => new DriverWorkOrdersPerTime
                               {
                                   PeriodName = g.Key,
                                   Count = g.Count()
                               }).AsEnumerable();
                        return DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>>.Success(DriverStatistic);
                    }
                    else
                    {
                        var DriverOrders = this._workOrderListRepository.GetQuery()
                       .Where(predicate)
                       .OrderBy(x => x.CreateTime).ToList();

                        var DriverStatistic = DriverOrders.GroupBy(i => GetWeekString((DateTime)i.CreateTime))
                        .Select(g => new DriverWorkOrdersPerTime
                        {
                            PeriodName = g.Key.ToString(),
                            Count = g.Count()
                        }).AsEnumerable();
                        return DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>>.Success(DriverStatistic);
                    }
                }


                return DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>>.Error("This service needs a serachType");

            }

            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetDriverWorkOrdersPerTime: "));
                return DescriptiveResponse<IEnumerable<DriverWorkOrdersPerTime>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetailsByOrderNumber(string orderNumber)
        {
            try
            {
                var orderDetails = this._orderBasicDetailsRepository.GetQuery().Where(order => order.OrderNumber == orderNumber).FirstOrDefault();

                if (orderDetails != null)
                {
                    var orderDetailsDTO = orderDetails.WrapToWorkOrderDTO();

                    orderDetailsDTO.WorkOrderStatusLogs = GetWorkOrderStatusLogs(orderDetailsDTO.WorkOrderID).Value.ToList();

                    return DescriptiveResponse<WorkOrderDTO>.Success(orderDetailsDTO);
                }

                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.NOT_FOUNT);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetOrderBasicDetailsByOrderNumber: "));
                return DescriptiveResponse<WorkOrderDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }



        public DescriptiveResponse<WorkOrderDTO> GetDriverWODetailsByNumber(string orderNumber)
        {
            try
            {
                var orderDetails = this._orderBasicDetailsRepository.GetQuery().Where(order => order.OrderNumber == orderNumber &&
                                    order.AssignedDriverID == this._loggedInUser.LoggedInUser.StaffId).FirstOrDefault();

                if (orderDetails != null)
                {
                    var orderDetailsDTO = orderDetails.WrapToWorkOrderDTO();

                    orderDetailsDTO.WorkOrderStatusLogs = GetWorkOrderStatusLogs(orderDetailsDTO.WorkOrderID).Value.ToList();


                    var city = this._zoneRepository.FindById(orderDetailsDTO.ZoneID);

                    if (city != null)
                    {
                        var branchSetting = this._BranchRepository.GetQuery().Where(x => x.Id == city.CityID).FirstOrDefault();

                        orderDetailsDTO.ValidateConfermationCode = (branchSetting != null &&
                                                                branchSetting.NWC_BranchSetting != null &&
                                                                branchSetting.NWC_BranchSetting.ValidateConfermationCode.HasValue) ?
                                                                branchSetting.NWC_BranchSetting.ValidateConfermationCode.Value : false;
                    }

                    return DescriptiveResponse<WorkOrderDTO>.Success(orderDetailsDTO);
                }

                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.NOT_FOUNT);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetDriverWODetailsByNumber: "));
                return DescriptiveResponse<WorkOrderDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<WorkOrderCommentDTO>> GetWorkOrderComments(long workOrderID)
        {
            var commentsList = this._workOrderComment.GetQuery()
                .Where(s => s.WorkOrderID == workOrderID
                            && s.IsDeleted != true
                            && s.NWC_StateWorkOrder.IsDeleted != true)
                            .OrderByDescending(v => v.CreatedTime).ToList();

            if (commentsList.Any())
            {
                return DescriptiveResponse<IEnumerable<WorkOrderCommentDTO>>
                        .Success(commentsList.Select(o => o.WrapToCommentDTO()).ToList());
            }
            return DescriptiveResponse<IEnumerable<WorkOrderCommentDTO>>.Success(null);
        }

        public DescriptiveResponse<IEnumerable<WorkOrderComplaintDTO>> GetWorkOrderComplaints(long OrderId)
        {
            var workOrderComplaints = new List<WorkOrderComplaintDTO>();
            try
            {
                var _workOrderComplaints = this._workOrderComplaint.GetQuery();
                workOrderComplaints = _workOrderComplaints.Where(o => o.WorkOrderID == OrderId && o.IsDeleted == false).ToList().Select(x => x.WrapToComplaintDTO()).ToList();

                return DescriptiveResponse<IEnumerable<WorkOrderComplaintDTO>>.Success(workOrderComplaints);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetWorkOrderComplaints: "));
                return DescriptiveResponse<IEnumerable<WorkOrderComplaintDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<WorkOrderStatusLogDTO>> GetWorkOrderStatusLogs(long workOrderId)
        {
            var workOrderStatusLogDTOs = new List<WorkOrderStatusLogDTO>();
            try
            {
                var actionLogTypeList = new List<int>();
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_Create);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_Assign);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_Arrived);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_Delivered);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_FailedToDeliver);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_OutForDelivery);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_OnHold);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_Cancelled);
                actionLogTypeList.Add((int)ActionLogTypeEnum.WorkOrder_NotAssigned);


                //Get Event WorkOrder changes
                workOrderStatusLogDTOs = this._vw_NWC_WorkOrderLogs.GetQuery().
                    Where(x => x.WorkOrderId == workOrderId && actionLogTypeList.Contains(x.ActionLogTypeID)).
                    ToList().Select(x => x.WrapToWorkOrderStatusLogDTO()).OrderBy(x => x.ChangedTime).ToList();

                return DescriptiveResponse<IEnumerable<WorkOrderStatusLogDTO>>.Success(workOrderStatusLogDTOs);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetWorkOrderStatusLogs: "));
                return DescriptiveResponse<IEnumerable<WorkOrderStatusLogDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<WorkOrderChangeLogDTO>> GetWorkOrderChangeLogs(long workOrderId)
        {
            List<WorkOrderChangeLogDTO> result;
            try
            {
                var myQuery = this._vw_NWC_WorkOrderLogs.GetQuery()
                       .Where(s => s.WorkOrderId == workOrderId)
                       .OrderBy(s => s.CreateTime);

                result = myQuery.AsEnumerable().Select(a => a.WrapToWorkOrderChangeLogDTO()).ToList();

                int[] eventTypeStatusList = { 1, 11, 10, 3, 5, 6, 7, 9, 8, 4 };


                //var myQuery = this._changesLogs.GetQuery()
                //        .Where(s => s.EventOrderID == workOrderId || s.ParentWorkOrderID == workOrderId);

                //result = myQuery.AsEnumerable().Select(a => a.WrapToWorkOrderChangeLogDTO()).ToList();

                //int[] eventTypeStatusList = {
                //        (int)EventTypeEnum.WorkOrder_Create,
                //        (int)EventTypeEnum.WorkOrder_NotAssigned,
                //        (int)EventTypeEnum.WorkOrder_OnHold,
                //        (int)EventTypeEnum.WO_Vehicle_Assign,
                //        (int)EventTypeEnum.WorkOrder_OutForDelivery,
                //        (int)EventTypeEnum.WorkOrder_Arrived,
                //        (int)EventTypeEnum.WorkOrder_Delivered,
                //        (int)EventTypeEnum.WorkOrder_FailedToDeliver,
                //        (int)EventTypeEnum.WorkOrder_Cancelled,
                //        (int)EventTypeEnum.WO_Vehicle_Deassign
                //    };

                int length = result.Count();
                for (int i = 0; i < length; i++)
                {

                    #region Previous Status Data
                    if (eventTypeStatusList.Contains(result[i].ActionLogTypeID))
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (eventTypeStatusList.Contains(result[j].ActionLogTypeID))
                            {
                                result[i].PreviousStatusName = result[j].StatusName;
                                result[i].PreviousStatusTime = result[j].StatusTime;
                                result[i].PreviousStatusComment = result[j].StatusComment;
                                result[i].PreviousVehicleCodePlateNo = result[j].VehicleCodePlateNo;
                                result[i].PreviousVehicleStatusName = result[j].VehicleStatusName;
                                result[i].PreviousDriverName = result[j].DriverName;
                                result[i].PreviousDeassignReasonName = result[j].DeassignReasonName;
                                result[i].PreviousOrderQuantity = result[j].OrderQuantity;
                                result[i].PreviousScheduledDeliveryTime = result[j].ScheduledDeliveryTime;
                                break;
                            }
                        }
                    }

                    #endregion

                    //    #region Load Location Data for create and update only
                    //    if (result[i].EventTypeID == (int)EventTypeEnum.WorkOrder_Create
                    //        || result[i].EventTypeID == (int)EventTypeEnum.WorkOrder_Update)
                    //    {
                    //        long filterId = result[i].EventOrderId;
                    //        var locationLog = this._changesLogsLocationData.GetQuery().FirstOrDefault(s => s.EventOrderID == filterId);

                    //        result[i].ServiceTypeName = LanguageIsEnglish ? locationLog.ServiceTypeEn : locationLog.ServiceTypeAr;
                    //        result[i].StationName = locationLog.StationName;
                    //        result[i].CustomerAddress = locationLog.CustomerAddress;
                    //        result[i].ZoneName = LanguageIsEnglish ? locationLog.ZoneNameEn : locationLog.ZoneNameAr;
                    //        result[i].CityName = locationLog.cityName;
                    //    }
                    //    #endregion

                    #region Previous update Data
                    if (result[i].ActionLogTypeID == 2)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (result[j].ActionLogTypeID == (int)EventTypeEnum.WorkOrder_Create
                                || result[j].ActionLogTypeID == (int)EventTypeEnum.WorkOrder_Update)
                            {
                                result[i].PreviousAccessories = result[j].Accessories;
                                result[i].PreviousDistance = result[j].Distance;

                                result[i].PreviousServiceTypeName = result[j].ServiceTypeName;
                                result[i].PreviousStationName = result[j].StationName;
                                result[i].PreviousCustomerAddress = result[j].CustomerAddress;
                                result[i].PreviousZoneName = result[j].ZoneName;
                                result[i].PreviousCityName = result[j].CityName;
                                result[i].PreviousOrderQuantity = result[j].OrderQuantity;
                                result[i].PreviousScheduledDeliveryTime = result[j].ScheduledDeliveryTime;
                                result[i].PreviousTotalCost = result[j].TotalCost;

                                break;
                            }
                        }
                    }
                    #endregion

                }

                return DescriptiveResponse<IEnumerable<WorkOrderChangeLogDTO>>.Success(result);

                //return DescriptiveResponse<IEnumerable<WorkOrderChangeLogDTO>>.Success(new List<WorkOrderChangeLogDTO>());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetWorkOrderChangeLogs: "));
                return DescriptiveResponse<IEnumerable<WorkOrderChangeLogDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<AccessoryDTO>> GetWorkOrderAccessory(long workOrderId)
        {
            var workOrderAccessory = new List<AccessoryDTO>();
            try
            {
                workOrderAccessory = this._workOrderAccessory.GetQuery().
                    Where(x => x.WorkOrderID == workOrderId).
                    ToList().Select(x => x.WrapToAccessoryDTO()).ToList();

                return DescriptiveResponse<IEnumerable<AccessoryDTO>>.Success(workOrderAccessory);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetWorkOrderAccessory: "));
                return DescriptiveResponse<IEnumerable<AccessoryDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<WorkOrderTransactionDTO>> GetWorkOrderPayments(long workOrderId)
        {
            var workOrderAccessory = new List<WorkOrderTransactionDTO>();
            try
            {
                workOrderAccessory = this._workOrderTransaction.GetQuery().
                    Where(x => x.ID == workOrderId && x.IsDeleted != true).
                    ToList().Select(x => x.WrapToWorkOrderTransactionDTO()).ToList();

                return DescriptiveResponse<IEnumerable<WorkOrderTransactionDTO>>.Success(workOrderAccessory);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetWorkOrderPayments: "));
                return DescriptiveResponse<IEnumerable<WorkOrderTransactionDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetAssignableWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            try
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.FilterModel.PageFilter != null)
                {
                    skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                    take = searchCriteria.FilterModel.PageFilter.PageSize;
                }
                #endregion

                var assignableWorkOrders = this._sp_NWC_GetAssignableWorkOrders.ExecWithStoredProcedure("sp_NWC_GetAssignableWorkOrders @SubID, @StatusId, @VehicleID, @OrderBy, @take, @skip",
                                            new SqlParameter("SubID", SqlDbType.UniqueIdentifier) { Value = this._loggedInUser.LoggedInUser.SubscriberId },
                                            new SqlParameter("StatusId", SqlDbType.Int) { Value = (int)WorkOrderStatusEnum.New },
                                            new SqlParameter("VehicleID", SqlDbType.UniqueIdentifier) { Value = searchCriteria.VehicleID },
                                            new SqlParameter("OrderBy", SqlDbType.NVarChar) { Value = "CreatedBy" },
                                            new SqlParameter("take", SqlDbType.Int) { Value = take },
                                            new SqlParameter("skip", SqlDbType.Int) { Value = skip });

                #region response
                var result = new SearchResult<WorkOrderDTO>();
                if (assignableWorkOrders != null && assignableWorkOrders.Any())
                {
                    var count = this._sp_NWC_GetAssignableWorkOrders.ExecWithStoredProcedure("sp_NWC_GetAssignableWorkOrders @SubID, @StatusId, @VehicleID, @OrderBy, @take, @skip",
                                            new SqlParameter("SubID", SqlDbType.UniqueIdentifier) { Value = this._loggedInUser.LoggedInUser.SubscriberId },
                                            new SqlParameter("StatusId", SqlDbType.Int) { Value = (int)WorkOrderStatusEnum.New },
                                            new SqlParameter("VehicleID", SqlDbType.UniqueIdentifier) { Value = searchCriteria.VehicleID },
                                            new SqlParameter("OrderBy", SqlDbType.NVarChar) { Value = "CreatedBy" },
                                            new SqlParameter("take", SqlDbType.Int) { Value = 0 },
                                            new SqlParameter("skip", SqlDbType.Int) { Value = skip }).Count();

                    result.Result = assignableWorkOrders.Select(x => x.WrapToOrderBasicDetailsDTO()).ToList();
                    result.TotalCount = count;
                }
                return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetAssignableWorkOrders: "));
                return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<DailyOrderSummaryDTO>> GetDailyOrderSummaryReport(DailyOrderReportSC searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_Report_DailyOrderSummary>(true);

            predicate = predicate.And(s => s.StationSubId == this._loggedInUser.LoggedInUser.SubscriberId);

            #region Lists Predicate

            //permitted serviceTypes
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            }

            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.StationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.StationID));
            }

            #endregion

            #region Date Time Predicate

            if (searchCriteria.DateFrom != null)
            {
                var from = searchCriteria.DateFrom.Date; //from the start of the day
                predicate = predicate.And(s => s.CreateDate >= from);
            }

            if (searchCriteria.DateTo != null)
            {
                var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day
                predicate = predicate.And(s => s.CreateDate < to);
            }
            #endregion

            #endregion

            IQueryable<vw_NWC_Report_DailyOrderSummary> workOrderList =
                this._DailyOrderSummaryRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.StationName).ThenBy(s => s.CreateDate);

            if (!searchCriteria.ExcelFlage)
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                workOrderList = workOrderList
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<DailyOrderSummaryDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._DailyOrderSummaryRepository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToDailyOrderSummaryDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<DailyOrderSummaryDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDailyOrderDetailsReport(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_Report_DailyOrderDetails>(true);

            predicate = predicate.And(s => s.IsDeleted != true);
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.OrderNumber.Contains(word));
            }

            #region Lists Predicate
            if (searchCriteria.CustomerIDs != null && searchCriteria.CustomerIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CustomerIDs.Any(a => a == s.CustomerID));
            }
            if (searchCriteria.ClassIDs != null && searchCriteria.ClassIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ClassIDs.Any(a => a == s.ClassID));
            }
            if (searchCriteria.PriorityIDs != null && searchCriteria.PriorityIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.PriorityIDs.Any(a => a == s.PriorityID));
            }
            //permitted serviceTypes
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            }

            //permitted cities
            if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            {
                var searchList = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.CityID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.SubBranches.Any(a => a == s.CityID));
            }

            if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ZoneIDs.Any(a => a == s.ZoneID));
            }
            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));
            }

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
            }
            if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.VehicleIDs.Any(a => a == s.VehicleID));
            }
            if (searchCriteria.DriverIDs != null && searchCriteria.DriverIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.DriverIDs.Any(a => a == s.DriverID));
            }
            #endregion

            if (!string.IsNullOrEmpty(searchCriteria.CustomerIdNumber))
            {
                var text = searchCriteria.CustomerIdNumber.Trim();
                predicate = predicate.And(s => s.CustomerIdNumber.Contains(text));
            }

            if (!string.IsNullOrEmpty(searchCriteria.CustomerMobile))
            {
                var text = searchCriteria.CustomerMobile.Trim();
                predicate = predicate.And(s => s.CustomerMobile.Contains(text));
            }

            #region Date Time Predicate
            if (searchCriteria.DateTimeFrom != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                        break;
                }
            }
            if (searchCriteria.DateTimeTo != null)
            {
                switch (searchCriteria.DatePeriod)
                {
                    case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                        predicate = predicate.And(s => s.CreateTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                        predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                        break;
                    case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                        predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                        break;
                }
            }
            #endregion

            #endregion

            IQueryable<vw_NWC_Report_DailyOrderDetails> workOrderList =
                this._DailyOrderDetaileRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.StationName).ThenBy(s => s.CreateTime);

            if (!searchCriteria.excelFlage)
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.FilterModel.PageFilter != null)
                {
                    skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                    take = searchCriteria.FilterModel.PageFilter.PageSize;
                }
                #endregion

                workOrderList = workOrderList
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<WorkOrderDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._DailyOrderDetaileRepository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToOrderBasicDetailsDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SearchResult<DeferredOrderDTO>> SearchDeferredWorkOrders(DeferredOrderSC searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<NWC_DeferredWorkOrder>(true);

            if (searchCriteria.Id != null && searchCriteria.Id > 0)
            {
                predicate = predicate.And(s => s.ID == searchCriteria.Id);
            }

            if (!string.IsNullOrEmpty(searchCriteria.OrderNo))
            {
                var searchText = searchCriteria.OrderNo.Trim();
                predicate = predicate.And(s => s.ORDERNUMBER.Contains(searchText));
            }

            if (searchCriteria.DateTimeFrom.HasValue)
            {
                var from = searchCriteria.DateTimeFrom.Value.Date; //from the start of the day
                predicate = predicate.And(s => s.CreateTime >= from);
            }

            if (searchCriteria.DateTimeTo.HasValue)
            {
                var to = searchCriteria.DateTimeTo.Value.Date.AddDays(1); //to the start of the next day
                predicate = predicate.And(s => s.CreateTime < to);
            }

            if (searchCriteria.StatusIds != null && searchCriteria.StatusIds.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIds.Any(a => a == s.StatusId));
            }

            if (!string.IsNullOrEmpty(searchCriteria.MobileNo))
            {
                var searchText = searchCriteria.MobileNo.Trim();
                predicate = predicate.And(s => s.MOBILENUMBER.Contains(searchText));
            }

            if (!string.IsNullOrEmpty(searchCriteria.PersonIdValue))
            {
                var searchText = searchCriteria.PersonIdValue.Trim();
                predicate = predicate.And(s => s.PERSONIDVALUE.Contains(searchText));
            }

            #endregion

            IQueryable<NWC_DeferredWorkOrder> DeferredWorkOrderList =
             this._DeferredWorkOrderRepository.GetQuery()
                 .Where(predicate)
                 .OrderByDescending(s => s.CreateTime);

            if (!searchCriteria.ExcelFlage)
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                DeferredWorkOrderList = DeferredWorkOrderList
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<DeferredOrderDTO>();
            if (DeferredWorkOrderList != null && DeferredWorkOrderList.Any())
            {
                var count = this._DeferredWorkOrderRepository.GetQuery().Count(predicate);
                result.Result = DeferredWorkOrderList.AsEnumerable().Select(a => a.WrapToOrderDeferredWorkOrderDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<DeferredOrderDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<int> GetNoOfOrdersForThisMonth(long customerAccount)
        {
            var now = DateTime.Today;
            var firstDayMonth = new DateTime(now.Year, now.Month, 1);

            var count = this._stateWorkOrderRepository.GetQuery()
                .Where(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.IsDeleted != true
                            && s.CustomerAccountId == customerAccount
                            && s.ScheduledDeliveryTime >= firstDayMonth)
                .Count();

            return DescriptiveResponse<int>.Success(count);
        }
        #endregion

        #region Hayat
        public DescriptiveResponse<SearchResult<HayatWorkOrderLogDTO>> GetHayatWorkOrderLogs(HayatWorkOrderLogsSC searchCriteria, int retrials, int holdInterval)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<NWC_Hayat_OrderStatusLog>(true);

            var retryTime = DateTimeHelper.GetDateTimeNow().AddMinutes(-holdInterval);

            //predicate = predicate.And(s => s.Retrials == 0 || (s.Retrials <= retrials) && (s.RetrialTime <= retryTime));

            predicate = predicate.And(s => searchCriteria.StatusIDs.Contains(s.StatusID));
            #endregion

            IQueryable<NWC_Hayat_OrderStatusLog> workOrderList = null;

            workOrderList = this._hayatOrderStatusLogRepository.GetQuery()
                .Where(predicate)
                .OrderBy(s => s.ID)
                .Take(searchCriteria.Take);

            #region response
            var result = new SearchResult<HayatWorkOrderLogDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._hayatOrderStatusLogRepository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(x => new HayatWorkOrderLogDTO()
                {
                    ID = x.ID,
                    OrderNumber = x.OrderNumber,
                    CreateTime = x.CreateTime,
                    CreatedBy = x.CreatedBy,
                    CurrentStatus = x.CurrentStatusID,
                    NewStatus = x.NewStatusID,
                    HayatRequest = x.HayatRequest,
                    HayatResponse = x.HayatResponse,
                    Retrials = x.Retrials,
                    RetrialTime = x.RetrialTime,
                    StatusID = x.StatusID
                }).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<HayatWorkOrderLogDTO>>.Success(result);
            #endregion
        }


        public DescriptiveResponse<Double> GetEstimatedDeliveryTimeByMinute(int ZoneID, Guid StationId, int range)
        {
            try
            {
                var Orders = this._workOrderListRepository.GetQuery().Where(x=> x.LastStatusID==(int)WorkOrderStatusEnum.Delivered && x.ZoneID== ZoneID && x.AssignedStationID== StationId && x.CreateTime.Value.Hour == DateTime.Now.Hour && DbFunctions.DiffDays(x.CreateTime,DateTime.Now) <= range)
                    .Select(x=> DbFunctions.DiffMinutes(x.CreateTime,x.LastStatusTime)).Average();
                return DescriptiveResponse<Double>.Success(Orders == null ? 0 : (Double)Orders);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetEstimatedDeliveryTimeByMinute: "));
                return DescriptiveResponse<Double>.Error(ex.Message);
            }
        }
        public DescriptiveResponse<Boolean> UpdateHayatWorkOrderLog(HayatWorkOrderLogDTO dto)
        {
            try
            {
                var workOrderLog = this._hayatOrderStatusLogRepository.FindById(dto.ID);

                if (dto.HayatResponse != null)
                    workOrderLog.HayatResponse = dto.HayatResponse;

                workOrderLog.StatusID = dto.StatusID;
                workOrderLog.Retrials = dto.Retrials;
                workOrderLog.RetrialTime = DateTimeHelper.GetDateTimeNow();

                using (_unitofWork)
                {
                    this._hayatOrderStatusLogRepository.Update(workOrderLog);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => UpdateHayatWorkOrderLog: "));
                return DescriptiveResponse<Boolean>.Error(ex.Message);
            }
        }
        #endregion

        #region Blacklist Service
        public DescriptiveResponse<Boolean> IsCustomerBlacklisted(long accountID)
        {
            try
            {
                if (CallAccountBlacklistService_config)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg($"IsCustomerBlacklisted accountId: {accountID}"));

                    var custAccountCity = this._vw_NWC_CustomerAccountCityRep.GetQuery().Where(x => x.ID == accountID).FirstOrDefault();

                    LoggerManager.LogMsg(c => c.TrackingMsg($"IsCustomerBlacklisted CISDivision: {custAccountCity.CISDivision}, IdNumber: {custAccountCity.IDNumber}"));

                    var cis = GetCISDivison(custAccountCity.CISDivision);

                    LoggerManager.LogMsg(c => c.TrackingMsg($"IsCustomerBlacklisted cis: {cis}"));

                    var blacklistServiceClient = new BlacklistServiceRef.CheckPersonInBlackListPortTypeClient();

                    var request = new BlacklistServiceRef.CheckPersonInBlackListExecuteRequest()
                    {
                        accountId = custAccountCity.AccountId_Integration,
                        nidNumber = custAccountCity.IDNumber,
                        sourceApplication = BlacklistServiceRef.CheckPersonInBlackListRequestSourceApplication.TMS,
                        cisDivison = cis
                    };

                    LoggerManager.LogMsg(c => c.TrackingMsg($"IsCustomerBlacklisted Request accountId: {request.accountId}, nidNumber: {request.nidNumber}, sourceApplication: {request.sourceApplication}, cisDivison: {request.cisDivison}"));

                    var result = Task.Run(async () => await blacklistServiceClient.CheckPersonInBlackListExecuteAsync(request)).ConfigureAwait(false);

                    var response = result.GetAwaiter().GetResult();

                    LoggerManager.LogMsg(c => c.TrackingMsg($"IsCustomerBlacklisted response.errorCode: {response.errorCode}, response.errorDescription: {response.errorDescription}"));

                    return new DescriptiveResponse<bool>()
                    {
                        Value = response.responseCode == "0",
                        //Value = !string.IsNullOrEmpty(response.errorCode) && (response.errorCode.ToUpper() == "TRBRB" || response.errorCode.ToUpper() == "COM" || response.errorCode.ToUpper() == "CUSR"),
                        ErrorDescription = response.errorDescription
                    };
                }
                else
                {
                    return new DescriptiveResponse<bool>()
                    {
                        Value = false
                    };
                }

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetAssignableWorkOrders: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        private static BlacklistServiceRef.cisDivison GetCISDivison(string cis)
        {
            var cisDiv = BlacklistServiceRef.cisDivison.AS;

            if (string.IsNullOrEmpty(cis))
                return cisDiv;

            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.BA.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.BA;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.BA.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.HA;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.HS.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.HS;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.JC.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.JC;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.JCBU.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.JCBU;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.JF.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.JF;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.JZ.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.JZ;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.JZBU.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.JZBU;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.MC.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.MC;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.MCBU.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.MCBU;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.MD.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.MD;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.MK.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.MK;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.NA.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.NA;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.NJ.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.NJ;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.QS.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.QS;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.RC.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.RC;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.RCBU.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.RCBU;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.RI.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.RI;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.SH.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.SH;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.TB.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.TB;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.TC.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.TC;
            }
            if (cis.Trim().ToUpper() == BlacklistServiceRef.cisDivison.TCBU.ToString())
            {
                cisDiv = BlacklistServiceRef.cisDivison.TCBU;
            }

            return cisDiv;
        }
        #endregion

        #region Helpers
        private bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }

        private string GetConfirmationCode(int length)
        {
            Random r = new Random();
            string digits7 = Math.Round(r.NextDouble() * Math.Pow(10, length), 0)
                .ToString(CultureInfo.InvariantCulture).PadLeft(length, '0');

            return digits7;
        }

        public string GenerateWorkOrderNumber()
        {
            Random r = new Random();
            string digits10 = Math.Round(r.NextDouble() * 1e+10, 0).ToString(CultureInfo.InvariantCulture).PadLeft(10, '0');

            return digits10;
        }

        private bool IsValidWorkOrderStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var workOrderStatus = this._workOrderStatusRep.FindById(currentStatusID);

            return workOrderStatus.NextStatusIDs != null && workOrderStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }

        private bool IsValidVehicleStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var vehicleStatus = this._transporterStatusRep.FindById(currentStatusID);

            return vehicleStatus.NextStatusIDs != null && vehicleStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }

        EventWorkOrderVehicleDTO SetVehicleDTO(Guid driverID)
        {
            return new EventWorkOrderVehicleDTO { DriverID = driverID, VehicleID = GetVehicleIDByDriverID(driverID), StatusID = 5 };
        }

        private Guid GetVehicleIDByDriverID(Guid driverID)
        {
            var driver = _transporterStaffRepository.GetQuery().Where(x => x.Staff == driverID).Select(x => x.Transporter).FirstOrDefault();

            if (driver != null)
                return (Guid)driver;
            else
                return Guid.Empty;

        }


        #endregion

        #region Sewer Section


        public DescriptiveResponse<SearchResult<WorkOrderDTO>> SearchSewerWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            searchCriteria.ServiceTypeIDs = new List<int> { (int)ServiceTypeEnum.SewageRemoval };
            return SearchWorkOrders(searchCriteria);
        }

        public DescriptiveResponse<WorkOrderDTO> GetSewerOrderBasicDetails(long orderId)
        {
            try
            {
                var orderDetails = this._orderBasicDetailsRepository.GetQuery().Where(order => order.ServiceTypeId == (int)ServiceTypeEnum.SewageRemoval && order.OrderId == orderId).FirstOrDefault();

                if (orderDetails != null)
                {
                    var orderDetailsDTO = orderDetails.WrapToWorkOrderDTO();
                    var city = _BranchRepository.GetQuery().FirstOrDefault(b => (!b.isDeleted.HasValue || !b.isDeleted.Value) && b.name == orderDetails.CityName);
                    orderDetailsDTO.CityID = city.Id;
               

                    return DescriptiveResponse<WorkOrderDTO>.Success(orderDetailsDTO);
                }

                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.NOT_FOUNT);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => GetOrderBasicDetails: "));
                return DescriptiveResponse<WorkOrderDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetDriverSewerWorkOrders(WorkOrderSearchCriteriaDTO searchCriteria)
        {
            searchCriteria.ServiceTypeIDs = new List<int> { (int)ServiceTypeEnum.SewageRemoval };
            return GetDriverWorkOrders(searchCriteria);
        }
        #endregion

        public static string GetWeekString(DateTime CreateTime)
        {
            return string.Format("Between {0} and {1}", GetFirstDayOfWeek(CreateTime).ToShortDateString(), GetLastDayOfWeek(CreateTime).ToShortDateString());
        }

        static DateTime GetFirstDayOfWeek(DateTime date)
        {
            var firstDayOfWeek = date.AddDays(-((date.DayOfWeek - DayOfWeek.Sunday + 7) % 7));
            if (firstDayOfWeek.Year != date.Year)
                firstDayOfWeek = new DateTime(date.Year, 1, 1);
            return firstDayOfWeek;
        }

        static DateTime GetLastDayOfWeek(DateTime date)
        {
            var lastDayOfWeek = date.AddDays((DayOfWeek.Saturday - date.DayOfWeek + 7) % 7);
            if (lastDayOfWeek.Year != date.Year)
                lastDayOfWeek = new DateTime(date.Year, 12, 31);
            return lastDayOfWeek;
        }
        public DescriptiveResponse<Boolean> IsZoneWithoutTankersByZoneInt(string zoneIntegrationID)
        {
            try
            {

                var zoneWithoutTankers = this._zoneRepository.GetQuery().Where(x => x.IntegrationId == zoneIntegrationID && x.IsDeleted != true).Select(x => x.ZoneWithoutTanker).FirstOrDefault();

                return (zoneWithoutTankers.HasValue && zoneWithoutTankers.Value) ? DescriptiveResponse<Boolean>.Success(true) : DescriptiveResponse<Boolean>.Success(false);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => IsZoneWithoutTankers: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<AvailableTankerSizesDTO>> CalculateWorkOrderCost(CostDTO costDTO)
        {
            try
            {
                var list = new List<AvailableTankerSizesDTO>();
                foreach (var Quantity in costDTO.orderQuantities)
                {
                    var NWC_ZoneStations = new NWC_ZoneStations { StationID = costDTO.StationID, ZoneID = costDTO.ZoneID };
                    var availableTankerSizesDTO = new AvailableTankerSizesDTO { TankerSize = Quantity, TankerPrice = WorkOrderCost.CalculateWorkOrderCost(0, DateTime.Now, 1, 1, NWC_ZoneStations, Quantity) };
                    list.Add(availableTankerSizesDTO);

                }
                return DescriptiveResponse<List<AvailableTankerSizesDTO>>.Success(list);
            }

            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderService => CalculateWorkOrderCost: "));
                return DescriptiveResponse<List<AvailableTankerSizesDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
    }
}