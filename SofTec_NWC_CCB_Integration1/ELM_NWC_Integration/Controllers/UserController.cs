using ELM_NWC_Integration.Infrastructure.Core;
using Newtonsoft.Json;
using NWC_CCB_Integration.DTO.Common.ELM;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Helpers;
using NWC_CCB_Integration.DTO.Models.ELM;
using NWC_CCB_Integration.DTO.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FromCCBToNWC.API.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiControllerBase
    {
        #region Properties
        private string authenticationAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthenticationAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["AuthenticationAPI_URL"] : string.Empty;
            }
        }
        #endregion

        [HttpPost]
        [Route("AuthenticateUser")]
        public DescriptiveResponse<ELMLoginDTO> AuthenticateUser([FromBody] ElmAccountDTO accountDTO)
        {
            var loginDTO = new ELMLoginDTO();
            try
            {
                // Log
                RegisterInfoLog(string.Format("AuthenticateUser - Request: {0}", JsonConvert.SerializeObject(accountDTO)));

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Origin", this.authenticationAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/ELMUser/AuthenticateUser", this.authenticationAPI_URL), accountDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<ELMLoginDTO>>();
                        readTask.Wait();

                        // Log
                        RegisterInfoLog(string.Format("AuthenticateUser - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Result)));
                        
                        var returnResult = readTask.Result;
                        if (returnResult.ResponseCode ==(int)ErrorStatus.DeletedUser)
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error("User is deleted", ErrorStatus.DeletedUser);
                        }
                        if (returnResult.ResponseCode == (int)ErrorStatus.blocked)
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error("Account is blocked", ErrorStatus.blocked);
                        }
                        if (returnResult.ResponseCode == (int)ErrorStatus.Bad_Request)
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error("Bad Request", ErrorStatus.Bad_Request);
                        }
                        if ((int)returnResult.ResponseCode == (int)ErrorStatus.WrongUsername)
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error("Incorrect username", ErrorStatus.WrongUsername);
                        }
                        if ((int)returnResult.ResponseCode == (int)ErrorStatus.WrongPassword)
                        {
                            return DescriptiveResponse<ELMLoginDTO>.Error("Incorrect password", ErrorStatus.WrongPassword);
                        }
                        loginDTO = readTask.Result.Result;
                    }
                    else
                    {
                        var content =  result.Content.ReadAsStringAsync();

                        // Log
                        RegisterErrorLog(new Exception(result.StatusCode + "" + content));
                        return DescriptiveResponse<ELMLoginDTO>.Error("Bad Request", ErrorStatus.Bad_Request);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log
                RegisterInfoLog(string.Format("AuthenticateUser - Exception"));

                RegisterErrorLog(ex);
                return DescriptiveResponse<ELMLoginDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

            return DescriptiveResponse<ELMLoginDTO>.Success(loginDTO);
        }
    }
}
