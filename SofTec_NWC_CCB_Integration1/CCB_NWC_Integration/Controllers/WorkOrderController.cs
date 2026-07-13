using FromCCBToNWC.API.Infrastructure.Core;
using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace FromCCBToNWC.API.Controllers
{
    [RoutePrefix("api/WorkOrder")]
    public class WorkOrderController : ApiControllerBase
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
            var workOrderDTOs = new SearchResult<WorkOrderDTO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", this.queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.PostAsJsonAsync("Search", searchCriteria);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<WorkOrderDTO>>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;

                        workOrderDTOs = readTask.Result.Value;
                    }
                    else
                    {
                        return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

            return DescriptiveResponse<SearchResult<WorkOrderDTO>>.Success(workOrderDTOs);
        }

        [HttpGet]
        [Route("GetOrderBasicDetails")]
        public DescriptiveResponse<WorkOrderDTO> GetOrderBasicDetails(long orderId, string token)
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

                    var postTask = client.GetAsync(string.Format("{0}/api/WorkOrder/GetMobileWorkOrderDetails?orderId={1}", this.queryAPI_URL, orderId));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<WorkOrderDTO>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
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

                    var postTask = client.GetAsync(string.Format("{0}/api/WorkOrder/GetMobileWODetailsByOrderNumber?orderNumber={1}", this.queryAPI_URL, orderNumber));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<WorkOrderDTO>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<WorkOrderDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        //[HttpPost]
        //[Route("ChangeWorkOrderStatus")]
        //public DescriptiveResponse<Boolean> ChangeWorkOrderStatus([FromBody] EventWorkOrderDTO request, string token)
        //{
        //    try
        //    {
        //        string actionName = string.Empty;

        //        switch (request.StatusID)
        //        {
        //            case 16://WorkOrder_OutForDelivery
        //                actionName = "OutForDeliveryWorkOrder";
        //                break;
        //            case 17://WorkOrder_Arrived
        //                actionName = "ArrivedWorkOrder";
        //                break;
        //            case 18://WorkOrder_Delivered
        //                actionName = "DeliveredWorkOrder";
        //                break;
        //            case 20://WorkOrder_FailedToDeliver
        //                actionName = "FailedToDeliver";
        //                break;
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", this.commandAPI_URL));
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            client.DefaultRequestHeaders.Add("Authorization", token);
        //            client.DefaultRequestHeaders.Add("IsIntigration", "true");
        //            client.DefaultRequestHeaders.Add("Lang", "en");
        //            client.DefaultRequestHeaders.Add("Origin", this.commandAPI_URL);

        //            var postTask = client.PostAsJsonAsync(actionName, request);
        //            postTask.Wait();

        //            var result = postTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
        //                readTask.Wait();

        //                var returnResult = readTask.Result;

        //                return readTask.Result;
        //            }
        //            else
        //            {
        //                return DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.GetExceptionLogger().LogException(ex);
        //        return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
        //    }
        //}

        [HttpPost]
        [Route("CreateWorkOrder")]
        public XElement CreateWorkOrder(HttpRequestMessage request)
        {
            try
            {
                ServiceReference1.WorkOrderClient client = new ServiceReference1.WorkOrderClient();

                var requestXml = XElement.Load(Request.Content.ReadAsStreamAsync().Result);
                return client.CreateWorkOrder(requestXml);
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return null;
            }
        }

        [HttpPost]
        [Route("UpdateWorkOrder")]
        public XElement UpdateWorkOrder(HttpRequestMessage request)
        {
            try
            {
                ServiceReference1.WorkOrderClient client = new ServiceReference1.WorkOrderClient();

                var requestXml = XElement.Load(Request.Content.ReadAsStreamAsync().Result);
                return client.UpdateWorkOrder(requestXml);
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return null;
            }
        }

        [HttpPost]
        [Route("Test")]
        public NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Output Test(HttpRequestMessage request)
        {
            try
            {
                ServiceReference2.TankerClient client = new ServiceReference2.TankerClient();

                var requestXml = XElement.Load(Request.Content.ReadAsStreamAsync().Result);

                var obj = new NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Schema();
                obj.Input = new NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Input()
                {
                    LONGLAT = ""
                };

                return client.GetAvailableTankerSizes(obj);
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return null;
            }
        }
    }
}
