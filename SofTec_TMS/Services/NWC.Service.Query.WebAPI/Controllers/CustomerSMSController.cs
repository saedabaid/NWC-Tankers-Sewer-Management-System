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
    [RoutePrefix("api/CustomerSMS")]
    public class CustomerSMSController : ApiControllerBase
    {
        private ICustomerSMSService _customerSMSService;

        public CustomerSMSController()
        {
            _customerSMSService = new CustomerSMSService(loggedInService);
        }

        [Route("GetCustomerSMSs")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<CustomerSMSDTO>> GetCustomerSMSs(CustomerSMSSearchCriteria searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<CustomerSMSDTO>>
                           .Try(() => _customerSMSService.GetCustomerSMSs(searchCriteria));
        }
    }
}