using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.BLL.Interfaces.TMS;
using NWC.BLL.Validators.TMS;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using NWC.DTO.SearchCriteria.TMS;
using NWC.DTO.User;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace NWC.BLL.Services.TMS
{
    public class StaffService : IStaffService
    {
        #region Properties
        private readonly IUnitofWork _unitofwork;
        private readonly IRepository<Staff> _staffListRepository;
        private readonly IRepository<StaffRoles> _staffRoleRepository;
        private readonly IRepository<UserBranchPermission> _userBranchPermistionRepository;
        private readonly IRepository<aspnet_Users> _userRepository;
        private readonly IRepository<Modules> _ModulesRepository;
        private readonly IRepository<Pages> _PagesRepository;
        private readonly IRepository<aspnet_Roles> _RolesRepository;
        private readonly IRepository<StaffRoleModulePermissions> _StaffRolePermissionsModule;
        private readonly IRepository<UserBranchPermission> _UserBranchPermission;

        private readonly DbContext _ctx;

        private MembershipCreateStatus status;

        private readonly ILoggedInUserService _LoggedInUserService;
        #endregion

        #region Query
        public StaffService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._LoggedInUserService = loggedInUser;

            _ctx = (context == null) ? new NWCContext() : context;
            this._unitofwork = new UnitofWork(_ctx);
            this._staffListRepository = new Repository<Staff>(_ctx);
            this._staffRoleRepository = new Repository<StaffRoles>(_ctx);
            this._userBranchPermistionRepository = new Repository<UserBranchPermission>(_ctx);
            this._userRepository = new Repository<aspnet_Users>(_ctx);
            this._ModulesRepository = new Repository<Modules>(_ctx);
            this._PagesRepository = new Repository<Pages>(_ctx);
            this._RolesRepository = new Repository<aspnet_Roles>(_ctx);
            this._StaffRolePermissionsModule = new Repository<StaffRoleModulePermissions>(_ctx);
            this._UserBranchPermission = new Repository<UserBranchPermission>(_ctx);
        }
        #endregion

        #region Command
        public DescriptiveResponse<SearchResult<StaffListDTO>> SearchStaff(StaffSCDTO searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<Staff>(true);
            predicate = predicate.And(x => x.isDeleted != true && x.subID == _LoggedInUserService.LoggedInUser.SubscriberId);

            if (!string.IsNullOrEmpty(searchCriteria.searchKeyword))
            {
                var word = searchCriteria.searchKeyword.Trim();
                predicate = predicate.And(x => x.FirstName.Contains(word) || x.MiddleName.Contains(word) || x.LastName.Contains(word) || x.code.Contains(word));
            }

            if (searchCriteria.RoleId != null && searchCriteria.RoleId.Count() > 0)
            {
                predicate = predicate.And(x => searchCriteria.RoleId.Any(role => x.StaffRoles.StaffRoleCategory.ID.ToString().Contains(role)));
            }

            if (searchCriteria.branchId != null && searchCriteria.branchId.Count() > 0)
            {
                predicate = predicate.And(x => searchCriteria.branchId.Any(subbranchs => x.AllocatedBranch.Value.ToString().Contains(subbranchs)));
            }

            if (searchCriteria.stationId != null && searchCriteria.stationId.Count() > 0)
            {
                predicate = predicate.And(x => searchCriteria.stationId.Any(allocatedlandmark => x.AllocatedLandmark.Value.ToString().Contains(allocatedlandmark)));
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

            var staffQuery = this._staffListRepository.GetQuery()
                .Where(predicate)
                .OrderBy(x => x.code)
                .Skip(skip)
                .Take(take);

            #region Response
            var result = new SearchResult<StaffListDTO>();
            if (staffQuery != null && staffQuery.Any())
            {
                var count = this._staffListRepository.GetQuery().Count(predicate);
                result.Result = staffQuery.AsEnumerable().Select(x => x.WarapToStaffListDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<StaffListDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<bool> Add(StaffDTO dto)
        {
            try
            {
                var staff = StaffDTO.MapToStaff(dto);
                staff.ID = Guid.NewGuid();
                staff.isDeleted = false;
                staff.CreatedBy = _LoggedInUserService.LoggedInUser.StaffId;
                staff.CreationTime = DateTimeHelper.GetDateTimeNow();
                staff.subID = _LoggedInUserService.LoggedInUser.SubscriberId;

                #region Validation
                var validator = new StaffValidator(ValidationMode.Create, this._LoggedInUserService, this._staffListRepository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion

                var user = Membership.CreateUser(dto.Username, dto.Password, dto.Email, null, null, true, out status);

                if (status != MembershipCreateStatus.Success)
                {
                    return DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL);
                }

                staff.UserID = (Guid)user.ProviderUserKey;

                using (_unitofwork)
                {
                    this._staffListRepository.Add(staff);
                }

                using (_unitofwork)
                {
                    var premittedBranches = dto.PermittedBranch.Union(dto.PermittedSubBranch).ToList();

                    if (premittedBranches.Count > 0)
                    {
                        List<UserBranchPermission> userBranches = new List<UserBranchPermission>();
                        foreach (var item in premittedBranches)
                        {
                            var premittedBranch = new UserBranchPermission()
                            {
                                StaffId = staff.ID,
                                BranchId = item.Value,
                                IsDeleted = false,
                                CreationDate = DateTimeHelper.GetDateTimeNow(),
                                CreatedBy = _LoggedInUserService.LoggedInUser.StaffId,
                            };
                            userBranches.Add(premittedBranch);
                        }
                        this._userBranchPermistionRepository.AddRange(userBranches);
                    }
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => Add: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }


        public DescriptiveResponse<bool> UpdateDriver(StaffDTO dto)
        {
            try
            {
                using (_unitofwork)
                {
                    var staff = this._staffListRepository.GetQuery()
                         .FirstOrDefault(s => s.personalID == dto.personalID
                         && s.isDeleted != true
                         && s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId);
                    this._staffListRepository.Update(staff);

                    staff.LastModificationDate = DateTimeHelper.GetDateTimeNow();
                    staff.FirstName = dto.FirstName;
                    staff.descr = dto.descr;
                    staff.code = dto.code;

                }
                return DescriptiveResponse<Boolean>.Success(true);

            }

            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => Update: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }
        public DescriptiveResponse<bool> Update(StaffDTO dto)
        {
            try
            {

                #region Validations
                var validator = new StaffValidator(ValidationMode.Update, this._LoggedInUserService, this._staffListRepository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion

                using (_unitofwork)
                {
                    var staff = this._staffListRepository.GetQuery()
                        .FirstOrDefault(s => s.ID == dto.ID
                        && s.isDeleted != true
                        && s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId);

                    #region Mapping
                    staff.LastModifiedBy = this._LoggedInUserService.LoggedInUser.StaffId;
                    staff.LastModificationDate = DateTimeHelper.GetDateTimeNow();
                    staff.FirstName = dto.FirstName;
                    staff.descr = dto.descr;
                    staff.code = dto.code;
                    staff.staffRoleID = dto.staffRoleID;
                    staff.isAllocated = dto.isAllocated;
                    staff.personalID = dto.personalID;
                    staff.AllocatedBranch = dto.AllocatedSubBranch;
                    staff.AllocatedLandmark = dto.AllocatedLandmark;
                    staff.mobileNumber = dto.mobileNumber;
                    staff.MonitorFlag = dto.MonitorFlag;
                    staff.LaborRate = dto.LaborRate;
                    staff.Gender = dto.Gender;
                    staff.MiddleName = dto.MiddleName;
                    staff.LastName = dto.LastName;
                    staff.Email = dto.Email;
                    #endregion

                    this._staffListRepository.Update(staff);


                    #region Update User
                    var user = _userRepository.GetQuery().Where(u => u.UserId == staff.UserID).FirstOrDefault();
                    user.UserName = dto.Username;
                    this._userRepository.Update(user);
                    #endregion

                    #region Update Permissions
                    var deleted = _userBranchPermistionRepository.GetQuery().Where(ubp => ubp.StaffId == dto.ID).ToList();
                    foreach (var item in deleted)
                    {
                        _userBranchPermistionRepository.Delete(item);
                    }

                    var premittedBranches = dto.PermittedBranch.Union(dto.PermittedSubBranch).ToList();

                    if (premittedBranches.Count > 0)
                    {
                        List<UserBranchPermission> userBranches = new List<UserBranchPermission>();
                        foreach (var item in premittedBranches)
                        {
                            var premittedBranch = new UserBranchPermission()
                            {
                                StaffId = staff.ID,
                                BranchId = item.Value,
                                IsDeleted = false,
                                CreationDate = DateTimeHelper.GetDateTimeNow(),
                                CreatedBy = _LoggedInUserService.LoggedInUser.StaffId,
                            };
                            userBranches.Add(premittedBranch);
                        }
                        this._userBranchPermistionRepository.AddRange(userBranches);
                    }
                    #endregion
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => Update: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> Delete(Guid staffId)
        {
            try
            {
                if (staffId == null)
                    return DescriptiveResponse<bool>.Error();

                var exisitStaff = this._staffListRepository.GetQuery()
                    .FirstOrDefault(s => s.ID == staffId
                        && s.isDeleted != true
                        && s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId);

                using (_unitofwork)
                {
                    #region Delete Permissions
                    var deleted = _userBranchPermistionRepository.GetQuery().Where(ubp => ubp.StaffId == exisitStaff.ID).ToList();
                    foreach (var item in deleted)
                    {
                        this._userBranchPermistionRepository.Delete(item);
                    }
                    #endregion

                    #region Delete User
                    var user = _userRepository.GetQuery().Where(u => u.UserId == exisitStaff.UserID).FirstOrDefault();
                    user.isDeleted = true;
                    this._userRepository.Update(user);
                    #endregion

                    exisitStaff.isDeleted = true;
                    exisitStaff.LastModifiedBy = this._LoggedInUserService.LoggedInUser.StaffId;
                    exisitStaff.LastModificationDate = DateTimeHelper.GetDateTimeNow();

                    this._staffListRepository.Update(exisitStaff);
                }
                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => Delete: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<StaffDTO> GetStaffById(Guid id)
        {
            try
            {
                if (id == null)
                    return DescriptiveResponse<StaffDTO>.Error();

                var staff = new StaffDTO();

                using (_unitofwork)
                {
                    staff = _staffListRepository.GetQuery()
                        .Include(x => x.Landmark)
                        .Include(x => x.StaffRoles)
                        .FirstOrDefault(x =>
                        x.isDeleted != true &&
                        x.ID == id).WarapToStaffDTO();

                    var permittedBranches = _userBranchPermistionRepository.GetQuery()
                        .Where(x => x.IsDeleted != true && x.StaffId == staff.ID);

                    staff.PermittedBranch = permittedBranches.Where(x => x.Branch.IsSubBranch == false)
                        .Select(s => s.BranchId).Cast<Guid?>().ToList();

                    staff.PermittedSubBranch = permittedBranches.Where(x => x.Branch.IsSubBranch == true)
                        .Select(s => s.BranchId).Cast<Guid?>().ToList();

                    staff.Username = _userRepository.GetQuery()
                        .Where(x => x.UserId == staff.UserID)
                        .Select(s => s.UserName)
                        .FirstOrDefault();
                }
                return DescriptiveResponse<StaffDTO>.Success(staff);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => GetStaffById: "));
                return DescriptiveResponse<StaffDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<StaffDTO> GetStaffByPersonalId(string personalId)
        {
            try
            {
                if (personalId == null)
                    return DescriptiveResponse<StaffDTO>.Error();

                var staff = new StaffDTO();

                using (_unitofwork)
                {
                    staff = _staffListRepository.GetQuery()
                        .FirstOrDefault(x =>
                        x.isDeleted != true &&
                        x.personalID == personalId).WarapToStaffDTO();

                }
                return DescriptiveResponse<StaffDTO>.Success(staff);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => GetStaffByPersonalId: "));
                return DescriptiveResponse<StaffDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<FilterResult<StaffRolesDTO>> GetStaffRoles()
        {
            try
            {
                #region skip & take

                var userQuery = this._staffRoleRepository.GetQuery()
                    .Where(x => x.subID == this._LoggedInUserService.LoggedInUser.SubscriberId && x.isDeleted != true);
                #endregion
                #region response
                var result = new FilterResult<StaffRolesDTO>();
                if (userQuery != null && userQuery.Any())
                {
                    var count = this._userRepository.GetQuery().Count();
                    result.FirstResult = userQuery.Include(x => x.StaffRoleCategory).AsEnumerable().Where(x => x.isDefault == false).Select(a => a.WarapToStaffRolesDTO()).ToList();
                    result.SecondResult = userQuery.Include(x => x.StaffRoleCategory).AsEnumerable().Where(x => x.isDefault == true).Select(a => a.WarapToStaffRolesDTO()).ToList();

                    result.TotalCount = count;
                }

                return DescriptiveResponse<FilterResult<StaffRolesDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => GetStaffRoles: "));
                return DescriptiveResponse<FilterResult<StaffRolesDTO>>.Error(ex.Message + '|' + ex.InnerException?.Message, ErrorStatus.UNEXPECTED_ERROR);

            }
        }
        public DescriptiveResponse<List<PagesDTO>> GetPages(Guid? key = null)
        {
            try
            {
                using (_unitofwork)
                {
                    var predicate = PredicateBuilder.New<Staff>(true);

                    predicate = predicate.And(s => s.isDeleted != true);
                    predicate = predicate.And(s => s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId);
                    predicate = predicate.And(s => s.ID == this._LoggedInUserService.LoggedInUser.StaffId);

                    var staff = this._staffListRepository.GetQuery()
                        .Where(predicate)
                        .FirstOrDefault();
                    if (staff == null)
                        return DescriptiveResponse<List<PagesDTO>>.Error(ErrorStatus.UNAUTHORIZED);

                    #region skip & take
                    var staffs = new List<PagesDTO>();
                    var Modules = _ModulesRepository.GetQuery().Select(x => new PagesDTO { ModuleID = x.ID, ID = x.ID, Name = x.Name }).ToList();
                    foreach (var Moduleitem in Modules)
                    {
                        var staffItem = new PagesDTO();
                        staffItem.Name = Moduleitem.Name;
                        staffItem.ID = Moduleitem.ID;
                        staffItem.ModuleID = Moduleitem.ModuleID;
                        bool exist;
                        staffItem.Pages = _PagesRepository.GetQuery().Where(x => x.ModuleID == Moduleitem.ID).Select(x => new PagesVM()
                        {
                            ID = x.ID,
                            Name = x.Name,
                            IsCarRental = x.IsCarRental,
                            IsGPS = x.IsGPS,
                            IsMaintenance = x.IsMaintenance,
                            ModuleID = x.ModuleID,
                            Path = x.Path,
                            UniqueName = x.UniqueName
                        }).ToList();
                        foreach (var item in staffItem.Pages)
                        {
                            var role = _StaffRolePermissionsModule.GetQuery().FirstOrDefault(x2 => x2.PageID == item.ID && x2.StaffRoleID == key.Value);
                            if (role != null)
                            {
                                item.status = role.AspNetRoleID.Value;
                                item.exist = true;
                            }
                            else
                            {
                                item.status = Guid.Empty;
                                item.exist = false;
                            }
                        }

                        staffs.Add(staffItem);
                    }

                    #endregion
                    return DescriptiveResponse<List<PagesDTO>>.Success(staffs);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "StaffService => GetStaffRoles: "));
                return DescriptiveResponse<List<PagesDTO>>.Error(ex.Message + '|' + ex.InnerException?.InnerException.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<List<RoleDTO>> GetAllRoles()
        {
            try
            {
                List<RoleDTO> AllRole = new List<RoleDTO>();
                var Roles = _RolesRepository.GetQuery().ToList();
                foreach (aspnet_Roles Role in Roles)
                {
                    RoleDTO OneRole = new RoleDTO();
                    OneRole.RoleId = Role.RoleId;
                    OneRole.RoleName = Role.RoleName;
                    AllRole.Add(OneRole);
                }
                return DescriptiveResponse<List<RoleDTO>>.Success(AllRole);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetAllRoles: "));
                return DescriptiveResponse<List<RoleDTO>>.Error(ex.Message + '|' + ex.InnerException?.InnerException.Message, ErrorStatus.UNEXPECTED_ERROR);

            }
        }

        public DescriptiveResponse<bool> changePermssion(List<RoleVM> model, StaffRoleDTO StaffRoleDTO, Guid? key = null)
        {
            using (NWCContext db = new NWCContext())
            {
                using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        key = key != Guid.Empty ? key : null;

                        if (!string.IsNullOrEmpty(StaffRoleDTO.staffRoleCategoryID.ToString())
                            && !string.IsNullOrEmpty(StaffRoleDTO.roleName))
                        {
                            //Add Mode (if key is empty)
                            StaffRoles NewStaffRoles = new StaffRoles();
                            if (key != null)
                            {

                                NewStaffRoles = db.StaffRoles.FirstOrDefault(x => x.ID == key);
                                if (NewStaffRoles == null)
                                {
                                    return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                                }
                            }
                            NewStaffRoles.name = StaffRoleDTO.roleName;
                            NewStaffRoles.NameAr = StaffRoleDTO.roleName;
                            NewStaffRoles.NameEn = StaffRoleDTO.roleName;
                            NewStaffRoles.descr = StaffRoleDTO.descr;
                            NewStaffRoles.isDeleted = false;
                            NewStaffRoles.CreatedBy = _LoggedInUserService.LoggedInUser.StaffId;
                            NewStaffRoles.CreationTime = DateTimeHelper.GetDateTimeNow();
                            NewStaffRoles.subID = _LoggedInUserService.LoggedInUser.SubscriberId;
                            NewStaffRoles.LastModificationDate = DateTimeHelper.GetDateTimeNow();
                            NewStaffRoles.category = StaffRoleDTO.staffRoleCategoryID;
                            NewStaffRoles.PageId = StaffRoleDTO.DefaultPageId;
                            NewStaffRoles.isDefault = true;
                            if (NewStaffRoles.PageId == null)
                                NewStaffRoles.isDefault = false;
                            if (key == null)
                            {
                                NewStaffRoles.ID = Guid.NewGuid();
                                db.StaffRoles.Add(NewStaffRoles);
                            }
                            db.SaveChanges();
                            key = NewStaffRoles.ID;
                        }
                        else
                            return DescriptiveResponse<bool>.Error(ErrorStatus.INPUT_IS_NULL);


                        var existRoles = db.StaffRoleModulePermissions
                          .Where(s => s.StaffRoleID == key
                              && s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId);
                        using (_unitofwork)
                        {
                            #region Delete Permissions
                            if (existRoles != null)
                            {
                                foreach (var item in existRoles)
                                {
                                    db.StaffRoleModulePermissions.Remove(item);
                                }
                            }
                            db.SaveChanges();
                            foreach (var item in model)
                            {
                                var StaffRoleModulePermissions = new StaffRoleModulePermissions();
                                if (!string.IsNullOrEmpty(item.status.ToString()) && !(item.status == Guid.Empty) && item.exist == true)
                                {
                                    StaffRoleModulePermissions.ID = Guid.NewGuid();
                                    StaffRoleModulePermissions.AspNetRoleID = item.status;
                                    StaffRoleModulePermissions.PageID = item.PageID;
                                    StaffRoleModulePermissions.ModuleID = item.ModuleID;
                                    StaffRoleModulePermissions.StaffRoleID = key;
                                    StaffRoleModulePermissions.CreationTime = DateTime.Now;
                                    StaffRoleModulePermissions.CreatedBy = this._LoggedInUserService.LoggedInUser.StaffId;
                                    StaffRoleModulePermissions.LastModifiedBy = this._LoggedInUserService.LoggedInUser.StaffId;
                                    StaffRoleModulePermissions.LastModificationDate = DateTime.Now;
                                    StaffRoleModulePermissions.SubID = this._LoggedInUserService.LoggedInUser.SubscriberId;
                                    db.StaffRoleModulePermissions.Add(StaffRoleModulePermissions);
                                }
                            }
                            db.SaveChanges();
                            dbTran.Commit();
                            #endregion
                            return DescriptiveResponse<bool>.Success(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback();
                        LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetAllRoles: "));
                        return DescriptiveResponse<bool>.Error(ex.Message + '|' + ex.InnerException?.InnerException.Message, ErrorStatus.UNEXPECTED_ERROR);
                    }

                }
            }

        }


        public DescriptiveResponse<List<StaffExcelDTO>> AddRange(List<StaffExcelDTO> dtos)
        {

            var failedList = new List<StaffExcelDTO>();
            var _branchRepository = new Repository<Branch>(_ctx);
            var _landmarkRepository = new Repository<Landmark>(_ctx);

            foreach (var dto in dtos)
            {
                var validator = new StaffListValidator(ValidationMode.Create, _LoggedInUserService, _staffListRepository, dtos);
                var results = validator.Validate(new StaffExcelDTO());

                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    dto.ExcelValidation = string.Join(", ", failures);

                    failedList.Add(dto);
                }
                else
                {
                    using (_unitofwork)
                    {


                        try
                        {
                            int AddEditMode = 0; //Add
                            var user = Membership.GetUser(dto.IDs.Trim());
                            if (user == null)
                                user = Membership.CreateUser(dto.IDs.Trim(), ConfigurationManager.AppSettings["StaffDefaultPassExcel"].ToString(), dto.Email, null, null, true, out status);
                            //ConfigurationManager.AppSettings["StaffDefaultPassExcel"].ToString;
                            if (status != MembershipCreateStatus.Success)
                            {
                                return DescriptiveResponse<List<StaffExcelDTO>>.Error(ErrorStatus.COMMIT_FAIL);
                            }
                            var entity = _staffListRepository.GetQuery().FirstOrDefault(x => x.UserID == (Guid)user.ProviderUserKey);
                            if (entity == null)
                            {
                                entity = dto.WrapDtoToStaff(new Staff());
                                entity.ID = Guid.NewGuid();
                                AddEditMode = 0;
                            }
                            else
                            {
                                entity = dto.WrapDtoToStaff(entity);
                                AddEditMode = 1;
                            }
                            entity.isDeleted = false;
                            entity.subID = _LoggedInUserService.LoggedInUser.SubscriberId;
                            entity.CreatedBy = _LoggedInUserService.LoggedInUser.StaffId;
                            entity.CreationTime = DateTimeHelper.GetDateTimeNow();
                            entity.LastModificationDate = DateTimeHelper.GetDateTimeNow();
                            entity.AllocatedBranch = _branchRepository.GetQuery().FirstOrDefault(x => x.name.Trim() == dto.SubBranch.Trim())?.Id;
                            entity.AllocatedLandmark = _landmarkRepository.GetQuery().FirstOrDefault(x => x.name.Trim() == dto.Landmark.Trim())?.Id;
                            entity.UserID = (Guid)user.ProviderUserKey;

                            var permittedBranch = _branchRepository.GetQuery().FirstOrDefault(x => x.name.Trim() == dto.PermittedBranch.Trim())?.Id;
                            var deleted = _userBranchPermistionRepository.GetQuery().Where(ubp => ubp.StaffId == entity.ID).ToList();
                            foreach (var item in deleted)
                            {
                                _userBranchPermistionRepository.Delete(item);
                            }
                            if (permittedBranch.HasValue)
                            {

                                var premittedbranch = new UserBranchPermission()
                                {
                                    StaffId = entity.ID,
                                    BranchId = permittedBranch.Value,
                                    IsDeleted = false,
                                    CreationDate = DateTimeHelper.GetDateTimeNow(),
                                    CreatedBy = _LoggedInUserService.LoggedInUser.StaffId,
                                };
                                this._userBranchPermistionRepository.Add(premittedbranch);
                            }
                            entity.staffRoleID = _staffRoleRepository.GetQuery().FirstOrDefault(x => x.name.Trim() == dto.StaffRole)?.ID;
                            if (entity.AllocatedBranch != null && entity.staffRoleID != null)
                            {
                                if (AddEditMode == 0)
                                    _staffListRepository.Add(entity);
                                else
                                    _staffListRepository.Update(entity);
                            }

                            else
                                failedList.Add(dto);
                        }
                        catch (Exception ex)
                        {
                            LoggerManager.LogMsg(c => c.Log(ex, "StaffService => AddRange: "));
                            failedList.Add(dto);
                        }
                    }
                }
            }

            return DescriptiveResponse<List<StaffExcelDTO>>.Success(failedList);
        }

        #endregion
    }
}
