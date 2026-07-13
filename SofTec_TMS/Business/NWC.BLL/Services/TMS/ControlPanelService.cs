using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Data.Entity;
using System.Linq;

namespace NWC.BLL.Services
{
    public partial class ControlPanelService : IControlPanelService
    {
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<vw_NWC_BranchSetting> _BranchSetting_View;
        private readonly IRepository<NWC_BranchSetting> _BranchSetting;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<Landmark> _landmarkRepository;

        public ControlPanelService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._BranchSetting_View = new Repository<vw_NWC_BranchSetting>(ctx);
            this._BranchSetting = new Repository<NWC_BranchSetting>(ctx);
            this._branchRepository = new Repository<Branch>(ctx);
            this._landmarkRepository = new Repository<Landmark>(ctx);
        }



        #region Query
        public DescriptiveResponse<SearchResult<BranchSettingDTO>> SearchCitySettings(BranchSearchCriteriaDTO searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_BranchSetting>(true);
                predicate = predicate.And(s => s.IsSubBranch && s.SubId == _loggedInUser.LoggedInUser.SubscriberId);

                if (!string.IsNullOrEmpty(searchCriteria.SearchKeyword))
                {
                    var searchText = searchCriteria.SearchKeyword.Trim();
                    predicate = predicate.And(s => s.name.Contains(searchText) || s.code.Contains(searchText));
                }
                #endregion

                #region Skip & Take
                int skip = 0, take = 20;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                var query = _BranchSetting_View.GetQuery()
                    .Where(predicate)
                    .OrderBy(x => x.code)
                    .Skip(skip)
                    .Take(take);

                var result = new SearchResult<BranchSettingDTO>();
                if (query != null && query.Any())
                {
                    result.TotalCount = _BranchSetting_View.GetQuery().Count(predicate);
                    result.Result = query.AsEnumerable().Select(x => x.WrapToBranchSettingDTO()).ToList();
                }

                return DescriptiveResponse<SearchResult<BranchSettingDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ControlPanelService => SearchCitySettings: "));
                return DescriptiveResponse<SearchResult<BranchSettingDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<BranchSettingDTO> GetCitySetting(Guid cityId)
        {
            try
            {
                var cityDto = _BranchSetting_View.GetQuery()
                    .FirstOrDefault(x => x.Id == cityId).WrapToBranchSettingDTO();
                return DescriptiveResponse<BranchSettingDTO>.Success(cityDto);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ControlPanelService => GetCitySetting: "));
                return DescriptiveResponse<BranchSettingDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region Command
        public DescriptiveResponse<bool> AddCitySettings(BranchSettingDTO dto)
        {
            try
            {
                using (_unitofWork)
                {
                    var branch = new Branch();
                    branch.Id = Guid.NewGuid();
                    branch.CreatedBy = _loggedInUser.LoggedInUser.StaffId;
                    branch.CreationTime = DateTimeHelper.GetDateTimeNow();
                    branch.SubId = _loggedInUser.LoggedInUser.SubscriberId;
                    branch.name = dto.name;
                    branch.IsSubBranch = true;
                    branch.NWC_BranchSetting = dto.WrapFromBranchSettingDTO();
                    branch.NWC_BranchSetting.BranchID = branch.Id;
                    _branchRepository.Add(branch);
                }
                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ControlPanelService => AddCitySettings: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> UpdateCitySettings(BranchSettingDTO dto)
        {
            try
            {
                using (_unitofWork)
                {
                    var branch = _branchRepository.FindById(dto.Id);
                    branch.name = dto.name;
                    dto.WrapFromBranchSettingDTO(branch.NWC_BranchSetting);
                    _branchRepository.Update(branch);
                }
                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ControlPanelService => UpdateCitySettings: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion
    }
}
