using NWC.BLL.Interfaces.TMS;
using NWC.BLL.Services.TMS;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers.TMS
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

        [HttpPost]
        [Route("Add")]
        public DescriptiveResponse<Boolean> Add(StaffDTO StaffDTO)
        {
            OnActionExecuting();
            
            return DescriptiveResponse<bool>.Try(() => _staffService.Add(StaffDTO));

        }

        [HttpPost]
        [Route("Update")]
        public DescriptiveResponse<Boolean> Update(StaffDTO StaffDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _staffService.Update(StaffDTO));

        }
        [HttpPost]
        [Route("UpdateDriver")]
        public DescriptiveResponse<Boolean> UpdateDriver(StaffDTO StaffDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _staffService.UpdateDriver(StaffDTO));

        }

        
        [Route("{id}")]
        [HttpDelete]
        public DescriptiveResponse<Boolean> Delete(Guid id)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _staffService.Delete(id));
        }

        [HttpPost]
        [Route("changePermssion")]
        public DescriptiveResponse<Boolean> changePermssion([FromBody] Body body, [FromUri] Guid? key)
        {
            OnActionExecuting();
            return DescriptiveResponse<bool>.Try(() => _staffService.changePermssion(body.userDetails,body.roleModel,key));
        }


        [Route("bulk")]
        [HttpPost]
        public DescriptiveResponse<List<StaffExcelDTO>> AddRange(List<StaffExcelDTO> dtos)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<StaffExcelDTO>>.Try(() => _staffService.AddRange(dtos));
        }

    }
}
