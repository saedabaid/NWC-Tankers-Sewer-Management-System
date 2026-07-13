using NWC.DTO.DomainModels;
using System;
using System.Collections.Generic;
using System.Web;

namespace NWC.BLL.Interfaces
{
    public interface ILoggedInUserService
    {
        LoggedInUser LoggedInUser { get; }
        void SetLoggedInUserData(HttpRequest request);
        void SetLoggedInUserData(string token, Guid subID, Guid staffID, Guid? staffRoleID, string lang = null);
        List<Guid> UserLandmarksIds { get; }
        List<long> PermittedZonesIds { get; }
        List<Guid> PermittedBranches { get; }
        List<Guid> Branches { get; }
        List<Guid> SubBranches { get; }
        List<int> UserServicesIds { get; }
    }
}
