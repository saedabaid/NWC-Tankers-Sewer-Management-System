using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IViolationService
    {
        DescriptiveResponse<SearchResult<VehicleViolationDTO>> GetVehicleViolations(Guid vehicleID);
    }
}
