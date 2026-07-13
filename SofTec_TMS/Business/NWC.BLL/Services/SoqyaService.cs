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
    public class SoqyaService : ISoqyaService
    {
        #region Properties
        private readonly IUnitofWork _unitofWork;
        private readonly ILoggedInUserService _loggedInUser;

        private readonly IRepository<NWC_SoqyaSchedules> _SoqyaSchedules;
        private readonly IRepository<vw_NWC_SoqyaSchedule> _SoqyaSchedulesView;
        private readonly IRepository<vw_NWC_Report_SoqyaSchedule> _ReportSoqyaSchedule;
        private readonly IRepository<NWC_CustomerAccount> _CustomerAccount;
        private readonly IRepository<NWC_Customer> _Customer;
        #endregion

        #region Constructors
        public SoqyaService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);

            this._unitofWork = new UnitofWork(ctx);
            this._SoqyaSchedules = new Repository<NWC_SoqyaSchedules>(ctx);
            this._SoqyaSchedulesView = new Repository<vw_NWC_SoqyaSchedule>(ctx);
            this._ReportSoqyaSchedule = new Repository<vw_NWC_Report_SoqyaSchedule>(ctx);
            this._CustomerAccount = new Repository<NWC_CustomerAccount>(ctx);
            this._Customer = new Repository<NWC_Customer>(ctx);
        }
        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<SoqyaScheduleDTO>> SearchSoqyaSchedules(SoqyaScheduleSC searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_SoqyaSchedule>(s => !s.IsDeleted);

            if (searchCriteria.Id != null && searchCriteria.Id > 0)
            {
                predicate = predicate.And(s => s.Id == searchCriteria.Id);
            }

            if (searchCriteria.CustomerAccountId.HasValue)
            {
                predicate = predicate.And(s => s.CustomerAccountId == searchCriteria.CustomerAccountId);
            }

            if (searchCriteria.MonthYear.HasValue)
            {
                var startDate = this.GetDate(searchCriteria.MonthYear.Value);
                var endDate = startDate.AddMonths(1);

                predicate = predicate.And(s => s.ScheduleDate >= startDate && s.ScheduleDate < endDate);
            }

            if (searchCriteria.StartDate.HasValue && searchCriteria.EndDate.HasValue)
            {
                predicate = predicate.And(s => s.ScheduleDate >= searchCriteria.StartDate.Value && s.ScheduleDate <= searchCriteria.EndDate.Value);
            }

            predicate = predicate.And(s => !s.IsGenerated.HasValue || !s.IsGenerated.Value);

            //if (searchCriteria.StationId.HasValue)
            //{
            //    predicate = predicate.And(s => s.StationId == searchCriteria.StationId);
            //}

            #endregion

            IQueryable<vw_NWC_SoqyaSchedule> DeferredWorkOrderList =
             this._SoqyaSchedulesView.GetQuery()
                 .Where(predicate)
                 .OrderByDescending(s => s.ScheduleDate);

            //if (!searchCriteria.ExcelFlage)
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
            var result = new SearchResult<SoqyaScheduleDTO>();
            if (DeferredWorkOrderList != null && DeferredWorkOrderList.Any())
            {
                var count = this._SoqyaSchedulesView.GetQuery().Count(predicate);
                result.Result = DeferredWorkOrderList.AsEnumerable().Select(a => a.WrapToSoqyaScheduleDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SoqyaBalanceDTO> GetBalanceAndUsed(long customerAccountId, int monthYear, long? excludedScheduleId = null, DateTime? _startDate = null)
        {
            try
            {
                var result = new SoqyaBalanceDTO();

                #region balance
                var existObject = this._CustomerAccount.GetQuery()
                          .FirstOrDefault(s => s.ID == customerAccountId && !s.IsDeleted && s.ServiceTypeId == 2); //soqya

                result.Balance = existObject != null && existObject.SoqyaBalance.HasValue  ? existObject.SoqyaBalance.Value * 12 : 0;
                #endregion

                #region used
                if (_startDate.HasValue || monthYear != 0)
                {
                    var startDate = _startDate.HasValue
                        ? _startDate.Value.AddDays(1 - _startDate.Value.Day)
                        : this.GetDate(monthYear);

                    var endDate = startDate.AddMonths(1);
                    var today = DateTimeHelper.GetDateTimeNow().Date;

                    result.UsedQuantity = this._SoqyaSchedulesView.GetQuery()
                        .Where(s => !s.IsDeleted
                                    && s.CustomerAccountId == customerAccountId
                                    && s.ScheduleDate >= startDate && s.ScheduleDate < endDate
                                    && (excludedScheduleId == null || s.Id != excludedScheduleId)

                                    //&& (s.LastStatusID == null || s.LastStatusID != 8 || s.LastStatusID != 3)
                                    && ((s.LastStatusID == null && s.ScheduleDate > today)
                                         || (s.LastStatusID != null && s.LastStatusID != 8)
                                         || (s.LastStatusID != null && s.LastStatusID != 3)
                                         )
                                    )
                        .Select(a => a.Quantity).DefaultIfEmpty(0)
                        .Sum();
                }
                #endregion

                return DescriptiveResponse<SoqyaBalanceDTO>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "SoqyaService => GetBalanceAndUsed: "));
                return DescriptiveResponse<SoqyaBalanceDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<SoqyaScheduleReportDTO>> GetSoqyaSchedulesReport(SoqyaScheduleReportSC searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_Report_SoqyaSchedule>(true);

            if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CityIDs.Contains(s.CityID));
            }

            if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.ZoneIDs.Contains(s.ZoneID));
            }

            //if (searchCriteria.MonthYearList != null && searchCriteria.MonthYearList.Any())
            //{
            //    var searchList = searchCriteria.MonthYearList.Select(s => s.ToString());
            //    predicate = predicate.And(s => searchList.Contains(s.YearMonth));
            //}

            if (searchCriteria.CustomerId.HasValue)
            {
                predicate = predicate.And(s => s.CustomerId == searchCriteria.CustomerId);
            }

            if (searchCriteria.CustomerAccountIDs != null && searchCriteria.CustomerAccountIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.CustomerAccountIDs.Contains(s.CustomerAccountId));
            }

            if (searchCriteria.ScheduleStatus != null && searchCriteria.ScheduleStatus.Any())
            {

                    if(searchCriteria.ScheduleStatus.Contains(SoqyaScheduleReportSC.ScheduleStatusEnum.FullScheduled))
                        predicate = predicate.And(s => s.ScheduledSum >= s.SoqyaBalance);
                        
                    if(searchCriteria.ScheduleStatus.Contains(SoqyaScheduleReportSC.ScheduleStatusEnum.PartiallyScheduled))
                        predicate = predicate.And(s => s.ScheduledSum < s.SoqyaBalance
                                                        && s.ScheduledSum > 0);

                    if(searchCriteria.ScheduleStatus.Contains(SoqyaScheduleReportSC.ScheduleStatusEnum.NotScheduled))
                        predicate = predicate.And(s => s.ScheduledSum == 0);
            }


            if (searchCriteria.DateFrom.HasValue)
            {
                var startYear = searchCriteria.DateFrom.Value.Year;
                var startMonth = searchCriteria.DateFrom.Value.Month;
                predicate = predicate.And(s => s.Year >= startYear && s.Month >= startMonth);
            }

            if (searchCriteria.DateTo.HasValue)
            {
                var endYear = searchCriteria.DateTo.Value.Year;
                var endMonth = searchCriteria.DateTo.Value.Month;
                predicate = predicate.And(s => s.Year <= endYear && s.Month <= endMonth);
            }

            if (searchCriteria.B_Operator > 0)
            {
                switch (searchCriteria.B_Operator)
                {
                    case Operator.Equal:
                        predicate = predicate.And(s => searchCriteria.Balance == s.SoqyaBalance);
                        break;

                    case Operator.LessThan:
                        predicate = predicate.And(s => s.SoqyaBalance < searchCriteria.Balance);
                        break;

                    case Operator.MoreThan:
                        predicate = predicate.And(s => s.SoqyaBalance > searchCriteria.Balance);
                        break;
                }
            }

            if (searchCriteria.NotScheduled_Operator > 0)
            {
                switch (searchCriteria.NotScheduled_Operator)
                {
                    case Operator.Equal:
                        predicate = predicate.And(s => searchCriteria.NotScheduledQty == s.ScheduledSum);
                        break;

                    case Operator.LessThan:
                        predicate = predicate.And(s => s.ScheduledSum < searchCriteria.NotScheduledQty);
                        break;

                    case Operator.MoreThan:
                        predicate = predicate.And(s => s.ScheduledSum > searchCriteria.NotScheduledQty);
                        break;
                }
            }

            #endregion

            IQueryable<vw_NWC_Report_SoqyaSchedule> queryableResult =
             this._ReportSoqyaSchedule.GetQuery()
                 .Where(predicate)
                 .OrderBy(s => s.CustomerName).ThenBy(s => s.Year).ThenBy(s => s.Month);

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

                queryableResult = queryableResult
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<SoqyaScheduleReportDTO>();
            if (queryableResult != null && queryableResult.Any())
            {
                var count = this._ReportSoqyaSchedule.GetQuery().Count(predicate);
                result.Result = queryableResult.AsEnumerable().Select(a => a.WrapToSoqyaScheduleReportDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<SoqyaScheduleReportDTO>>.Success(result);
            #endregion
        }
        #endregion

        #region Command
        public DescriptiveResponse<bool> AddSoqyeScheduleRecord(SoqyaScheduleDTO dto)
        {
            dto.ScheculeDayListAdd = dto.ScheculeDayListAdd.Distinct().ToList();
            dto.MonthYearAddIds = dto.MonthYearAddIds.Distinct().ToList();

            #region Validations
            var validator = new SoyaScheduleValidator(ValidationMode.Create, this._SoqyaSchedules, this);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<bool>.Error(failures);
            }
            #endregion

            var schedule = HelperWrapToSoqyaSchedule(dto);   // dto.WrapToSoqyaSchedule();

            #region prepare model
            //schedule.IsDeleted = false;
            //schedule.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
            //schedule.CreatedDate = DateTimeHelper.GetDateTimeNow();
            #endregion

            using (_unitofWork)
            {
                this._SoqyaSchedules.AddRange(schedule);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<bool> EditSoqyeScheduleRecord(SoqyaScheduleDTO dto)
        {
            #region Validations
            var validator = new SoyaScheduleValidator(ValidationMode.Update, this._SoqyaSchedules, this);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<bool>.Error(failures);
            }
            #endregion

            var todayDate = DateTimeHelper.GetDateTimeNow().Date;

            var existObject = this._SoqyaSchedules.GetQuery()
                .FirstOrDefault(s => s.ID == dto.Id
                                 && !s.IsDeleted
                                 && s.WorkOrderId == null
                                 && s.ScheduleDate > todayDate);


            #region prepare model
            existObject.ScheduleDate = dto.ScheduleDate;
            existObject.Quantity = dto.Quantity;
            existObject.TimeSlotFrom = !string.IsNullOrEmpty(dto.TimeSlotFrom) ? TimeSpan.Parse(dto.TimeSlotFrom) : TimeSpan.Zero;
            existObject.TimeSlotTo = !string.IsNullOrEmpty(dto.TimeSlotTo) ? TimeSpan.Parse(dto.TimeSlotTo) : TimeSpan.Zero;


            existObject.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
            existObject.UpdatedDate = DateTimeHelper.GetDateTimeNow();
            #endregion

            using (_unitofWork)
            {
                this._SoqyaSchedules.Update(existObject);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<bool> DeleteSoqyeScheduleRecord(long scheduleId)
        {
            #region Validations
            //var validator = new DeviceMeterReadingValidator(ValidationMode.Create, this._loggedInUser, _readingRepository);
            //var results = validator.Validate(dto);
            //if (!results.IsValid)
            //{
            //    var failures = results.Errors.Select(s => s.ErrorMessage);
            //    return DescriptiveResponse<long?>.Error(failures);
            //}
            #endregion

            var todayDate = DateTimeHelper.GetDateTimeNow().Date;

            var existObject = this._SoqyaSchedules.GetQuery()
                .FirstOrDefault(s => s.ID == scheduleId
                                 && !s.IsDeleted
                                 && s.WorkOrderId == null
                                 && s.ScheduleDate > todayDate);

            using (_unitofWork)
            {

                #region prepare model
                existObject.IsDeleted = true;
                existObject.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                existObject.UpdatedDate = DateTimeHelper.GetDateTimeNow();
                #endregion


                this._SoqyaSchedules.Update(existObject);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<List<SoqyaScheduleDTO>> AddRange(List<SoqyaScheduleDTO> SoqyaScheduleDTO)
        {
            #region fetch Ids
            foreach (var dto in SoqyaScheduleDTO)
            {
                var customerID = this._Customer.GetQuery().Where(x => x.IDNumber == dto.CustomerIdNumber)?.Select(c => c.ID).FirstOrDefault();
                if (customerID != null)
                {
                    var customerAccountId = this._CustomerAccount.GetQuery().Where(x =>
                    x.CustomerId == customerID &&
                    x.AccountId_Integration == dto.AccountId &&
                    x.ServiceTypeId == 2)?.Select(c => c.ID).FirstOrDefault();
                    if (customerAccountId != null && customerAccountId > 0)
                    {
                        dto.CustomerAccountId = customerAccountId.Value;
                    }
                }
            }
            #endregion

            var failedList = new List<SoqyaScheduleDTO>();
            var addList = new List<NWC_SoqyaSchedules>();

            //var index = 1;
            foreach (var dto in SoqyaScheduleDTO)
            {
                #region Validations
                var validator = new SoyaScheduleValidator(ValidationMode.CreateExcel, this._SoqyaSchedules, this, SoqyaScheduleDTO);
                var results = validator.Validate(dto);
                #endregion

                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s =>
                        Regex.Replace(s.ErrorMessage, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1"));
                    dto.ExcelValidation = string.Join(", ", failures);
                    //dto.ExcelSheetRowId = index;
                    failedList.Add(dto);
                }
                else
                {
                    var schedule = HelperWrapToSoqyaScheduleExcel(dto);
                    addList.Add(schedule);
                }

            }

            using (_unitofWork)
            {
                this._SoqyaSchedules.AddRange(addList);
            }

            return DescriptiveResponse<List<SoqyaScheduleDTO>>.Success(failedList);
        }

        public DescriptiveResponse<bool> UpdateGeneratedSoqyaSchedule(List<SoqyaScheduleDTO> dtos)
        {
            try
            {
                using (_unitofWork)
                {
                    foreach (var dto in dtos)
                    {
                        var schedule = this._SoqyaSchedules.FindById(dto.Id);

                        schedule.IsGenerated = dto.IsGenerated;
                        schedule.GeneratedTime = dto.GeneratedTime;

                        this._SoqyaSchedules.Update(schedule);
                    }
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "SoqyaService => UpdateGeneratedSoqyaSchedule: "));
                return DescriptiveResponse<bool>.Error(ex.Message);
            }
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

        private List<NWC_SoqyaSchedules> HelperWrapToSoqyaSchedule(SoqyaScheduleDTO input)
        {
            if (input == null
                || input.MonthYearAddIds == null || !input.MonthYearAddIds.Any()
                || input.ScheculeDayListAdd == null || !input.ScheculeDayListAdd.Any()) return null;

            #region parse time
            //int hour = 0;
            //int minute = 0;

            //var timeArr = input.TimeSlotFrom.Split(':');
            //int.TryParse(timeArr[0], out hour);
            //int.TryParse(timeArr[1], out minute);

            TimeSpan timeFrom = TimeSpan.Zero;
            TimeSpan timeTo = TimeSpan.Zero;

            if (!string.IsNullOrEmpty(input.TimeSlotFrom))
            {
                timeFrom = TimeSpan.Parse(input.TimeSlotFrom);
            }
            if (!string.IsNullOrEmpty(input.TimeSlotTo))
            {
                timeTo = TimeSpan.Parse(input.TimeSlotTo);
            }
            #endregion

            var to = new List<NWC_SoqyaSchedules>();

            foreach (var yearMonthId in input.MonthYearAddIds)
            {
                //var balanceObj = this._SoqyaCustomerAccountBalance.GetQuery().FirstOrDefault(s => s.ID == balanceId);

                //if (balanceObj != null)
                {
                    #region year month
                    int year = (int)(yearMonthId / 100);
                    int month = (int)(yearMonthId - (year * 100));
                    #endregion

                    foreach (var day in input.ScheculeDayListAdd)
                    {
                        var newSchedule = new NWC_SoqyaSchedules
                        {
                            CustomerAccountId = input.CustomerAccountId,
                            ScheduleDate = new DateTime(year, month, day, timeFrom.Hours, timeFrom.Minutes, 0),
                            Quantity = input.Quantity,
                            TimeSlotFrom = timeFrom,
                            TimeSlotTo = timeTo,

                            IsDeleted = false,
                            CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                            CreatedDate = DateTimeHelper.GetDateTimeNow()
                        };

                        to.Add(newSchedule);
                    }

                }

            }

            return to;
        }

        private NWC_SoqyaSchedules HelperWrapToSoqyaScheduleExcel(SoqyaScheduleDTO dto)
        {
            if (dto == null) return null;

            #region parse time

            TimeSpan timeFrom = TimeSpan.Zero;
            TimeSpan timeTo = TimeSpan.Zero;

            if (!string.IsNullOrEmpty(dto.TimeSlotFrom))
            {
                timeFrom = TimeSpan.Parse(dto.TimeSlotFrom);
            }
            if (!string.IsNullOrEmpty(dto.TimeSlotTo))
            {
                timeTo = TimeSpan.Parse(dto.TimeSlotTo);
            }
            #endregion

            var to = new List<NWC_SoqyaSchedules>();
            var newSchedule = new NWC_SoqyaSchedules
            {
                CustomerAccountId = dto.CustomerAccountId,
                ScheduleDate = dto.ScheduleDate,
                Quantity = dto.Quantity,
                TimeSlotFrom = timeFrom,
                TimeSlotTo = timeTo,
                IsDeleted = false,
                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                CreatedDate = DateTimeHelper.GetDateTimeNow(),
                IsGenerated = dto.IsGenerated,
                GeneratedTime = dto.GeneratedTime
            };

            return newSchedule;

        }

        private DateTime GetDate(int MonthYear, int day = 1, int hour = 0, int minute = 0)
        {
            var year = MonthYear / 100;
            var month = MonthYear - (year * 100);

            return new DateTime(year, month, day, hour, minute, 0);
        }
        #endregion
    }
}
