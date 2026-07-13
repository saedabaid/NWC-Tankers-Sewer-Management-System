using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NWC.BLL.Interfaces.TMS;
using NWC.BLL.Services.TMS;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using NWC.DTO.SearchCriteria.TMS;
using NWC.DTO.User;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers.TMS
{
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Staff")]
    public class StaffController : ApiControllerBase
    {
        private IStaffService _staffService;

        public StaffController()
        {
            _staffService = new StaffService(loggedInService);
        }
       

        [Route("Search")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<StaffListDTO>> Search([FromBody] StaffSCDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<StaffListDTO>>
                           .Try(() => _staffService.SearchStaff(searchCriteria));
        }

        [Route("GetStaffById")]
        [HttpGet]
        public DescriptiveResponse<StaffDTO> GetStaffById(Guid id)
        {
            OnActionExecuting();

            return DescriptiveResponse<StaffDTO>
                           .Try(() => _staffService.GetStaffById(id));
        }

        [Route("GetStaffByPersonalId")]
        [HttpGet]
        public DescriptiveResponse<StaffDTO> GetStaffByPersonalId(string personalId)
        {
            OnActionExecuting();

            return DescriptiveResponse<StaffDTO>
                           .Try(() => _staffService.GetStaffByPersonalId(personalId));
        }
        [Route("GetStaffRoles")]
        [HttpGet]
        public DescriptiveResponse<FilterResult<StaffRolesDTO>> GetStaffRoles()
        {
            OnActionExecuting();

            return DescriptiveResponse<FilterResult<StaffRolesDTO>>
                           .Try(() => _staffService.GetStaffRoles());
        }
        [Route("GetPages")]
        [HttpGet]
        public DescriptiveResponse<List<PagesDTO>> GetPages([FromUri] Guid? key=null)
        {
            OnActionExecuting();
            if (key == null)
                key = Guid.Empty;
            return DescriptiveResponse<List<PagesDTO>>
                           .Try(() => _staffService.GetPages(key.Value));
        }

        [Route("GetAllRoles")]
        [HttpGet]


        public DescriptiveResponse<List<RoleDTO>> GetAllRoles()
        {
          
            OnActionExecuting();

            return DescriptiveResponse<List<RoleDTO>>
                   .Try(() => _staffService.GetAllRoles());
          


        }

    }
}
