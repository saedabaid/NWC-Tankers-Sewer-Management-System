using ELM_NWC_Integration.Infrastructure.Core;
using Newtonsoft.Json;
using NWC_CCB_Integration.DTO.Common.ELM;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.Models.ELM;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ELM_NWC_Integration.Controllers
{
    [RoutePrefix("api/Tanker")]
    public class TankerController : ApiControllerBase
    {
        #region Properties
        private string CommandAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["CommandAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["CommandAPI_URL"] : string.Empty;
            }
        }
        #endregion
        // GET: Tanker
        [HttpPost]
        [Route("SaveTankerPermit")]
        public DescriptiveResponse<string> SaveTankerPermit([FromBody] TankerDTO TankerDTO)
        {
            try
            {
                if (ActionContext.Request.Headers
                           .Authorization == null)
                {
                    RegisterInfoLog(string.Format("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));

                    return DescriptiveResponse<string>.Error("Authentication is required ", ErrorStatus.ELM_NoToken);
                }
                var token = ActionContext.Request.Headers
                           .Authorization.Scheme;
                // Log
                RegisterInfoLog(string.Format("SaveTankerPermit - Request: {0}", JsonConvert.SerializeObject(TankerDTO)));
                if (string.IsNullOrEmpty(token))
                {
                    RegisterInfoLog(string.Format("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));

                    return DescriptiveResponse<string>.Error("Authentication is required ", ErrorStatus.ELM_NoToken);

                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("Origin", this.CommandAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/tanker/SaveTankerPermit", this.CommandAPI_URL), TankerDTO);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<string>>();
                        readTask.Wait();
                        // Log
                        RegisterInfoLog(string.Format("SaveTankerPermit - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Result)));
                        if (readTask.Result.ResponseCode == (int)ErrorStatus.INPUT_INVALID)
                        {
                            return DescriptiveResponse<string>.Error(readTask.Result.ResponseDescription, ErrorStatus.INPUT_INVALID);

                        }
                        if (readTask.Result.ResponseCode == (int)ErrorStatus.Existsbefore)
                        {
                            return DescriptiveResponse<string>.Error("Exists before", ErrorStatus.Existsbefore);

                        }
                        if (readTask.Result.ResponseCode == (int)ErrorStatus.language_Not_Supported)
                        {
                            return DescriptiveResponse<string>.Error("Not supported language", ErrorStatus.language_Not_Supported);

                        }
                        if (readTask.Result.ResponseCode == (int)ErrorStatus.Invaliddatatype)
                        {
                            return DescriptiveResponse<string>.Error("Invalid data type format", ErrorStatus.Invaliddatatype);

                        }
                        readTask.Result.TimeStamp = DateTime.Now;
                        return readTask.Result;

                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RegisterErrorLog(new Exception("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));
                        return DescriptiveResponse<string>.Error("Sending wrong, expired or invalid authentication token. ", ErrorStatus.EM_UNAUTHORIZED);
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        RegisterErrorLog(new Exception("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));
                        return DescriptiveResponse<string>.Error("Invalid or expired Token, failed to authenticate token", ErrorStatus.expired_Token);
                    }
                    else
                    {
                        // Log
                        RegisterErrorLog(new Exception("SaveTankerPermit - COMMIT_FAIL - Bad Request"));
                        return DescriptiveResponse<string>.Error("Bad Request", ErrorStatus.Bad_Request);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log
                RegisterInfoLog(string.Format("SaveTankerPermit - Exception"));

                RegisterErrorLog(ex);
                return DescriptiveResponse<string>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

        }

        // GET: Tanker
        [HttpPost]
        [Route("TankerAccessAuthorization")]
        public DescriptiveResponse<ELMTransactionDTO> TankerAccessAuthorization([FromBody] TankerAccessDTO TankerDTO)
        {
            try
            {
                if (ActionContext.Request.Headers
                    .Authorization == null)
                {
                    RegisterInfoLog(string.Format("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));

                    return DescriptiveResponse<ELMTransactionDTO>.Error("Authentication is required ", ErrorStatus.ELM_NoToken);
                }
                var token = ActionContext.Request.Headers
                             .Authorization.Scheme;
                // Log
                RegisterInfoLog(string.Format("TankerAccessDTO - Request: {0}", JsonConvert.SerializeObject(TankerDTO)));
                if (string.IsNullOrEmpty(token))
                {
                    RegisterInfoLog(string.Format("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));

                    return DescriptiveResponse<ELMTransactionDTO>.Error("Authentication is required ", ErrorStatus.ELM_NoToken);

                }
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("Origin", this.CommandAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/tanker/TankerAccessAuthorization", this.CommandAPI_URL), TankerDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<ELMTransactionDTO>>();
                        readTask.Wait();

                        // Log
                        RegisterInfoLog(string.Format("TankerAccessAuthorization - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Result)));
                        return readTask.Result;

                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RegisterErrorLog(new Exception(string.Format("SaveTankerPermit - Sending wrong, expired or invalid authentication token.")));

                        return DescriptiveResponse<ELMTransactionDTO>.Error("Sending wrong, expired or invalid authentication token. ", ErrorStatus.EM_UNAUTHORIZED);
                    }
                    else
                    {
                        // Log
                        RegisterErrorLog(new Exception(string.Format("TankerAccessAuthorization - COMMIT_FAIL")));

                        return DescriptiveResponse<ELMTransactionDTO>.Error("Bad Request", ErrorStatus.Bad_Request);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log
                RegisterInfoLog(string.Format("TankerAccessAuthorization - Exception"));

                RegisterErrorLog(ex);
                return DescriptiveResponse<ELMTransactionDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

        }

        // GET: Tanker
        [HttpPost]
        [Route("TankerCheckStatus")]
        public DescriptiveResponse<ELMTransactionDTO> TankerCheckStatus([FromBody] TankerAccessDTO TankerDTO)
        {
            try
            {
                if (ActionContext.Request.Headers
                     .Authorization == null)
                {
                    RegisterInfoLog(string.Format("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));

                    return DescriptiveResponse<ELMTransactionDTO>.Error("Authentication is required ", ErrorStatus.ELM_NoToken);
                }
                var token = ActionContext.Request.Headers
                             .Authorization.Scheme;
                // Log
                RegisterInfoLog(string.Format("TankerAccessDTO - Request: {0}", JsonConvert.SerializeObject(TankerDTO)));
                if (string.IsNullOrEmpty(token))
                {
                    RegisterInfoLog(string.Format("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));

                    return DescriptiveResponse<ELMTransactionDTO>.Error("Authentication is required. ", ErrorStatus.ELM_NoToken);

                }
                TankerDTO.token = token;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("Origin", this.CommandAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/tanker/TankerCheckStatus?baseurl=" + CommandAPI_URL, this.CommandAPI_URL), TankerDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<ELMTransactionDTO>>();
                        readTask.Wait();

                        // Log
                        RegisterInfoLog(string.Format("TankerCheckStatus - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Result)));
                        return readTask.Result;

                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RegisterErrorLog(new Exception("SaveTankerPermit - Sending wrong, expired or invalid authentication token."));
                        return DescriptiveResponse<ELMTransactionDTO>.Error("Sending wrong, expired or invalid authentication token. ", ErrorStatus.EM_UNAUTHORIZED);
                    }
                    else
                    {
                        // Log
                        RegisterErrorLog(new Exception(string.Format("TankerCheckStatus - COMMIT_FAIL")));

                        return DescriptiveResponse<ELMTransactionDTO>.Error("Bad Request", ErrorStatus.Bad_Request);
                    }
                }
            }
            catch (Exception ex)
            {
                RegisterInfoLog(string.Format("AuthenticateUser - Exception"));

                RegisterErrorLog(ex);

                // Log

                return DescriptiveResponse<ELMTransactionDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

        }


    }
}