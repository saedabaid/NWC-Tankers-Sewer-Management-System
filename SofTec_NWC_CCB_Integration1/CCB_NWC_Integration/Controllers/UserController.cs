using FromCCBToNWC.API.Infrastructure.Core;
using Newtonsoft.Json;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Helpers;
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
        public DescriptiveResponse<LoginDTO> AuthenticateUser([FromBody] AccountDTO accountDTO)
        {
            var loginDTO = new LoginDTO();
            try
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("AuthenticateUser - Request: {0}", JsonConvert.SerializeObject(accountDTO)));

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Origin", this.authenticationAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/User/AuthenticateUser", this.authenticationAPI_URL), accountDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<LoginDTO>>();
                        readTask.Wait();

                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("AuthenticateUser - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Value)));
                        
                        var returnResult = readTask.Result;

                        loginDTO = readTask.Result.Value;
                    }
                    else
                    {
                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("AuthenticateUser - COMMIT_FAIL"));

                        return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("AuthenticateUser - Exception"));

                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

            return DescriptiveResponse<LoginDTO>.Success(loginDTO);
        }
    }
}
