using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System.Collections.Generic;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
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

        #region Contract
        [Route("AddContract")]
        [HttpPost]
        public DescriptiveResponse<long?> AddContract([FromBody] ContractDTO contract)
        {
            OnActionExecuting();

            return DescriptiveResponse<long?>
                .Try(() => _ContractService.AddContract(contract));
        }

        [Route("AddViolationApproval")]
        [HttpPost]
        public DescriptiveResponse<long?> AddViolationApproval([FromBody] ViolationApprovalsDTO contract)
        {
            OnActionExecuting();

            return DescriptiveResponse<long?>
                .Try(() => _ContractService.AddViolationApproval(contract));
        }
        
        [Route("EditContract")]
        [HttpPost]
        public DescriptiveResponse<bool> EditContract([FromBody] ContractDTO contract)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.EditContract(contract));
        }

        [Route("DeleteContract")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteContract([FromBody] long contractId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.DeleteContract(contractId));
        }

        [Route("TerminateContract")]
        [HttpPost]
        public DescriptiveResponse<bool> TerminateContract([FromBody] long contractId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.TerminateContract(contractId));
        }

        [Route("AddViolationDecision")]
        [HttpPost]
        public DescriptiveResponse<bool> AddViolationDecision(ViolationApproveReject ViolationApproveReject)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.AddViolationDecision(ViolationApproveReject.violationId, ViolationApproveReject.Approval));
        }
        #endregion

        #region Tariff
        [Route("AddTariff")]
        [HttpPost]
        public DescriptiveResponse<AddItemsResponse> AddTariff([FromBody] ContractTariffDTO tariff)
        {
            OnActionExecuting();

            return DescriptiveResponse<AddItemsResponse>
                .Try(() => _ContractService.AddTariff(tariff));
        }

        [Route("EditTariff")]
        [HttpPost]
        public DescriptiveResponse<bool> EditTariff([FromBody] ContractTariffDTO tariff)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.EditTariff(tariff));
        }

        [Route("DeleteTariff")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteTariff([FromBody] long tariffId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.DeleteTariff(tariffId));
        }

        [Route("AddTariffRange")]
        [HttpPost]
        public DescriptiveResponse<List<ContractTariffDTO>> AddTariffRange([FromBody] List<ContractTariffDTO> tariffs)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<ContractTariffDTO>>
                .Try(() => _ContractService.AddTariffRange(tariffs));
        }


        #endregion

        #region Station
        [Route("AddStation")]
        [HttpPost]
        public DescriptiveResponse<AddItemsResponse> AddStation(ContractStationDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<AddItemsResponse>
               .Try(() => _ContractService.AddStation(dto));
        }
        [Route("UpdateStation")]
        [HttpPost]
        public DescriptiveResponse<bool> UpdateStation(ContractStationDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
               .Try(() => _ContractService.UpdateStation(dto));
        }

        [Route("DeleteStation")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteStation([FromBody]ContractStationListDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
               .Try(() => _ContractService.DeleteStation(dto));
        }
        #endregion

        #region Price
        [Route("UpdatePriceList")]
        [HttpPut]
        public DescriptiveResponse<bool> UpdatePriceList([FromBody]List<ContractPriceDTO> ContractPriceList)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
               .Try(() => _ContractService.UpdatePriceList(ContractPriceList));
        }
        #endregion

        #region Accessory
        [Route("AddContractAccessories")]
        [HttpPost]
        public DescriptiveResponse<AddItemsResponse> SaveContractAccessories(ContractAccessoryDTO contractAccessoryDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<AddItemsResponse>
               .Try(() => _ContractService.AddContractAccessories(contractAccessoryDTO));
        }

        [Route("UpdateContractAccessory")]
        [HttpPost]
        public DescriptiveResponse<long> UpdateContractAccessory(ContractAccessoryDTO contractAccessoryDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<long>
               .Try(() => _ContractService.UpdateContractAccessory(contractAccessoryDTO));
        }

        [Route("DeleteContractAccessory")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteContractAccessory([FromBody]long contractAccessoryID)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
               .Try(() => _ContractService.DeleteContractAccessory(contractAccessoryID));
        }
        #endregion

        #region Terms
        [Route("AddTerm")]
        [HttpPost]
        public DescriptiveResponse<AddItemsResponse> AddTerm(ContractTermDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<AddItemsResponse>
               .Try(() => _ContractService.AddTerm(dto));
        }
        [Route("UpdateTerm")]
        [HttpPut]
        public DescriptiveResponse<bool> UpdateTerm(ContractTermDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
               .Try(() => _ContractService.UpdateTerm(dto));
        }

        [Route("DeleteTerm")]
        [HttpDelete]
        public DescriptiveResponse<bool> DeleteTerm(long TermID)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
               .Try(() => _ContractService.DeleteTerm(TermID));
        }
        #endregion


        #region Violation
        [Route("AddContractViolation")]
        [HttpPost]
        public DescriptiveResponse<long?> AddContractViolation(ContractTermsViolationsDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<long?>
               .Try(() => _ContractService.AddContractViolation(dto));
        }

        [Route("EditContractViolation")]
        [HttpPost]
        public DescriptiveResponse<bool> EditContractViolation([FromBody] ContractTermsViolationsDTO violation)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.EditContractViolation(violation));
        }

        [Route("DeleteContractViolation")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteContractViolation([FromBody] long violationId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.DeleteContractViolation(violationId));
        }

        [Route("DeleteViolationApproval")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteViolationApproval([FromBody] long violationApprovalId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _ContractService.DeleteViolationApproval(violationApprovalId));
        }
        #endregion


    }
}