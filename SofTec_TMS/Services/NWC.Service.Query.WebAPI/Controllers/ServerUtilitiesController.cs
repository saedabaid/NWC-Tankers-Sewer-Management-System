using NWC.DTO.Common;
using NWC.DTO.Helpers;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    [AuthenticationTokenFilter]
    [RoutePrefix("api/ServerUtilities")]
    public class ServerUtilitiesController : ApiControllerBase
    {



        [Route("GetDateTimeNow")]
        [HttpGet]
        public DescriptiveResponse<DateTime> GetDateTimeNow()
        {
            //OnActionExecuting();

            return DescriptiveResponse<DateTime>
                .Try(() => {
                    return DescriptiveResponse<DateTime>.Success(DateTimeHelper.GetDateTimeNow());
                });
        }


    }
}
