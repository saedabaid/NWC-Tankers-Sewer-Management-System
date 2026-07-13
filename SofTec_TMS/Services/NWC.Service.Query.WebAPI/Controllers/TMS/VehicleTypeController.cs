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

        [Route("searchVehicleType")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<VehicleTypeDTO>> searchVehicleType(VehicleTypeDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<VehicleTypeDTO>>.Try(() => _service.searchVehicleType(searchCriteria));
        }
    }
}