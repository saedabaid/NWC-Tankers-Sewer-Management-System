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
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Data.Entity;
using System.Linq;

namespace NWC.BLL.Services
{
    public class LandmarksService : ILandmarksService
    {
        private readonly ILoggedInUserService _LoggedInUserService;
        private readonly IUnitofWork _unitofwork;
        private readonly IRepository<Landmark> _repository;

        public LandmarksService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            _LoggedInUserService = loggedInUser;

            var _ctx = (context == null) ? new NWCContext() : context;
            _unitofwork = new UnitofWork(_ctx);
            _repository = new Repository<Landmark>(_ctx);
        }

        public DescriptiveResponse<SearchResult<LandmarkListDto>> Search(LandmarkSearchDto searchCriteria)
        {
            var predicate = PredicateBuilder.New<Landmark>(true);
            predicate = predicate.And(x => x.isDeleted == searchCriteria.IsDeleted && x.SubId == _LoggedInUserService.LoggedInUser.SubscriberId);

            if (!string.IsNullOrEmpty(searchCriteria.SearchKeyword))
            {
                var word = searchCriteria.SearchKeyword.Trim();
                predicate = predicate.And(x => x.name.Contains(word));
            }

            #region Skip & Take
            int skip = 0, take = 20;
            if (searchCriteria.PageFilter != null)
            {
                skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                take = searchCriteria.PageFilter.PageSize;
            }
            #endregion

            var query = _repository.GetQuery()
                .Where(predicate)
                .OrderBy(x => x.code)
                .Skip(skip)
                .Take(take);

            #region Response
            var result = new SearchResult<LandmarkListDto>();
            if (query != null && query.Any())
            {
                var count = this._repository.GetQuery().Count(predicate);
                result.Result = query.AsEnumerable().Select(x => new LandmarkListDto(x)).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<LandmarkListDto>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<LandmarkDto> GetById(Guid id)
        {
            try
            {
                if (id == null)
                    return DescriptiveResponse<LandmarkDto>.Error();
                var entity = _repository.GetQuery().FirstOrDefault(x => x.isDeleted != true && x.Id == id);
                return DescriptiveResponse<LandmarkDto>.Success(new LandmarkDto(entity));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LandmarkService => GetById: "));
                return DescriptiveResponse<LandmarkDto>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> Add(LandmarkDto dto)
        {
            try
            {
                #region Validation
                var validator = new LandmarksValidator(ValidationMode.Create, _LoggedInUserService, _repository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion

                var entity = dto.MapToEntity();
                entity.Id = Guid.NewGuid();
                entity.isDeleted = false;
                entity.CreatedBy = _LoggedInUserService.LoggedInUser.StaffId;
                entity.CreationTime = DateTimeHelper.GetDateTimeNow();
                entity.SubId = _LoggedInUserService.LoggedInUser.SubscriberId;

                using (_unitofwork)
                {
                    _repository.Add(entity);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LandmarkService => Add: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> Update(LandmarkDto dto)
        {
            try
            {
                #region Validations
                var validator = new LandmarksValidator(ValidationMode.Update, _LoggedInUserService, _repository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion

                var entity = _repository.GetQuery()
                    .FirstOrDefault(s => s.Id == dto.Id
                    && s.isDeleted != true
                    && s.SubId == _LoggedInUserService.LoggedInUser.SubscriberId);

                dto.MapToEntity(entity);
                entity.LastModifiedBy = _LoggedInUserService.LoggedInUser.StaffId;
                entity.LastModificationDate = DateTimeHelper.GetDateTimeNow();

                using (_unitofwork)
                {
                    _repository.Update(entity);
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LandmarkService => Update: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> Delete(Guid id)
        {
            try
            {
                if (id == null)
                    return DescriptiveResponse<bool>.Error();

                var entity = _repository.GetQuery()
                    .FirstOrDefault(s => s.Id == id
                        && s.isDeleted != true
                        && s.SubId == _LoggedInUserService.LoggedInUser.SubscriberId);

                entity.isDeleted = true;
                entity.LastModifiedBy = _LoggedInUserService.LoggedInUser.StaffId;
                entity.LastModificationDate = DateTimeHelper.GetDateTimeNow();

                using (_unitofwork)
                {
                    _repository.Update(entity);
                }
                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LandmarkService => Delete: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }
    }
}
