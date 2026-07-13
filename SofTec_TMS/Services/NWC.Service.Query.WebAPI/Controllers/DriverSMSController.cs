using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
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
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/DriverSMS")]
    public class DriverSMSController : ApiControllerBase
    {
        private IDriverSMSService _driverSMSService;

        public DriverSMSController()
        {
            _driverSMSService = new DriverSMSService(loggedInService);
        }

        [Route("GetDriverSMSs")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<DriverSMSDTO>> GetDriverSMSs(DriverSMSSearchCriteria searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<DriverSMSDTO>>
                           .Try(() => _driverSMSService.GetDriverSMSs(searchCriteria));
        }
    }
}