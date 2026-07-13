using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NWC.Service.Authentication.WebAPI.Models
{
    public class Constants
    {
        public static int TokenExipration = ConfigurationManager.AppSettings["TokenExpireIn"] != null ? int.Parse(ConfigurationManager.AppSettings["TokenExpireIn"]) : 200000000; //default 20 min
    }
}