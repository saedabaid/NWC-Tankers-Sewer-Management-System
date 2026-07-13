using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IVehicleService
    {
        DescriptiveResponse<Boolean> SaveVehicleNWCSettings(List<VehicleNWCSettingsDTO> vehicleNWCSettingsDTOs);
        DescriptiveResponse<string> AddPermit(PermitDTO permitDto);
        DescriptiveResponse<string> UpdateTanker(VehicleDTO permitDto);
        DescriptiveResponse<Boolean> SaveVehicleNWCSettingsBulk(VehicleNWCSettingsBulkUpdateDTO updateModel);
         DescriptiveResponse<List<int>> GetDefaultTankerSizesByCIS(string CIS);
        DescriptiveResponse<SearchResult<VehicleNWCSettingsDTO>> GetVehicleNWCSettings(VehicleSettingsSC searchCriteria);
        DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>> GetAvailableTankerSizesByZoneIntID(long zoneID, long defaultZoneID, int classID, int serviceTypeID);
        DescriptiveResponse<SearchResult<VehicleLogsDTO>> GetVehicleLogReport(VehicleLogReportSC searchCriteria);
        DescriptiveResponse<SearchResult<VehiclePerformanceDTO>> GetVehiclePerformanceReport(VehiclePerformanceReportSC searchCriteria);
        DescriptiveResponse<SearchResult<PermitDTO>> GetPermitList(PermitListSC searchCriteria);
        DescriptiveResponse<PermitDTO> GetPermit(Guid id);
        DescriptiveResponse<SearchResult<VehicleDataDTO>> GetVehicleDataReport(VehicleDataReportSC searchCriteria);
    }
}
