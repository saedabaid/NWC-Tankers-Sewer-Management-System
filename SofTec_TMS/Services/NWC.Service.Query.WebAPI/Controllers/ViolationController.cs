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
    [RoutePrefix("api/Violation")]
    public class ViolationController : ApiControllerBase
    {
        private IViolationService _violationService;

        public ViolationController()
        {
            _violationService = new ViolationService(loggedInService);
        }

        [Route("GetVehicleViolations")]
        [HttpGet]
        public DescriptiveResponse<SearchResult<VehicleViolationDTO>> GetVehicleViolations(Guid vehicleID)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<VehicleViolationDTO>>
                           .Try(() => _violationService.GetVehicleViolations(vehicleID));
        }
    }
}