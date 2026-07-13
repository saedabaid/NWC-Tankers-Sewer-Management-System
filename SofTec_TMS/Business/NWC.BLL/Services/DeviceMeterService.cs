using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using System;
using System.Data.Entity;
using System.Linq;

namespace NWC.BLL.Services
{
    public class DeviceMeterService : IDeviceMeterService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<NWC_DeviceMeter> _deviceMeterRepository;
        //private readonly IRepository<vw_NWC_DeviceMeter> _deviceMeterListRepository;
        private readonly IRepository<NWC_DeviceMeterReading> _readingRepository;
        private readonly IRepository<vw_NWC_DeviceMeterReading> _readingListRepository;
        #endregion

        #region Constructors
        public DeviceMeterService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);
            this._deviceMeterRepository = new Repository<NWC_DeviceMeter>(ctx);
            //this._deviceMeterListRepository = new Repository<vw_NWC_DeviceMeter>(ctx); //TODO : 
            this._readingRepository = new Repository<NWC_DeviceMeterReading>(ctx);
            this._readingListRepository = new Repository<vw_NWC_DeviceMeterReading>(ctx);
        }
        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<DeviceMeterDTO>> SearchDeviceMeter(DeviceMeterSC searchCriteria)
        {
            //TODO ; not completed

            //#region Predicate
            //var predicate = PredicateBuilder.New<vw_NWC_DeviceMeter>(true);

            //predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);
            //predicate = predicate.And(s => this._loggedInUser.UserLandmarksIds.Contains(s.StationID));

            //if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            //{
            //    var word = searchCriteria.FilterModel.SearchKeyword.Trim();
            //    predicate = predicate.And(s => s.StationCode.Contains(word) || s.StationName.Contains(word));
            //}

            //#region Advanced search
            //if (!string.IsNullOrEmpty(searchCriteria.ConnectorTubeNumber))
            //{
            //    var word = searchCriteria.ConnectorTubeNumber.Trim();
            //    predicate = predicate.And(s => s.ConnectorTubeNumber.Contains(word));
            //}
            //if (!string.IsNullOrEmpty(searchCriteria.MeterSerialNumber))
            //{
            //    var word = searchCriteria.MeterSerialNumber.Trim();
            //    predicate = predicate.And(s => s.MeterSerialNumber.Contains(word));
            //}
            //if (!string.IsNullOrEmpty(searchCriteria.ManholeNumber))
            //{
            //    var word = searchCriteria.ManholeNumber.Trim();
            //    predicate = predicate.And(s => s.ManholeNumber.Contains(word));
            //}
            //if (searchCriteria.ServiceTypeID.HasValue)
            //{
            //    predicate = predicate.And(s => s.ServiceTypeID == searchCriteria.ServiceTypeID);
            //}
            //if (searchCriteria.StationID.HasValue && searchCriteria.StationID.Value != Guid.Empty)
            //{
            //    predicate = predicate.And(s => s.StationID == searchCriteria.StationID.Value);
            //}
            //#endregion
            //#endregion

            //#region skip & take
            //var skip = 0;
            //var take = 20;
            //if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
            //{
            //    skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
            //    take = searchCriteria.FilterModel.PageFilter.PageSize;
            //}
            //#endregion

            //var contractorQuery = this._deviceMeterListRepository.GetQuery()
            //   .Where(predicate)
            //   .OrderBy(s => s.StationName)
            //   .Skip(skip)
            //   .Take(take);

            //#region response
            //var result = new SearchResult<DeviceMeterDTO>();
            //if (contractorQuery != null && contractorQuery.Any())
            //{
            //    var count = this._deviceMeterListRepository.GetQuery().Count(predicate);
            //    result.Result = contractorQuery.AsEnumerable().Select(a => a.WrapToDeviceMeterDTO()).ToList();
            //    result.TotalCount = count;
            //}

            //return DescriptiveResponse<SearchResult<DeviceMeterDTO>>.Success(result);
            //#endregion

            return null;
        }

        public DescriptiveResponse<SearchResult<MeterReadingDTO>> SearchReadingList(MeterReadingSC searchCriteria)
        {

            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_DeviceMeterReading>(true);
            predicate = predicate.And(s => s.IsDeleted == false);
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);
            predicate = predicate.And(s => this._loggedInUser.UserLandmarksIds.Contains(s.StationID));

            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.StationCode.Contains(word) || s.StationName.Contains(word));
            }

            if (searchCriteria.Id.HasValue)
            {
                predicate = predicate.And(s => s.ID == searchCriteria.Id.Value);
            }


            #region Advanced search
            if (searchCriteria.DeviceMeterIDs != null && searchCriteria.DeviceMeterIDs.Any())
            {
                predicate = predicate.And(s => searchCriteria.DeviceMeterIDs.Any(a => a == s.DeviceMeterID));
            }

            if (searchCriteria.DateTimeFrom != null)
            {
                predicate = predicate.And(s => s.ReadingTime >= searchCriteria.DateTimeFrom);

            }
            if (searchCriteria.DateTimeTo != null)
            {
                predicate = predicate.And(s => s.ReadingTime <= searchCriteria.DateTimeTo);
            }
            #endregion

            #endregion

            #region skip & take
            var skip = 0;
            var take = 20;
            if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
            {
                skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                take = searchCriteria.FilterModel.PageFilter.PageSize;
            }
            #endregion

            var contractorQuery = this._readingListRepository.GetQuery()
               .Where(predicate)
               .OrderBy(s => s.StationName)
               .Skip(skip)
               .Take(take);

            #region response
            var result = new SearchResult<MeterReadingDTO>();
            if (contractorQuery != null && contractorQuery.Any())
            {
                var count = this._readingListRepository.GetQuery().Count(predicate);
                result.Result = contractorQuery.AsEnumerable().Select(a => a.WrapToReadingDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<MeterReadingDTO>>.Success(result);
            #endregion

        }

        #endregion

        #region Command
        public DescriptiveResponse<long?> AddDeviceMeter(DeviceMeterDTO dto)
        {
            #region Validations
            var validator = new DeviceMeterValidator(ValidationMode.Create, this._loggedInUser, _deviceMeterRepository);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<long?>.Error(failures);
            }
            #endregion

            var device = dto.WrapToDeviceMeter();

            #region prepare model
            device.SubID = _loggedInUser.LoggedInUser.SubscriberId;
            #endregion

            using (_unitofWork)
            {
                this._deviceMeterRepository.Add(device);
            }

            return DescriptiveResponse<long?>.Success(device.ID);
        }

        public DescriptiveResponse<long?> AddReading(MeterReadingDTO dto)
        {
            #region Validations
            var validator = new DeviceMeterReadingValidator(ValidationMode.Create, this._loggedInUser, _readingRepository);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<long?>.Error(failures);
            }
            #endregion

            var reading = dto.WrapToReading();

            #region prepare model
            reading.IsDeleted = false;
            reading.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
            reading.CreatedDate = DateTimeHelper.GetDateTimeNow();
            //reading.SubID = _loggedInUser.LoggedInUser.SubscriberId;
            #endregion

            using (_unitofWork)
            {
                this._readingRepository.Add(reading);
            }

            return DescriptiveResponse<long?>.Success(reading.ID);
        }

        public DescriptiveResponse<bool> DeleteReading(long readingId)
        {
            var existContract = this._readingRepository.GetQuery()
                .FirstOrDefault(a => a.ID == readingId
                                     && a.IsDeleted != true
                                     && a.NWC_DeviceMeter.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

            using (_unitofWork)
            {

                #region prepare model
                existContract.IsDeleted = true;
                existContract.DetetedBy = this._loggedInUser.LoggedInUser.StaffId;
                existContract.DeletedDate = DateTimeHelper.GetDateTimeNow();
                #endregion
                this._readingRepository.Update(existContract);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        #endregion
    }
}
