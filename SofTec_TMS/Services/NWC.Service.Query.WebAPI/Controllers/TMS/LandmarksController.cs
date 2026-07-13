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
    [RoutePrefix("api/landmarks")]
    public class LandmarksController : ApiControllerBase
    {
        private ILandmarksService _service;
        public LandmarksController()
        {
            _service = new LandmarksService(loggedInService);
        }

        [HttpPost]
        [Route("list")]
        public DescriptiveResponse<SearchResult<LandmarkListDto>> Search(LandmarkSearchDto searchCriteria)
        {
            OnActionExecuting();
            return DescriptiveResponse<SearchResult<LandmarkListDto>>.Try(() => _service.Search(searchCriteria));
        }

        [HttpGet]
        [Route("{id}")]
        public DescriptiveResponse<LandmarkDto> GetById(Guid id)
        {
            OnActionExecuting();
            return DescriptiveResponse<LandmarkDto>.Try(() => _service.GetById(id));
        }
    }
}