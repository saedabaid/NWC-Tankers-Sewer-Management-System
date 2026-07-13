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
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Report")]
    public class ReportController : ApiControllerBase
    {
        private IReportService _reportService;

        public ReportController()
        {
            this._reportService = new ReportService(loggedInService);
        }


        [HttpPost]
        [Route("GetOrdersPerZone")]
        public DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetOrdersPerZone([FromBody] ReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<Report_OrderPerZone>>
                .Try(() => this._reportService.GetOrdersPerZone(searchCriteria));
        }

        [HttpPost]
        [Route("GetStationOrderCapacity")]
        public DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetStationOrderCapacity([FromBody] ReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<Report_OrderPerZone>>
                .Try(() => this._reportService.GetStationOrderCapacity(searchCriteria));
        }

        [HttpPost]
        [Route("GetStationServiceTime")]
        public DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetStationServiceTime([FromBody] ReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<Report_OrderPerZone>>
                .Try(() => this._reportService.GetStationServiceTime(searchCriteria));
        }

        [HttpPost]
        [Route("GetTankerPermissionStatus")]
        public DescriptiveResponse<SearchResult<Report_TankersPermissionsStatus>> GetTankerPermissionStatus([FromBody] ReportTankersPermissionsStatusSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<Report_TankersPermissionsStatus>>
                .Try(() => this._reportService.GetTankerPermissionStatus(searchCriteria));
        }

        [HttpPost]
        [Route("GetSoqyaSchedulePerDay")]
        public DescriptiveResponse<IEnumerable<Report_SoqyaScheduledPerDay>> GetSoqyaSchedulePerDay([FromBody] ReportScheduledPerDaySC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<Report_SoqyaScheduledPerDay>>
                .Try(() => this._reportService.GetSoqyaSchedulePerDay(searchCriteria));
        }

        [HttpPost]
        [Route("ContractTariffReport")]
        public DescriptiveResponse<SearchResult<ContractTariffDTO>> ContractTariffReport([FromBody] ContractTariffSc searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<ContractTariffDTO>>
                .Try(() => this._reportService.ContractTariffReport(searchCriteria));
        }





    }
}
