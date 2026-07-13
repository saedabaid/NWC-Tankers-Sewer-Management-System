using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/DeviceMeter")]
    public class DeviceMeterController : ApiControllerBase
    {
        private IDeviceMeterService _deviceMeterService;

        public DeviceMeterController()
        {
            _deviceMeterService = new DeviceMeterService(loggedInService);
        }

        [Route("AddDeviceMeter")]
        [HttpPost]
        public DescriptiveResponse<long?> AddDeviceMeter([FromBody] DeviceMeterDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<long?>
                .Try(() => _deviceMeterService.AddDeviceMeter(dto));
        }

        [Route("AddReading")]
        [HttpPost]
        public DescriptiveResponse<long?> AddReading([FromBody] MeterReadingDTO reading)
        {
            OnActionExecuting();

            return DescriptiveResponse<long?>
                .Try(() => _deviceMeterService.AddReading(reading));
        }

        [Route("DeleteReading")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteReading([FromBody] long readingId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _deviceMeterService.DeleteReading(readingId));
        }

    }
}