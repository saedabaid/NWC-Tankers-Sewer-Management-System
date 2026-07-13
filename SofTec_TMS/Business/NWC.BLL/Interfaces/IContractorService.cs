using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IContractorService
    {
        IEnumerable<LookUpDTO<long>> GetLite(string searchText, out ReturnResult result);
        DescriptiveResponse<SearchResult<ContractorDTO>> SearchContractorList(ContractorSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractorAttachments(long contractorId);

        DescriptiveResponse<long?> AddContractor(ContractorDTO dto);
        DescriptiveResponse<bool> EditContractor(ContractorDTO dto);


        DescriptiveResponse<bool> ActivateContractor(long id);
        DescriptiveResponse<bool> DeActivateContractor(long id);
        DescriptiveResponse<bool> AddContractorToBlackListed(long id);
        DescriptiveResponse<bool> RemoveContractorFromBlackListed(long id);

    }
}
