using Infrastructure;
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
    [AuthenticationTokenFilter]
    [RoutePrefix("api/UsersManage")]
    public class UsersManageController : ApiControllerBase
    {
        private IUserService _userService;
        public UsersManageController()
        {
            this._userService = new UserService(loggedInService);
        }

        [HttpPost]
        [Route("Unlock")]

        public DescriptiveResponse<Boolean> Unlock([FromBody] string Name)
        {
            OnActionExecuting();
            var flag = this._userService.Unlock(Name);
            return flag;
        }
        [HttpPost]
        [Route("lock")]

        public DescriptiveResponse<Boolean> Lock([FromBody] string Name)
        {
            OnActionExecuting();
            var flag = this._userService.Lock(Name);
            return flag;
        }
        [HttpPost]
        [Route("Delete")]

        public DescriptiveResponse<Boolean> Delete([FromBody] string Name)
        {
            OnActionExecuting();
            var flag = this._userService.Delete(Name);
            return flag;
        }

        // POST api/Account/ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        public DescriptiveResponse<Boolean> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            OnActionExecuting();
            var flag = this._userService.ChangePassword(model);
            return flag;
        }
    }
}