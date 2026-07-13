using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Constants;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Routing;

namespace NWC.Service.Authentication.WebAPI.Infrastructure.Filters
{
    public class AuthenticationTokenFilter : ActionFilterAttribute
    {
        //public override void OnActionExecuting(HttpActionContext actionContext)
        //{
        //    try
        //    {
        //        // HotFix
        //        loggedInService.EmptyData();

        //        if (!actionContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.SubId) ||
        //            !actionContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.StaffId))
        //        {
        //            var token = actionContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.Authorization) ?
        //               actionContext.Request.Headers.GetValues(RequestHeaderKeys.Authorization).FirstOrDefault() : string.Empty;

        //            if (!string.IsNullOrEmpty(token))
        //            {
        //                //AuthenticationDTO tokenObject = Security.Base64Decode(token.Trim().Replace("Basic ", ""));
        //                AuthenticationDTO tokenObject = Security.Base64Decode(token.Trim());

        //                var loginDTO = userService.AuthenticateUser(tokenObject.UserName, tokenObject.Password);

        //                if (!loginDTO.IsErrorState && loginDTO.Value != null)
        //                {
        //                    string lang = actionContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.Lang) ?
        //                        WebUtility.UrlDecode(actionContext.Request.Headers.GetValues(RequestHeaderKeys.Lang).FirstOrDefault()) : string.Empty;

        //                    //tracing lang bug -----------------------------------------------------------------------------------
        //                    if (string.IsNullOrEmpty(lang))
        //                    {
        //                        ExceptionManager.GetExceptionLogger().LogInformation("fire from AuthenticationTokenFilter Header didn't recieve lang key");
        //                    }
        //                    //----------------------------------------------------------------------------------------------------

        //                    loggedInService.SetLoggedInUserData(loginDTO.Value.Token, loginDTO.Value.Context.Account.SubID, loginDTO.Value.Context.Account.StaffID, lang);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var token = actionContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.Authorization) ?
        //              WebUtility.UrlDecode(actionContext.Request.Headers.GetValues(RequestHeaderKeys.Authorization).FirstOrDefault()) : string.Empty;

        //            Guid subId;
        //            Guid.TryParse(WebUtility.UrlDecode(actionContext.Request.Headers.GetValues(RequestHeaderKeys.SubId).FirstOrDefault()), out subId);

        //            Guid staffid;
        //            Guid.TryParse(WebUtility.UrlDecode(actionContext.Request.Headers.GetValues(RequestHeaderKeys.StaffId).FirstOrDefault()), out staffid);

        //            string lang = actionContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.Lang) ?
        //                WebUtility.UrlDecode(actionContext.Request.Headers.GetValues(RequestHeaderKeys.Lang).FirstOrDefault()) : string.Empty;

        //            loggedInService.SetLoggedInUserData(token, subId, staffid, lang);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerManager.LogMsg(c => c.Log(ex));
        //    }
        //}

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                //var context = HttpContext.Current.User.Identity.Name.ToString().Split(',');

                //actionExecutedContext.Request.Headers.

                var token = actionExecutedContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.Authorization) ?
                      actionExecutedContext.Request.Headers.GetValues(RequestHeaderKeys.Authorization).FirstOrDefault() : string.Empty;

                if (string.IsNullOrEmpty(token))
                {
                    token = actionExecutedContext.Request.Headers.Any(k => k.Key == RequestHeaderKeys.token) ?
                        actionExecutedContext.Request.Headers.GetValues(RequestHeaderKeys.token).FirstOrDefault() : string.Empty;
                }


                //if (actionExecutedContext.Response != null && context != null)
                if (!string.IsNullOrEmpty(token))
                {
                    //var tokenobject = new AuthenticationDTO
                    //{
                    //    UserId = context[2],
                    //    UserName = context[0],
                    //    Password = context[1],
                    //    LastActiveTime = DateTimeHelper.GetDateTimeNow().ToString("yyyyMMddHHmmss")
                    //};

                    // actionExecutedContext.Response.Headers.Add("Authorization", string.Concat("Basic ", Security.Base64Encode(tokenobject)));
                    actionExecutedContext.Response.Headers.Add(RequestHeaderKeys.Authorization, token);//Security.Base64Encode(tokenobject));
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
            }
        }
    }
}