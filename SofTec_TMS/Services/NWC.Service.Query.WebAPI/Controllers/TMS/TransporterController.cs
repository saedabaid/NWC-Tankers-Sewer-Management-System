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
    [RoutePrefix("api/transporters")]
    public class TransporterController : ApiControllerBase
    {
        private ITransporterService _service;

        public TransporterController()
        {
            _service = new TransporterService(loggedInService);
        }

        [Route("list")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<TransporterDTO>> Search(TransporterSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<TransporterDTO>>.Try(() => _service.Search(searchCriteria));
        }

        [Route("GetTransporterByNumber")]
        [HttpGet]
        public DescriptiveResponse<TransporterDTO> GetTransporterByNumber(string transporterNo)
        {
            OnActionExecuting();

            return DescriptiveResponse<TransporterDTO>.Try(() => _service.GetTransporterByNumber(transporterNo));
        }

        [Route("searchVehicleType")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<VehicleTypeDTO>> searchVehicleType(VehicleTypeDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<VehicleTypeDTO>>.Try(() => _service.searchVehicleType(searchCriteria));
        }
        [Route("{id}")]
        [HttpGet]
        public DescriptiveResponse<TransporterDTO> GetOne(Guid id)
        {
            OnActionExecuting();

            return DescriptiveResponse<TransporterDTO>.Try(() => _service.GetOne(id));
        }
    }
}