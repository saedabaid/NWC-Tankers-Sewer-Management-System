using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NWC.Service.SignalR.Models;
using System;
using System.Threading.Tasks;

namespace NWC.Service.SignalR.Hubs
{
    [Authorize]
    public class DriversHub : Hub
    {
        #region drivers hub config
        private readonly DriversConnectionMapping<string> _connections;
        public DriversHub(DriversConnectionMapping<string> connections)
        {
            _connections = connections;
        }

        public override async Task OnConnectedAsync()
        {
            _connections.Add(Context.UserIdentifier, Context);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connections.Remove(Context.UserIdentifier, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        #endregion

        public async Task Ping(string msg)
        {
            var user = new User(Context);
            await Clients.All.SendAsync("Ping", $"Pong: {msg} by {user?.Location}");
        }
    }
}
