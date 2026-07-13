using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
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

        [Route("UpdateSuccessDriverSMS")]
        [HttpGet]
        public DescriptiveResponse<bool> UpdateSuccessDriverSMS(long smsID)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _driverSMSService.UpdateSuccessDriverSMS(smsID));
        }

        [Route("UpdateFailDriverSMS")]
        [HttpGet]
        public DescriptiveResponse<bool> UpdateFailDriverSMS(long smsID)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _driverSMSService.UpdateFailDriverSMS(smsID));
        }
    }
}