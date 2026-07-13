using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiControllerBase
    {
        private IDashboardService _dashboardService;
        private VehicleService _VehicleService;

        public DashboardController()
        {
            this._dashboardService = new DashboardService(loggedInService);
            this._VehicleService = new VehicleService(loggedInService);
        }

        [HttpPost]
        [Route("GetWorkOrdersCountPerStatus")]
        public DescriptiveResponse<int> GetWorkOrdersCountPerStatus([FromBody] DashboardSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<int>
                .Try(() => this._dashboardService.GetWorkOrdersCountPerStatus(searchCriteria));
        }

        [HttpPost]
        [Route("GetVehicleDataReport")]
        public DescriptiveResponse<int> GetVehicleDataReport([FromBody] DashboardSC searchCriteria)
        {
            OnActionExecuting();
   
            return DescriptiveResponse<int>
                .Try(() => DescriptiveResponse<int>.Success(this._VehicleService.GetVehicleDataReport(this._VehicleService.WrapVehicleToDashboardSC(searchCriteria)).Value.Result.Count())) ;
        }

        [HttpPost]
        [Route("GetOrdersCountGroupByDayHours")]
        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByDayHours([FromBody] DashboardSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<DashboardXYChartDTO>>
                .Try(() => this._dashboardService.GetOrdersCountGroupByDayHours(searchCriteria));
        }

        [HttpPost]
        [Route("GetOrdersCountGroupByTop10Zones")]
        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByTop10Zones([FromBody] DashboardSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<DashboardXYChartDTO>>
                .Try(() => this._dashboardService.GetOrdersCountGroupByTop10Zones(searchCriteria));
        }

        [HttpPost]
        [Route("GetOrdersCountGroupByStatus")]
        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByStatus([FromBody] DashboardSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<DashboardXYChartDTO>>
                .Try(() => this._dashboardService.GetOrdersCountGroupByStatus(searchCriteria));
        }

        [HttpPost]
        [Route("GetOrdersCountGroupByDate")]
        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByDate([FromBody] DashboardSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<DashboardXYChartDTO>>
                .Try(() => this._dashboardService.GetOrdersCountGroupByDate(searchCriteria));
        }


        [HttpPost]
        [Route("GetOrdersCountGroupByExecuteTime")]
        public DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByExecuteTime([FromBody] DashboardSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<DashboardXYChartDTO>>
                .Try(() => this._dashboardService.GetOrdersCountGroupByExecuteTime(searchCriteria));
        }

        [HttpPost]
        [Route("GetAreasWithNoPrices")]
        public DescriptiveResponse<SearchResult<ZonePriceListDTO>> GetAreasWithNoPrices([FromBody] ZonePriceSCDTO searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ZonePriceListDTO>>
                .Try(() => this._dashboardService.GetAreasWithNoPrices(searchCriteria));
        }
      
    }
}
