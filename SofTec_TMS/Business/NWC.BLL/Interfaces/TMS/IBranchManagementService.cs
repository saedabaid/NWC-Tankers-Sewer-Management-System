using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;

namespace NWC.BLL.Interfaces
{
    public interface IBranchManagementService
    {
        DescriptiveResponse<SearchResult<BranchDTO>> SearchBranchesList(BranchSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<BranchDTO> GetBranchDetails(Guid BranchId);

        DescriptiveResponse<bool> Add(BranchDTO dto);

        DescriptiveResponse<bool> Update(BranchDTO dto);

        DescriptiveResponse<bool> Delete(Guid BranchID);

    }
}
