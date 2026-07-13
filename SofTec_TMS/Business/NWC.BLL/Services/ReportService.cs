using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace NWC.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<vw_NWC_Report_OrdersPerZone> _OrdersPerZone;
        private readonly IRepository<vw_NWC_Report_StationOrderCapacitySummary> _StationOrderCapacitySummary;
        private readonly IRepository<vw_NWC_Report_StationServiceTimeSummary> _StationServiceTimeSummary;
        private readonly IRepository<vw_NWC_Report_TankersPermissionsStatus> _TankersPermissionsStatus;
        private readonly IRepository<vw_NWC_Report_SoqyaScheduledQuantities> _SoqyaScheduledQuantities;
        private readonly IRepository<vw_NWC_ContractTariff> _tariffListRepository;

        public ReportService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._OrdersPerZone = new Repository<vw_NWC_Report_OrdersPerZone>(ctx);
            this._StationOrderCapacitySummary = new Repository<vw_NWC_Report_StationOrderCapacitySummary>(ctx);
            this._StationServiceTimeSummary = new Repository<vw_NWC_Report_StationServiceTimeSummary>(ctx);
            this._TankersPermissionsStatus = new Repository<vw_NWC_Report_TankersPermissionsStatus>(ctx);
            this._SoqyaScheduledQuantities = new Repository<vw_NWC_Report_SoqyaScheduledQuantities>(ctx);
            this._tariffListRepository = new Repository<vw_NWC_ContractTariff>(ctx);
        }

        public DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetOrdersPerZone(ReportSC searchCriteria)
        {
            if (searchCriteria.DateFrom > searchCriteria.DateTo)
            {
                return DescriptiveResponse<SearchResult<Report_OrderPerZone>>.Error();
            }

            #region Predicate

            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day


            var predicate = PredicateBuilder.New<vw_NWC_Report_OrdersPerZone>(s =>
                            s.ISDeleted != true
                            && s.SubId == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.ScheduledDeliveryTime >= from
                            && s.ScheduledDeliveryTime < to);

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
                predicate = predicate.And(s => searchList.Any(a => a == s.StationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.StationID));
            }

            #endregion

            var myQuerable = this._OrdersPerZone.GetQuery()
                    .Where(predicate)
                    .GroupBy(k => new { k.CityName, k.ZoneName, k.StationName })
                    .AsQueryable();

            var workOrderList = myQuerable
                    .OrderBy(o => o.Key.CityName)
                    .AsQueryable();
             
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

            var result = new SearchResult<Report_OrderPerZone>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = myQuerable.Count();
                result.Result = workOrderList
                    .Select(g => new Report_OrderPerZone
                    {
                        CityName = g.Key.CityName,
                        ZoneName = g.Key.ZoneName,
                        StationName = g.Key.StationName,
                        TotalNoOfOrders = g.Count(),
                        TotalQuantity = 0//g.Sum(s => s.OrderQuantity) TODO: not completed
                    }).ToList();
                result.TotalCount = count;
            };

            return DescriptiveResponse<SearchResult<Report_OrderPerZone>>.Success(result);
        }

        public DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetStationOrderCapacity(ReportSC searchCriteria)
        {
            if (searchCriteria.DateFrom > searchCriteria.DateTo)
            {
                return DescriptiveResponse<SearchResult<Report_OrderPerZone>>.Error();
            }

            #region Predicate

            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day


            var predicate = PredicateBuilder.New<vw_NWC_Report_StationOrderCapacitySummary>(s =>
                            s.IsDeleted != true
                            && s.SubId == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.ScheduledDeliveryTime >= from
                            && s.ScheduledDeliveryTime < to);

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

            var myQuerable = this._StationOrderCapacitySummary.GetQuery()
                    .Where(predicate)
                    .GroupBy(k => new { k.City, k.StationName })
                    .AsQueryable();

            var myQuerableList = myQuerable
                    .OrderBy(o => o.Key.City)
                    .AsQueryable();

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

                myQuerableList = myQuerableList
                    .Skip(skip)
                    .Take(take);
            }

            var result = new SearchResult<Report_OrderPerZone>();
            if (myQuerableList != null && myQuerableList.Any())
            {
                var count = myQuerable.Count();
                result.Result = myQuerableList
                    .Select(g => new Report_OrderPerZone
                    {
                        CityName = g.Key.City,
                        StationName = g.Key.StationName,
                        TotalNoOfOrders = g.Count(),
                        TotalQuantity = g.Sum(s => s.OrderQuantity)
                    }).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<Report_OrderPerZone>>.Success(result);
        }

        public DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetStationServiceTime(ReportSC searchCriteria)
        {
            if (searchCriteria.DateFrom > searchCriteria.DateTo)
            {
                return DescriptiveResponse<SearchResult<Report_OrderPerZone>>.Error();
            }

            #region Predicate

            var from = searchCriteria.DateFrom.Date; //from the start of the day
            var to = searchCriteria.DateTo.Date.AddDays(1); //to the start of the next day


            var predicate = PredicateBuilder.New<vw_NWC_Report_StationServiceTimeSummary>(s =>
                            s.IsDeleted != true
                            && s.SubId == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.ScheduledDeliveryTime >= from
                            && s.ScheduledDeliveryTime < to
                            //&& s.DeliveryTotalTime != null
                            //&& s.OrderServiceTotalTime != null
                            && s.CreateToDeliveredTotalTime >= 0
                            && s.CreateToOutTotalTime >= 0
                            && s.OutToDeliveredTotalTime >= 0
                            );

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

            var myQuerable = this._StationServiceTimeSummary.GetQuery()
                    .Where(predicate)
                    .GroupBy(k => new { k.City, k.StationName })
                    .AsQueryable();

            var workOrderList = myQuerable
                    .OrderBy(o => o.Key.City)
                    .AsQueryable();

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

            var result = new SearchResult<Report_OrderPerZone>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = myQuerable.Count();
                result.Result = workOrderList
                    .Select(g => new Report_OrderPerZone
                    {
                        CityName = g.Key.City,
                        StationName = g.Key.StationName,
                        TotalNoOfOrders = g.Count(),
                        AvgCreateToDeliveredTime = g.Average(a => a.CreateToDeliveredTotalTime),
                        AvgCreateToOutTime = g.Average(a => a.CreateToOutTotalTime),
                        AvgOutToDeliveredTime = g.Average(a => a.OutToDeliveredTotalTime),
                        TotalQuantity = g.Sum(s => s.OrderQuantity)
                    }).ToList();
                result.TotalCount = count;

                foreach (var item in result.Result)
                {
                    if (item.AvgCreateToDeliveredTime.HasValue)
                    {
                        item.AvgCreateToDeliveredTime = Math.Floor(item.AvgCreateToDeliveredTime.Value);
                    }
                    if (item.AvgCreateToOutTime.HasValue)
                    {
                        item.AvgCreateToOutTime = Math.Floor(item.AvgCreateToOutTime.Value);
                    }
                    if (item.AvgOutToDeliveredTime.HasValue)
                    {
                        item.AvgOutToDeliveredTime = Math.Floor(item.AvgOutToDeliveredTime.Value);
                    }
                }

            }

            return DescriptiveResponse<SearchResult<Report_OrderPerZone>>.Success(result);
        }

        public DescriptiveResponse<SearchResult<Report_TankersPermissionsStatus>> GetTankerPermissionStatus(ReportTankersPermissionsStatusSC searchCriteria)
        {
            #region Predicate
            var todayDate = DateTime.Now.Date;

            var predicate = PredicateBuilder.New<vw_NWC_Report_TankersPermissionsStatus>(s =>
                            s.IsDeleted != true
                            && s.SubId == this._loggedInUser.LoggedInUser.SubscriberId
                            );

            if (!string.IsNullOrEmpty(searchCriteria.Tanker))
            {
                var searchText = searchCriteria.Tanker.Trim();
                predicate = predicate.And(s => s.Code.Contains(searchText) || s.PlateNo.Contains(searchText));
            }

            if (searchCriteria.PermissionStatus.HasValue)
            {
                switch (searchCriteria.PermissionStatus.Value)
                {
                    case ReportTankersPermissionsStatusSC.ExpiryStatusEnum.Valid:
                        predicate = predicate.And(s => s.PermissionExpiryDate >= todayDate);
                        break;
                    case ReportTankersPermissionsStatusSC.ExpiryStatusEnum.Expiry:
                        predicate = predicate.And(s => s.PermissionExpiryDate < todayDate);
                        break;
                    default:
                        break;
                }
            }

            if (searchCriteria.LicenseStatus.HasValue)
            {
                switch (searchCriteria.LicenseStatus.Value)
                {
                    case ReportTankersPermissionsStatusSC.ExpiryStatusEnum.Valid:
                        predicate = predicate.And(s => s.licenseExpiryDate >= todayDate);
                        break;
                    case ReportTankersPermissionsStatusSC.ExpiryStatusEnum.Expiry:
                        predicate = predicate.And(s => s.licenseExpiryDate < todayDate);
                        break;
                    default:
                        break;
                }
            }

            #region Lists
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

            #region Date

            if (searchCriteria.LicenseExpiryDateFrom.HasValue)
            {
                var from = searchCriteria.LicenseExpiryDateFrom.Value.Date;
                predicate = predicate.And(s => s.licenseExpiryDate >= from);
            }

            if (searchCriteria.LicenseExpiryDateTo.HasValue)
            {
                var to = searchCriteria.LicenseExpiryDateTo.Value.Date.AddDays(1);
                predicate = predicate.And(s => s.licenseExpiryDate < to);
            }

            if (searchCriteria.PermissionExpiryDateFrom.HasValue)
            {
                var from = searchCriteria.PermissionExpiryDateFrom.Value.Date;
                predicate = predicate.And(s => s.PermissionExpiryDate >= from);
            }

            if (searchCriteria.PermissionExpiryDateTo.HasValue)
            {
                var to = searchCriteria.PermissionExpiryDateTo.Value.Date.AddDays(1);
                predicate = predicate.And(s => s.PermissionExpiryDate < to);
            }

            #endregion

            #endregion

            var workOrderList = this._TankersPermissionsStatus.GetQuery()
                    .Where(predicate)
                    //.GroupBy(k => new { k.City, k.StationName })
                    .OrderBy(o => o.Code)
                    .AsQueryable();

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

            var result = new SearchResult<Report_TankersPermissionsStatus>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._TankersPermissionsStatus.GetQuery().Count(predicate);
                result.Result = workOrderList
                    .AsEnumerable()
                    .Select(g => g.WrapReport_OrderPerZone()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<Report_TankersPermissionsStatus>>.Success(result);
        }


        public DescriptiveResponse<IEnumerable<Report_SoqyaScheduledPerDay>> GetSoqyaSchedulePerDay(ReportScheduledPerDaySC searchCriteria)
        {
            if (!searchCriteria.SelectedDate.HasValue
                || searchCriteria.CityIDs == null || !searchCriteria.CityIDs.Any()
                || searchCriteria.StationIDs == null || !searchCriteria.StationIDs.Any()
                )
            {
                return DescriptiveResponse<IEnumerable<Report_SoqyaScheduledPerDay>>.Error();
            }

            #region Predicate

            var from = new DateTime(searchCriteria.SelectedDate.Value.Year, searchCriteria.SelectedDate.Value.Month, 1);
            var to = from.AddMonths(1);

            var predicate = PredicateBuilder.New<vw_NWC_Report_SoqyaScheduledQuantities>(s =>
                            s.IsDeleted != true
                            && s.StationIsMain
                            && s.ScheduleDate >= from
                            && s.ScheduleDate < to);

            //permitted cities
            if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            {
                var searchId = _loggedInUser.SubBranches.Intersect(searchCriteria.CityIDs).FirstOrDefault();
                predicate = predicate.And(s => s.CityID == searchId);
            }

            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchId = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs).FirstOrDefault();
                predicate = predicate.And(s => s.StationID == searchId);
            }

            #endregion

            var myQuerable = this._SoqyaScheduledQuantities.GetQuery()
                    .Where(predicate)
                    .GroupBy(k => new { dayOfMonth = DbFunctions.TruncateTime(k.ScheduleDate), k.Quantity })
                    .Select(g => new
                    {
                        name = g.Select(s => s.name).FirstOrDefault(),
                        g.Key.dayOfMonth,
                        g.Key.Quantity,
                        schedulesCount = g.Count(),
                        TotalQuantity = g.Sum(a => a.Quantity)
                    });

            var result = new List<Report_SoqyaScheduledPerDay>();
            if (myQuerable != null && myQuerable.Any())
            {
                var list = myQuerable.ToList();

                var daysOfMonth = list.Select(s => s.dayOfMonth).Distinct().OrderBy(o => o);
                var quantities = list.Select(s => s.Quantity).Distinct().OrderBy(o => o);

                foreach (var day in daysOfMonth)
                {
                    var newRecord = new Report_SoqyaScheduledPerDay
                    {
                        StationName = list.Select(x => x.name).FirstOrDefault(),
                        DayOfMonth = day,
                        TotalCounts = list.Where(w => w.dayOfMonth == day).Sum(s => s.schedulesCount),
                        SumQuantities = list.Where(w => w.dayOfMonth == day).Sum(s => s.TotalQuantity),
                        CapacityList = new List<Report_SoqyaScheduledPerDay.CapacitySum>()
                    };

                    foreach (var myQuantity in quantities)
                    {
                        var newCap = new Report_SoqyaScheduledPerDay.CapacitySum
                        {
                            Quantity = myQuantity,
                            SchedulesCount = list.Where(s => s.dayOfMonth == day && s.Quantity == myQuantity)
                                                .Select(a => a.schedulesCount)
                                                .FirstOrDefault()
                        };
                        newRecord.CapacityList.Add(newCap);
                    }
                    result.Add(newRecord);
                }

            }

            return DescriptiveResponse<IEnumerable<Report_SoqyaScheduledPerDay>>.Success(result);
        }

        public DescriptiveResponse<SearchResult<ContractTariffDTO>> ContractTariffReport(ContractTariffSc searchCriteria)
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

            #region Predicate

            var predicate = PredicateBuilder.New<vw_NWC_ContractTariff>(true);
            predicate = predicate.And(s => s.IsDeleted != true);
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));
            }
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.StationID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.StationID));
            }
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
     

            if (searchCriteria.CustomerLocationClassIDs != null && searchCriteria.CustomerLocationClassIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CustomerLocationClassIDs.Contains(s.CustomerLocationClassID));
            }

            if (searchCriteria.TankerCapacities != null && searchCriteria.TankerCapacities.Any())
            {
                predicate = predicate.And(s => searchCriteria.TankerCapacities.Contains((int)s.TanckerCapacityId));
            }

            if (searchCriteria.TarrifStatus != null && searchCriteria.TarrifStatus !=(int)TarifStatus.all)
            {
                var DateNow = DateTimeHelper.ConvertDateToHijriAsLong((DateTime)DateTime.Now);
                switch (searchCriteria.TarrifStatus)
                {
                    case (int)TarifStatus.valid:
                        predicate.And(s => s.DateFromHijri <= DateNow && s.DateToHijri >= DateNow);
                        break;
                    case (int)TarifStatus.notValid:
                        predicate.And(s =>  (s.DateFromHijri > DateNow || s.DateToHijri < DateNow));
                        break;
                    default:
                        break;
                }
            }

            var contractTariffQuery = this._tariffListRepository.GetQuery()
                .Where(predicate)
                .OrderBy(s => s.StationName).ThenBy(s => s.DateFrom)
                .Skip(skip)
                .Take(take);
            #endregion
            #region response
            var result = new SearchResult<ContractTariffDTO>();
            if (contractTariffQuery != null && contractTariffQuery.Any())
            {
                var count = this._tariffListRepository.GetQuery().Count(predicate);
                result.Result = contractTariffQuery.AsEnumerable().Select(a => a.WrapToTariffDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<ContractTariffDTO>>.Success(result);
            #endregion
        }

        enum TarifStatus
        {
            all = 1,
            valid =2,
            notValid=3
        }
    }
}
