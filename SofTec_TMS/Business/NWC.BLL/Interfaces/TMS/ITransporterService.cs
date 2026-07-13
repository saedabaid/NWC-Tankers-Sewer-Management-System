using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface ITransporterService
    {
        DescriptiveResponse<SearchResult<TransporterDTO>> Search(TransporterSC searchCriteria);
        DescriptiveResponse<TransporterDTO> GetTransporterByNumber(string transporterNo);
        DescriptiveResponse<SearchResult<VehicleTypeDTO>> searchVehicleType(VehicleTypeDTO searchCriteria);
        DescriptiveResponse<TransporterDTO> GetOne(Guid id);
        DescriptiveResponse<Guid?> Add(TransporterDTO dto);
        DescriptiveResponse<bool> Edit(TransporterDTO dto);
        DescriptiveResponse<bool> Delete(Guid id);
        DescriptiveResponse<List<TransporterExcelDTO>> AddRange(List<TransporterExcelDTO> dtos);

        DescriptiveResponse<bool> updateVehicleStatus(EditTransporterStatusDTO dto);
        DescriptiveResponse<bool> DeleteVehicleType(Guid Id);

    }
}
