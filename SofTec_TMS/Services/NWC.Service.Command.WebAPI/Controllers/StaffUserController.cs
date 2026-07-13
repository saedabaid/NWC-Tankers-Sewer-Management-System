using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
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

        [HttpPost]
        [Route("SaveUserPermittedLandmarks")]
        public DescriptiveResponse<Boolean> SaveUserPermittedLandmarks(List<UserLandmarkPermissionDTO> userPermittedLandmarks)
        {
            OnActionExecuting();

            var result = this._userService.SaveUserPermittedLandmarks(userPermittedLandmarks);

            return result;
        }
    }
}
