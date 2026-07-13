using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NWC.Service.Command.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            if (Request.Headers.AllKeys.Contains("Origin", StringComparer.CurrentCultureIgnoreCase)
                && Request.HttpMethod == "OPTIONS")
            {
                Response.AddHeader("Access-Control-Allow-Headers", $"Content-Type,X-Requested-With, Accept, Pragma, Cache-Control, {RequestHeaderKeys.Authorization}, {RequestHeaderKeys.StaffId}, {RequestHeaderKeys.SubId}, {RequestHeaderKeys.Lang}");
                Response.End();
            }

            //var loggedInService = (LoggedInUserService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILoggedInUserService));
            //loggedInService.SetLoggedInUserData(Server, Request);
        }
    }
}
