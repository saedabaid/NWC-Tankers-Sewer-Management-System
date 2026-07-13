using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    public class OrderController : ApiController
    {

        //[HttpPost]
        //[ActionName("GetOrderList")]
        //[ResponseType(typeof(IEnumerable<OrderDTO>))]
        //public IEnumerable<OrderDTO> GetOrderList(OrderSearchCriteria request)
        //{
        //    try
        //    {
        //        var response = new OrderQuery().GetOrderList(request);

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerManager.LogMsg(c => c.Log(ex));
        //        throw ex;
        //    }
        //}
    }
}
