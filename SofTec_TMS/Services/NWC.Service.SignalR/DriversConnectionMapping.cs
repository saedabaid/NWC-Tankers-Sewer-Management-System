using Microsoft.AspNetCore.SignalR;
using NWC.Service.SignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWC.Service.SignalR
{
    public class DriversConnectionMapping<T>
    {
        private readonly Dictionary<T, User> _users = new Dictionary<T, User>();

        public int Count
        {
            get => _users.Count;
        }

        public void Add(T key, HubCallerContext context)
        {
            lock (_users)
            {
                User user;
                if (!_users.TryGetValue(key, out user))
                    _users.Add(key, new User(context));
                else
                    user.Connections.Add(context.ConnectionId);
            }
        }

        public User GetUserInfo(T key) => _users[key];

        public IEnumerable<string> GetUsersConnections(List<T> keys)
        {
            HashSet<string> allConnections = new HashSet<string>();
            foreach (var key in keys)
                allConnections.Concat(GetUserConnections(key));
            return allConnections;
        }

        public IEnumerable<string> GetUserConnections(T key)
        {
            User user;
            if (_users.TryGetValue(key, out user))
                return user.Connections;
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetUserLocationConnections(T key)
        {
            User user;
            if (_users.TryGetValue(key, out user))
                if (!string.IsNullOrEmpty(user.Location))
                    return GetLocationConnections(user.Location);
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetUserLocationConnectionsExceptHim(T key)
        {
            User user;
            if (_users.TryGetValue(key, out user))
                if (!string.IsNullOrEmpty(user.Location))
                    return _users.Values
                        .Where(u => u.Location == user.Location && user.Id != key.ToString())
                        .SelectMany(u => u.Connections).ToHashSet();
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetLocationsConnections(List<string> locations) =>
             locations.SelectMany(l => GetLocationConnections(l)).ToHashSet();

        public IEnumerable<string> GetLocationConnections(string location) =>
            _users.Values.Where(u => u.Location == location).SelectMany(u => u.Connections).ToHashSet();

        public IEnumerable<string> GetLocationConnectionsExceptUser(string location, T key) =>
            _users.Values.Where(u => u.Location == location && u.Id != key.ToString()).SelectMany(u => u.Connections).ToHashSet();

        public IEnumerable<string> GetLocationConnectionsExceptUser(string location, T key, List<T> Keys)
        {
            Keys.Add(key);
         return   _users.Values.Where(u => u.Location == location && !Keys.ToString().Contains(u.Id)).SelectMany(u => u.Connections).ToHashSet();
        }


        public void Remove(T key, string connectionId)
        {
            lock (_users)
            {
                User user;
                if (_users.TryGetValue(key, out user))
                {
                    lock (user)
                    {
                        user.Connections.Remove(connectionId);
                        if (user.Connections.Count == 0)
                            _users.Remove(key);
                    }
                }
            }
        }
    }
}