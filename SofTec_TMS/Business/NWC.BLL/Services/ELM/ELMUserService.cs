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
using NWC.DTO.Common.ELM;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Enums;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using NWC.DTO.User;
using NWC.DTO.Helpers;
using NWC.BLL.Interfaces.ELM;

namespace NWC.BLL.Services.ELM
{
    public class ELMUserService : IELMUserService
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
        private ILoggedInUserService _loggedInUser;

        #endregion

        #region Constructors
        public ELMUserService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            var ctx = (context == null ? new NWCContext() : context);
            this._staffRepository = new Repository<Staff>(ctx);
            this._userRepository = new Repository<aspnet_Users>(ctx);
            this._loggedInUser = loggedInUser;
            this._unitofWork = new UnitofWork(ctx);
        }
        #endregion

        #region Authenticate

        public DescriptiveResponse<ELMLoginDTO> AuthenticateUser(string userName, string password)
        {
            try
            {
                ELMLoginDTO logindto = new ELMLoginDTO();
                var membershipCtx = new MembershipContext();
                if (userName == null || password == null)
                    return DescriptiveResponse<ELMLoginDTO>.Error("Bad Request", ErrorStatus.Bad_Request);

                MembershipUser selectedUser = Membership.GetUser(userName);

                if (selectedUser == null)
                {
                    return DescriptiveResponse<ELMLoginDTO>.Error(ErrorStatus.WrongUsername);
                }
                else
                {
                    if (Membership.ValidateUser(userName, password))
                    {
                      

                        Guid userKey = Guid.Parse(selectedUser.ProviderUserKey.ToString());
                        var entityUser = this._userRepository.GetQuery().Where(a => a.UserId == userKey).FirstOrDefault();//.Where(a => a.UserId == userKey).FirstOrDefault();
                        if (entityUser.isDeleted==true)
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error(ErrorStatus.DeletedUser);

                        }
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
                        logindto.TimeStamp = DateTime.Now;
                        logindto.Context.Account.Name = userName;
                        logindto.Context.Account.ID = entityUser.UserId;
                        logindto.Context.Account.StaffID = staff.ID;
                        logindto.Context.Account.SubID = staff.subID;
                        logindto.Context.Account.StaffRoleID = staff.staffRoleID;
                        //logindto.Context.Account.FullName = 
                        //    (!string.IsNullOrEmpty(staff.FirstName) ? staff.FirstName : string.Empty) 
                        //    + ' ' + 
                        //    (!string.IsNullOrEmpty(staff.LastName) ? staff.LastName : string.Empty);

                        return DescriptiveResponse<ELMLoginDTO>.Success(logindto);
                    }
                    else
                    {

                        if (selectedUser.IsLockedOut)
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error(ErrorStatus.blocked);

                        }
                        else
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error(ErrorStatus.WrongPassword);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ELMUserService => AuthenticateUser: "));

                return DescriptiveResponse<ELMLoginDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        #endregion
    }
}
