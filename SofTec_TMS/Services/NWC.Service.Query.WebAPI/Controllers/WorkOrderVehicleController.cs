using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/WorkOrderVehicle")]
    public class WorkOrderVehicleController : ApiControllerBase
    {

        private IWorkOrderVehicleService WorkOrderVehicleService;

        public WorkOrderVehicleController()
        {
            WorkOrderVehicleService = new WorkOrderVehicleService(loggedInService);
        }

        [Route("GetWorkOrderVehicles")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<StateWorkOrderVehicleDTO>> GetWorkOrderVehicles(WorkOrderVehicleSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return WorkOrderVehicleService.GetWorkOrderVehicles(searchCriteria);
        }

        [Route("GetStateVehicles")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<StateVehicleDTO>> GetStateVehicles(StateVehicleSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            return WorkOrderVehicleService.GetStateVehicles(searchCriteria);
        }
        [Route("GetAssignableVehicles")]
        [HttpPost]                // to order based on quantity , accessory , Serving zone of customer allows vehicle types
        public DescriptiveResponse<SearchResult<StateVehicleDTO>> GetAssignableVehicles(WorkOderFilter filter)
        {
            OnActionExecuting();

            return WorkOrderVehicleService.GetAssignableVehicles(filter);
        }
        [Route("GetPrintDriverInvoice")]
        [HttpPost]
        public DescriptiveResponse<PrintDriverInvoice> GetPrintDriverInvoice(PrintDTO PrintDTO)
        {
            OnActionExecuting();

            return WorkOrderVehicleService.GetPrintDriverInvoice(PrintDTO);
        }

        [Route("GetPrintCustomerInvoice")]
        [HttpPost]
        public DescriptiveResponse<PrintCustomerInvoice> GetPrintCustomerInvoice(PrintDTO PrintDTO)
        {
            OnActionExecuting();

            return WorkOrderVehicleService.GetPrintCustomerInvoice(PrintDTO);
        }

        [Route("GetPrintVehicleInvoice")]
        [HttpPost]
        public DescriptiveResponse<PrintVehicleInvoice> GetPrintVehicleInvoice(PrintDTO PrintDTO)
        {
            OnActionExecuting();

            return WorkOrderVehicleService.GetPrintVehicleInvoice(PrintDTO);
        }

        [Route("GetOrderReassignmentReport")]
        [HttpPost]
        public DescriptiveResponse<SearchResult<OrderReassignmentDTO>> GetOrderReassignmentReport(OrderReassignmentReportSC searchCriteria)
        {
            OnActionExecuting();

            return WorkOrderVehicleService.GetOrderReassignmentReport(searchCriteria);
        }
    }
}