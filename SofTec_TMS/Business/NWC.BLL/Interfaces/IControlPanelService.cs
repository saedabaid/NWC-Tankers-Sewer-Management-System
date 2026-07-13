using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public partial interface IControlPanelService
    {
        DescriptiveResponse<IEnumerable<BranchSettingDTO>> GetBranchSettings(string branchName);
        DescriptiveResponse<bool> SaveBranchSettings(IEnumerable<BranchSettingDTO> dto);
        DescriptiveResponse<bool> AddArea(AreaDTO dto);
        DescriptiveResponse<bool> EditArea(AreaDTO dto);
        DescriptiveResponse<bool> AddLandmark(AreaDTO dto);
        DescriptiveResponse<bool> EditLandmark(AreaDTO dto);
        DescriptiveResponse<AreaDTO> getAreaById(Guid Id);
        DescriptiveResponse<AreaDTO> getLandmarkById(Guid Id);
        DescriptiveResponse<bool> DeleteArea(Guid Id);
        DescriptiveResponse<bool> DeleteLandmark(Guid Id);

    }
}
