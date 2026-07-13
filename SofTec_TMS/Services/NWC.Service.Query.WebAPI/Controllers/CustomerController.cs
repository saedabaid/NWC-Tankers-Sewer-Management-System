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
    [RoutePrefix("api/Customer")]
    public class CustomerController : ApiControllerBase
    {
        private ICustomerService _customerService;

        public CustomerController()
        {
            _customerService = new CustomerService(loggedInService);
        }

        [Route("GetCustomerLocByIntegrationID")]
        [HttpPost]
        public DescriptiveResponse<CustomerLocationDTO> GetCustomerLocByIntegrationID(string integrationID)
        {
            OnActionExecuting();

            return DescriptiveResponse<CustomerLocationDTO>
                           .Try(() => _customerService.GetCustomerLocByIntegrationID(integrationID));
        }


        [Route("SearchCustomerList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<CustomerDTO>> SearchCustomerList(CustomerSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<CustomerDTO>>
                           .Try(() => _customerService.SearchCustomerList(searchCriteria));
        }

        [Route("SearchCustomerAccountList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<CustomerAccountDTO>> SearchCustomerAccountList(CustomerAccountSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<CustomerAccountDTO>>
                           .Try(() => _customerService.SearchCustomerAccountList(searchCriteria));
        }

    }
}