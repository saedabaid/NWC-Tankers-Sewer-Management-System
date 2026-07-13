using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using NWC.DTO.SearchCriteria.TMS;
using NWC.DTO.User;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces.TMS
{
    public interface IStaffService
    {
        DescriptiveResponse<SearchResult<StaffListDTO>> SearchStaff(StaffSCDTO searchCriteria);
        DescriptiveResponse<bool> Add(StaffDTO dto);
        DescriptiveResponse<StaffDTO> GetStaffById(Guid id);
        DescriptiveResponse<StaffDTO> GetStaffByPersonalId(string personalId);
        DescriptiveResponse<bool> Update(StaffDTO dto);
        DescriptiveResponse<bool> UpdateDriver(StaffDTO dto);
        DescriptiveResponse<bool> Delete(Guid staffId);
        DescriptiveResponse<FilterResult<StaffRolesDTO>> GetStaffRoles();
        DescriptiveResponse<List<PagesDTO>> GetPages(Guid? key);
        DescriptiveResponse<List<RoleDTO>> GetAllRoles();

        DescriptiveResponse<bool> changePermssion(List<RoleVM> model, StaffRoleDTO StaffRoleDTO,Guid? key);
        DescriptiveResponse<List<StaffExcelDTO>> AddRange(List<StaffExcelDTO> dtos);

    }
}
