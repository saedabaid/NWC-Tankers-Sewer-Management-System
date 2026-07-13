using Newtonsoft.Json;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.CustomerSMS.WinService.SMSServiceRef;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NWC.CustomerSMS.WinService.Managers
{
    public class SMSManager
    {
        public NWCContext Context { get; private set; }

        public SMSManager(NWCContext ctx)
        {
            if (ctx == null)
                Context = new NWCContext();
            else
                Context = ctx;
        }

        public DescriptiveResponse<LoginDTO> AuthenticateUser(string authenticationAPI_URL, string userName, string password)
        {
            var userDTO = new LoginDTO();
            try
            {
                var accountDTO = new AccountDTO()
                {
                    Name = userName,
                    Password = password
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Origin", authenticationAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/User/AuthenticateUser", authenticationAPI_URL), accountDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<LoginDTO>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        userDTO = readTask.Result.Value;

                        return DescriptiveResponse<LoginDTO>.Success(userDTO);
                    }

                    return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return DescriptiveResponse<LoginDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public Task<DescriptiveResponse<SearchResult<CustomerSMSDTO>>> GetCustomerSMSs(string queryAPI_URL, string token, CustomerSMSSearchCriteria searchCriteria)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/CustomerSMS/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.PostAsJsonAsync("GetCustomerSMSs", searchCriteria);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<CustomerSMSDTO>>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<SearchResult<DriverSMSDTO>>>.FromResult(DescriptiveResponse<SearchResult<CustomerSMSDTO>>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return Task<DescriptiveResponse<SearchResult<DriverSMSDTO>>>.FromResult(DescriptiveResponse<SearchResult<CustomerSMSDTO>>.Error(ErrorStatus.COMMIT_FAIL));
            }
        }

        public Task<DescriptiveResponse<Boolean>> UpdateSuccessDriverSMS(string commandAPI_URL, string token, long smsID)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/CustomerSMS/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.GetAsync(string.Format("UpdateSuccessCustomerSMS?smsID={0}", smsID));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<Boolean>>.FromResult(DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return Task<DescriptiveResponse<Boolean>>.FromResult(DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL));
            }
        }

        public Task<DescriptiveResponse<Boolean>> UpdateFailDriverSMS(string commandAPI_URL, string token, long smsID)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/CustomerSMS/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.GetAsync(string.Format("UpdateFailCustomerSMS?smsID={0}", smsID));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<Boolean>>.FromResult(DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return Task<DescriptiveResponse<Boolean>>.FromResult(DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL));
            }
        }

        public bool SendCustomerSMS(long smsID, string customerMobNo, string orderNumber, string smsText, string smsLang, int smsType, string eventSource = "TMS", string eventPriority = "9")
        {
            var smsClient = new SmsWebServiceClient();

            try
            {
                var rec = new rcpntDtls()
                {
                    Ntfctn_Mthd = "10000",
                    Mbl_Tel = customerMobNo,
                };

                if (!string.IsNullOrEmpty(smsLang))
                    rec.Ntfctn_Lng = smsLang;

                var smsRequest = new eaiMessage()
                {
                    SMS = smsText,
                    Evnt_Id = string.Format("TMSC{0}", smsID),
                    Evnt_Source = eventSource,
                    Evnt_Priority = eventPriority,
                    SMS_Type = smsType,
                    //Due_Dt = DateTimeHelper.GetDateTimeNow().ToString("dd-MM-yyyy HH:mm:ss"),
                    Rcpnt_Lst = new rcpntDtls[1] { rec }
                };

                if (smsType > 0)
                    smsRequest.SMS_Type = smsType;

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CustomerSMS: SMS Request: {0}", JsonConvert.SerializeObject(smsRequest))));

                if (smsClient.State != System.ServiceModel.CommunicationState.Opened)
                    smsClient.Open();

                var smsResponse = smsClient.sendAsync(smsRequest).Result;

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("CustomerSMS: SMS Response: {0}", JsonConvert.SerializeObject(smsResponse))));

                if (smsResponse != null && smsResponse.Status != null && smsResponse.Status.ToUpper() == "OK")
                    return true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
            }
            finally
            {
                smsClient.Close();
            }
            return false;
        }

        public int RandomNumber(int min, int max)
        {
            Random _random = new Random();
            return _random.Next(min, max);
        }
    }
}
