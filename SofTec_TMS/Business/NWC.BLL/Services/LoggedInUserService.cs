using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Constants;
using NWC.DTO.DomainModels;
using NWC.DTO.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace NWC.BLL.Services
{
    public class LoggedInUserService : ILoggedInUserService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<UserBranchPermission> _userBranchPermission;
        private readonly IRepository<NWC_UserLandmarkPermission> _userLandmarkPermission;
        private readonly IRepository<NWC_UserServicePermission> _userServicesPermission;
        private readonly IRepository<Branch> _branch;
        private IRepository<vw_NWC_StaffRoleModulePermissions> _viewstaffRoleModulePermissions;
        private IRepository<NWC_ZoneStations> _ZoneStations;

        public LoggedInUserService(DbContext context = null)
        {
            var ctx = (context == null ? new NWCContext() : context);

            _unitofWork = new UnitofWork(ctx);
            _userBranchPermission = new Repository<UserBranchPermission>(ctx);
            _userLandmarkPermission = new Repository<NWC_UserLandmarkPermission>(ctx);
            _userServicesPermission = new Repository<NWC_UserServicePermission>(ctx);
            _branch = new Repository<Branch>(ctx);
            this._viewstaffRoleModulePermissions = new Repository<vw_NWC_StaffRoleModulePermissions>(ctx);
            this._ZoneStations = new Repository<NWC_ZoneStations>(ctx);
        }

        public void SetLoggedInUserData(HttpRequest request)
        {
            try
            {
                this.LoggedInUser = new LoggedInUser();
                this._userLandmarksIds = null;
                this._userServicesIds = null;
                this._PermittedBranches = null;
                this._Branches = null;
                this._SubBranches = null;

                this.LoggedInUser.Token = request.Headers.AllKeys.Any(k => k == RequestHeaderKeys.Authorization) ?
                   WebUtility.UrlDecode(request.Headers.GetValues(RequestHeaderKeys.Authorization).FirstOrDefault()) : string.Empty;

                Guid subId;
                if (request.Headers.AllKeys.Any(k => k == RequestHeaderKeys.SubId) &&
                    Guid.TryParse(WebUtility.UrlDecode(request.Headers.GetValues(RequestHeaderKeys.SubId).FirstOrDefault()), out subId))
                {
                    this.LoggedInUser.SubscriberId = subId;
                }

                Guid staffid;
                if (request.Headers.AllKeys.Any(k => k == RequestHeaderKeys.StaffId) &&
                    Guid.TryParse(WebUtility.UrlDecode(request.Headers.GetValues(RequestHeaderKeys.StaffId).FirstOrDefault()), out staffid))
                {
                    this.LoggedInUser.StaffId = staffid;
                }

                this.LoggedInUser.Lang = request.Headers.AllKeys.Any(k => k == RequestHeaderKeys.Lang) ?
                    WebUtility.UrlDecode(request.Headers.GetValues(RequestHeaderKeys.Lang).FirstOrDefault()) : string.Empty;

                //System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
                //    (this.LoggedInUser.Lang == RequestHeaderKeys.ar)
                //    ? new System.Globalization.CultureInfo(LanguagesKeys.Arabic)
                //    : new System.Globalization.CultureInfo(LanguagesKeys.English);

                Thread.CurrentThread.CurrentCulture =
                    (this.LoggedInUser.Lang == RequestHeaderKeys.ar)
                    ? new CultureInfo(LanguagesKeys.Arabic)
                    : new CultureInfo(LanguagesKeys.English);


                //ExceptionManager.GetExceptionLogger().LogInformation($"thread lang: {Thread.CurrentThread.CurrentCulture.Name}");
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LoggedInUserService => SetLoggedInUserData: "));
            }
        }

        public void EmptyData()
        {
            this.LoggedInUser = new LoggedInUser();
            this._userLandmarksIds = null;
            this._userServicesIds = null;
            this._PermittedBranches = null;
            this._Branches = null;
            this._SubBranches = null;

            //ExceptionManager.GetExceptionLogger().LogInformation("***EmptyData***");
        }

        public void SetLoggedInUserData(string token, Guid subID, Guid staffID, Guid? staffRoleID , string lang = null)
        {
            this.LoggedInUser = new LoggedInUser();
            this._userLandmarksIds = null;
            this._userServicesIds = null;
            this._PermittedBranches = null;
            this._Branches = null;
            this._SubBranches = null;

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            try
            {
                this.LoggedInUser.Token = token;
                this.LoggedInUser.SubscriberId = subID;
                this.LoggedInUser.StaffId = staffID;
                this.LoggedInUser.StaffRoleID = staffRoleID;

                if (!string.IsNullOrEmpty(lang))
                    this.LoggedInUser.Lang = lang;

                //CultureInfo.DefaultThreadCurrentCulture =
                //    (this.LoggedInUser.Lang == RequestHeaderKeys.ar)
                //    ? new CultureInfo(LanguagesKeys.Arabic)
                //    : new CultureInfo(LanguagesKeys.English);

                Thread.CurrentThread.CurrentCulture =
                    (this.LoggedInUser.Lang == RequestHeaderKeys.ar)
                    ? new CultureInfo(LanguagesKeys.Arabic)
                    : new CultureInfo(LanguagesKeys.English);

                //test------------------------------------------------------------------------------------------------
                //var text = $"LoggenInService@LoggedInUser.Lang: {this.LoggedInUser.Lang}@thread lang (SearchWorkOrders): {Thread.CurrentThread.CurrentCulture.Name}";
                //text = text.Replace("@", System.Environment.NewLine);
                //ExceptionManager.GetExceptionLogger().LogInformation(text);
                //----------------------------------------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LoggedInUserService => SetLoggedInUserData: "));
            }
        }

        public LoggedInUser LoggedInUser { get; private set; }


        private List<Guid> _userLandmarksIds;
        public List<Guid> UserLandmarksIds
        {
            get
            {
                if (this._userLandmarksIds == null)
                {
                    this._userLandmarksIds = this._userLandmarkPermission.GetQuery()
                                .Where(s => s.StaffId == this.LoggedInUser.StaffId
                                            && !s.IsDeleted
                                            && s.Landmark.isDeleted != true
                                            && s.Landmark.SubId == this.LoggedInUser.SubscriberId)
                                .Select(s => s.LandmarkID).ToList();
                }
                return _userLandmarksIds;
            }
        }

        private List<long> _permittedZonesIds;
        public List<long> PermittedZonesIds
        {
            get
            {
                if (this._permittedZonesIds == null)
                {
                    this._permittedZonesIds = this._ZoneStations.GetQuery()
                                .Where(s => //UserLandmarksIds.Any(a => a == s.StationID)
                                            s.Landmark.NWC_UserLandmarkPermission.Any(a => a.StaffId == this.LoggedInUser.StaffId && !a.IsDeleted)
                                            && s.Landmark.isDeleted != true
                                            && s.Landmark.SubId == this.LoggedInUser.SubscriberId
                                            )
                                .Select(s => s.ZoneID)
                                .Distinct().ToList();
                }
                return _permittedZonesIds;
            }
        }


        private List<Guid> _PermittedBranches;
        public List<Guid> PermittedBranches
        {
            get
            {
                if (this._PermittedBranches == null)
                {
                    this._PermittedBranches = this._userBranchPermission.GetQuery()
                                .Where(s => s.StaffId == this.LoggedInUser.StaffId && !s.IsDeleted)
                                .Select(s => s.BranchId).ToList();
                }
                return _PermittedBranches;
            }
        }

        private List<Guid> _Branches;
        public List<Guid> Branches
        {
            get
            {
                if (this._Branches == null)
                {
                    var permitted = this.PermittedBranches.Select(a => a).ToList();
                    permitted.AddRange(this._branch.GetQuery().Where(s => permitted.Any(a => a == s.Id) && s.parentBranchId != null).Select(x => x.Branch2.Id));

                    this._Branches = this._branch.GetQuery()
                                .Where(s => //permitted.Any(a => a == s.Id || a == s.parentBranchId)
                                            PermittedBranches.Contains(s.Id)
                                            && s.isDeleted != true
                                            && s.SubId == this.LoggedInUser.SubscriberId
                                            && s.parentBranchId == null)
                                .Select(s => s.Id).ToList();
                }
                return _Branches;
            }
        }

        private List<Guid> _SubBranches;
        public List<Guid> SubBranches
        {
            get
            {
                if (this._SubBranches == null)
                {
                    //this._SubBranches = this._branch.GetQuery()
                    //            .Where(s => //PermittedBranches.Any(a => a == s.Id || a == s.parentBranchId)
                    //                        PermittedBranches.Contains(s.Id)
                    //                        && s.isDeleted != true
                    //                        && s.SubId == this.LoggedInUser.SubscriberId
                    //                        && s.parentBranchId != null)
                    //            .Select(s => s.Id).ToList();

                    this._SubBranches = this._branch.GetQuery()
                                .Where(s => s.UserBranchPermission.Any(a => !a.IsDeleted && a.StaffId == this.LoggedInUser.StaffId)
                                            && s.isDeleted != true
                                            && s.SubId == this.LoggedInUser.SubscriberId
                                            && s.parentBranchId != null)
                                .Select(s => s.Id).ToList();
                }
                return _SubBranches;
            }
        }

        private List<int> _userServicesIds;
        public List<int> UserServicesIds
        {
            get
            {
                if (this._userServicesIds == null)
                {
                    this._userServicesIds = this._userServicesPermission.GetQuery()
                                .Where(s => s.StaffId == this.LoggedInUser.StaffId
                                            && !s.IsDeleted
                                            && s.NWC_ServiceType.SubID == this.LoggedInUser.SubscriberId)
                                .Select(s => s.ServiceID).ToList();
                }
                return _userServicesIds;
            }
        }

        private string GetPermission(string pageUniqueName)
        {
            return this._viewstaffRoleModulePermissions.GetQuery()
                .Where(s => s.SubID == LoggedInUser.SubscriberId
                            && s.isDeleted != true
                            && s.StaffRoleID == LoggedInUser.StaffRoleID
                            && s.PageUniqueName == pageUniqueName)
                .Select(a => a.RoleName)
                .FirstOrDefault();
        }

        public bool HasFullControlPermission(string pageUniqueName)
        {
            return this.GetPermission(pageUniqueName) == "Full Control";
        }

        public bool HasAddEditPermission(string pageUniqueName)
        {
            var roleName = this.GetPermission(pageUniqueName);
            return roleName == "Full Control" || roleName == "Add and Edit";
        }

        public bool HasViewPermission(string pageUniqueName)
        {
            var roleName = this.GetPermission(pageUniqueName);
            return roleName == "Full Control" || roleName == "Add and Edit" || roleName == "View Only";
        }

    }
}
