using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/vehicletype")]
    public class VehicleTypeController : ApiControllerBase
    {
        private IVehicleTypeService _service;

        public VehicleTypeController()
        {
            _service = new VehicleTypeService(loggedInService);
        }

        [Route("deleteVehicleType")]
        [HttpPost]
        public DescriptiveResponse<bool> deleteVehicleType([FromBody] Guid ID)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _service.DeleteVehicleType(ID));
        }
     
    }
}