using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Zone")]
    public class ZoneController : ApiControllerBase
    {
        private IZoneService _zoneService;

        public ZoneController()
        {
            _zoneService = new ZoneService(loggedInService);
        }

        [Route("Search")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<ZoneListDTO>> Search([FromBody] ZoneSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ZoneListDTO>>
                           .Try(() => _zoneService.SearchZones(searchCriteria));
        }

        [Route("GetZoneDetails")]
        [HttpGet]
        public DescriptiveResponse<ZoneDTO> GetZoneDetails(long ZoneId)
        {
            OnActionExecuting();

            return DescriptiveResponse<ZoneDTO>
                           .Try(() => _zoneService.GetZoneDetails(ZoneId));
        }

        [Route("GetZoneByIntegrationID")]
        [HttpGet]
        public DescriptiveResponse<ZoneDTO> GetZoneByIntegrationID(string zoneIntegrationId)
        {
            OnActionExecuting();

            return DescriptiveResponse<ZoneDTO>
                           .Try(() => _zoneService.GetZoneByIntegrationID(zoneIntegrationId));
        }


        [Route("CallGISService")]
        [HttpGet]
        public DescriptiveResponse<ZoneDTO> GetZoneByCallGISService(string premiseCoordinates, string orderNumber, string sourceApp, string transactionId)
        {
            OnActionExecuting();

            var zoneIntegrationId = _zoneService.CallGISService(premiseCoordinates, orderNumber, sourceApp, transactionId);
            if (!string.IsNullOrEmpty(zoneIntegrationId))
            {
                return DescriptiveResponse<ZoneDTO>
                       .Try(() => _zoneService.GetZoneByIntegrationID(zoneIntegrationId));
            }

            return DescriptiveResponse<ZoneDTO>.Error("Empty Response from GIS");
        }
    }
}