using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/transporters")]
    public class TransporterController : ApiControllerBase
    {
        private ITransporterService _service;

        public TransporterController()
        {
            _service = new TransporterService(loggedInService);
        }

        [Route("")]
        [HttpPost]
        public DescriptiveResponse<Guid?> Add(TransporterDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<Guid?>.Try(() => _service.Add(dto));
        }

        [Route("{id}")]
        [HttpPut]
        public DescriptiveResponse<bool> Edit(Guid id, TransporterDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _service.Edit(dto));
        }

        [Route("{id}")]
        [HttpDelete]
        public DescriptiveResponse<bool> Delete(Guid id)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _service.Delete(id));
        }
        [Route("bulk")]
        [HttpPost]
        public DescriptiveResponse<List<TransporterExcelDTO>> AddRange(List<TransporterExcelDTO> dtos)
        {
            OnActionExecuting();

            return DescriptiveResponse<List<TransporterExcelDTO>>.Try(() => _service.AddRange(dtos));
        }

        [Route("UpdateVehicleStatus")]
        [HttpPost]
        public DescriptiveResponse<bool> UpdateVehicleStatus(EditTransporterStatusDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>.Try(() => _service.updateVehicleStatus(dto));
        }
    }
}