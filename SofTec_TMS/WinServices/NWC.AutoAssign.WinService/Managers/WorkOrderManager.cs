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

namespace NWC.AutoAssign.WinService.Managers
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
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : AuthenticateUser Success At : " + DateTime.Now));
                        return DescriptiveResponse<LoginDTO>.Success(userDTO);
                    }
                    else
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : AuthenticateUser Fail At : " + DateTime.Now));
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

        public Task<DescriptiveResponse<SearchResult<WorkOrderDTO>>> GetNotAssignedWorkOrders(string queryAPI_URL, string token, WorkOrderSearchCriteriaDTO searchCriteria, int retrials, int holdInterval)
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

                    var postTask = client.PostAsJsonAsync(string.Format("GetNotAssignedWorkOrders?retrials={0}&holdInterval={1}", retrials, holdInterval), searchCriteria);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<WorkOrderDTO>>>();
                        //readTask.Wait();
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : GetNotAssignedWorkOrders Success At : " + DateTime.Now));
                        return readTask;
                    }
                    else
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : GetNotAssignedWorkOrders Fail At : " + DateTime.Now));
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

        public DescriptiveResponse<SearchResult<StateVehicleDTO>> GetAssignableVehicles(string queryAPI_URL, string token, WorkOderFilter dto)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrderVehicle/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.PostAsJsonAsync("GetAssignableVehicles", dto);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<StateVehicleDTO>>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : GetAssignableVehicles Success At : " + DateTime.Now));
                        return DescriptiveResponse<SearchResult<StateVehicleDTO>>.Success(returnResult.Value);
                    }
                    else
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : GetAssignableVehicles Fail At : " + DateTime.Now));
                    }
                }

                return DescriptiveResponse<SearchResult<StateVehicleDTO>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                return DescriptiveResponse<SearchResult<StateVehicleDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public Task<DescriptiveResponse<Boolean>> AssignedWorkOrders(string commandAPI_URL, string token, DispatchWorkOrderDTO dto)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("AssignWorkOrder", dto);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        //readTask.Wait();
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : AssignedWorkOrders Success At : " + DateTime.Now));
                        return readTask;
                    }
                    else
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : AssignedWorkOrders Fail At : " + DateTime.Now));
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

        public Task<DescriptiveResponse<Boolean>> UpdateWorkOrderRetrials(string commandAPI_URL, string token, long workOrderID, int holdInterval)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PutAsync(string.Format("UpdateWorkOrderAssignRetrials?workOrderID={0}&holdInterval={1}", workOrderID, holdInterval), null);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        readTask.Wait();
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : UpdateWorkOrderRetrials Success At : " + DateTime.Now));
                        return readTask;
                    }
                    else
                    {
                        LoggerManager.LogMsg(c => c.TrackingMsg("Call : UpdateWorkOrderRetrials Fail At : " + DateTime.Now));
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
