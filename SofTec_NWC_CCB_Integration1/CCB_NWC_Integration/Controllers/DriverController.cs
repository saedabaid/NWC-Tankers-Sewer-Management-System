using FromCCBToNWC.API.Infrastructure.Core;
using Newtonsoft.Json;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
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
    [RoutePrefix("api/Driver")]
    public class DriverController : ApiControllerBase
    {
        #region Properties
        private string commandAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["CommandAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["CommandAPI_URL"] : string.Empty;
            }
        }

        private string queryAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["QueryAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["QueryAPI_URL"] : string.Empty;
            }
        }
        #endregion

        [HttpPost]
        [Route("GetWorkOrders")]
        public DescriptiveResponse<SearchResult<WorkOrderDTO>> GetWorkOrders([FromBody] WorkOrderSearchCriteriaDTO searchCriteria, string token)
        {
            try
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetWorkOrders - Request: {0}", JsonConvert.SerializeObject(searchCriteria)));

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", this.queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.PostAsJsonAsync("GetDriverWorkOrders", searchCriteria);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<WorkOrderDTO>>>();
                        readTask.Wait();

                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetWorkOrders - Response - TotalCount: {0}", JsonConvert.SerializeObject(readTask.Result.Value.TotalCount)));

                        return readTask.Result;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetWorkOrders - UNAUTHORIZED"));

                        return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.UNAUTHORIZED);
                    }

                    // Log
                    ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetWorkOrders - COMMIT_FAIL"));

                    return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetWorkOrders - Exception"));

                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        [HttpGet]
        [Route("GetOrderBasicDetails")]
        public DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetails(long orderId, string token)
        {
            try
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetails - OrderId: {0}", orderId));

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/WorkOrder/GetDriverWorkOrderDetails?orderId={1}", this.queryAPI_URL, orderId));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<WorkOrderDTO>>();
                        readTask.Wait();

                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetails - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Value)));

                        return readTask.Result;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetails - Unauthorized"));

                        return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.UNAUTHORIZED);
                    }

                    // Log
                    ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetails - COMMIT_FAIL"));

                    return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetails - Exception"));

                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        [HttpGet]
        [Route("GetOrderBasicDetailsByOrderNumber")]
        public DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetailsByOrderNumber(string orderNumber, string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/WorkOrder/GetDriverWODetailsByNumber?orderNumber={1}", this.queryAPI_URL, orderNumber));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<WorkOrderDTO>>();
                        readTask.Wait();

                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetailsByOrderNumber - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Value)));

                        return readTask.Result;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetailsByOrderNumber - UNAUTHORIZED"));

                        return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.UNAUTHORIZED);
                    }

                    // Log
                    ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetailsByOrderNumber - COMMIT_FAIL"));

                    return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetOrderBasicDetailsByOrderNumber - Exception"));

                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        [HttpPost]
        [Route("ChangeWorkOrderStatus")]
        public DescriptiveResponse<Boolean> ChangeWorkOrderStatus([FromBody] EventWorkOrderDTO request, string token)
        {
            try
            {
                string actionName = string.Empty;

                switch (request.StatusID)
                {
                    case 6:// OutForDelivery
                        actionName = "OutForDeliveryWorkOrder";
                        break;
                    case 7:// Arrived
                        actionName = "ArrivedWorkOrder";
                        break;
                    case 4:// Delivered
                        actionName = "DeliveredWorkOrder";
                        break;
                    case 3:// FailedToDeliver
                        actionName = "FailedToDeliver";
                        break;
                }

                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - Request: {1}", actionName, JsonConvert.SerializeObject(request)));

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", this.commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.commandAPI_URL);

                    var postTask = client.PostAsJsonAsync(actionName, request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;

                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - Success - Response: {1}", actionName, JsonConvert.SerializeObject(returnResult.Value)));

                        return readTask.Result;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - Unauthorized", actionName));

                        return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNAUTHORIZED);
                    }

                    // Log
                    ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - COMMIT_FAIL", actionName));

                    return DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - Exception"));

                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        [HttpGet]
        [Route("GetUserProfile")]
        public DescriptiveResponse<ProfileDTO> GetUserProfile(string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/StaffUser/GetUserProfile", this.queryAPI_URL));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<ProfileDTO>>();
                        readTask.Wait();

                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetUserProfile - Response: {0}", JsonConvert.SerializeObject(readTask.Result.Value)));
                        
                        return readTask.Result;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetUserProfile - UNAUTHORIZED"));

                        return DescriptiveResponse<ProfileDTO>.Error(ErrorStatus.UNAUTHORIZED);
                    }

                    // Log
                    ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetUserProfile - COMMIT_FAIL"));

                    return DescriptiveResponse<ProfileDTO>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("GetUserProfile - Exception"));

                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<ProfileDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
    }
}
