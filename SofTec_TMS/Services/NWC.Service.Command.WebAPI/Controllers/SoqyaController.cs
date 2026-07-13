using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System.Collections.Generic;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
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

        [Route("AddSoqyeScheduleRecord")]
        [HttpPost]
        public DescriptiveResponse<bool> AddSoqyeScheduleRecord([FromBody] SoqyaScheduleDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _soqyaService.AddSoqyeScheduleRecord(dto));
        }

        [Route("EditSoqyeScheduleRecord")]
        [HttpPost]
        public DescriptiveResponse<bool> EditSoqyeScheduleRecord([FromBody] SoqyaScheduleDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _soqyaService.EditSoqyeScheduleRecord(dto));
        }

        [Route("DeleteSoqyeScheduleRecord")]
        [HttpPost]
        public DescriptiveResponse<bool> DeleteSoqyeScheduleRecord([FromBody] long scheduleId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _soqyaService.DeleteSoqyeScheduleRecord(scheduleId));
        }


        [Route("UpdateGeneratedSoqyaSchedule")]
        [HttpPost]
        public DescriptiveResponse<bool> UpdateGeneratedSoqyaSchedule([FromBody] List<SoqyaScheduleDTO> SoqyaScheduleDTO)
        {
            OnActionExecuting();
            return DescriptiveResponse<bool>
                .Try(() => _soqyaService.UpdateGeneratedSoqyaSchedule(SoqyaScheduleDTO));
        }

    }
}