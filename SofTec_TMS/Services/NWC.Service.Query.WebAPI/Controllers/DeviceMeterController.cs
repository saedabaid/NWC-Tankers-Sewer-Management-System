using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/DeviceMeter")]
    public class DeviceMeterController: ApiControllerBase
    {
        private IDeviceMeterService _deviceMeterService;

        public DeviceMeterController()
        {

            _deviceMeterService = new DeviceMeterService(loggedInService);
        }

        [Route("SearchDeviceMeter")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<DeviceMeterDTO>> SearchDeviceMeter([FromBody] DeviceMeterSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<DeviceMeterDTO>>
                .Try(() => _deviceMeterService.SearchDeviceMeter(searchCriteria));
        }

        [Route("SearchReadingList")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<MeterReadingDTO>> SearchReadingList([FromBody] MeterReadingSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<MeterReadingDTO>>
                .Try(() => _deviceMeterService.SearchReadingList(searchCriteria));
        }
    }
}