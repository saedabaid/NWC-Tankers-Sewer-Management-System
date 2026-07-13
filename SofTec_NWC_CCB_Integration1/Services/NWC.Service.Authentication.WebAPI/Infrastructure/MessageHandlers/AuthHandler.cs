using NWC.BLL.Services;
using NWC.DTO.Constants;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NWC.Service.Authentication.WebAPI.Infrastructure.MessageHandlers
{
    public class AuthHandler : DelegatingHandler
    {
        #region Properties
        //private IUserService _userService;
        #endregion

        IEnumerable<string> authHeaderValues = null;
        IEnumerable<string> cultureHeaderValues = null;
        MembershipContext membershipCtx = null;

        public AuthHandler()
        {

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.RequestUri.PathAndQuery.StartsWith("/swagger"))
                    return await base.SendAsync(request, cancellationToken);

                request.Headers.TryGetValues(RequestHeaderKeys.Authorization, out authHeaderValues);
                if (authHeaderValues == null || authHeaderValues.Count() < 1 || string.IsNullOrEmpty(authHeaderValues.FirstOrDefault()))
                {
                    request.Headers.TryGetValues(RequestHeaderKeys.token, out authHeaderValues);
                }
                //request.Headers.TryGetValues("Culture", out cultureHeaderValues);


                //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Headers Count: " + (request.Headers.Count()));
                //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Headers Token: " + (request.Headers.Authorization));
                //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Headers host: " + (request.Headers.Host));
                //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Headers AcceptLanguage: " + (request.Headers.AcceptLanguage));
                //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Headers Token: " + (request.Headers.AcceptLanguage));

                //foreach (var item in request.Headers)
                //{
                //    ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Headers key: " + (item.Key));
                //    ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Headers value: " + string.Join(", ", item.Value));
                //}

                //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Authorization Token: " + (authHeaderValues != null ? authHeaderValues.FirstOrDefault() : "Null" ));


                if (cultureHeaderValues != null)
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureHeaderValues.FirstOrDefault());

                if (authHeaderValues == null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return await tsc.Task;
                }
                //return await base.SendAsync(request, cancellationToken);

                var tokens = authHeaderValues.FirstOrDefault();
                tokens = tokens.Trim();

                if (!string.IsNullOrEmpty(tokens))
                {
                    // AuthenticationDTO tokenObject = Security.Base64Decode(tokens.Replace("Basic ", ""));
                    //AuthenticationDTO tokenObject = Security.Base64Decode(tokens);
                    AuthenticationDTO tokenObject = Security.DecryptToken(tokens);

                    var x = new UserService(null, null).ValidateUserAsync(tokenObject.UserName, tokenObject.Password);
                    membershipCtx = x.Context;
                    var dt = DateTime.ParseExact(tokenObject != null ? tokenObject.LastActiveTime : DateTime.Now.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                    if (x == null || x.Context == null)
                    {
                        //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - InternalServerError");

                        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        var tsc = new TaskCompletionSource<HttpResponseMessage>();
                        tsc.SetResult(response);
                        return await tsc.Task;
                    }
                    else if (x.Context.IsValid() && DateTime.Now.Subtract(dt).TotalMinutes <= Constants.TokenExipration)
                    {
                        //IPrincipal principal = x.Context.Principal;
                        //Thread.CurrentPrincipal = principal;
                        //HttpContext.Current.User = principal;

                        //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Subtract-Minutes: {DateTime.Now.Subtract(dt).Minutes} - TokenExipration: {Constants.TokenExipration}");
                    }
                    else // Unauthorized access - wrong crededentials
                    {
                        //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Unauthorized");

                        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        var tsc = new TaskCompletionSource<HttpResponseMessage>();
                        tsc.SetResult(response);
                        return await tsc.Task;
                    }
                }
                else
                {
                    //ExceptionManager.GetExceptionLogger().LogInformation($"SendAsync - Forbidden");

                    var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return await tsc.Task;
                }
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return await tsc.Task;
            }
        }
    }
}