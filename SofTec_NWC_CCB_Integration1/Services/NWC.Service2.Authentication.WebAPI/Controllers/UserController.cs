using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Authentication.WebAPI.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiControllerBase
    {
        #region Properties

        private IUserService _userService;

        #endregion

        public UserController()
        {
            this._userService = new UserService(loggedInService);
        }

        //[Authorize]
        //[AuthenticationTokenFilter]
        [Route("staffPermissions")]
        [HttpGet]
        public HttpResponseMessage GetUserAuthenticatePermissions(HttpRequestMessage request, Guid userId, Guid subId)
        {
            //OnActionExecuting();

            ReturnResult result;
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                return response = request.CreateResponse(HttpStatusCode.OK, this._userService.GetUserAuthenticatePermissions(userId, subId, out result));
            });
        }

        //[Authorize]
        //[AuthenticationTokenFilter]
        [Route("AuthenticateUser")]
        [HttpPost]
        public DescriptiveResponse<LoginDTO> AuthenticateUser([FromBody] AccountDTO accountDTO)
        {
            //OnActionExecuting();

            var loginDTO = this._userService.AuthenticateUser(accountDTO.Name, accountDTO.Password);

            return loginDTO;
        }

    }
}