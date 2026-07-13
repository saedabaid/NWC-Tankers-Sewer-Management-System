using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    public partial class ControlPanelController : ApiControllerBase
    {
        [Route("GetBranchSettings")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<BranchSettingDTO>> GetBranchSettings(string branchName)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<BranchSettingDTO>>
                .Try(() => _controlPanelService.GetBranchSettings(branchName));
        }

        [Route("getAreaById")]
        [HttpGet]
        public DescriptiveResponse<AreaDTO> getAreaById(string id)
        {
            OnActionExecuting();

            return DescriptiveResponse<AreaDTO>.Try(() => _controlPanelService.getAreaById(Guid.Parse(id)));
        }

        [Route("getLandmarkById")]
        [HttpGet]
        public DescriptiveResponse<AreaDTO> getLandmarkById(string id)
        {
            OnActionExecuting();

            return DescriptiveResponse<AreaDTO>.Try(() => _controlPanelService.getLandmarkById(Guid.Parse(id)));
        }
    }
}
