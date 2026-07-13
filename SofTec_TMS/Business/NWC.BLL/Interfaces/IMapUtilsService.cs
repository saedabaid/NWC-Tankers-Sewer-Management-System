using NWC.DTO.Common;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Interfaces
{
    public interface IMapUtilsService
    {
       // Task<HttpResponseMessage> GetJob(string serviceUrl);
      //  Task<HttpResponseMessage> GetRoutes(string serviceUrl);
      //  Task<HttpResponseMessage> GetStops(string serviceUrl);
        Task<DescriptiveResponse<DirectionsResponse>> CalculateRoute(DirectionsServiceRequestObject requestParams);
        DescriptiveResponse<Boolean> SaveOrderRoute(WorkOrderPlannedRoutDTO route);
        Task<DescriptiveResponse<DirectionsResponse>> GetDistanceAsync(DirectionsServiceRequestObject requestParams);

    }
}
