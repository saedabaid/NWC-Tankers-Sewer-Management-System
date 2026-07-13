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
    [RoutePrefix("api/CustomerSMS")]
    public class CustomerSMSController : ApiControllerBase
    {
        private ICustomerSMSService _customerSMSService;

        public CustomerSMSController()
        {
            _customerSMSService = new CustomerSMSService(loggedInService);
        }

        [Route("UpdateSuccessCustomerSMS")]
        [HttpGet]
        public DescriptiveResponse<bool> UpdateSuccessCustomerSMS(long smsID)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _customerSMSService.UpdateSuccessCustomerSMS(smsID));
        }

        [Route("UpdateFailCustomerSMS")]
        [HttpGet]
        public DescriptiveResponse<bool> UpdateFailCustomerSMS(long smsID)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _customerSMSService.UpdateFailCustomerSMS(smsID));
        }
    }
}