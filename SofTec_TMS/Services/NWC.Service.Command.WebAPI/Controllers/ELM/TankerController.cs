using NWC.BLL.Interfaces;
using NWC.BLL.Interfaces.ELM;
using NWC.BLL.Services;
using NWC.BLL.Services.ELM;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common.ELM;
using NWC.DTO.Models;
using NWC.DTO.Models.ELM;
using NWC.DTO.Models.TMS;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers.ELM
{
    //[AuthenticationTokenFilter]
    [RoutePrefix("api/tanker")]
    public class TankerController : ApiControllerBase
    {
        // GET: Tanker
        private ITankerService _service;

        public TankerController()
        {
            _service = new TankerService(loggedInService);
        }

        [Route("SaveTankerPermit")]
        [HttpPost]
        public DescriptiveResponse<string> SaveTankerPermit([FromBody] TankerDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<string>.Try(() => _service.Add(dto));
        }
        [Route("SaveTankerToTransporter")]
        [HttpPost]
        public DescriptiveResponse<string> SaveTankerToTransporter([FromUri] int? retrials, [FromUri] Guid subid)
        {
            OnActionExecuting();

            return DescriptiveResponse<string>.Try(() => _service.SaveTankerToTransporter(subid, retrials));
        }

        [Route("TankerAccessAuthorization")]
        [HttpPost]
        public DescriptiveResponse<ELMTransactionDTO> TankerAccessAuthorization([FromBody] TankerAccessDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<ELMTransactionDTO>.Try(() => _service.TankerAccessAuthorization(dto));
        }

        [Route("TankerCheckStatus")]
        [HttpPost]
        public DescriptiveResponse<ELMTransactionDTO> TankerCheckStatus([FromUri] string baseurl, [FromBody] TankerAccessDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<ELMTransactionDTO>.Try(() => _service.TankerCheckStatus(baseurl,dto));
        }
    }
}