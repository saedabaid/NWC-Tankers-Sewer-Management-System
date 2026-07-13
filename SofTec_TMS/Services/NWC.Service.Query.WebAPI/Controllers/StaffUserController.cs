using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.User;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/StaffUser")]
    public class StaffUserController : ApiControllerBase
    {
        private IUserService _userService;

        public StaffUserController()
        {
            this._userService = new UserService(loggedInService);
        }

        [Route("GetUserPermittedLandmarks")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<UserLandmarkPermissionDTO>> GetUserPermittedLandmarks(UserStationPermissionSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<UserLandmarkPermissionDTO>>
                .Try(() => _userService.GetUserPermittedLandmarks(searchCriteria));
        }

    
        [Route("GetUserProfile")]
        [HttpGet]
        public DescriptiveResponse<ProfileDTO> GetUserProfile()
        {
            OnActionExecuting();

            return DescriptiveResponse<ProfileDTO>
                .Try(() => _userService.GetUserProfile());
        }

        [Route("GetUserPermissions")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<StaffPermissionDTO>> GetUserPermissions()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<StaffPermissionDTO>>
                .Try(() => _userService.GetUserPermissions());
        }

    }
}
