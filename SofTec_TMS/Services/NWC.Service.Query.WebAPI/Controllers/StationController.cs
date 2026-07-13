using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
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
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Station")]
    public class StationController : ApiControllerBase
    {
        private IStationService _StationService;

        public StationController()
        {
            this._StationService = new StationService(loggedInService);
        }

        [HttpPost]
        [Route("GetStationNWCSettings")]
        public DescriptiveResponse<SearchResult<StationNWCSettingsDTO>> GetStationNWCSettings(StationSettingsSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<StationNWCSettingsDTO>>
                           .Try(() => _StationService.GetStationNWCSettings(searchCriteria));
        }

        [HttpGet]
        [Route("GetStationNWCSetting")]
        public DescriptiveResponse<StationNWCSettingsDTO> GetStationNWCSetting([FromUri] Guid stationId)
        {
            OnActionExecuting();

            return DescriptiveResponse<StationNWCSettingsDTO>
                           .Try(() => _StationService.GetStationNWCSetting(stationId));
        }


        [HttpGet]
        [Route("GetStationDefaultSizes")]
        public DescriptiveResponse<StationSizesDTO> GetStationDefaultSizes([FromUri] Guid stationId)
        {
            OnActionExecuting();
            return DescriptiveResponse<StationSizesDTO>
                           .Try(() => _StationService.GetStationDefaultSizes(stationId));
        }
        #region Integration
        [HttpGet]
        [Route("IsStationExceededQuota")]
        public DescriptiveResponse<bool> IsStationExceededQuota([FromUri()] Guid id)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _StationService.IsStationExceededQuota(id));
        }
        #endregion
    }
}
