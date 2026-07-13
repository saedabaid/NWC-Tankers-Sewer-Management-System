using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NWC.Service.SignalR.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }
        public HashSet<string> Connections = new HashSet<string>();
        public User(HubCallerContext context)
        {
            var nameIdentifierClaim = context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var nameClaim = context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name);
            var givenNameClaim = context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName);
            var localityClaim = context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Locality);
            Id = context.UserIdentifier;
            if (!string.IsNullOrEmpty(nameClaim?.Value)) Username = nameClaim.Value;
            if (!string.IsNullOrEmpty(givenNameClaim?.Value)) FullName = givenNameClaim.Value;
            if (!string.IsNullOrEmpty(localityClaim?.Value)) Location = localityClaim.Value;
            Connections.Add(context.ConnectionId);
        }
    }
}
