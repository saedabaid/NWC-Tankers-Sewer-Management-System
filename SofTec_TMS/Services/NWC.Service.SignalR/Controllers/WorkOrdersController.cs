using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.SignalR.Hubs;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NWC.Service.SignalR.Controllers
{
    [Authorize]
    [ApiController]
    [Route("work-orders/notifications")]
    public class WorkOrdersController : ControllerBase
    {

        private readonly ILogger<TestsController> _logger;
        private readonly IHubContext<DriversHub> _driversHub;
        private readonly DriversConnectionMapping<string> _connections;
        public WorkOrdersController(ILogger<TestsController> logger, IHubContext<DriversHub> driversHub, DriversConnectionMapping<string> connections)
        {
            _logger = logger;
            _driversHub = driversHub;
            _connections = connections;
        }

        [HttpPost]
        [Route("WorkOrderCreated")]
        public DescriptiveResponse<bool> WorkOrderCreated([FromBody] WorkOrderDTO workOrderDTO)
        {
            if (workOrderDTO.CityID.HasValue)
            {
                var connections = _connections.GetLocationConnections(workOrderDTO.CityID.ToString());
                _driversHub.Clients.Clients(connections).SendAsync("WorkOrderCreated", workOrderDTO);
                return new DescriptiveResponse<bool> { Value = true };
            }
            return new DescriptiveResponse<bool> { IsErrorState = true };
        }

        [HttpPost]
        [Route("WorkOrderConfirmed")]
        public DescriptiveResponse<bool> WorkOrderConfirmed([FromBody] SignalRWorkOrderEvent dto)
        {
            if (dto.CityId.HasValue)
            {
                //var userId = GetUserIdFromClaims(HttpContext);
                //var connections = _connections.GetLocationConnectionsExceptUser(dto.CityId.Value.ToString(), userId);
                //_driversHub.Clients.Clients(connections).SendAsync("WorkOrderConfirmed", dto.WorkOrderId);
                return new DescriptiveResponse<bool> { Value = true };
            }
            return new DescriptiveResponse<bool> { IsErrorState = true };
        }

        [HttpPost]
        [Route("WorkOrderAssigned")]
        public DescriptiveResponse<bool> WorkOrderAssigned([FromBody] SignalRWorkOrderEvent dto)
        {
            if (dto.CityId.HasValue)
            {
                var userId = GetUserIdFromClaims(HttpContext);
                var connections = _connections.GetLocationConnectionsExceptUser(dto.CityId.Value.ToString(), userId);
                _driversHub.Clients.Clients(connections).SendAsync("WorkOrderAssigned", dto.WorkOrderId);
                return new DescriptiveResponse<bool> { Value = true };
            }
            return new DescriptiveResponse<bool> { IsErrorState = true };
        }

        [HttpPost]
        [Route("WorkOrderDeAssigned")]
        public DescriptiveResponse<bool> WorkOrderDeAssigned([FromBody] WorkOrderDTO workOrderDTO)
        {
            if (workOrderDTO.CityID.HasValue)
            {
              if(  workOrderDTO.Comments == "Auto")

                    {
                    var connection = _connections.GetUserConnections(workOrderDTO.AssignedDriverID.ToString());
                    _driversHub.Clients.Clients(connection).SendAsync("WorkOrderAssigned", workOrderDTO.WorkOrderID);
                    var connections = _connections.GetLocationConnectionsExceptUser(workOrderDTO.CityID.Value.ToString(), workOrderDTO.AssignedDriverID.ToString(), workOrderDTO.PrevuosAssignedDriverIDs.Select(x => x.ToString()).ToList());
                    _driversHub.Clients.Clients(connections).SendAsync("WorkOrderDeAssigned", workOrderDTO);
                }
                else
                {
                    var userId = GetUserIdFromClaims(HttpContext);
                    var connections = _connections.GetLocationConnectionsExceptUser(workOrderDTO.CityID.Value.ToString(), userId);
                    _driversHub.Clients.Clients(connections).SendAsync("WorkOrderDeAssigned", workOrderDTO);
                }
         
                return new DescriptiveResponse<bool> { Value = true };
            }
            return new DescriptiveResponse<bool> { IsErrorState = true };
        }

        [HttpPost]
        [Route("WorkOrderCancelled")]
        public DescriptiveResponse<bool> WorkOrderCancelled([FromBody] SignalRWorkOrderEvent dto)
        {
            if (dto.CityId.HasValue)
            {
                var connections = _connections.GetLocationConnections(dto.CityId.Value.ToString());
                _driversHub.Clients.Clients(connections).SendAsync("WorkOrderCancelled", dto.WorkOrderId);
                return new DescriptiveResponse<bool> { Value = true };
            }
            return new DescriptiveResponse<bool> { IsErrorState = true };
        }

        private string GetUserIdFromClaims(HttpContext httpContext) => httpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        private string GetLocationFromClaims(HttpContext httpContext) => httpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Locality).Value;
    }
}
