using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Contractor")]
    public class ContractorController : ApiControllerBase
    {
        private IContractorService _contractorService;

        public ContractorController()
        {
            _contractorService = new ContractorService(loggedInService);
        }

        [Route("AddContractor")]
        [HttpPost]
        public DescriptiveResponse<long?> AddContractor([FromBody] ContractorDTO contractor)
        {
            OnActionExecuting();

            return DescriptiveResponse<long?>
                .Try(() => _contractorService.AddContractor(contractor));
        }

        [Route("EditContractor")]
        [HttpPost]
        public DescriptiveResponse<bool> EditContractor([FromBody] ContractorDTO contractor)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _contractorService.EditContractor(contractor));
        }

        [Route("ActivateContractor")]
        [HttpPost]
        public DescriptiveResponse<bool> ActivateContractor([FromBody] long contractorId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _contractorService.ActivateContractor(contractorId));
        }

        [Route("DeActivateContractor")]
        [HttpPost]
        public DescriptiveResponse<bool> DeActivateContractor([FromBody] long contractorId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _contractorService.DeActivateContractor(contractorId));
        }

        [Route("AddContractorToBlackListed")]
        [HttpPost]
        public DescriptiveResponse<bool> AddContractorToBlackListed([FromBody] long contractorId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _contractorService.AddContractorToBlackListed(contractorId));
        }

        [Route("RemoveContractorFromBlackListed")]
        [HttpPost]
        public DescriptiveResponse<bool> RemoveContractorFromBlackListed([FromBody] long contractorId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _contractorService.RemoveContractorFromBlackListed(contractorId));
        }

    }
}