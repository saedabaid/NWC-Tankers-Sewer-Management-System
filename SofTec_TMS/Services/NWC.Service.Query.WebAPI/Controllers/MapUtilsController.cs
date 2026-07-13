using Newtonsoft.Json.Linq;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace NWC.Service.Query.WebAPI.Controllers

{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/MapUtils")]
    public class MapUtilsController : ApiControllerBase
    {
        private readonly IMapUtilsService _MapUtilsServiceService;

        public MapUtilsController()
        {
            _MapUtilsServiceService = new MapUtilsService(loggedInService);
        }

        #region Actions

        [HttpPost]
        [Route("calculateRoute")]
        public async Task<DescriptiveResponse<DirectionsResponse>> CalculateRoute(DirectionsServiceRequestObject request)
        {
            OnActionExecuting();

            return await _MapUtilsServiceService.CalculateRoute(request);
        }

        [HttpPost]
        [Route("GetDistance")]
        public async Task<DescriptiveResponse<DirectionsResponse>> GetDistance(DirectionsServiceRequestObject requestParams)
        {
            OnActionExecuting();

            return await  _MapUtilsServiceService.GetDistanceAsync( requestParams);
        }




        #endregion
    }
}
