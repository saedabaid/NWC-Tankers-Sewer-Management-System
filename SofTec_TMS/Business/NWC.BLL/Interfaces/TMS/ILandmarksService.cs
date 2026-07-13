using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;

namespace NWC.BLL.Interfaces
{
    public interface ILandmarksService
    {
        DescriptiveResponse<SearchResult<LandmarkListDto>> Search(LandmarkSearchDto searchCriteria);
        DescriptiveResponse<LandmarkDto> GetById(Guid id);
        DescriptiveResponse<bool> Add(LandmarkDto dto);
        DescriptiveResponse<bool> Update(LandmarkDto dto);
        DescriptiveResponse<bool> Delete(Guid staffId);
    }
}
