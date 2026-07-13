using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{

    [AuthenticationTokenFilter]
    [RoutePrefix("api/User")]

    public class UserController : ApiControllerBase
    {
        private IUserService _userService;

        public UserController()
        {
            _userService = new UserService(loggedInService);
        }

        //[Route("Search")]
        //[HttpPost]
        //public DescriptiveResponse<SearchResult<UserListDTO>> Search([FromBody] UserSearchCriteriaDTO searchCriteria)
        //{
        //    OnActionExecuting();

        //    return DescriptiveResponse<SearchResult<UserListDTO>>
        //                   .Try(() => _userService.SearchUsers(searchCriteria));
        //}

  
    }
}
