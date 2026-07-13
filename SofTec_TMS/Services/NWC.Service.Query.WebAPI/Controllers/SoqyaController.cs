using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Soqya")]
    public class SoqyaController : ApiControllerBase
    {
        private ISoqyaService _soqyaService;

        public SoqyaController()
        {
            _soqyaService = new SoqyaService(loggedInService);
        }


        [Route("SearchSoqyaSchedules")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<SoqyaScheduleDTO>> SearchSoqyaSchedules([FromBody] SoqyaScheduleSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>
                .Try(() => _soqyaService.SearchSoqyaSchedules(searchCriteria));
        }


        [Route("GetBalanceAndUsed")]
        [HttpGet]
        public DescriptiveResponse<SoqyaBalanceDTO> GetBalanceAndUsed([FromUri] long customerAccountId, [FromUri] int monthYear)
        {
            OnActionExecuting();

            return DescriptiveResponse<SoqyaBalanceDTO>
                .Try(() => _soqyaService.GetBalanceAndUsed(customerAccountId, monthYear));
        }

        [Route("GetSoqyaSchedulesReport")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<SoqyaScheduleReportDTO>> GetSoqyaSchedulesReport([FromBody] SoqyaScheduleReportSC searchCriteria)
        {
            OnActionExecuting();

            return DescriptiveResponse<SearchResult<SoqyaScheduleReportDTO>>
                .Try(() => _soqyaService.GetSoqyaSchedulesReport(searchCriteria));
        }

    }
}