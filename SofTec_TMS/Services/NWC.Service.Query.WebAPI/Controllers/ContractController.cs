using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System.Collections.Generic;
using System.Web.Http;


namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Contract")]
    public class ContractController : ApiControllerBase
    {
        private IContractService _ContractService;

        public ContractController()
        {
            _ContractService = new ContractService(loggedInService);
        }

        [Route("SearchContractList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ContractDTO>> SearchContractList([FromBody] ContractSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractDTO>>
                .Try(() => _ContractService.SearchContractList(searchCriteria));
        }

        [Route("GetContractAttachments")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractAttachments(long contractId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<AttachmentDTO>>
                .Try(() => _ContractService.GetContractAttachments(contractId));
        }

        [Route("SearchTariffList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ContractTariffDTO>> SearchTariffList([FromBody] Filters<long> searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractTariffDTO>>
                .Try(() => _ContractService.SearchTariffList(searchCriteria));
        }

        [Route("SearchStattionList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ContractStationListDTO>> SearchStationList([FromBody] searchCriteriaContractStationDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractStationListDTO>>
                .Try(() => _ContractService.SearchStationList(searchCriteria));
        }

        [Route("SearchContractAccessories")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ContractAccessoryDTO>> SearchContractAccessories([FromBody] ContractAccessorySC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractAccessoryDTO>>
                .Try(() => _ContractService.SearchContractAccessories(searchCriteria));
        }

        
        [Route("GetContractPriceList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ContractPriceDTO>> GetContractPriceList([FromBody]searchCriteriaContractDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractPriceDTO>>
                .Try(() => _ContractService.GetContractPriceList(searchCriteria));
        }


        [Route("GetContractTermsList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>> GetContractTermsList([FromBody]searchCriteriaContractDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>>
                .Try(() => _ContractService.GetContractTermsList(searchCriteria));
        }

        [Route("GetTermValueUnit")]
        [HttpGet]
        public DescriptiveResponse<vw_NWC_ContractTermsDTO> GetTermValueUnit(long termId)
        {
            OnActionExecuting();

            return DescriptiveResponse<vw_NWC_ContractTermsDTO>
                .Try(() => _ContractService.GetTermValueUnit(termId));
        }

        [Route("GetContractViolations")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ContractTermsViolationsDTO>> GetContractViolations([FromBody] ContractViolationSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractTermsViolationsDTO>>
                .Try(() => _ContractService.GetContractViolations(searchCriteria));
        }

        [Route("GetContractApprovalViolationLogs")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<ContractTermsApprovalViolationsLogsDTO>> GetContractApprovalViolationLogs(long violationId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<ContractTermsApprovalViolationsLogsDTO>>
                .Try(() => _ContractService.GetContractApprovalViolationLogs(violationId));
        }
        
        [Route("GetContractApprovalViolation")]

        [HttpPost]
        public DescriptiveResponse<SearchResult<ViolationApprovalsDTO>> GetContractApprovalViolation([FromBody] ContractApprovalViolation searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ViolationApprovalsDTO>>
                .Try(() => _ContractService.GetContractApprovalViolation(searchCriteria));
        }

        
        [Route("HavePermmissionToDecide")]
        [HttpPost]
        public DescriptiveResponse<bool> HavePermmissionToDecide(long violationId)
        {
            OnActionExecuting();
            return DescriptiveResponse<bool>
                .Try(() => _ContractService.HavePermmissionToDecide(violationId));
        }

        [Route("GetContractViolationsAttachments")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractViolationsAttachments(long violationId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<AttachmentDTO>>
                .Try(() => _ContractService.GetContractViolationsAttachments(violationId));
        }

        [Route("GetContractViolationLogs")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<ContractTermsViolationsLogsDTO>> GetContractViolationLogs(long violationId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<ContractTermsViolationsLogsDTO>>
                .Try(() => _ContractService.GetContractViolationLogs(violationId));
        }

        [Route("GetTermViolationInvoice")]
        [HttpGet]
        public DescriptiveResponse<ContractTermsViolationsInVoiceDTO> GetTermViolationInvoice(long violationId)
        {
            OnActionExecuting();

            return DescriptiveResponse< ContractTermsViolationsInVoiceDTO>
                .Try(() => _ContractService.GetTermViolationInvoice(violationId));
        }

    }
}