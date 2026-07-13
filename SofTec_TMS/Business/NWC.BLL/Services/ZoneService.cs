using Infrastructure;
using LinqKit;
using Newtonsoft.Json;
using NWC.BLL.GISServiceRef;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
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
using System.Linq.Expressions;
using System.Threading.Tasks;
using static NWC.DTO.Models.ZoneDTO;

namespace NWC.BLL.Services
{
    public class ZoneService : IZoneService
    {
        #region Properties
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<vw_NWC_ZoneList> _zoneListRepository;
        private readonly IRepository<NWC_Zone> _ZonesRepository;
        private readonly IRepository<NWC_ZoneStations> _ZoneStationRepository;
        private readonly IRepository<Landmark> _StationRepository;
        private readonly IRepository<Branch> _BranchRepository;
        private readonly IRepository<NWC_RestrictedZoneVehicleType> _RestrictedZoneVehicleTypeRepository;
        private readonly IRepository<NWC_ContractTariff> _ContractTariffRepository;
        private readonly IRepository<NWC_CustomerLocation> _CustomerLocationRepository;

        private readonly ILoggedInUserService _LoggedInUserService;
        // private readonly IRepository<NWC> _zoneRepository;
        #endregion

        #region Constructors
        public ZoneService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._LoggedInUserService = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._zoneListRepository = new Repository<vw_NWC_ZoneList>(ctx);
            this._ZonesRepository = new Repository<NWC_Zone>(ctx);
            this._ZoneStationRepository = new Repository<NWC_ZoneStations>(ctx);
            this._StationRepository = new Repository<Landmark>(ctx);
            this._BranchRepository = new Repository<Branch>(ctx);
            this._RestrictedZoneVehicleTypeRepository = new Repository<NWC_RestrictedZoneVehicleType>(ctx);
            this._ContractTariffRepository = new Repository<NWC_ContractTariff>(ctx);
            this._CustomerLocationRepository = new Repository<NWC_CustomerLocation>(ctx);
        }
        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<ZoneListDTO>> SearchZones(ZoneSearchCriteriaDTO searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_ZoneList>(true);
            predicate = predicate.And(s => s.IsDeleted != true
                                           && s.SubID == _LoggedInUserService.LoggedInUser.SubscriberId);

            if (!string.IsNullOrEmpty(searchCriteria.NameOrCode))
            {
                var name = searchCriteria.NameOrCode.Trim();
                predicate = predicate.And(s => s.ZoneName.Contains(name) || s.Code.Contains(name));
            }

            //permitted cities
            if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            {
                var searchList = _LoggedInUserService.SubBranches.Intersect(searchCriteria.CityIDs);
                predicate = predicate.And(s => searchList.Contains(s.CityID));
            }
            else
            {
                predicate = predicate.And(s => this._LoggedInUserService.SubBranches.Contains(s.CityID));
            }

            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _LoggedInUserService.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => (a == s.MainStationId)));
            }

            #endregion

            #region Sort
            // TODO: handle localization
            Expression<Func<vw_NWC_ZoneList, object>> sort;
            switch (searchCriteria.OrderBy)
            {
                case ZoneSearchCriteriaDTO.OrderByExepression.ZoneCode:
                    sort = x => x.Code;
                    break;

                case ZoneSearchCriteriaDTO.OrderByExepression.ZoneName:
                default:
                    sort = x => x.ZoneName;
                    break;
            }
            #endregion

            #region skip & take
            var skip = 0;
            var take = 20;
            if (searchCriteria.PageFilter != null)
            {
                skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                take = searchCriteria.PageFilter.PageSize;
            }
            #endregion

            var zoneQuery = this._zoneListRepository.GetQuery()
              .Where(predicate)
              .OrderBy(s => s.ZoneName)
              .Skip(skip)
              .Take(take);

            #region response
            var result = new SearchResult<ZoneListDTO>();
            if (zoneQuery != null && zoneQuery.Any())
            {
                var count = this._zoneListRepository.GetQuery().Count(predicate);
                result.Result = zoneQuery.AsEnumerable().Select(a => a.WrapToZoneListDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<ZoneListDTO>>.Success(result);
            #endregion

        }

        public DescriptiveResponse<ZoneDTO> GetZoneDetails(long ZoneId)
        {
            try
            {
                ZoneDTO zone = new ZoneDTO();
                using (_unitofWork)
                {
                    zone = this._ZonesRepository.GetQuery()
                        .FirstOrDefault(s => s.ID == ZoneId
                                             && s.IsDeleted != true
                                             && s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                             && this._LoggedInUserService.SubBranches.Contains(s.CityID))
                        .WrapToZoneDTO();

                    zone.MainStation = this._ZoneStationRepository.GetQuery().
                    Where(st => st.IsMain == true && st.ZoneID == ZoneId).Select(s => new Station()
                    {
                        ID = s.StationID,
                        Distance = s.Distance
                    }).FirstOrDefault();

                    zone.BackupStations = this._ZoneStationRepository.GetQuery().
                    Where(st => st.IsMain == false && st.ZoneID == ZoneId).Select(s =>
                        new Station()
                        {
                            ID = s.StationID,
                            Distance = s.Distance
                        }).ToList();


                    zone.RestrictedTankerTypes = this._RestrictedZoneVehicleTypeRepository.GetQuery()
                        .Where(r => r.ZoneID == ZoneId).ToList().Select(tanker => new LookUpDTO<Guid>()
                        {
                            Id = tanker.VehicleTypeID,
                            Name = ""
                        }).ToList();

                }
                using (_unitofWork)
                {
                    zone.AreaID = this._BranchRepository.GetQuery().Where(s => (s.parentBranchId != null && s.Id == zone.CityID)).Select(x => x.parentBranchId).FirstOrDefault();
                }

                return DescriptiveResponse<ZoneDTO>.Success(zone);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ZoneService => GetZoneDetails: "));
                return DescriptiveResponse<ZoneDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<ZoneDTO> GetZoneByIntegrationID(string zoneIntegrationId)
        {
            try
            {
                var zone = new ZoneDTO();

                zone = this._ZonesRepository.GetQuery()
                    .FirstOrDefault(s => s.IntegrationId == zoneIntegrationId
                                         && s.IsDeleted != true
                                         && s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                         //&& this._LoggedInUserService.SubBranches.Contains(s.CityID))
                    .WrapToZoneDTO();

                return DescriptiveResponse<ZoneDTO>.Success(zone);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ZoneService => GetZoneByIntegrationID: "));
                return DescriptiveResponse<ZoneDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region command
        public DescriptiveResponse<bool> Add(ZoneDTO dto)
        {
            try
            {
                var zone = ZoneDTO.MapToZone(dto);
                zone.IsDeleted = false;
                zone.CreatedBy = _LoggedInUserService.LoggedInUser.StaffId;
                zone.CreatedDate = DateTimeHelper.GetDateTimeNow();
                zone.SubID = _LoggedInUserService.LoggedInUser.SubscriberId;
              

                #region Validations
                var validator = new ZoneValidator(ValidationMode.Create, this._LoggedInUserService, this._ZonesRepository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion

                using (_unitofWork)
                {
                    this._ZonesRepository.Add(zone);

                }
                using (_unitofWork)
                {
                    this._ZoneStationRepository.Add(new NWC_ZoneStations
                    {
                        ZoneID = zone.ID,
                        StationID = dto.MainStation.ID,
                        IsMain = true,
                        Distance = dto.MainStation.Distance
                    });

                    if (dto.BackupStations.Count > 0)
                    {
                        var BackupStations = dto.BackupStations.Select(BackupStation =>
                               new NWC_ZoneStations()
                               {
                                   ZoneID = zone.ID,
                                   StationID = BackupStation.ID,
                                   IsMain = false,
                                   Distance = BackupStation.Distance
                               }).ToList();
                        this._ZoneStationRepository.AddRange(BackupStations);
                    }

                    var RestrictedTankerTypes = dto.RestrictedTankerTypes.Select(tanker => new NWC_RestrictedZoneVehicleType()
                    {
                        VehicleTypeID = tanker.Id,
                        ZoneID = zone.ID
                    }).ToList();

                    this._RestrictedZoneVehicleTypeRepository.AddRange(RestrictedTankerTypes);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ZoneService => Add: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> Update(ZoneDTO dto)
        {
            try
            {
                #region Validations
                var validator = new ZoneValidator(ValidationMode.Update, this._LoggedInUserService, this._ZonesRepository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion

                using (_unitofWork)
                {
                    var zone = this._ZonesRepository.GetQuery()
                        .FirstOrDefault(s => s.ID == dto.ID
                                             && s.IsDeleted != true
                                             && s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                             && this._LoggedInUserService.SubBranches.Contains(s.CityID));


                    zone.UpdatedBy = _LoggedInUserService.LoggedInUser.StaffId;
                    zone.UpdatedDate = DateTimeHelper.GetDateTimeNow();
                    zone.Name = dto.Name;
                    zone.CityID = dto.CityID;
                    zone.Code = dto.Code;
                    zone.ZoneWithoutTanker = dto.ZoneWithoutTanker;
                    this._ZonesRepository.Update(zone);

                    var ZoneStations = this._ZoneStationRepository.GetQuery().Where(st => st.ZoneID == dto.ID).ToList();
                    // delete all station zone
                    foreach (var z in ZoneStations)
                    {
                        this._ZoneStationRepository.Delete(z);
                    }

                    this._ZoneStationRepository.Add(new NWC_ZoneStations
                    {
                        ZoneID = zone.ID,
                        StationID = dto.MainStation.ID,
                        IsMain = true,
                        Distance = dto.MainStation.Distance
                    });

                    if (dto.BackupStations.Count > 0)
                    {
                        var BackupStations = dto.BackupStations.Select(BackupStation =>
                               new NWC_ZoneStations()
                               {
                                   ZoneID = zone.ID,
                                   StationID = BackupStation.ID,
                                   IsMain = false,
                                   Distance = BackupStation.Distance
                               }).ToList();
                        this._ZoneStationRepository.AddRange(BackupStations);
                    }

                    var RestrictedZoneVehicleType = this._RestrictedZoneVehicleTypeRepository.GetQuery().Where(st => st.ZoneID == dto.ID).ToList();
                    // delete all RestrictedZoneVehicleType zone
                    foreach (var z in RestrictedZoneVehicleType)
                    {
                        this._RestrictedZoneVehicleTypeRepository.Delete(z);
                    }

                    var RestrictedTankerTypes = dto.RestrictedTankerTypes.Select(tanker => new NWC_RestrictedZoneVehicleType()
                    {
                        VehicleTypeID = tanker.Id,
                        ZoneID = zone.ID
                    }).ToList();

                    this._RestrictedZoneVehicleTypeRepository.AddRange(RestrictedTankerTypes);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ZoneService => Update: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> Delete(long ZoneId)
        {
            try
            {
                //TODO: temporary validation
                if (ZoneId == 0)
                    return DescriptiveResponse<bool>.Error();

                var dto = new ZoneDTO() { ID = ZoneId };
                #region Validations
                var validator = new ZoneValidator(ValidationMode.Delete, this._LoggedInUserService, this._ZonesRepository, this._ContractTariffRepository, this._CustomerLocationRepository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion

                using (_unitofWork)
                {
                    var zone = this._ZonesRepository.GetQuery()
                        .FirstOrDefault(s => s.ID == ZoneId
                                             && s.IsDeleted != true
                                             && s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                             && this._LoggedInUserService.SubBranches.Contains(s.CityID));

                    zone.UpdatedBy = _LoggedInUserService.LoggedInUser.StaffId;
                    zone.UpdatedDate = DateTimeHelper.GetDateTimeNow();
                    zone.IsDeleted = true;
                    this._ZonesRepository.Update(zone);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ZoneService => Delete: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<ZoneDTO>> AddRange(List<ZoneDTO> zonesDTO)
        {
            #region fetch Ids
            foreach (var dto in zonesDTO)
            {
                dto.CityID = this.GetCityId(dto.CityName);
                if (dto.MainStation != null)
                {
                    dto.MainStation.ID = this.GetStationId(dto.MainStation.StationName);
                }
                if (dto.BackupStations != null && dto.BackupStations.Any())
                {
                    foreach (var item in dto.BackupStations)
                    {
                        item.ID = this.GetStationId(item.StationName);
                    }
                }
            }
            #endregion

            var failedList = new List<ZoneDTO>();
            var addList = new List<NWC_Zone>();

            foreach (var dto in zonesDTO)
            {
                #region Validations
                var validator = new ZoneValidator(ValidationMode.CreateLogic2, _LoggedInUserService, _ZonesRepository, null,null, zonesDTO);
                var results = validator.Validate(dto);
                #endregion

                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    dto.ExcelValidation = string.Join(", ", failures);

                    failedList.Add(dto);
                }
                else
                {
                    var newZone = ZoneDTO.MapToZoneWithStations(dto); //dto.WrapToTariff();

                    #region prepare model
                    newZone.IsDeleted = false;
                    newZone.CreatedBy = this._LoggedInUserService.LoggedInUser.StaffId;
                    newZone.CreatedDate = DateTimeHelper.GetDateTimeNow();
                    newZone.SubID = this._LoggedInUserService.LoggedInUser.SubscriberId;
                    #endregion

                    addList.Add(newZone);
                }

            }

            using (_unitofWork)
            {
                this._ZonesRepository.AddRange(addList);
            }

            return DescriptiveResponse<List<ZoneDTO>>.Success(failedList);
        }

        #endregion

        private Guid GetCityId(string name)
        {
            if (string.IsNullOrEmpty(name)) return Guid.Empty;

            var cities = this._BranchRepository.GetQuery()
                        .Where(a =>
                            this._LoggedInUserService.SubBranches.Contains(a.Id)
                            && a.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId
                            && a.isDeleted != true
                            && a.name == name
                        )
                        .Select(s => s.Id)
                        .ToList();

            return (cities == null) || (cities.Count() < 1) ? Guid.Empty : cities[0];
        }

        private Guid GetStationId(string name)
        {
            if (string.IsNullOrEmpty(name)) return Guid.Empty;

            var stations = this._StationRepository.GetQuery()
                        .Where(a =>
                            this._LoggedInUserService.UserLandmarksIds.Contains(a.Id)
                            && a.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId
                            && a.isDeleted != true
                            && a.name == name
                        )
                        .Select(s => s.Id)
                        .ToList();

            return (stations == null) || (stations.Count() < 1) ? Guid.Empty : stations[0];
        }
        
        #region GIS
        public string CallGISService(string premiseCoordinates, string orderNumber, string sourceApp, string transactionId)
        {
            try
            {
                //check if zoneID not exist call GIS service
                var gisServiceClient = new GISServiceRef.echannelGetLocationInfoPortTypeClient();

                var gisRequest = new GISServiceRef.echannelGetLocationInfoRequest()
                {
                    orgCode = "KSA",
                    locType = "LONGLAT",
                    locValue = premiseCoordinates,
                    outputinfoType = "FSZONE",
                    sourceApplication = GetGISSourceApp(sourceApp),
                    transactionId = transactionId
                };

                var result = Task.Run(async () => await gisServiceClient.echannelGetLocationInfoOperationAsync(gisRequest)).ConfigureAwait(false);

                var gisResponse = result.GetAwaiter().GetResult().echannelGetLocationInfoResponse;

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CallingGIS: {0}", JsonConvert.SerializeObject(gisResponse))));

                if (gisResponse != null && gisResponse.status.ToUpper() == "OK")
                    return gisResponse.outputValue;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ZoneService => CallGISService: "));
                return null;
            }
        }

        private echannelGetLocationInfoRequestSourceApplication GetGISSourceApp(string source)
        {
            var sourceApp = echannelGetLocationInfoRequestSourceApplication.CCB;

            if (source.ToUpper() == echannelGetLocationInfoRequestSourceApplication.EBRANCH.ToString())
                sourceApp = echannelGetLocationInfoRequestSourceApplication.EBRANCH;
            if (source.ToUpper() == echannelGetLocationInfoRequestSourceApplication.MOBAPP.ToString())
                sourceApp = echannelGetLocationInfoRequestSourceApplication.MOBAPP;
            if (source.ToUpper() == echannelGetLocationInfoRequestSourceApplication.TMS.ToString())
                sourceApp = echannelGetLocationInfoRequestSourceApplication.TMS;

            return sourceApp;
        }
        #endregion
    }
}
