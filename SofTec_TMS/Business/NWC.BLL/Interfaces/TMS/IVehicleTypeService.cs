using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IVehicleTypeService
    {
        DescriptiveResponse<SearchResult<VehicleTypeDTO>> searchVehicleType(VehicleTypeDTO searchCriteria);
        DescriptiveResponse<bool> DeleteVehicleType(Guid Id);

    }
}
