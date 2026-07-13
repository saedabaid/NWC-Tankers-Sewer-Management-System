using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;

namespace NWC.BLL.Interfaces
{
    public partial interface IControlPanelService
    {
        DescriptiveResponse<SearchResult<BranchSettingDTO>> SearchCitySettings(BranchSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<BranchSettingDTO> GetCitySetting(Guid cityId);
        DescriptiveResponse<bool> AddCitySettings(BranchSettingDTO dto);
        DescriptiveResponse<bool> UpdateCitySettings(BranchSettingDTO dto);
    }
}
