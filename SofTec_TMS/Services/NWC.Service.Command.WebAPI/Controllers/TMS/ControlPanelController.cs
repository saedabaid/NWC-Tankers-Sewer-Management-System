using NWC.BLL.Interfaces;
using NWC.BLL.Services;
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

namespace NWC.Service.Query.WebAPI.Controllers
{
    [AuthenticationTokenFilter]
    [RoutePrefix("api/ControlPanel")]
    public partial class ControlPanelController : ApiControllerBase
    {
        private IControlPanelService _controlPanelService;

        public ControlPanelController()
        {
            _controlPanelService = new ControlPanelService(loggedInService);
        }

        [Route("AddCitySettings")]
        [HttpPost]
        public DescriptiveResponse<bool> AddCitySettings(BranchSettingDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.AddCitySettings(dto));
        }

        [Route("UpdateCitySettings")]
        [HttpPut]
        public DescriptiveResponse<bool> UpdateCitySettings(BranchSettingDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.UpdateCitySettings(dto));
        }
        [Route("SaveBranchSettings")]
        [HttpPost]
        public DescriptiveResponse<bool> SaveBranchSettings(IEnumerable<BranchSettingDTO> dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.SaveBranchSettings(dto));
        }


        [Route("AddArea")]
        [HttpPost]
        public DescriptiveResponse<bool> AddArea(AreaDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.AddArea(dto));
        }

        [Route("EditArea")]
        [HttpPost]
        public DescriptiveResponse<bool> EditArea(AreaDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.EditArea(dto));
        }

        [Route("DeleteArea")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteArea(AreaDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.DeleteArea(dto.Id));
        }

        [Route("AddLandmark")]
        [HttpPost]
        public DescriptiveResponse<bool> AddLandmark(AreaDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.AddLandmark(dto));
        }

        [Route("EditLandmark")]
        [HttpPost]
        public DescriptiveResponse<bool> EditLandmark(AreaDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.EditLandmark(dto));
        }

        [Route("DeleteLandmark")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteLandmark(AreaDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _controlPanelService.DeleteLandmark(dto.Id));
        }
    }
}
