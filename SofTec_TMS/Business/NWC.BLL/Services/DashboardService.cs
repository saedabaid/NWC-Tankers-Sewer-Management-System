using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace NWC.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<vw_NWC_WorkOrderList> _workOrderListRep;
        private readonly IRepository<vw_NWC_ZoneWithNoPrices> _zoneWithNoPriceListRep;
        private readonly IRepository<sp_NWC_DashboardOrdersCountsViceExcuteTime_Result> _sp_NWC_DashboardOrdersCountsViceExcuteTimeRepository;
        #endregion

        #region Constructors
        public DashboardService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._workOrderListRep = new Repository<vw_NWC_WorkOrderList>(ctx);
            this._zoneWithNoPriceListRep = new Repository<vw_NWC_ZoneWithNoPrices>(ctx);
            this._sp_NWC_DashboardOrdersCountsViceExcuteTimeRepository = new Repository<sp_NWC_DashboardOrdersCountsViceExcuteTime_Result>(ctx);
        }
        #endregion

        #region Query
        public DescriptiveResponse<int> GetWorkOrdersCountPerStatus(DashboardSC searchCriteria)
        {
            //validation
            if (
                //searchCriteria.DateFrom.Date > DateTime.Now.Date
                //|| searchCriteria.DateTo.Date > DateTime.Now.Date
                searchCriteria.DateFrom > searchCriteria.DateTo
                //|| searchCriteria.StationIDs == null || !searchCriteria.StationIDs.Any()
                || !_loggedInUser.UserServicesIds.Contains(searchCriteria.ServiceTypeID)
                )
            {
                return DescriptiveResponse<int>.Error();
            }

            #region Predicate
            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day
            
            //var permittedStations = _loggedInUser.UserLandmarksIds
            //                  .Intersect<Guid>(searchCriteria.StationIDs).ToList();

            //var permittedServiceTypes = _loggedInUser.UserServicesIds.Find(s => s == searchCriteria.ServiceTypeID);


            //predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            predicate = predicate.And(s => s.IsDeleted != true
                                            && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                            && s.ServiceTypeID == searchCriteria.ServiceTypeID
                                            //&& permittedStations.Contains(s.AssignedStationID)
                                            && s.CreateTime >= from
                                            && s.CreateTime < to);

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.LastStatusID));
            }

            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var permittedStations = _loggedInUser.UserLandmarksIds
                              .Intersect<Guid>(searchCriteria.StationIDs).ToList();
                predicate = predicate.And(s => permittedStations.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.AssignedStationID));
            }

            //predicate = predicate.And(s => s.IsDeleted != true);
            //predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);
            //predicate = predicate.And(s => s.ServiceTypeID == searchCriteria.ServiceTypeID);
            //predicate = predicate.And(s => permittedStations.Contains(s.AssignedStationID));
            //predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.AssignedStationID));

            //if (searchCriteria.DateFrom != null)
            //{
            //    var from = searchCriteria.DateFrom.Date; //from the start of the day
            //    predicate = predicate.And(s => s.CreateTime >= from);
            //}

            //if (searchCriteria.DateTo != null)
            //{
            //    var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day
            //    predicate = predicate.And(s => s.CreateTime < to);
            //}

            //if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            //{
            //    predicate = predicate.And(s => searchCriteria.StationIDs.Contains(s.AssignedStationID));
            //}

            #endregion

            var count = this._workOrderListRep.GetQuery().Where(predicate).Count();
            return DescriptiveResponse<int>.Success(count);

        }
      
        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByDayHours(DashboardSC searchCriteria)
        {
            //validation
            if (
                searchCriteria.DateFrom > searchCriteria.DateTo
                //|| searchCriteria.StationIDs == null || !searchCriteria.StationIDs.Any()
                || !_loggedInUser.UserServicesIds.Contains(searchCriteria.ServiceTypeID)
                )
            {
                return DescriptiveResponse<List<DashboardXYChartDTO>>.Error();
            }

            #region Predicate
            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day

            //var permittedStations = _loggedInUser.UserLandmarksIds
            //                  .Intersect<Guid>(searchCriteria.StationIDs).ToList();


            //predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            predicate = predicate.And(s => s.IsDeleted != true
                                            && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                            && s.ServiceTypeID == searchCriteria.ServiceTypeID
                                            //&& permittedStations.Contains(s.AssignedStationID)
                                            && s.CreateTime >= from
                                            && s.CreateTime < to);

            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var permittedStations = _loggedInUser.UserLandmarksIds
                              .Intersect<Guid>(searchCriteria.StationIDs).ToList();
                predicate = predicate.And(s => permittedStations.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.AssignedStationID));
            }

            #endregion

            var data = this._workOrderListRep.GetQuery().Where(predicate)
                .GroupBy(a => a.CreateTime.Value.Hour)
                .Select(s => new DashboardXYChartDTO
                {
                    Name = s.Key.ToString(),
                    Count = s.Count()
                })
                .ToList();

            List<DashboardXYChartDTO> result = new List<DashboardXYChartDTO>();
            for (int i = 0; i < 24; i++)
            {
                var h = data.FirstOrDefault(a => a.Name == i.ToString());
                if (h != null)
                {
                    result.Add(h);
                }
                else
                {
                    result.Add(new DashboardXYChartDTO
                    {
                        Name = i.ToString(),
                        Count = 0
                    });
                }
            }

            return DescriptiveResponse<List<DashboardXYChartDTO>>.Success(result);
            
        }

        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByTop10Zones(DashboardSC searchCriteria)
        {
            //validation
            if (
                searchCriteria.DateFrom > searchCriteria.DateTo
                //|| searchCriteria.StationIDs == null || !searchCriteria.StationIDs.Any()
                || !_loggedInUser.UserServicesIds.Contains(searchCriteria.ServiceTypeID)
                )
            {
                return DescriptiveResponse<List<DashboardXYChartDTO>>.Error();
            }

            #region Predicate
            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day

            //var permittedStations = _loggedInUser.UserLandmarksIds
            //                  .Intersect<Guid>(searchCriteria.StationIDs).ToList();


            //predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            predicate = predicate.And(s => s.IsDeleted != true
                                            && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                            && s.ServiceTypeID == searchCriteria.ServiceTypeID
                                            //&& permittedStations.Contains(s.AssignedStationID)
                                            && s.CreateTime >= from
                                            && s.CreateTime < to);

            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var permittedStations = _loggedInUser.UserLandmarksIds
                              .Intersect<Guid>(searchCriteria.StationIDs).ToList();
                predicate = predicate.And(s => permittedStations.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.AssignedStationID));
            }

            #endregion


            var data = this._workOrderListRep.GetQuery().Where(predicate)
                .GroupBy(a => a.ZoneName)
                .OrderByDescending(s => s.Count())
                .Take(10)
                .Select(s => new DashboardXYChartDTO
                {
                    Name = s.Key,
                    Count = s.Count()
                })
                .ToList();

            return DescriptiveResponse<List<DashboardXYChartDTO>>.Success(data);

        }

        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByStatus(DashboardSC searchCriteria)
        {
            //validation
            if (
                searchCriteria.DateFrom > searchCriteria.DateTo
                //|| searchCriteria.StationIDs == null || !searchCriteria.StationIDs.Any()
                || !_loggedInUser.UserServicesIds.Contains(searchCriteria.ServiceTypeID)
                )
            {
                return DescriptiveResponse<List<DashboardXYChartDTO>>.Error();
            }

            #region Predicate
            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day

            //var permittedStations = _loggedInUser.UserLandmarksIds
            //                  .Intersect<Guid>(searchCriteria.StationIDs).ToList();


            //predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);

            predicate = predicate.And(s => s.IsDeleted != true
                                            && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                            && s.ServiceTypeID == searchCriteria.ServiceTypeID
                                            //&& permittedStations.Contains(s.AssignedStationID)
                                            && s.CreateTime >= from
                                            && s.CreateTime < to);

            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var permittedStations = _loggedInUser.UserLandmarksIds
                              .Intersect<Guid>(searchCriteria.StationIDs).ToList();
                predicate = predicate.And(s => permittedStations.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.AssignedStationID));
            }

            #endregion

            var data = this._workOrderListRep.GetQuery().Where(predicate)
                .GroupBy(a => new { 
                    gStatus = LanguageIsEnglish ? a.LastStatusNameEn : a.LastStatusNameAr, 
                    gColor = a.StatusColor 
                })
                .Select(s => new DashboardXYChartDTO
                {
                    Name = s.Key.gStatus,
                    Count = s.Count(),
                    Color = s.Key.gColor
                })
                .ToList();

            return DescriptiveResponse<List<DashboardXYChartDTO>>.Success(data);

        }

        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByDate(DashboardSC searchCriteria)
        {
            //validation
            if (
                searchCriteria.DateFrom > searchCriteria.DateTo
                //|| searchCriteria.StationIDs == null || !searchCriteria.StationIDs.Any()
                || !_loggedInUser.UserServicesIds.Contains(searchCriteria.ServiceTypeID)
                )
            {
                return DescriptiveResponse<List<DashboardXYChartDTO>>.Error();
            }

            #region Predicate
            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day

            //var permittedStations = _loggedInUser.UserLandmarksIds
            //                  .Intersect<Guid>(searchCriteria.StationIDs).ToList();


            //predicate
            var predicate = PredicateBuilder.New<vw_NWC_WorkOrderList>(true);
            predicate = predicate.And(s => s.IsDeleted != true
                                            && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                            && s.ServiceTypeID == searchCriteria.ServiceTypeID
                                            //&& permittedStations.Contains(s.AssignedStationID)
                                            && s.CreateTime >= from
                                            && s.CreateTime < to);

            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var permittedStations = _loggedInUser.UserLandmarksIds
                              .Intersect<Guid>(searchCriteria.StationIDs).ToList();
                predicate = predicate.And(s => permittedStations.Any(a => a == s.AssignedStationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.AssignedStationID));
            }

            #endregion

            var data = this._workOrderListRep.GetQuery().Where(predicate)
                .GroupBy(a => DbFunctions.TruncateTime(a.CreateTime))
                .Select(s => new DashboardXYChartDTO
                {
                    Name = s.Key.ToString(),
                    Count = s.Count()
                })
                .ToList();

            return DescriptiveResponse<List<DashboardXYChartDTO>>.Success(data);

        }


        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByExecuteTime(DashboardSC searchCriteria)
        {
            //validation
            if (
                searchCriteria.DateFrom > searchCriteria.DateTo
                //|| searchCriteria.StationIDs == null || !searchCriteria.StationIDs.Any()
                || !_loggedInUser.UserServicesIds.Contains(searchCriteria.ServiceTypeID)
                )
            {
                return DescriptiveResponse<List<DashboardXYChartDTO>>.Error();
            }

            #region Predicate

            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day

            //var stations = _loggedInUser.UserLandmarksIds
            //                    .Intersect<Guid>(searchCriteria.StationIDs)
            //                    .Select(a => a.ToString());
            //string stationIds = string.Join(", ", stations);

            var stations = (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
                ? _loggedInUser.UserLandmarksIds.Intersect<Guid>(searchCriteria.StationIDs)
                : _loggedInUser.UserLandmarksIds;
            string stationIds = string.Join(", ", stations.Select(a => a.ToString()));

            #endregion

            var storedResult = this._sp_NWC_DashboardOrdersCountsViceExcuteTimeRepository
                .ExecWithStoredProcedure("sp_NWC_DashboardOrdersCountsViceExcuteTime @SubID, @StationIDs, @ServiceTypeID, @DateFrom, @DateTo",
                                            new SqlParameter("SubID", SqlDbType.UniqueIdentifier) { Value = this._loggedInUser.LoggedInUser.SubscriberId },
                                            new SqlParameter("StationIDs", SqlDbType.NVarChar) { Value = stationIds },
                                            new SqlParameter("ServiceTypeID", SqlDbType.Int) { Value = searchCriteria.ServiceTypeID },
                                            new SqlParameter("DateFrom", SqlDbType.DateTime) { Value = from },
                                            new SqlParameter("DateTo", SqlDbType.DateTime) { Value = to });


            var data = storedResult
                .Select(s => new DashboardXYChartDTO
                {
                    Name = s.Name,
                    Count = s.Count.HasValue ? s.Count.Value : 0
                })
                .ToList();

            return DescriptiveResponse<List<DashboardXYChartDTO>>.Success(data);

        }

        public DescriptiveResponse<SearchResult<ZonePriceListDTO>> GetAreasWithNoPrices(ZonePriceSCDTO searchCriteria)
        {

            #region Predicate

            var predicate = PredicateBuilder.New<vw_NWC_ZoneWithNoPrices>(true);

            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var permittedStations = _loggedInUser.UserLandmarksIds
                              .Intersect<Guid>(searchCriteria.StationIDs).ToList();
                predicate = predicate.And(s => permittedStations.Any(a => a == s.StationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.StationID));
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

            var zoneQuery = this._zoneWithNoPriceListRep.GetQuery()
                .Where(predicate)
                .OrderBy(s => s.Zone)
                .Skip(skip)
                .Take(take);

            #region response
            var result = new SearchResult<ZonePriceListDTO>();
            if (zoneQuery != null && zoneQuery.Any())
            {
                var count = this._zoneWithNoPriceListRep.GetQuery().Count(predicate);
                result.Result = zoneQuery.AsEnumerable().Select(a => a.WrapToZonePriceDTO()).ToList();
                result.TotalCount = count;
            }
            #endregion

            return DescriptiveResponse<SearchResult<ZonePriceListDTO>>.Success(result);
        }

        #endregion

        #region Commands
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
