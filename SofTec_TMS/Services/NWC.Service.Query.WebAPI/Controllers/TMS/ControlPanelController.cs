using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
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


        [Route("SearchCitySettings")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<BranchSettingDTO>> SearchCitySettings(BranchSearchCriteriaDTO searchCriteriaDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<BranchSettingDTO>>
                .Try(() => _controlPanelService.SearchCitySettings(searchCriteriaDTO));
        }

        [Route("GetCitySetting")]
        [HttpGet]
        public DescriptiveResponse<BranchSettingDTO> GetCitySetting(Guid cityId)
        {
            OnActionExecuting();

            return DescriptiveResponse<BranchSettingDTO>
                .Try(() => _controlPanelService.GetCitySetting(cityId));
        }
    }
}
