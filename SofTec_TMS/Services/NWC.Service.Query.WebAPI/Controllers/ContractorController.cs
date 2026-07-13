using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
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

        [Route("SearchContractorList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ContractorDTO>> SearchContractorList([FromBody] ContractorSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractorDTO>>
                .Try(() => _contractorService.SearchContractorList(searchCriteria));
        }

        [Route("GetContractorAttachments")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractorAttachments(long contractorId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<AttachmentDTO>>
                .Try(() => _contractorService.GetContractorAttachments(contractorId));
        }





    }
}