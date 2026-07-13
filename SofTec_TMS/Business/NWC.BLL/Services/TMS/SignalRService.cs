using Newtonsoft.Json;
using NWC.BLL.Interfaces;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Net.Http;
using System.Text;

namespace NWC.BLL.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly string signalRWorkOrdersController = "/work-orders/notifications";
        private readonly ILoggedInUserService _LoggedInUserService;
        private readonly HttpClient httpClient;

        public SignalRService(ILoggedInUserService loggedInUser)
        {
            _LoggedInUserService = loggedInUser;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(KeyConfig.SignalRApiUrl);
        }

        public DescriptiveResponse<bool> WorkOrderCreated(WorkOrderDTO workOrderDTO)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg($"Calling SignalR WorkOrderCreated wo# {0}", workOrderDTO.WorkOrderID));
            if (!KeyConfig.IsSignalREnabled)
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("SignalR not enabled"));
                return new DescriptiveResponse<bool> { IsErrorState = true, ErrorDescription = "SignalR not enabled" };
            }
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", _LoggedInUserService.LoggedInUser.Token);
            workOrderDTO.WorkOrderStatusLogs = null;
            var data = new StringContent(JsonConvert.SerializeObject(workOrderDTO), Encoding.UTF8, "application/json");

            var result = httpClient.PostAsync($"{signalRWorkOrdersController}/WorkOrderCreated", data).Result;
            LoggerManager.LogMsg(c => c.TrackingMsg($"SignalR WorkOrderCreated wo# {0} responded by succeed: {1} => status code {2}", workOrderDTO.WorkOrderID, result.IsSuccessStatusCode, result.StatusCode));
            return new DescriptiveResponse<bool> { IsErrorState = !result.IsSuccessStatusCode, ResponseCode = result.StatusCode, Value = result.IsSuccessStatusCode };
        }

        public DescriptiveResponse<bool> WorkOrderConfirmed(SignalRWorkOrderEvent dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg($"Calling SignalR WorkOrderConfirmed wo# {0}", dto.WorkOrderId));
            if (!KeyConfig.IsSignalREnabled)
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("SignalR not enabled"));
                return new DescriptiveResponse<bool> { IsErrorState = true, ErrorDescription = "SignalR not enabled" };
            }
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", _LoggedInUserService.LoggedInUser.Token);
            var data = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync($"{signalRWorkOrdersController}/WorkOrderConfirmed", data).Result;
            LoggerManager.LogMsg(c => c.TrackingMsg($"SignalR WorkOrderConfirmed wo# {0} responded by succeed: {1} => status code {2}", dto.WorkOrderId, result.IsSuccessStatusCode, result.StatusCode));
            return new DescriptiveResponse<bool> { IsErrorState = !result.IsSuccessStatusCode, ResponseCode = result.StatusCode, Value = result.IsSuccessStatusCode };
        }

        public DescriptiveResponse<bool> WorkOrderAssigned(SignalRWorkOrderEvent dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg($"Calling SignalR WorkOrderAssigned wo# {0}", dto.WorkOrderId));
            if (!KeyConfig.IsSignalREnabled)
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("SignalR not enabled"));
                return new DescriptiveResponse<bool> { IsErrorState = true, ErrorDescription = "SignalR not enabled" };
            }
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", _LoggedInUserService.LoggedInUser.Token);
            var data = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync($"{signalRWorkOrdersController}/WorkOrderAssigned", data).Result;
            LoggerManager.LogMsg(c => c.TrackingMsg($"SignalR WorkOrderAssigned wo# {0} responded by succeed: {1} => status code {2}", dto.WorkOrderId, result.IsSuccessStatusCode, result.StatusCode));
            return new DescriptiveResponse<bool> { IsErrorState = !result.IsSuccessStatusCode, ResponseCode = result.StatusCode, Value = result.IsSuccessStatusCode };
        }

        public DescriptiveResponse<bool> WorkOrderDeAssigned(WorkOrderDTO dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg($"Calling SignalR WorkOrderDeAssigned wo# {0}", dto.WorkOrderID));
            if (!KeyConfig.IsSignalREnabled)
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("SignalR not enabled"));
                return new DescriptiveResponse<bool> { IsErrorState = true, ErrorDescription = "SignalR not enabled" };
            }
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", _LoggedInUserService.LoggedInUser.Token);
            dto.WorkOrderStatusLogs = null;
            var data = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync($"{signalRWorkOrdersController}/WorkOrderDeAssigned", data).Result;
            LoggerManager.LogMsg(c => c.TrackingMsg($"SignalR WorkOrderDeAssigned wo# {0} responded by succeed: {1} => status code {2}", dto.WorkOrderID, result.IsSuccessStatusCode, result.StatusCode));
            return new DescriptiveResponse<bool> { IsErrorState = !result.IsSuccessStatusCode, ResponseCode = result.StatusCode, Value = result.IsSuccessStatusCode };
        }

        public DescriptiveResponse<bool> WorkOrderCancelled(SignalRWorkOrderEvent dto)
        {
            LoggerManager.LogMsg(c => c.TrackingMsg($"Calling SignalR WorkOrderCancelled wo# {0}", dto.WorkOrderId));
            if (!KeyConfig.IsSignalREnabled)
            {
                LoggerManager.LogMsg(c => c.TrackingMsg("SignalR not enabled"));
                return new DescriptiveResponse<bool> { IsErrorState = true, ErrorDescription = "SignalR not enabled" };
            }
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", _LoggedInUserService.LoggedInUser.Token);
            var data = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync($"{signalRWorkOrdersController}/WorkOrderCancelled", data).Result;
            LoggerManager.LogMsg(c => c.TrackingMsg($"SignalR WorkOrderCancelled wo# {0} responded by succeed: {1} => status code {2}", dto.WorkOrderId, result.IsSuccessStatusCode, result.StatusCode));
            return new DescriptiveResponse<bool> { IsErrorState = !result.IsSuccessStatusCode, ResponseCode = result.StatusCode, Value = result.IsSuccessStatusCode };
        }
    }
}
