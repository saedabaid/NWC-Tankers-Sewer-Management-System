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
    [RoutePrefix("api/Vehicle")]
    public class VehicleController : ApiControllerBase
    {
        private IVehicleService _VehicleService;

        public VehicleController()
        {
            this._VehicleService = new VehicleService(loggedInService);
        }


        [HttpPost]
        [Route("GetVehicleNWCSettings")]
        public DescriptiveResponse<SearchResult<VehicleNWCSettingsDTO>> GetVehicleNWCSettings(VehicleSettingsSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<VehicleNWCSettingsDTO>>
                           .Try(() => _VehicleService.GetVehicleNWCSettings(searchCriteria));
        }

        [HttpGet]
        [Route("GetAvailableTankerSizesByZoneIntID")]
        public DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>> GetAvailableTankerSizesByZoneIntID(long zoneID, long defaultZoneID, int classID, int serviceTypeID)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>>
                           .Try(() => _VehicleService.GetAvailableTankerSizesByZoneIntID(zoneID, defaultZoneID, classID, serviceTypeID));
        }

        [HttpGet]
        [Route("GetDefaultTankerSizesByCIS")]
        public DescriptiveResponse<List<int>> GetDefaultTankerSizesByCIS(string CIS)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<int>>
                           .Try(() => _VehicleService.GetDefaultTankerSizesByCIS(CIS));
        }
        [HttpPost]
        [Route("GetVehicleLogReport")]
        public DescriptiveResponse<SearchResult<VehicleLogsDTO>> GetVehicleLogReport(VehicleLogReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<VehicleLogsDTO>>
                           .Try(() => _VehicleService.GetVehicleLogReport(searchCriteria));
        }

        [HttpPost]
        [Route("GetVehicleDataReport")]
        public DescriptiveResponse<SearchResult<VehicleDataDTO>> GetVehicleDataReport(VehicleDataReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<VehicleDataDTO>>
                           .Try(() => _VehicleService.GetVehicleDataReport(searchCriteria));
        }

        [HttpPost]
        [Route("GetVehiclePerformanceReport")]
        public DescriptiveResponse<SearchResult<VehiclePerformanceDTO>> GetVehiclePerformanceReport(VehiclePerformanceReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<VehiclePerformanceDTO>>
                           .Try(() => _VehicleService.GetVehiclePerformanceReport(searchCriteria));
        }

        [Route("GetPermitList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<PermitDTO>> GetPermitList(PermitListSC searchCriteria)
        {
            OnActionExecuting();

            return _VehicleService.GetPermitList(searchCriteria);
        }

        [Route("GetPermit")]
        [HttpGet]
        public DescriptiveResponse<PermitDTO> GetPermit(Guid updateId)
        {
            OnActionExecuting();

            return _VehicleService.GetPermit(updateId);
        }
    }
}
