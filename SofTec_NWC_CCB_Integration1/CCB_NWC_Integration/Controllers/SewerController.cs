using FromCCBToNWC.API.Infrastructure.Core;
using Newtonsoft.Json;
using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Logger;
using NWC_CCB_Integration.DTO.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace FromCCBToNWC.API.Controllers
{
    [RoutePrefix("api/sewer")]
    public class SewerController : ApiControllerBase
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

        private string username
        {
            get
            {
                return ConfigurationManager.AppSettings["UserName"] != null ?
                    ConfigurationManager.AppSettings["UserName"] : string.Empty;
            }
        }

        private string password
        {
            get
            {
                return ConfigurationManager.AppSettings["Password"] != null ?
                    ConfigurationManager.AppSettings["Password"] : string.Empty;
            }
        }
        #endregion

        #region "Sewer"
        [HttpPost]
        [Route("ChangeSewerWorkOrderStatus")]
        public DescriptiveResponse<Boolean> ChangeSewerWorkOrderStatus([FromBody] EventSewerWorkOrderDTO request, string token)
        {
            try
            {
                // Log
                ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - Request: {1}", request.StatusName, JsonConvert.SerializeObject(request)));

                using (var client = new HttpClient())
                {
                    #region client Setting
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", this.commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.commandAPI_URL); 
                    #endregion

                    var postTask = GetResponse(request.StatusName, client, request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;

                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - Success - Response: {1}", request.StatusName, JsonConvert.SerializeObject(returnResult.Value)));

                        return readTask.Result;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Log
                        ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - Unauthorized", request.StatusName));

                        return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNAUTHORIZED);
                    }

                    // Log
                    ExceptionManager.GetExceptionLogger().LogInformation(string.Format("ChangeWorkOrderStatus - ActionName: {0} - COMMIT_FAIL", request.StatusName));

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

        #region Not Used
        //[HttpPost]
        //[Route("AssignWorkOrder")]
        public Task<DescriptiveResponse<Boolean>> AssignWorkOrder(string token, DispatchWorkOrderDTO request)
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

                    var postTask = client.PostAsJsonAsync("AssignWorkOrder", request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        //[HttpPost]
        //[Route("SewerConfirmWorkOrder")]
        public Task<DescriptiveResponse<Boolean>> SewerConfirmWorkOrder(string token, SewerConfirmedWorkOrderDTO request)
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

                    var postTask = client.PostAsJsonAsync("SewerConfirmWorkOrder", request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        //[HttpPost]
        //[Route("SewerCompleteWorkOrder")]
        public Task<DescriptiveResponse<Boolean>> SewerCompleteWorkOrder(string token, SewerCompletedWorkOrderDTO request)
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

                    var postTask = client.PostAsJsonAsync("SewerCompleteWorkOrder", request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        //[HttpPost]
        //[Route("SewerArrivedWorkOrder")]
        public Task<DescriptiveResponse<Boolean>> SewerArrivedWorkOrder(string token, EventWorkOrderDTO request)
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

                    var postTask = client.PostAsJsonAsync("ArrivedWorkOrder", request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        #endregion

        #endregion

        #region "Wrap"
        private DispatchWorkOrderDTO GetAssignDTO(EventSewerWorkOrderDTO dto)
        {
            if (dto.DriverID == null || dto.WorkOrderID == 0)
            {
                return null;
            }

            DispatchWorkOrderDTO outDTO = new DispatchWorkOrderDTO();
            var eventWorkOrderDTO = new EventWorkOrderDTO() {WorkOrderID = dto.WorkOrderID,DriverID = dto.DriverID };
           
            outDTO.EventWorkOrderDTO = eventWorkOrderDTO;
            outDTO.EventWorkOrderVehicleDTO = new EventWorkOrderVehicleDTO();
            return outDTO;

        }
        private SewerConfirmedWorkOrderDTO GetConfirmDTO(EventSewerWorkOrderDTO dto)
        {
            if (dto.DriverID == null || dto.WorkOrderID == 0)
            {
                return null;
            }
            //Need to get vechile ID
            return new SewerConfirmedWorkOrderDTO() { WorkOrderID = dto.WorkOrderID, DriverID = dto.DriverID };
        }  
        private SewerCompletedWorkOrderDTO GetCompleteDTO(EventSewerWorkOrderDTO dto)
        {
            if (dto.DriverID == null || dto.WorkOrderID == 0)
            {
                return null;
            }
            //Need to get vechile ID
            return new SewerCompletedWorkOrderDTO() { WorkOrderID = dto.WorkOrderID, DriverID = dto.DriverID };
        }
        private EventWorkOrderDTO GetArrivedDTO(EventSewerWorkOrderDTO dto)
        {
            if (dto.DriverID == null || dto.WorkOrderID == 0)
            {
                return null;
            }
            //Need to get vechile ID
            return new EventWorkOrderDTO() { WorkOrderID = dto.WorkOrderID, DriverID = dto.DriverID };
        }
        #endregion

        private Task<HttpResponseMessage> GetResponse(int actionName, HttpClient client, EventSewerWorkOrderDTO request)
        {
            //var a = GetAssignDTO(request);
           
            switch (actionName)
            {

                case 1://Assign
                    return client.PostAsJsonAsync("AssignWorkOrder", GetAssignDTO(request));
                   
                case 2://SewerConfirmWorkOrder
                    return client.PostAsJsonAsync("SewerConfirmWorkOrder", GetConfirmDTO(request));

                case 3://SewerCompleteWorkOrder
                    return client.PostAsJsonAsync("SewerCompleteWorkOrder", GetCompleteDTO(request));

                case 4://SewerArrivedWorkOrder
                    return client.PostAsJsonAsync("ArrivedWorkOrder", GetArrivedDTO(request));
                default:
                    throw new Exception($"Receive an error action name for order { request.WorkOrderID} ");
            }
        }
    }
}
