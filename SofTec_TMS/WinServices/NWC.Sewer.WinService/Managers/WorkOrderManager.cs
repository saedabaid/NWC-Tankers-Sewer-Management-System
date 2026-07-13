using NLog;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NWC.Sewer.WinService.Managers
{
    public class WorkOrderManager
    {
        private readonly string _token;

        public WorkOrderManager(string token)
        {
            _token = token;
        }

        public DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>> GetSewerNewWorkOrdersWithZoneDetails(string queryAPI_URL)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("GetSewerNewWorkOrdersWithZoneDetails"));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        if (!returnResult.IsErrorState && returnResult.Value != null)
                            return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>.Success(returnResult.Value);
                        else
                            return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>.Error(returnResult.ErrorDescription);
                    }
                }
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Info, $"This method WorkOrderManager.GetSewerNewWorkOrdersWithZoneDetails return unsuccess response.");

                return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"WorkOrderManager.GetSewerNewWorkOrdersWithZoneDetails : { ex.Message}");

                return DescriptiveResponse<IEnumerable<WorkOrderWithZoneDto>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> CancelSewerWorkOrdersReadyToCancel(string baseUrl, int? retrials, int? holdInterval)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", baseUrl));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", baseUrl);

                    var postTask = client.PostAsJsonAsync(string.Format("CancelSewerWorkOrdersReadyToCancel?retrials={0}&holdInterval={1}", retrials, holdInterval), new { });
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        if (!returnResult.IsErrorState && returnResult.Value)
                            return DescriptiveResponse<bool>.Success(returnResult.Value);
                        else
                            return DescriptiveResponse<bool>.Error(returnResult.ErrorDescription);
                    }
                }
                return DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"WorkOrderManager.CancelSewerWorkOrdersReadyToCancel : { ex.Message}");
                return DescriptiveResponse<bool>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> DeAssignSewerWorkOrdersAfterTimeout(string baseUrl, int period)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", baseUrl));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", baseUrl);

                    var postTask = client.PostAsJsonAsync(string.Format("DeAssignSewerWorkOrdersAfterTimeout?period={0}", period), new { });
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        if (!returnResult.IsErrorState)
                            return DescriptiveResponse<bool>.Success(returnResult.Value);
                        else
                            return DescriptiveResponse<bool>.Error(returnResult.ErrorDescription);
                    }
                }
                return DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"WorkOrderManager.DeAssignSewerWorkOrdersAfterTimeout : { ex.Message}");
                return DescriptiveResponse<bool>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<bool>> ExitSewerVehicleAfterTimeout(string baseUrl, int period)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", baseUrl));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", baseUrl);

                    var postTask = client.PostAsJsonAsync(string.Format("ExitSewerVehicleAfterTimeout?period={0}", period), new { });
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<List<bool>>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        if (!returnResult.IsErrorState)
                            return DescriptiveResponse<List<bool>>.Success(returnResult.Value);
                        else
                            return DescriptiveResponse<List<bool>>.Error(returnResult.ErrorDescription);
                    }
                }
                return DescriptiveResponse<List<bool>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"WorkOrderManager.DeAssignSewerWorkOrdersAfterTimeout : { ex.Message}");
                return DescriptiveResponse<List<bool>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<WorkOrderDTO>> GetSewerPreAssignWorkOrdersReadyToAutoCancel(string queryAPI_URL, int? retrials, int? holdInterval)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("GetSewerPreAssignWorkOrdersReadyToAutoCancel?retrials={0}&holdInterval={1}", retrials, holdInterval));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<WorkOrderDTO>>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        if (!returnResult.IsErrorState && returnResult.Value != null)
                            return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Success(returnResult.Value);
                        else
                            return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Error(returnResult.ErrorDescription);
                    }
                }
                return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"WorkOrderManager.GetSewerPreAssignWorkOrdersReadyToAutoCancel : { ex.Message}");
                return DescriptiveResponse<IEnumerable<WorkOrderDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<List<long>> CancelWorkOrder(string queryAPI_URL, EventWorkOrderDTO eventWorkOrderDTO)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("CancelWorkOrder"), eventWorkOrderDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<List<long>>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        if (!returnResult.IsErrorState && returnResult.Value != null)
                            return DescriptiveResponse<List<long>>.Success(returnResult.Value);
                        else
                            return DescriptiveResponse<List<long>>.Error(returnResult.ErrorDescription);
                    }
                }
                return DescriptiveResponse<List<long>>.Error(ErrorStatus.COMMIT_FAIL);
            }           
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"WorkOrderManager.CancelWorkOrder : { ex.Message}");
                return DescriptiveResponse<List<long>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
    }
}
