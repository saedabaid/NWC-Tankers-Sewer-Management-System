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
    [RoutePrefix("api/Customer")]
    public class CustomerController : ApiControllerBase
    {
        private ICustomerService _customerService;

        public CustomerController()
        {
            _customerService = new CustomerService(loggedInService);
        }

        [Route("CreateCustomer")]
        [HttpPost]
        public DescriptiveResponse<CustomerDTO> CreateCustomer([FromBody] CustomerDTO customer)
        {
            OnActionExecuting();

            return DescriptiveResponse<CustomerDTO>
                .Try(() => _customerService.CreateCustomer(customer));
        }

        [Route("CreateCustomerLocation")]
        [HttpPost]
        public DescriptiveResponse<CustomerLocationDTO> CreateCustomerLocation([FromBody] CustomerLocationDTO customer)
        {
            OnActionExecuting();

            return DescriptiveResponse<CustomerLocationDTO>
                .Try(() => _customerService.CreateCustomerLocation(customer));
        }

        [Route("CreateCustomerAccount")]
        [HttpPost]
        public DescriptiveResponse<CustomerAccountDTO> CreateCustomerAccount([FromBody] CustomerAccountDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<CustomerAccountDTO>
                .Try(() => _customerService.CreateCustomerAccount(dto));
        }

        [Route("CreateCustomerAndLocations")]
        [HttpPost]
        public DescriptiveResponse<CustomerDTO> CreateCustomerAndLocations([FromBody] CustomerDTO customer)
        {
            OnActionExecuting();

            return DescriptiveResponse<CustomerDTO>
                .Try(() => _customerService.CreateCustomerAndLocations(customer));
        }

        [Route("EditCustomerAndLocations")]
        [HttpPost]
        public DescriptiveResponse<bool> EditCustomerAndLocations([FromBody] CustomerDTO customer)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _customerService.EditCustomerAndLocations(customer));
        }

        [Route("DeleteCustomer")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteCustomer([FromBody] long customerId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _customerService.DeleteCustomer(customerId));
        }


        [Route("CreateCustomerBalance")]
        [HttpPost]
        public DescriptiveResponse<SoqyaCustomerBalanceDTO> CreateCustomerBalance([FromBody] SoqyaCustomerBalanceDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<SoqyaCustomerBalanceDTO>
                .Try(() => _customerService.CreateCustomerBalance(dto));
        }

        //[Route("InsertCustomerAccounts")]
        //[HttpPost]
        //public string InsertCustomerAccounts(object dto)
        //{
        //    OnActionExecuting();

        //    return _customerService.InsertCustomerAccounts();
        //}

        //[Route("UpdateWorkOrders")]
        //[HttpPost]
        //public string UpdateWorkOrders(object dto)
        //{
        //    OnActionExecuting();

        //    return _customerService.UpdateWorkOrders();
        //}
    }
}