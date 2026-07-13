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
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace NWC.BLL.Services
{
    public class StationService : IStationService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<Transporter> _vehicleRepository;
        private readonly IRepository<NWC_StationServiceType> _NWC_StationServiceTypeRep;
        private readonly IRepository<NWC_StationCustomerLocationClass> _NWC_StationCustClassRep;
        private readonly IRepository<vw_NWC_LandmarkNWCSettings> _vw_NWC_LandmarkNWCSettingsRep;
        private readonly IRepository<vw_NWC_StationSizes> _vw_NWC_StationSizesRep;
        private readonly IRepository<NWC_CustomerLocationClass> _customerLocationClassRep;
        private readonly IRepository<NWC_ServiceType> _serviceTypeRep;
        private readonly IRepository<Landmark> _Landmark;
        private readonly IRepository<NWC_Zone> _zoneRepository;
        private readonly IRepository<NWC_ZoneStations> _zoneStationRepository;
        private readonly IRepository<NWC_StationDefaultTankers> _StationDefaultTankersRepository;
        private readonly WorkOrderService workOrderService;
        #endregion

        #region Constructors
        public StationService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._vehicleRepository = new Repository<Transporter>(ctx);
            this._NWC_StationServiceTypeRep = new Repository<NWC_StationServiceType>(ctx);
            this._NWC_StationCustClassRep = new Repository<NWC_StationCustomerLocationClass>(ctx);
            this._vw_NWC_LandmarkNWCSettingsRep = new Repository<vw_NWC_LandmarkNWCSettings>(ctx);
            this._serviceTypeRep = new Repository<NWC_ServiceType>(ctx);
            this._vw_NWC_StationSizesRep = new Repository<vw_NWC_StationSizes>(ctx);
            this._customerLocationClassRep = new Repository<NWC_CustomerLocationClass>(ctx);
            this._Landmark = new Repository<Landmark>(ctx);
            this._zoneRepository = new Repository<NWC_Zone>(ctx);
            this._zoneStationRepository = new Repository<NWC_ZoneStations>(ctx);
            this._StationDefaultTankersRepository = new Repository<NWC_StationDefaultTankers>(ctx);

            this.workOrderService = new WorkOrderService(loggedInUser, ctx);
        }
        #endregion

        #region Command
        public DescriptiveResponse<Boolean> SaveStationNWCSettings(StationNWCSettingsDTO dto)
        {
            return dto.StationId == null ? AddStationNWCSettings(dto) : UpdateStationNWCSettings(dto);
        }
        public DescriptiveResponse<Boolean> AddStationNWCSettings(StationNWCSettingsDTO dto)
        {
            try
            {
                using (_unitofWork)
                {
                    var station = dto.StationSettingDtoToEntity();
                    station.CreatedBy = _loggedInUser.LoggedInUser.StaffId;
                    station.SubId = _loggedInUser.LoggedInUser.SubscriberId;
                    foreach (var stID in dto.StationServiceIds)
                        station.NWC_StationServiceType.Add(new NWC_StationServiceType()
                        {
                            StationID = station.Id,
                            ServiceTypeID = stID
                        });
                    foreach (var classID in dto.CustomerClassIds)
                        station.NWC_StationCustomerLocationClass.Add(new NWC_StationCustomerLocationClass()
                        {
                            StationID = station.Id,
                            CustomerLocationClassID = classID
                        });
                    _Landmark.Add(station);

                    // Add Default Tanker List
                    if(dto.SelectedCapacities != null && dto.SelectedCapacities.Any())
                    {
                        SaveStationDefaultTankers(dto.SelectedCapacities, station.Id);
                    }
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StationService => SaveStationNWCSettings: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<Boolean> UpdateStationNWCSettings(StationNWCSettingsDTO dto)
        {
            try
            {
                using (_unitofWork)
                {

                    var station = this._Landmark.GetQuery()
                        .FirstOrDefault(s => s.Id == dto.StationId
                                        && s.SubId == this._loggedInUser.LoggedInUser.SubscriberId
                                        && s.isDeleted != true);

                    this._Landmark.Update(dto.StationSettingDtoToEntity(station));


                    // Add or Delete station service type
                    var dbStationServiceTypeList = this._NWC_StationServiceTypeRep.GetQuery()
                        .Where(s => s.StationID == dto.StationId);

                    foreach (var sServiceType in dbStationServiceTypeList)
                    {
                        if (!dto.StationServiceIds.Contains(sServiceType.ServiceTypeID))
                        {
                            this._NWC_StationServiceTypeRep.Delete(sServiceType);
                        }
                    }

                    foreach (var stID in dto.StationServiceIds)
                    {
                        var stationServiceType = new NWC_StationServiceType()
                        {
                            StationID = dto.StationId.Value,
                            ServiceTypeID = stID
                        };

                        if (!dbStationServiceTypeList.Select(x => x.ServiceTypeID).Contains(stID))
                        {
                            this._NWC_StationServiceTypeRep.Add(stationServiceType);
                        }
                    }


                    // Add or Delete station customer location class
                    var dbStationClassList = this._NWC_StationCustClassRep.GetQuery()
                        .Where(s => s.StationID == dto.StationId);

                    foreach (var stClass in dbStationClassList)
                    {
                        if (!dto.CustomerClassIds.Contains(stClass.CustomerLocationClassID))
                        {
                            this._NWC_StationCustClassRep.Delete(stClass);
                        }
                    }

                    foreach (var classID in dto.CustomerClassIds)
                    {
                        var stationCustClass = new NWC_StationCustomerLocationClass()
                        {
                            StationID = dto.StationId.Value,
                            CustomerLocationClassID = classID
                        };

                        if (!dbStationClassList.Select(x => x.CustomerLocationClassID).Contains(classID))
                        {
                            this._NWC_StationCustClassRep.Add(stationCustClass);
                        }
                    }

                    // Add New Default Tanker List
                    if (dto.SelectedCapacities != null && dto.SelectedCapacities.Any())
                    {
                        SaveStationDefaultTankers(dto.SelectedCapacities, station.Id);
                    }
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StationService => SaveStationNWCSettings: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public void SaveStationDefaultTankers(List<LookUpDTO<int>> Capacities, Guid stationId)
        {
            try
            {
                using (_unitofWork)
                {
                    var oldCapacities = this._StationDefaultTankersRepository.GetQuery().Where(x => x.StationID == stationId).ToList();

                    foreach (var capacity in oldCapacities)
                    {
                        this._StationDefaultTankersRepository.Delete(capacity);
                    }

                    if (Capacities.Count() > 0)
                    {
                        var tankers = Capacities.Select(tnkr => new NWC_StationDefaultTankers()
                        {
                            StationID = stationId,
                            Capacity = tnkr.Id,
                            CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                            CreationDate = DateTimeHelper.GetDateTimeNow()

                        }).ToList();

                        this._StationDefaultTankersRepository.AddRange(tankers);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StationService => SaveStationDefaultTanker:"));
            }
        }
        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<StationNWCSettingsDTO>> GetStationNWCSettings(StationSettingsSC searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_LandmarkNWCSettings>(true);

                predicate = predicate.And(s => s.IsDeleted == searchCriteria.IsDeleted);
                predicate = predicate.And(s => s.SubId == _loggedInUser.LoggedInUser.SubscriberId);

                if (!string.IsNullOrEmpty(searchCriteria.SearchKeyword))
                {
                    var searchText = searchCriteria.SearchKeyword.Trim();
                    predicate = predicate.And(s => s.Name.Contains(searchText));
                }
                if (searchCriteria.AreaIds != null && searchCriteria.AreaIds.Any())
                {
                    var branches = _loggedInUser.Branches.Intersect(searchCriteria.AreaIds);
                    predicate = predicate.And(s => branches.Contains(s.AreaId));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.Branches.Contains(s.AreaId));
                }
                if (searchCriteria.CityIds != null && searchCriteria.CityIds.Any())
                {
                    var subBranches = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIds);
                    predicate = predicate.And(s => subBranches.Contains(s.CityId));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CityId));
                }
                if (searchCriteria.CustomerClass.HasValue)
                {
                    predicate = predicate.And(s => s.CustomerClasses.Contains(searchCriteria.CustomerClass.ToString()));
                }
                if (searchCriteria.ServiceType.HasValue)
                {
                    predicate = predicate.And(s => s.StationServices.Contains(searchCriteria.ServiceType.ToString()));
                }
                if (searchCriteria.Status.HasValue)
                {
                    predicate = predicate.And(s => s.StatusId == searchCriteria.Status);
                }
                #endregion

                #region skip & take
                var skip = 0;
                var take = 20;
                if (searchCriteria != null && searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                var stationNWCSettings = _vw_NWC_LandmarkNWCSettingsRep.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.Id)
                    .Skip(skip)
                    .Take(take);

                var result = new SearchResult<StationNWCSettingsDTO>();
                result.TotalCount = _vw_NWC_LandmarkNWCSettingsRep.GetQuery().Count(predicate);
                result.Result = stationNWCSettings.AsEnumerable().Select(a => a.WrapToStationNWCSettingsDTO()).ToList();
                return DescriptiveResponse<SearchResult<StationNWCSettingsDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StationService => GetStationNWCSettings: "));
                return DescriptiveResponse<SearchResult<StationNWCSettingsDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<StationNWCSettingsDTO> GetStationNWCSetting(Guid stationId)
        {
            try
            {
                var entity = _vw_NWC_LandmarkNWCSettingsRep.GetQuery().FirstOrDefault(s => s.Id == stationId);
                return DescriptiveResponse<StationNWCSettingsDTO>.Success(entity.WrapToStationNWCSettingsDTO());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StationService => GetStationNWCSettings: "));
                return DescriptiveResponse<StationNWCSettingsDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<StationSizesDTO> GetStationDefaultSizes(Guid StationID)
        {
            var entity = _vw_NWC_StationSizesRep.GetQuery().FirstOrDefault(s => s.id == StationID);
            return DescriptiveResponse<StationSizesDTO>.Success(entity.WrapStationSizes());
        }

        #region Integration
        public DescriptiveResponse<bool> IsStationExceededQuota(Guid StationID)
        {
            try
            {
                var station = this._Landmark.FindById(StationID);

                if (station.DailyQuota == null)
                    return DescriptiveResponse<bool>.Success(false);


                var searchCriteria = new WorkOrderSearchCriteriaDTO()
                {
                    StatusIDs = new List<int>() { (int)WorkOrderStatusEnum.New },
                    DatePeriod = WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate,
                    DateTimeFrom = DateTimeHelper.GetDateTimeNow().AddDays(-1),
                    DateTimeTo = DateTimeHelper.GetDateTimeNow().AddDays(1),
                    StationIDs = new List<Guid>() { station.Id },
                };

                var workOrdersResults = this.workOrderService.SearchWorkOrders(searchCriteria);
                var workOrders = workOrdersResults.Value.Result;

                return DescriptiveResponse<bool>.Success(workOrdersResults.Value.TotalCount >= station.DailyQuota);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StationService => IsStationExceededQuota: "));
                //return DescriptiveResponse<bool>.Error((ErrorStatus.UNEXPECTED_ERROR);
                throw ex;
            }
        }
        #endregion
        #endregion

        #region Helpers
        private bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
        #endregion
    }
}
