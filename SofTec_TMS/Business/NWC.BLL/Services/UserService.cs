using NWC.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Security;
using LinqKit;
using System.Configuration;
using System.Data.Entity;
using System.Linq.Expressions;
using NWC.DAL.NWCEntities;
using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Enums;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using NWC.DTO.User;
using NWC.DTO.Helpers;

namespace NWC.BLL.Services
{
    public class UserService : IUserService
    {
        private string imagePath
        {
            get
            {
                return ConfigurationManager.AppSettings["ImagePath"] != null ?
                    ConfigurationManager.AppSettings["ImagePath"] : string.Empty;
            }
        }

        #region Properties
        private IUnitofWork _unitofWork;

        private IRepository<aspnet_Users> _userRepository;
        private IRepository<aspnet_Membership> _memberRepository;
        private IRepository<Staff> _staffRepository;
        private IRepository<Transporter_Staff> _Transporter_StaffRepository;
        private IRepository<StaffRoleModulePermissions> _staffRoleModulePermissionsRepository;
        private IRepository<NWC_UserLandmarkPermission> _NWC_UserLandmarkPermission;
        private IRepository<vw_NWC_UserLandmarkPermission> _vw_NWC_UserLandmarkPermission;
        private IRepository<NWC_UserServicePermission> _NWC_UserServicePermission;
        private ILoggedInUserService _loggedInUser;
        private IRepository<vw_NWC_StaffRoleModulePermissions> _viewstaffRoleModulePermissions;
        private IRepository<SubscriberAdminSettings> _SubscriberAdminSettings;
        #endregion

        #region Constructors
        public UserService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            var ctx = (context == null ? new NWCContext() : context);
            this._staffRepository = new Repository<Staff>(ctx);
            this._Transporter_StaffRepository = new Repository<Transporter_Staff>(ctx);

            this._userRepository = new Repository<aspnet_Users>(ctx);
            this._memberRepository = new Repository<aspnet_Membership>(ctx);
            this._staffRoleModulePermissionsRepository = new Repository<StaffRoleModulePermissions>(ctx);
            this._NWC_UserLandmarkPermission = new Repository<NWC_UserLandmarkPermission>(ctx);
            this._vw_NWC_UserLandmarkPermission = new Repository<vw_NWC_UserLandmarkPermission>(ctx);
            this._viewstaffRoleModulePermissions = new Repository<vw_NWC_StaffRoleModulePermissions>(ctx);
            this._SubscriberAdminSettings = new Repository<SubscriberAdminSettings>(ctx);


            this._NWC_UserServicePermission = new Repository<NWC_UserServicePermission>(ctx);


            this._loggedInUser = loggedInUser;

            this._unitofWork = new UnitofWork(ctx);
        }
        #endregion

        #region Command
        public DescriptiveResponse<Boolean> SaveUserPermittedLandmarks(List<UserLandmarkPermissionDTO> userPermittedLandmarks)
        {
            try
            {
                using (_unitofWork)
                {
                    foreach (var item in userPermittedLandmarks)
                    {

                        #region save landmark

                        var dbUserPermittedLandmarks = this._NWC_UserLandmarkPermission.GetQuery()
                           .Where(s => s.StaffId == item.StaffID &&
                                        s.IsDeleted != true);

                        foreach (var station in dbUserPermittedLandmarks)
                        {
                            if (!item.PermittedLandmarkIDs.Contains(station.LandmarkID))
                            {
                                this._NWC_UserLandmarkPermission.Delete(station);
                            }
                        }

                        item.PermittedLandmarkIDs = item.PermittedLandmarkIDs.Distinct().ToList();
                        foreach (var landmarkID in item.PermittedLandmarkIDs)
                        {
                            var station = new NWC_UserLandmarkPermission()
                            {
                                StaffId = item.StaffID,
                                LandmarkID = landmarkID,
                                CreationDate = DateTimeHelper.GetDateTimeNow(),
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                IsDeleted = false
                            };

                            if (!dbUserPermittedLandmarks.Select(x => x.LandmarkID).Contains(landmarkID))
                            {
                                this._NWC_UserLandmarkPermission.Add(station);
                            }
                        }

                        #endregion


                        #region save services

                        var dbUserPermittedServices = this._NWC_UserServicePermission.GetQuery()
                           .Where(s => s.StaffId == item.StaffID &&
                                        s.IsDeleted != true);

                        foreach (var service in dbUserPermittedServices)
                        {
                            if (!item.PermittedServiceIDs.Contains(service.ServiceID))
                            {
                                this._NWC_UserServicePermission.Delete(service);
                            }
                        }

                        foreach (var serviceID in item.PermittedServiceIDs)
                        {
                            var service = new NWC_UserServicePermission()
                            {
                                StaffId = item.StaffID,
                                ServiceID = serviceID,
                                CreationDate = DateTimeHelper.GetDateTimeNow(),
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                IsDeleted = false
                            };

                            if (!dbUserPermittedServices.Select(x => x.ServiceID).Contains(serviceID))
                            {
                                this._NWC_UserServicePermission.Add(service);
                            }
                        }

                        #endregion
                    }
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => SaveUserPermittedLandmarks: "));

                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        #endregion

        #region Query

        public DescriptiveResponse<SearchResult<UserListDTO>> SearchUsers(UserSearchCriteriaDTO searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<aspnet_Users>(true);
            predicate = predicate.And(s => s.isDeleted == false);

            if (!string.IsNullOrEmpty(searchCriteria.Name))
            {
                var name = searchCriteria.Name.Trim();
                predicate = predicate.And(s => s.UserName.Contains(name));
            }


            #endregion

            #region Sort
            // TODO: handle localization
            Expression<Func<aspnet_Users, object>> sort;
            switch (searchCriteria.OrderBy)
            {
                case UserSearchCriteriaDTO.OrderByExepression.Name:
                    sort = x => x.Name;
                    break;

                case UserSearchCriteriaDTO.OrderByExepression.Email:
                default:
                    sort = x => x.aspnet_Membership.Email;
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

            var userQuery = this._userRepository.GetQuery().Include("aspnet_Membership")
              .Where(predicate)
              .OrderBy(s => s.Name)
              .Skip(skip)
              .Take(take);

            #region response
            var result = new SearchResult<UserListDTO>();
            if (userQuery != null && userQuery.Any())
            {
                var count = this._userRepository.GetQuery().Count(predicate);
                result.Result = userQuery.Include("aspnet_Membership").AsEnumerable().Select(a => a.WrapToUserListDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<UserListDTO>>.Success(result);
            #endregion
        }
        public DescriptiveResponse<SearchResult<UserLandmarkPermissionDTO>> GetUserPermittedLandmarks(UserStationPermissionSC searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_UserLandmarkPermission>(true);

                predicate = predicate.And(s => s.IsDeleted != true);
                predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

                if (!string.IsNullOrEmpty(searchCriteria.UserName))
                {
                    var searchText = searchCriteria.UserName.Trim();
                    predicate = predicate.And(s => s.FullName.Contains(searchText));
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

                var userPermittedLandmarks = this._vw_NWC_UserLandmarkPermission.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.FullName)
                    .Skip(skip)
                    .Take(take);

                var result = new SearchResult<UserLandmarkPermissionDTO>();

                if (userPermittedLandmarks != null && userPermittedLandmarks.Any())
                {
                    var count = this._vw_NWC_UserLandmarkPermission.GetQuery().Count(predicate);
                    result.Result = userPermittedLandmarks.AsEnumerable().Select(a => a.WrapToUserLandmarkPermissionDTO()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<UserLandmarkPermissionDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => GetUserPermittedLandmarks: "));

                return DescriptiveResponse<SearchResult<UserLandmarkPermissionDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<ProfileDTO> GetUserProfile()
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<Staff>(true);

                predicate = predicate.And(s => s.isDeleted != true);
                predicate = predicate.And(s => s.subID == this._loggedInUser.LoggedInUser.SubscriberId);
                predicate = predicate.And(s => s.ID == this._loggedInUser.LoggedInUser.StaffId);
                #endregion

                var staff = this._staffRepository.GetQuery()
                    .Include("Landmark")
                    .Where(predicate)
                    .FirstOrDefault();

                var staffDTO = new ProfileDTO();
                var TransporterStaff = _Transporter_StaffRepository.GetQuery().Where(x => x.Staff == staff.ID).FirstOrDefault();

                if (staff != null)
                {
                    var imgPath = string.Format("{0}{1}\\{2}", this.imagePath, staff.subID, staff.image);

                    var imgData = ImageHelper.ImageToByteArray(imgPath);

                    staffDTO.ID = staff.ID;
                    staffDTO.FullName = string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.LastName);
                    staffDTO.Username = staff.aspnet_Users.UserName;
                    staffDTO.MobileNumber = staff.mobileNumber;
                    staffDTO.ProfileImage = imgData;
                    staffDTO.Branch = TransporterStaff == null ? staff.Landmark?.branchId : TransporterStaff.Transporter1.branch;
                    staffDTO.StaffRoleName = staff.StaffRoleName;
                    staffDTO.Email = staff.Email;
                    staffDTO.personalID = staff.personalID;
                    staffDTO.UserID = staff.UserID.Value;
                    if (TransporterStaff != null && TransporterStaff.Transporter1 != null)
                    {
                        staffDTO.TankerNumber = TransporterStaff.Transporter1 == null ? string.Empty : TransporterStaff.Transporter1.code;
                        staffDTO.StationName = TransporterStaff.Transporter1.Landmark1.name;
                        if (TransporterStaff.Transporter1.licenseExpiryDate != null)
                        {
                            var licenseExpiryDate = (DateTime)TransporterStaff.Transporter1.licenseExpiryDate;
                            staffDTO.licenseExpiryDate = licenseExpiryDate;
                            staffDTO.licenseStatus = GetLiecenceStatus(licenseExpiryDate);
                            staffDTO.RemaininglicenseExpiry = (int)licenseExpiryDate.Subtract(DateTime.Now).TotalDays;
                        }
                    }


                }

                var logo = this._SubscriberAdminSettings.GetQuery()
                   .Where(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId)
                   .Select(s => s.applicationLogo)
                   .FirstOrDefault();

                staffDTO.SubLogo = !string.IsNullOrEmpty(logo) ? $"~/Files/{this._loggedInUser.LoggedInUser.SubscriberId}/{logo}" : string.Empty;

                return DescriptiveResponse<ProfileDTO>.Success(staffDTO);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => GetUserProfile: "));

                return DescriptiveResponse<ProfileDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<StaffPermissionDTO>> GetUserPermissions()
        {
            try
            {
                using (_unitofWork)
                {
                    var result = this._viewstaffRoleModulePermissions.GetQuery()
                    .Where(s => s.SubID == _loggedInUser.LoggedInUser.SubscriberId
                                && s.isDeleted != true
                                && s.StaffRoleID == _loggedInUser.LoggedInUser.StaffRoleID
                                //&& s.StaffRoles.Staff.Any(s => s.UserID == userId)
                                )
                    .Select(r => new StaffPermissionDTO
                    {
                        //StaffPermissionId = r.ID,
                        ModuleUniqueName = r.ModuleUniqueName,
                        PageUniqueName = r.PageUniqueName,
                        RoleName = r.RoleName
                    }).OrderBy(x => x.ModuleUniqueName).ToList();

                    return DescriptiveResponse<IEnumerable<StaffPermissionDTO>>.Success(result);
                }

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => GetUserPermissions: "));

                return DescriptiveResponse<IEnumerable<StaffPermissionDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        private int GetLiecenceStatus(DateTime licenseExpiryDate)
        {
            return DateTime.Now >= licenseExpiryDate ? 3 : DateTime.Now.AddDays(30) >= licenseExpiryDate ? 2 : 1;
        }
        #endregion

        #region Authenticate

        public DescriptiveResponse<LoginDTO> AuthenticateUser(string userName, string password)
        {
            try
            {
                LoginDTO logindto = new LoginDTO();
                var membershipCtx = new MembershipContext();

                MembershipUser selectedUser = Membership.GetUser(userName);

                //password = selectedUser.ResetPassword();
                //selectedUser.ChangePassword(password, "123456");
                if (selectedUser == null)
                {
                    return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.InvalidUsername);
                }
                else
                {
                    if (Membership.ValidateUser(userName, password))
                    {
                        Guid userKey = Guid.Parse(selectedUser.ProviderUserKey.ToString());
                        var entityUser = this._userRepository.GetQuery().Where(a => a.UserId == userKey).FirstOrDefault();//.Where(a => a.UserId == userKey).FirstOrDefault();
                        var staff = this._staffRepository.GetQuery().Where(x => x.UserID == entityUser.UserId).FirstOrDefault();

                        membershipCtx.Account = new AccountDTO
                        {
                            ID = entityUser.UserId,
                            Name = entityUser.UserName
                        };

                        var identity = new GenericIdentity(string.Concat(userName, ",", password, ",", entityUser.UserId));
                        membershipCtx.Principal = new GenericPrincipal(identity, null);

                        var authObj = new AuthenticationDTO() { UserId = entityUser.UserId.ToString(), UserName = userName, Password = password, LastActiveTime = DateTimeHelper.GetDateTimeNow().ToString("yyyyMMddHHmmss") };

                        if (logindto.Context == null)
                        {
                            logindto.Context = new MembershipContext();
                            logindto.Context.Account = new AccountDTO();
                        }

                        // logindto.Token = string.Concat("Basic ", Security.Base64Encode(authObj));
                        //logindto.Token = Security.Base64Encode(authObj);
                        logindto.Token = Security.EncrptToken(authObj);
                        logindto.Context.Account.Name = userName;
                        logindto.Context.Account.ID = entityUser.UserId;
                        logindto.Context.Account.StaffID = staff.ID;
                        logindto.Context.Account.SubID = staff.subID;
                        logindto.Context.Account.StaffRoleID = staff.staffRoleID;
                        //logindto.Context.Account.FullName = 
                        //    (!string.IsNullOrEmpty(staff.FirstName) ? staff.FirstName : string.Empty) 
                        //    + ' ' + 
                        //    (!string.IsNullOrEmpty(staff.LastName) ? staff.LastName : string.Empty);

                        return DescriptiveResponse<LoginDTO>.Success("sucess", logindto);

                    }
                    else
                    {
                        if (!selectedUser.IsLockedOut)
                        {
                            return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.LockedOut);
                        }
                        else
                        {
                            return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.InvalidPassword);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => AuthenticateUser: "));

                return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public IEnumerable<StaffPermissionDTO> GetUserAuthenticatePermissions(Guid userId, Guid subId, out ReturnResult result)
        {
            var staffPermissionDTOs = new List<StaffPermissionDTO>();

            try
            {
                using (_unitofWork)
                {
                    staffPermissionDTOs = this._staffRoleModulePermissionsRepository.Get(new Specification<StaffRoleModulePermissions>(x => x.SubID == subId && (!x.isDeleted.HasValue || !x.isDeleted.Value) && x.StaffRoles.Staff.Any(s => s.UserID == userId)))
                  .Select(r => new StaffPermissionDTO
                  {
                      //StaffPermissionId = r.ID,
                      ModuleUniqueName = r.Modules.UniqueName,
                      PageUniqueName = r.Pages.UniqueName
                  }).ToList();
                }

                // TODO : Insert record in the log with the action type, date, user and detail
                result = new ReturnResult() { ResultCode = ReturnResultCode.Success };
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => GetUserAuthenticatePermissions: "));
                result = new ReturnResult() { ResultCode = ReturnResultCode.Exception, ResultMessage = ex.Message };
            }

            return staffPermissionDTOs;
        }

        public LoginDTO ValidateUserAsync(string name, string password)
        {
            LoginDTO logindto = new LoginDTO();

            try
            {
                var membershipCtx = new MembershipContext();
                MembershipUser selectedUser = Membership.GetUser(name);
                if (selectedUser == null)
                {
                    logindto.Status = LoginStatus.InvalidUsername;
                }
                else
                {
                    if (Membership.ValidateUser(name, password))
                    {
                        Guid userKey = Guid.Parse(selectedUser.ProviderUserKey.ToString());

                        using (var context = new NWCContext())
                        {
                            var entityUser = context.aspnet_Users.Where(a => a.UserId == userKey).FirstOrDefault();
                            membershipCtx.Account = new AccountDTO { ID = entityUser.UserId, Name = entityUser.UserName };
                            var identity = new GenericIdentity(string.Concat(name, ",", password, ",", entityUser.UserId));
                            membershipCtx.Principal = new GenericPrincipal(identity, null);

                            logindto.Status = LoginStatus.Success;
                        }
                    }
                    else
                    {
                        if (selectedUser.IsLockedOut)
                        {
                            logindto.Status = LoginStatus.LockedOut;
                        }

                        else
                        {
                            logindto.Status = LoginStatus.InvalidPassword;
                        }
                    }
                }
                logindto.Context = membershipCtx;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => ValidateUserAsync: "));
            }

            return logindto;
        }

        public DescriptiveResponse<bool> Unlock(string Name)
        {
            try
            {
                using (_unitofWork)
                {
                    var UserDTOs = this._memberRepository.GetQuery().FirstOrDefault(x => x.aspnet_Users.UserName == Name);
                    if (UserDTOs != null)
                    {
                        UserDTOs.IsLockedOut = false;
                        UserDTOs.LastLockoutDate = DateTime.Now;
                        this._memberRepository.Update(UserDTOs);
                        return DescriptiveResponse<bool>.Success(true);
                    }
                    else
                    {
                        return DescriptiveResponse<bool>.Error(ErrorStatus.InvalidUsername);

                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => unlock: "));

                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<bool> Lock(string Name)
        {
            using (_unitofWork)
            {
                var UserDTOs = this._memberRepository.GetQuery().FirstOrDefault(x => x.aspnet_Users.UserName == Name);
                if (UserDTOs != null)
                {
                    UserDTOs.IsLockedOut = true;
                    this._memberRepository.Update(UserDTOs);
                    return DescriptiveResponse<bool>.Success(true);
                }
                else
                {
                    return DescriptiveResponse<bool>.Error(ErrorStatus.InvalidUsername);

                }
            }
        }
        public DescriptiveResponse<bool> Delete(string Name)
        {
            try
            {
                using (_unitofWork)
                {

                    var acc = this._userRepository.GetQuery().FirstOrDefault(s =>
                                    s.UserName == Name);
                    if (acc != null)
                    {
                        Random rand = new Random();

                        acc.LoweredUserName = "Del_" + rand.Next() + "_" + acc.UserName;
                        acc.UserName = "Del_" + rand.Next() + "_" + acc.UserName;
                        acc.isDeleted = true;
                        

                    
                        if (acc.aspnet_Membership != null)
                        {
                            acc.aspnet_Membership.Email = "Del_" + rand.Next() + "_" + acc.aspnet_Membership.Email;
                            acc.aspnet_Membership.LoweredEmail = "Del_" + rand.Next() + "_" + acc.aspnet_Membership.LoweredEmail;
                        }
                        this._userRepository.Update(acc);
                        this._memberRepository.Update(acc.aspnet_Membership);

                        var staff = this._staffRepository.GetQuery().Where(x => x.UserID == acc.UserId).FirstOrDefault();
                        if (staff != null)
                        {

                            staff.personalID = "Del_" + rand.Next() + "_" + staff.personalID;
                            staff.isDeleted = true;
                            this._staffRepository.Update(staff);
                        }

                    }
                }
                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => Delete: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> ChangePassword(ChangePasswordDTO model)
        {
            try
            {
                var membershipCtx = new MembershipContext();
                var User = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == model.Id);
                if (User == null)
                    return DescriptiveResponse<bool>.Error("No User exist");

                MembershipUser selectedUser = Membership.GetUser(User.UserName);
                if (selectedUser == null)
                {
                    return DescriptiveResponse<bool>.Error(ErrorStatus.InvalidUsername);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.OldPassword))
                    {
                        return DescriptiveResponse<bool>.Error("Old Password must be supplied for password change.");
                    }
                    if (!Membership.ValidateUser(User.UserName, model.OldPassword))
                    {
                        return DescriptiveResponse<bool>.Error("Old Password is not correct");
                    }
                    else
                    {
                        selectedUser.ChangePassword(model.OldPassword, model.NewPassword);
                        return DescriptiveResponse<bool>.Success(true);

                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => Delete: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> ForgotPassword(string email)
        {
            try
            {
                var membershipCtx = new MembershipContext();
                string username = Membership.GetUserNameByEmail(email);
                if (string.IsNullOrEmpty(username))
                {
                    return DescriptiveResponse<bool>.Error(ErrorStatus.InvalidUsername);
                }
                else
                {
                    var user = Membership.GetUser(username);

                    if (!user.IsApproved)
                    {
                        return DescriptiveResponse<bool>.Error("The email is not confirmed");
                    }
                    else
                    {

                        user.GetPassword();
                        return DescriptiveResponse<bool>.Success(true);

                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "UserService => Delete: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        #endregion
    }
}
