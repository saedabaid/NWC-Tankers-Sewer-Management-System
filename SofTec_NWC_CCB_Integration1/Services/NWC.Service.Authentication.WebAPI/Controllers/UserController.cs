
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Models;
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