using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IStationService
    {
        DescriptiveResponse<Boolean> SaveStationNWCSettings(StationNWCSettingsDTO dto);
        DescriptiveResponse<SearchResult<StationNWCSettingsDTO>> GetStationNWCSettings(StationSettingsSC searchCriteria);
        DescriptiveResponse<StationNWCSettingsDTO> GetStationNWCSetting(Guid stationId);
        DescriptiveResponse<bool> IsStationExceededQuota(Guid StationID);

        DescriptiveResponse<StationSizesDTO> GetStationDefaultSizes(Guid StationID);
    }
}
