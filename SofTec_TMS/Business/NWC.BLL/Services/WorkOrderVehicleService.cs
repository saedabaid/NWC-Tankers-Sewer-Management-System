using Infrastructure;
using LinqKit;
using NWC.BL.Denormalizer.CoreBusiness;
using NWC.BLL.Interfaces;
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
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NWC.BLL.Services
{
    public class WorkOrderVehicleService : IWorkOrderVehicleService
    {
        #region Properties
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<vw_NWC_WorkOrderVehicle> _WorkOrderVehicleListRepository;
        private readonly IRepository<vw_NWC_StateVehicle> _StateVehicleViewRepository;
        private readonly IRepository<vw_NWC_WorkOrderBasicDetails> _orderBasicDetailsRepository;
        private readonly IRepository<vw_NWC_AssignableVehicles> _AssignableVehiclesRepository;
        private readonly IRepository<NWC_WorkOrderAccessory> _WorkOrderAccessoryRepository;
        private readonly IRepository<Transporter> _VehicleRepository;
        private readonly IRepository<NWC_StateWorkOrder> _stateWorkOrderRepository;
        private readonly IRepository<vw_NWC_PrintCustomerInvoice> _PrintCustomerInvoice;
        private readonly IRepository<vw_NWC_PrintDriverInvoice> _PrintDriverInvoice;
        private readonly IRepository<sp_NWC_GetAssignableVehicles_Result> _sp_NWC_GetAssignableVehiclesRepository;
        private readonly IRepository<Transporter> _transporter;
        private readonly IRepository<NWC_VehicleLog> _vehicleLog;
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IRepository<vw_NWC_PrintVehicleInvoice> _PrintVehicleInvoice;
        private readonly IRepository<NWC_VehicleCustomerLocationClass> _VehicleCustomerLocationClass;
        private readonly IRepository<NWC_BranchSetting> _BranchSetting;
        private readonly IRepository<vw_NWC_Report_OrderReassignment> _vwOrderReassignmentRepo;
        private readonly IRepository<sp_NWC_Report_OrderReassignment_Result> _sp_NWC_Report_OrderReassignment;
        private static bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
        #endregion

        #region Constructors
        public WorkOrderVehicleService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._WorkOrderVehicleListRepository = new Repository<vw_NWC_WorkOrderVehicle>(ctx);
            this._StateVehicleViewRepository = new Repository<vw_NWC_StateVehicle>(ctx);
            this._orderBasicDetailsRepository = new Repository<vw_NWC_WorkOrderBasicDetails>(ctx);
            this._AssignableVehiclesRepository = new Repository<vw_NWC_AssignableVehicles>(ctx);
            this._WorkOrderAccessoryRepository = new Repository<NWC_WorkOrderAccessory>(ctx);
            this._VehicleRepository = new Repository<Transporter>(ctx);
            this._stateWorkOrderRepository = new Repository<NWC_StateWorkOrder>(ctx);
            this._sp_NWC_GetAssignableVehiclesRepository = new Repository<sp_NWC_GetAssignableVehicles_Result>(ctx);
            this._PrintCustomerInvoice = new Repository<vw_NWC_PrintCustomerInvoice>(ctx);
            this._PrintDriverInvoice = new Repository<vw_NWC_PrintDriverInvoice>(ctx);
            this._transporter = new Repository<Transporter>(ctx);
            this._vehicleLog = new Repository<NWC_VehicleLog>(ctx);
            this._PrintVehicleInvoice = new Repository<vw_NWC_PrintVehicleInvoice>(ctx);
            this._VehicleCustomerLocationClass = new Repository<NWC_VehicleCustomerLocationClass>(ctx);
            this._BranchSetting = new Repository<NWC_BranchSetting>(ctx);
            this._vwOrderReassignmentRepo = new Repository<vw_NWC_Report_OrderReassignment>(ctx);
            _sp_NWC_Report_OrderReassignment = new Repository<sp_NWC_Report_OrderReassignment_Result>(ctx);
        }
        #endregion

        #region Command
        public DescriptiveResponse<Boolean> ArriveVehicleToStation(Guid vehicleID, List<int> customerClassesIds) //int customerClassId)
        {
            try
            {
                var vehicle = this._transporter.FindById(vehicleID);
                if (vehicle != null && vehicle.status == (int)VehicleStatusEnum.Parking)
                {
                    using (_unitofWork)
                    {
                        vehicle.status = (int)VehicleStatusEnum.Available;
                        vehicle.LastModificationDate = DateTime.Now;

                        #region update vehicle Location Class
                        var canChangeCustomerClass =
                            this._BranchSetting.GetQuery()
                                .Where(s => s.BranchID == vehicle.branch)
                                .Select(s => s.ShowCustomerClassEntryGate)
                                .FirstOrDefault();

                        if (canChangeCustomerClass.GetValueOrDefault())
                        {
                            var vehicleClasses = this._VehicleCustomerLocationClass.GetQuery()
                                                .Where(s => s.VehicleID == vehicleID);

                            //var flagClassExist = false;
                            if (vehicleClasses.Any())
                            {
                                foreach (var item in vehicleClasses)
                                {
                                    //if (item.CustomerLocationClassID == customerClassId)
                                    //{
                                    //    flagClassExist = true;
                                    //}
                                    //else
                                    //{
                                    //    this._VehicleCustomerLocationClass.Delete(item);
                                    //}

                                    if (!customerClassesIds.Any(a => a == item.CustomerLocationClassID))
                                    {
                                        this._VehicleCustomerLocationClass.Delete(item);
                                    }
                                }
                            }

                            //if (!flagClassExist)
                            //{
                            //    var newVehicleClass = new NWC_VehicleCustomerLocationClass
                            //    {
                            //        VehicleID = vehicleID,
                            //        CustomerLocationClassID = customerClassId
                            //    };
                            //    this._VehicleCustomerLocationClass.Add(newVehicleClass);
                            //}

                            if (customerClassesIds.Any())
                            {
                                foreach (var item in customerClassesIds)
                                {

                                    if (!vehicleClasses.Any(a => a.CustomerLocationClassID == item))
                                    {
                                        var newVehicleClass = new NWC_VehicleCustomerLocationClass
                                        {
                                            VehicleID = vehicleID,
                                            CustomerLocationClassID = item
                                        };
                                        this._VehicleCustomerLocationClass.Add(newVehicleClass);

                                        //this._VehicleCustomerLocationClass.Delete(item);
                                    }
                                }
                            }

                        }
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

                        var classes = this._VehicleCustomerLocationClass.GetQuery()
                                                .Where(s => s.VehicleID == vehicleID)
                                                .Select(s => s.NWC_CustomerLocationClass).Distinct().ToList();
                        var classesAr = string.Empty;
                        var classesEn = string.Empty;
                        if (classes.Any())
                        {
                            classesAr = string.Join(", ", classes.Select(s => s.NameAr));
                            classesEn = string.Join(", ", classes.Select(s => s.NameEn));
                        }

                        this._vehicleLog.Add(new NWC_VehicleLog()
                        {
                            VehicleID = vehicle.ID,
                            DriverID = driverId,
                            StatusID = (int)VehicleStatusEnum.Available,
                            CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                            CreateTime = DateTimeHelper.GetDateTimeNow(),
                            ActionLogTypeID = (int)VehicleActionLogTypeEnum.Entry,
                            CustomerClassesAr = classesAr,
                            CustomerClassesEn = classesEn
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

        public DescriptiveResponse<Boolean> OutForParking(Guid vehicleID)
        {
            try
            {
                using (_unitofWork)
                {
                    var vehicle = this._transporter.FindById(vehicleID);

                    if (vehicle != null && vehicle.status == (int)VehicleStatusEnum.Available)
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
                            StatusID = (int)VehicleStatusEnum.Available,
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

        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<StateWorkOrderVehicleDTO>> GetWorkOrderVehicles(WorkOrderVehicleSearchCriteriaDTO searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_WorkOrderVehicle>(x =>
                   x.TransporterIsDeleted != true &&
                   x.IsAssigned == true &&
                   x.SubID == this._loggedInUser.LoggedInUser.SubscriberId &&
                   (this._loggedInUser.UserLandmarksIds.Contains(x.AssignedStationID)
                   || (this._loggedInUser.SubBranches.Contains((Guid)x.CityId) && (x.VehicleCustomerLocationClassesIds.Contains("2") && !x.VehicleCustomerLocationClassesIds.Contains("1")))));

                //TODO : ServiceType - GetWorkOrderVehicles
                if (searchCriteria.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    predicate = predicate.And(v => v.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval);
                }
                else
                {
                    predicate = predicate.And(v => v.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval);
                }

                if (searchCriteria.WorkOrderStatusIDs != null && searchCriteria.WorkOrderStatusIDs.Any())
                {
                    predicate = predicate.And(v => searchCriteria.WorkOrderStatusIDs.Contains(v.LastStatusID));
                }

                if (!String.IsNullOrEmpty(searchCriteria.VechilePlateNumberOrCode))
                {
                    var searchText = searchCriteria.VechilePlateNumberOrCode.Trim();
                    predicate = predicate.And(v => v.TransporterCode.Contains(searchText) || v.TransporterPlateNo.Contains(searchText));
                }

                if (!String.IsNullOrEmpty(searchCriteria.Driver))
                {
                    var searchDriver = searchCriteria.Driver.Trim();
                    var driverArr = searchCriteria.Driver.Trim().Split(' ');
                    if (driverArr.Length == 1)
                    {
                        predicate = predicate.And(s => s.DriverFirstName.Contains(searchDriver)
                                                    || s.DriverMiddleName.Contains(searchDriver)
                                                    || s.DriverLastName.Contains(searchDriver));
                    }
                    else
                    {
                        predicate = predicate.And(s =>
                                (s.DriverFirstName + " " + s.DriverMiddleName + " " + s.DriverLastName).Contains(searchDriver)
                                || (s.DriverFirstName + " " + s.DriverLastName).Contains(searchDriver));
                    }
                }

                if (!String.IsNullOrEmpty(searchCriteria.DriverCode))
                {
                    var searchText = searchCriteria.DriverCode.Trim();
                    predicate = predicate.And(v => v.DriverCode.Contains(searchText));
                }

                if (!string.IsNullOrEmpty(searchCriteria.WorkOrderNumber))
                {
                    var searchText = searchCriteria.WorkOrderNumber.Trim();
                    predicate = predicate.And(v => v.OrderNumber.Contains(searchText));
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

                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                var workOrderVehicleList = this._WorkOrderVehicleListRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.VehicleStatusID)
                    .Skip(skip)
                    .Take(take);

                #region response
                var result = new SearchResult<StateWorkOrderVehicleDTO>();
                if (workOrderVehicleList != null && workOrderVehicleList.Any())
                {
                    var count = this._WorkOrderVehicleListRepository.GetQuery().Count(predicate);
                    result.Result = workOrderVehicleList.AsEnumerable().Select(x => x.WrapToWorkOrderVehicleDTO()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<StateWorkOrderVehicleDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => GetWorkOrderVehicles: "));
                return DescriptiveResponse<SearchResult<StateWorkOrderVehicleDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<StateVehicleDTO>> GetStateVehicles(StateVehicleSearchCriteriaDTO searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_StateVehicle>(true);

                predicate = predicate.And(x => x.isDeleted != true
                                                && x.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                                && this._loggedInUser.SubBranches.Any(a => a == x.branch)
                                                && (this._loggedInUser.UserLandmarksIds.Any(a => a == x.landmark)
                                                || (x.VehicleCustomerLocationClassesIds.Contains("2")  && !x.VehicleCustomerLocationClassesIds.Contains("1"))));
                //TODO : ServiceType - GetStateVehicles
                if (searchCriteria.ServiceTypeID != null && searchCriteria.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    predicate = predicate.And(v => v.serviceTypeId == (int)ServiceTypeEnum.SewageRemoval);
                }
                else
                {
                    predicate = predicate.And(v => v.serviceTypeId != (int)ServiceTypeEnum.SewageRemoval);
                }

                if (searchCriteria.StatusIDList != null && searchCriteria.StatusIDList.Any())
                {
                    predicate = predicate.And(v => searchCriteria.StatusIDList.Any(a => a == v.VehicleStatusId));
                }

                if (!String.IsNullOrEmpty(searchCriteria.VechilePlateNumberOrCode))
                {
                    var searchText = searchCriteria.VechilePlateNumberOrCode.Trim();
                    predicate = predicate.And(v => v.VehicleCode.Contains(searchText) || v.VehiclePlateNo.Contains(searchText));
                }

                if (!String.IsNullOrEmpty(searchCriteria.Driver))
                {
                    var searchDriver = searchCriteria.Driver.Trim();
                    var driverArr = searchCriteria.Driver.Trim().Split(' ');
                    if (driverArr.Length == 1)
                    {
                        predicate = predicate.And(s => s.DriverFirstName.Contains(searchDriver)
                                                    || s.DriverMiddleName.Contains(searchDriver)
                                                    || s.DriverLastName.Contains(searchDriver));
                    }
                    else
                    {
                        predicate = predicate.And(s =>
                                (s.DriverFirstName + " " + s.DriverMiddleName + " " + s.DriverLastName).Contains(searchDriver)
                                || (s.DriverFirstName + " " + s.DriverLastName).Contains(searchDriver));
                    }
                }

                if (!String.IsNullOrEmpty(searchCriteria.DriverCode))
                {
                    var searchText = searchCriteria.DriverCode.Trim();
                    predicate = predicate.And(v => v.DriverCode.Contains(searchText));
                }

                if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.VehicleIDs.Contains(s.VehicleID));
                }

                if (searchCriteria.DriverIDs != null && searchCriteria.DriverIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.DriverIDs.Any(a => a == s.DriverId));
                }
                #endregion

                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                var StateVehicleList = this._StateVehicleViewRepository.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.VehicleStatusId)
                    .Skip(skip)
                    .Take(take);

                #region response
                var result = new SearchResult<StateVehicleDTO>();
                if (StateVehicleList != null && StateVehicleList.Any())
                {
                    var count = this._StateVehicleViewRepository.GetQuery().Count(predicate);
                    result.Result = StateVehicleList.AsEnumerable().Select(x => x.WrapToStateVehicleDTO()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<StateVehicleDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => GetStateVehicles: "));
                return DescriptiveResponse<SearchResult<StateVehicleDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<StateVehicleDTO>> GetAssignableVehicles(WorkOderFilter filter)
        {
            try
            {
                var predicate = PredicateBuilder.New<vw_NWC_AssignableVehicles>(true);

                // get details of this order
                var orderDetails = this._stateWorkOrderRepository.FindById(filter.OrderId).WrapToStateWorkOrderDTO();

                #region skip & take
                var skip = 0;
                var take = 10;
                if (filter.PageFilter != null)
                {
                    skip = (filter.PageFilter.PageIndex - 1) * filter.PageFilter.PageSize;
                    take = filter.PageFilter.PageSize;
                }
                #endregion

                var assignableVehicleList = this._sp_NWC_GetAssignableVehiclesRepository.ExecWithStoredProcedure("sp_NWC_GetAssignableVehicles @SubID, @WorkOrderID, @OrderQuantity, @ZoneID, @StationID, @VehicleStatusId, @ServiceTypeID, @ClassID,@CategoryID",
                                            new SqlParameter("SubID", SqlDbType.UniqueIdentifier) { Value = this._loggedInUser.LoggedInUser.SubscriberId },
                                            new SqlParameter("workOrderID", SqlDbType.BigInt) { Value = filter.OrderId },
                                            new SqlParameter("OrderQuantity", SqlDbType.Int) { Value = orderDetails.OrderQuantity },
                                            new SqlParameter("ZoneID", SqlDbType.BigInt) { Value = orderDetails.ZoneID },
                                            new SqlParameter("StationID", SqlDbType.UniqueIdentifier) { Value = orderDetails.AssignedStationID },
                                            new SqlParameter("VehicleStatusId", SqlDbType.Int) { Value = orderDetails.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval ? (int)VehicleStatusEnum.Parking : (int)VehicleStatusEnum.Available },
                                            new SqlParameter("ServiceTypeID", SqlDbType.Int) { Value = orderDetails.ServiceTypeID },
                                            new SqlParameter("ClassID", SqlDbType.Int) { Value = orderDetails.CustomerLocationClassID },
                                            new SqlParameter("CategoryID", SqlDbType.Int) { Value = orderDetails.CategoryID });
                var VehicleList = assignableVehicleList
                                    //.OrderByDescending(s => s.ActionLogTime)
                                    .Skip(skip)
                                    .Take(take).ToList();

                #region response
                var result = new SearchResult<StateVehicleDTO>();
                if (VehicleList != null && VehicleList.Any())
                {
                    var count = assignableVehicleList.Count();
                    result.Result = VehicleList.AsEnumerable().Select(x => x.WrapToStateVehicleDTO()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<StateVehicleDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => GetAssignableVehicles: "));
                return DescriptiveResponse<SearchResult<StateVehicleDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<PrintDriverInvoice> GetPrintDriverInvoice(PrintDTO PrintDTO)
        {
            try
            {
                #region response
                var result = this._PrintDriverInvoice.GetQuery()
                    .Where(p => p.VehicleID == PrintDTO.VehicleID && p.OrderId == PrintDTO.WorkOrderID).FirstOrDefault();

                PrintDriverInvoice PrintDriverInvoice = result.WrapToPrintDriverInvoiceDTO();
                return DescriptiveResponse<PrintDriverInvoice>.Success(PrintDriverInvoice);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => GetPrintDriverInvoice: "));
                return DescriptiveResponse<PrintDriverInvoice>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<PrintCustomerInvoice> GetPrintCustomerInvoice(PrintDTO PrintDTO)
        {
            try
            {
                #region response
                var result = this._PrintCustomerInvoice.GetQuery()
                    .Where(p => p.VehicleID == PrintDTO.VehicleID && p.OrderId == PrintDTO.WorkOrderID).FirstOrDefault();

                PrintCustomerInvoice PrintCustomerInvoice = result.WrapToPrintCustomerInvoiceDTO();
                return DescriptiveResponse<PrintCustomerInvoice>.Success(PrintCustomerInvoice);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => GetPrintCustomerInvoice: "));
                return DescriptiveResponse<PrintCustomerInvoice>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<PrintVehicleInvoice> GetPrintVehicleInvoice(PrintDTO PrintDTO)
        {
            try
            {
                #region response
                var query = this._PrintVehicleInvoice.GetQuery()
                    .Where(p => p.VehicleId == PrintDTO.VehicleID
                                //&& p.ContractStationsIsActive != false
                                && p.ContractStationsIsDeleted != true
                                && p.VehicleSubId == this._loggedInUser.LoggedInUser.SubscriberId
                                )
                    .FirstOrDefault();

                var result = query.WrapToPrintVehicleInvoiceDTO();

                if (query.VehicleServiceTypeID.HasValue && query.VehicleCapacity.HasValue && query.StationId.HasValue)
                {
                    var zoneStation = new NWC_ZoneStations
                    {
                        StationID = query.StationId.Value,
                        ZoneID = 0,
                        Distance = 0
                    };

                    var netcost = WorkOrderCost.CalculateWorkOrderCost(0, DateTime.Now, query.CustomerLocationClassID,
                        query.VehicleServiceTypeID.Value, zoneStation, query.VehicleCapacity.Value);

                    var vatValue = WorkOrderCost.CalculateVat(netcost);

                    result.NetCost = Math.Round(netcost, 3);
                    result.VatValue = Math.Round(vatValue, 3);
                    result.TotalCost = Math.Round(netcost + vatValue, 3);
                }


                return DescriptiveResponse<PrintVehicleInvoice>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => GetPrintVehicleInvoice: "));
                return DescriptiveResponse<PrintVehicleInvoice>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<OrderReassignmentDTO>> GetOrderReassignmentReport(OrderReassignmentReportSC searchCriteria)
        {
            try
            {
                //#region Predicate
                //var predicate = PredicateBuilder.New<vw_NWC_Report_OrderReassignment>(x => x.SubID == _loggedInUser.LoggedInUser.SubscriberId);

                //if (searchCriteria.AreaIDs != null && searchCriteria.AreaIDs.Any())
                //{
                //    predicate = predicate.And(s => s.AssignedStationID != null ? searchCriteria.AreaIDs.Any(a => a == s.AreaId) : false);
                //}
                ////else
                ////{
                ////    predicate = predicate.And(s => s.AssignedStationID != null ? _loggedInUser.PermittedBranches.Any(a => a == s.AreaId) : false);
                ////}

                //if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
                //{
                //    predicate = predicate.And(s => s.AssignedStationID != null ? searchCriteria.CityIDs.Any(a => a == s.CityId) : false);
                //}
                //// else
                //// {
                ////     predicate = predicate.And(s => s.AssignedStationID != null ? _loggedInUser.PermittedBranches.Any(a => a == s.CityId) : false);
                //// }

                ////permitted stations
                //if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
                //{
                //    var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                //    predicate = predicate.And(s => searchList.Any(a => a == s.StationId));
                //}
                //else
                //{
                //    predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.StationId));
                //}

                //if (!string.IsNullOrEmpty(searchCriteria.OrderNumber))
                //{
                //    var searchText = searchCriteria.OrderNumber.Trim();
                //    predicate = predicate.And(v => v.OrderNumber.Contains(searchText));
                //}

                //if (!string.IsNullOrEmpty(searchCriteria.NewTankerCode))
                //{
                //    var searchText = searchCriteria.NewTankerCode.Trim();
                //    predicate = predicate.And(v => v.NewTankerCode.Contains(searchText));
                //}

                //if (!string.IsNullOrEmpty(searchCriteria.OldTankerCode))
                //{
                //    var searchText = searchCriteria.OldTankerCode.Trim();
                //    predicate = predicate.And(v => v.OldTankerCode.Contains(searchText));
                //}

                //if (searchCriteria.DateTimeFrom.HasValue)
                //{
                //    predicate = predicate.And(s => s.ReassignmentDate >= searchCriteria.DateTimeFrom.Value);
                //}

                //if (searchCriteria.DateTimeTo.HasValue)
                //{
                //    predicate = predicate.And(s => s.ReassignmentDate < searchCriteria.DateTimeTo.Value);
                //}
                //#endregion

                //IQueryable<vw_NWC_Report_OrderReassignment> woList = _vwOrderReassignmentRepo.GetQuery()
                //    .Where(predicate)
                //    .OrderBy(s => s.ReassignmentDate);

                //if (!searchCriteria.ExcelFlage)
                //{
                //    #region skip & take
                //    var skip = 0;
                //    var take = 10;
                //    if (searchCriteria.PageFilter != null)
                //    {
                //        skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                //        take = searchCriteria.PageFilter.PageSize;
                //    }
                //    #endregion

                //    woList = woList.Skip(skip).Take(take);
                //}

                //#region response
                //var result = new SearchResult<OrderReassignmentDTO>();
                //result.TotalCount = _vwOrderReassignmentRepo.GetQuery().Count(predicate);
                //result.Result = woList.AsEnumerable().Select(x =>
                //{
                //    var dto = new OrderReassignmentDTO();
                //    dto.Id = x.WorkOrderId;
                //    dto.OrderNumber = x.OrderNumber;
                //    dto.Area = x.AreaName;
                //    dto.City = x.CityName;
                //    dto.Station = x.StationName;
                //    dto.ReassignmentDate = x.ReassignmentDate;
                //    dto.NewTankerCode = x.NewTankerCode;
                //    dto.NewDriver = x.NewDriver;
                //    dto.OldTankerCode = x.OldTankerCode;
                //    dto.OldDriver = x.OldDriver;
                //    dto.OrderStatusName = LanguageIsEnglish ? x.EN_LastStatusName : x.AR_LastStatusName;
                //    dto.Reason = x.LastStatusReason;
                //    dto.CreatedBy = x.AssignmentCreatedBy;
                //    return dto;
                //}).ToList();

                //return DescriptiveResponse<SearchResult<OrderReassignmentDTO>>.Success(result);
                //#endregion

                int PageIndex = searchCriteria.ExcelFlage ? 0 : searchCriteria.PageFilter?.PageIndex - 1 ?? 0,
                PageSize = searchCriteria.ExcelFlage ? 999999999 : searchCriteria.PageFilter?.PageSize ?? 10;

                var list = _sp_NWC_Report_OrderReassignment
                    .ExecWithStoredProcedure("sp_NWC_Report_OrderReassignment @PageIndex, @PageSize, @SubID, @AreaIDs, @CityIDs, @StationIDs, @OldTanker, @NewTanker, @OrderNumber, @DateFrom, @DateTo",
                                                new SqlParameter("PageIndex", SqlDbType.Int) { Value = SetDBValue(PageIndex) },
                                                new SqlParameter("PageSize", SqlDbType.Int) { Value = SetDBValue(PageSize) },
                                                new SqlParameter("SubID", SqlDbType.UniqueIdentifier) { Value = SetDBValue(_loggedInUser.LoggedInUser.SubscriberId) },
                                                new SqlParameter("AreaIDs", SqlDbType.NVarChar) { Value = SetDBValue(Join(searchCriteria.AreaIDs)) },
                                                new SqlParameter("CityIDs", SqlDbType.NVarChar) { Value = SetDBValue(Join(searchCriteria.CityIDs)) },
                                                new SqlParameter("StationIDs", SqlDbType.NVarChar) { Value = SetDBValue(Join(searchCriteria.StationIDs)) },
                                                new SqlParameter("OldTanker", SqlDbType.NVarChar) { Value = SetDBValue(searchCriteria.OldTankerCode) },
                                                new SqlParameter("NewTanker", SqlDbType.NVarChar) { Value = SetDBValue(searchCriteria.NewTankerCode) },
                                                new SqlParameter("OrderNumber", SqlDbType.NVarChar) { Value = SetDBValue(searchCriteria.OrderNumber) },
                                                new SqlParameter("DateFrom", SqlDbType.DateTime) { Value = SetDBValue(searchCriteria.DateTimeFrom) },
                                                new SqlParameter("DateTo", SqlDbType.DateTime) { Value = SetDBValue(searchCriteria.DateTimeTo) });

                #region response
                var result = new SearchResult<OrderReassignmentDTO>();
                result.TotalCount = list.FirstOrDefault()?.TotalCount ?? 0;
                result.Result = list.Select(x =>
                {
                    var dto = new OrderReassignmentDTO();
                    dto.Id = x.WorkOrderId;
                    dto.OrderNumber = x.OrderNumber;
                    dto.Area = x.AreaName;
                    dto.City = x.CityName;
                    dto.Station = x.StationName;
                    dto.ReassignmentDate = x.ReassignmentDate;
                    dto.NewTankerCode = x.NewTankerCode;
                    dto.NewDriver = x.NewDriver;
                    dto.OldTankerCode = x.OldTankerCode;
                    dto.OldDriver = x.OldDriver;
                    dto.OrderStatusName = LanguageIsEnglish ? x.EN_LastStatusName : x.AR_LastStatusName;
                    dto.Reason = x.LastStatusReason;
                    dto.CreatedBy = x.AssignmentCreatedBy;
                    return dto;
                }).ToList();

                return DescriptiveResponse<SearchResult<OrderReassignmentDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "WorkOrderVehicleService => GetOrderReassignmentReport: "));
                return DescriptiveResponse<SearchResult<OrderReassignmentDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public object SetDBValue(object value) => value == null ? DBNull.Value : value;
        public string Join<T>(IEnumerable<T> list, string separator = ",") => list != null && list.Any() ? string.Join(separator, list) : null;
        #endregion
    }
}
