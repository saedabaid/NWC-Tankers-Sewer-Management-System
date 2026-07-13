using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NWC.CancelExceededQuota.WinService.Managers
{
    public class WorkOrderManager
    {
        public NWCContext Context { get; private set; }

        public WorkOrderManager(NWCContext ctx)
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

        public Task<DescriptiveResponse<IEnumerable<CityDTO>>> GetPermittedCities(string queryAPI_URL, string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Lookup/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync("GetPermittedCities");
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<CityDTO>>>();
                        //readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<IEnumerable<CityDTO>>>.FromResult(DescriptiveResponse<IEnumerable<CityDTO>>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return Task<DescriptiveResponse<IEnumerable<CityDTO>>>.FromResult(DescriptiveResponse<IEnumerable<CityDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public Task<DescriptiveResponse<SearchResult<WorkOrderDTO>>> GetWorkOrders(string queryAPI_URL, string token, WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("GetWorkOrdersExceededQuota?retrials={0}&holdInterval={1}", retrials, holdInterval), searchCriteria);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<WorkOrderDTO>>>();
                        //readTask.Wait();

                        return readTask;
                    }
                }
                
                return Task<DescriptiveResponse<SearchResult<WorkOrderDTO>>>.FromResult(DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return Task<DescriptiveResponse<SearchResult<WorkOrderDTO>>>.FromResult(DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public Task<DescriptiveResponse<Boolean>> CancelWorkOrder(string commandAPI_URL, string token, long workOrderID, int statusReasonID, string statusComment)
        {
            try
            {
                var dto = new EventWorkOrderDTO()
                {
                    WorkOrderID = workOrderID,
                    StatusReasonID = statusReasonID,
                    StatusComment = statusComment,
                    StatusTime = DateTimeHelper.GetDateTimeNow()
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("CancelWorkOrder", dto);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        //readTask.Wait();

                        return readTask;
                    }
                }
                
                return Task<DescriptiveResponse<Boolean>>.FromResult(DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return Task<DescriptiveResponse<Boolean>>.FromResult(DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }
    }
}
