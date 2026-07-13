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
    [RoutePrefix("api/User")]
    public class UserController : ApiControllerBase
    {
        private IUserService _userService;
        public UserController()
        {
            this._userService = new UserService(loggedInService);
        }

    }
}
