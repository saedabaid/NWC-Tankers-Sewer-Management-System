using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IContractService
    {
        DescriptiveResponse<SearchResult<ContractDTO>> SearchContractList(ContractSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<SearchResult<ContractTariffDTO>> SearchTariffList(Filters<long> filter);
        DescriptiveResponse<long?> AddContract(ContractDTO dto);
        DescriptiveResponse<bool> EditContract(ContractDTO dto);
        DescriptiveResponse<bool> DeleteContract(long contractId);
        DescriptiveResponse<bool> TerminateContract(long contractId);
        DescriptiveResponse<AddItemsResponse> AddTariff(ContractTariffDTO dto);
        DescriptiveResponse<bool> EditTariff(ContractTariffDTO dto);
        DescriptiveResponse<bool> DeleteTariff(long tariffId);
        DescriptiveResponse<bool> HavePermmissionToDecide(long violationId);

        DescriptiveResponse<bool> AddViolationDecision(long violationId, bool Approval);
        DescriptiveResponse<long?> AddViolationApproval(ViolationApprovalsDTO contract);

        DescriptiveResponse<List<ContractTariffDTO>> AddTariffRange(List<ContractTariffDTO> tariffsDTO);

        DescriptiveResponse<SearchResult<ContractStationListDTO>> SearchStationList( searchCriteriaContractStationDTO searchCriteria);
        DescriptiveResponse<AddItemsResponse> AddStation(ContractStationDTO dto);
        DescriptiveResponse<bool> DeleteStation(ContractStationListDTO dto);
        DescriptiveResponse<bool> UpdateStation(ContractStationDTO dto);
         DescriptiveResponse<SearchResult<ViolationApprovalsDTO>> GetContractApprovalViolation(ContractApprovalViolation searchCriteria);

        DescriptiveResponse<SearchResult<ContractAccessoryDTO>> SearchContractAccessories(ContractAccessorySC searchCriteria);
        DescriptiveResponse<AddItemsResponse> AddContractAccessories(ContractAccessoryDTO contractAccessoryDTO);
        DescriptiveResponse<long> UpdateContractAccessory(ContractAccessoryDTO contractAccessoryDTO);
        DescriptiveResponse<bool> DeleteContractAccessory(long contractAccessoryID);
        DescriptiveResponse<SearchResult<ContractPriceDTO>> GetContractPriceList(searchCriteriaContractDTO searchCriteria);
        DescriptiveResponse<bool> UpdatePriceList(List<ContractPriceDTO> ContractPriceList);
        DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>> GetContractTermsList(searchCriteriaContractDTO searchCriteria);
        DescriptiveResponse<vw_NWC_ContractTermsDTO> GetTermValueUnit(long termId);
        DescriptiveResponse<AddItemsResponse> AddTerm(ContractTermDTO dto);
        DescriptiveResponse<bool> UpdateTerm(ContractTermDTO dto);
        DescriptiveResponse<bool> DeleteTerm(long TermID);
        DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractAttachments(long contractId);
        DescriptiveResponse<IEnumerable<ContractTermsApprovalViolationsLogsDTO>> GetContractApprovalViolationLogs(long violationId);

        DescriptiveResponse<SearchResult<ContractTermsViolationsDTO>> GetContractViolations(ContractViolationSC searchCriteria);
        DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractViolationsAttachments(long violationId);
        DescriptiveResponse<IEnumerable<ContractTermsViolationsLogsDTO>> GetContractViolationLogs(long violationId);
        DescriptiveResponse<ContractTermsViolationsInVoiceDTO> GetTermViolationInvoice(long violationId);
        DescriptiveResponse<long?> AddContractViolation(ContractTermsViolationsDTO dto);
        DescriptiveResponse<bool> EditContractViolation(ContractTermsViolationsDTO dto);
        DescriptiveResponse<bool> DeleteContractViolation(long violationId);
        DescriptiveResponse<bool> DeleteViolationApproval(long violationApprovalId);

    }
}
