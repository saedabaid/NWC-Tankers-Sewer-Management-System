using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
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

        [HttpPost]
        [Route("Add")]
        public DescriptiveResponse<Boolean> Add(ZoneDTO ZoneDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _zoneService.Add(ZoneDTO));

        }

        [HttpPost]
        [Route("Update")]
        public DescriptiveResponse<Boolean> Update([FromBody]ZoneDTO ZoneDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _zoneService.Update(ZoneDTO));
        }

        [HttpPost]
        [Route("Delete")]
        public DescriptiveResponse<Boolean> Delete([FromBody]long id)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _zoneService.Delete(id));

        }

        [HttpPost]
        [Route("AddRange")]
        public DescriptiveResponse<List<ZoneDTO>> AddRange(List<ZoneDTO> ZoneDTO)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<ZoneDTO>> .Try(() => _zoneService.AddRange(ZoneDTO));
        }
    }
}