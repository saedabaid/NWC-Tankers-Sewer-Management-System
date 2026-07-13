using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Station")]
    public class StationController : ApiControllerBase
    {
        private IStationService _stationService;

        public StationController()
        {
            _stationService = new StationService(loggedInService);
        }

        [HttpPost]
        [Route("SaveStationNWCSettings")]
        public DescriptiveResponse<Boolean> SaveStationNWCSettings(StationNWCSettingsDTO dto)
        {
            OnActionExecuting();

            var result = this._stationService.SaveStationNWCSettings(dto);

            return result;
        }

        //[HttpPost]
        //[Route("SaveStationDefaultTankers")]
        //public DescriptiveResponse<Boolean> SaveStationDefaultTankers(StationDefaultTankersDTO dto)
        //{
        //    OnActionExecuting();

        //    return DescriptiveResponse<Boolean>.Try(() => this._stationService.SaveStationDefaultTankers(dto));

        //}
    }
}
