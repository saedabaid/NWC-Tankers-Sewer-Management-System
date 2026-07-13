using Newtonsoft.Json;
using NWC.BLL.Services;
using NWC.DTO.Constants;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Authentication.WebAPI.Infrastructure.Core
{
    public class ApiControllerBase : ApiController
    {
        protected LoggedInUserService loggedInService;

        /// <summary>
        /// in case we need to handle a database operation 
        /// </summary>
        //protected readonly IAltairUnitOfWork _unitOfWork;
        //protected readonly IGPSUnitOfWork _gpsUnitOfWork;
        //public ApiControllerBase() { }
        //public ApiControllerBase(IAltairUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;

        //}
        //public ApiControllerBase(IGPSUnitOfWork gpsUnitOfWork)
        //{
        //    _gpsUnitOfWork = gpsUnitOfWork;
        //}
        //public ApiControllerBase(IAltairUnitOfWork unitOfWork, IGPSUnitOfWork gpsUnitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //    _gpsUnitOfWork = gpsUnitOfWork;
        //}

        public ApiControllerBase()
        {
            loggedInService = new LoggedInUserService();
        }
        public void OnActionExecuting()
        {
            try
            {
                // HotFix
                loggedInService.EmptyData();
                {
                    var token = Request.Headers.Any(k => k.Key == RequestHeaderKeys.Authorization) ?
                       Request.Headers.GetValues(RequestHeaderKeys.Authorization).FirstOrDefault() : string.Empty;

                    if (string.IsNullOrEmpty(token))
                    {
                        token = Request.Headers.Any(k => k.Key == RequestHeaderKeys.token) ?
                            Request.Headers.GetValues(RequestHeaderKeys.token).FirstOrDefault() : string.Empty;
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        //AuthenticationDTO tokenObject = Security.Base64Decode(token.Trim().Replace("Basic ", ""));
                        //AuthenticationDTO tokenObject = Security.Base64Decode(token.Trim());
                        AuthenticationDTO tokenObject = Security.DecryptToken(token.Trim());

                        var loginDTO = new UserService(this.loggedInService).AuthenticateUser(tokenObject.UserName, tokenObject.Password);

                        if (!loginDTO.IsErrorState && loginDTO.Value != null)
                        {
                            string lang = Request.Headers.Any(k => k.Key == RequestHeaderKeys.Lang) ?
                                WebUtility.UrlDecode(Request.Headers.GetValues(RequestHeaderKeys.Lang).FirstOrDefault()) : string.Empty;

                            //tracing bug -----------------------------------------------------------------------------------
                            //LoggerManager.LogMsg(c => c.TrackingMsg($"Autentication by Token UserName: {tokenObject.UserName}, UserId: {loginDTO.Value.Context.Account.ID}, StaffId: {loginDTO.Value.Context.Account.StaffID}");
                            //----------------------------------------------------------------------------------------------------

                            loggedInService.SetLoggedInUserData(loginDTO.Value.Token, loginDTO.Value.Context.Account.SubID, loginDTO.Value.Context.Account.StaffID, loginDTO.Value.Context.Account.StaffRoleID, lang);
                        }
                        else
                        {
                            //tracing bug -----------------------------------------------------------------------------------
                            LoggerManager.LogMsg(c => c.TrackingMsg("Autentication by Token loginDTO is null or error"));
                            //----------------------------------------------------------------------------------------------------

                        }
                    }
                    else
                    {
                        //tracing bug -----------------------------------------------------------------------------------
                        LoggerManager.LogMsg(c => c.TrackingMsg("Autentication by Token is null or empty"));
                        //----------------------------------------------------------------------------------------------------

                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
            }
        }

        #region Helper Method 
        public bool IsUserAllowToCreateSpecialOrder()
        {

            #region Get Token and UserName
            var token = Request.Headers.Any(k => k.Key == RequestHeaderKeys.Authorization) ?
                         Request.Headers.GetValues(RequestHeaderKeys.Authorization).FirstOrDefault() : string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                token = Request.Headers.Any(k => k.Key == RequestHeaderKeys.token) ?
                    Request.Headers.GetValues(RequestHeaderKeys.token).FirstOrDefault() : string.Empty;
            }
            #endregion

            #region Check - is user in json then true or not then false
            if (!string.IsNullOrEmpty(token))
            {
                //var jsonFiles = ConfigurationManager.AppSettings["jsonFiles"];
                AuthenticationDTO tokenObject = Security.DecryptToken(token.Trim());
                var path = @"c:\jsonFiles\userData.json";
                var jsonFile = System.IO.File.ReadAllText(path);
                List<UserDataPerminent> userData = JsonConvert.DeserializeObject<List<UserDataPerminent>>(jsonFile);

                return userData.Where(x => x.UserName.ToLower() == tokenObject.UserName.ToLower()).Any();
            }
            return false;
            #endregion
        }
        #endregion
        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;
            try
            {
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }

            catch (Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        private void LogError(Exception ex)
        {
            LoggerManager.LogMsg(c => c.Log(ex));
        }
    }

    public class ExcelSheets
    {
        public List<UserDataPerminent> Users { get; set; }
    }

    public class UserDataPerminent
    {
        public string UserName { get; set; }
    }
}