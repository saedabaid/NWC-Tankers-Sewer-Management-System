using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Interfaces.ELM;
using NWC.BLL.Services;
using NWC.BLL.Services.ELM;
using NWC.DTO.Common.ELM;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Authentication.WebAPI.Controllers.ELM
{
    [RoutePrefix("api/ELMUser")]
    public class ElmUserController : ApiControllerBase
    {
        #region Properties

        private IELMUserService _userService;

        #endregion

        public ElmUserController()
        {
            this._userService = new ELMUserService(loggedInService);
        }

        [Route("AuthenticateUser")]
        [HttpPost]
        public DescriptiveResponse<ELMLoginDTO> AuthenticateUser([FromBody] ElmAccountDTO accountDTO)
        {
            //OnActionExecuting();

            var loginDTO = this._userService.AuthenticateUser(accountDTO.UserName, accountDTO.Password);

            return loginDTO;
        }

    }
}
