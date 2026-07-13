using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
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
        [Route("")]
        public DescriptiveResponse<bool> Add(LandmarkDto dto)
        {
            OnActionExecuting();
            return DescriptiveResponse<bool>.Try(() => _service.Add(dto));
        }

        [HttpPut]
        [Route("{id}")]
        public DescriptiveResponse<bool> Update(Guid id, LandmarkDto dto)
        {
            OnActionExecuting();
            return DescriptiveResponse<bool>.Try(() => _service.Update(dto));
        }

        [HttpDelete]
        [Route("{id}")]
        public DescriptiveResponse<bool> Delete(Guid id)
        {
            OnActionExecuting();
            return DescriptiveResponse<bool>.Try(() => _service.Delete(id));
        }
    }
}
