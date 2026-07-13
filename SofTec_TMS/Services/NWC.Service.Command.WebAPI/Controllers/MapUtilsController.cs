using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    public class MapUtilsController : ApiControllerBase
    {

        private readonly IMapUtilsService _MapUtilsServiceService;
        public MapUtilsController()
        {
            _MapUtilsServiceService = new MapUtilsService(loggedInService);
        }


        #region Actions

        [HttpPost]
        [Route("SaveOrderRoute")]
        public DescriptiveResponse<Boolean> SaveOrderRoute(WorkOrderPlannedRoutDTO route )
        {

            OnActionExecuting();

            return _MapUtilsServiceService.SaveOrderRoute(route);
        }





        #endregion

    }
}