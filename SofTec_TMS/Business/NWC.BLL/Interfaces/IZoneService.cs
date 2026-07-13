using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IZoneService
    {
        DescriptiveResponse<SearchResult<ZoneListDTO>> SearchZones(ZoneSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<ZoneDTO> GetZoneDetails(long ZoneId);
        DescriptiveResponse<ZoneDTO> GetZoneByIntegrationID(string ZoneId);
        DescriptiveResponse<bool> Add(ZoneDTO dto);
        DescriptiveResponse<bool> Update(ZoneDTO ZoneDTO);
        DescriptiveResponse<bool> Delete(long id);
        DescriptiveResponse<List<ZoneDTO>> AddRange(List<ZoneDTO> zonesDTO);

        string CallGISService(string premiseCoordinates, string orderNumber, string sourceApp, string transactionId);
    }
}
