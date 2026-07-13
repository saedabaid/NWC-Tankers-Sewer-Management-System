using NWC.BL.Denormalizer.Denormalizers;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Vehicle")]
    public class VehicleController : ApiControllerBase
    {
        private IWorkOrderService _workOrderService;
        private IDenormalizer _denormalizer;

        private IWorkOrderVehicleService _workOrderVehicleService;
        private IVehicleService _vehicleService;

        private static bool AssignInEntryGate
        {
            get
            {
                var AssignInEntryGate = ConfigurationManager.AppSettings["AssignInEntryGate"];
                if (string.IsNullOrEmpty(AssignInEntryGate) || AssignInEntryGate.ToLower() == "true")
                {
                    return true;
                }
                return false;
            }
        }

        public VehicleController()
        {
            _workOrderService = new WorkOrderService(loggedInService);
            _denormalizer = new Denormalizer();

            _workOrderVehicleService = new WorkOrderVehicleService(loggedInService);
            _vehicleService = new VehicleService(loggedInService);
        }

        [HttpPost]
        [Route("ArriveVehicleToStation")]
        public DescriptiveResponse<Boolean> ArriveVehicleToStation([FromUri] string vehicleID, [FromBody] List<int> customerClassesIds)//[FromUri] int customerClassId)
        {
            OnActionExecuting();

            var result = _workOrderVehicleService.ArriveVehicleToStation(new Guid(vehicleID), customerClassesIds);

            if (AssignInEntryGate)
            {
                if (result.Value)
                {
                    var workOrderToAssign = _workOrderService.GetMatchedWorkOrderToAssign(new Guid(vehicleID));

                    if (workOrderToAssign != null && workOrderToAssign.EventWorkOrderDTO != null && workOrderToAssign.EventWorkOrderDTO.WorkOrderID > 0)
                    {
                        var response = _workOrderService.AssignWorkOrder(workOrderToAssign);

                        if (!response.IsErrorState && response.Value.Any())
                        {
                            var states = _denormalizer.DenormalizeStates(response.Value);
                        }
                    }
                }
            }


            return result;
        }

        [HttpPost]
        [Route("SaveVehicleNWCSettings")]
        public DescriptiveResponse<Boolean> SaveVehicleNWCSettings(List<VehicleNWCSettingsDTO> vehicleNWCSettingsDTOs)
        {
            OnActionExecuting();

            var result = _vehicleService.SaveVehicleNWCSettings(vehicleNWCSettingsDTOs);

            return result;
        }

        [HttpPost]
        [Route("AddPermit")]
        public DescriptiveResponse<string> AddPermit([FromBody] PermitDTO permitDto)
        {
            OnActionExecuting();

            var result = _vehicleService.AddPermit(permitDto);

            return result;
        }

        [HttpPost]
        [Route("UpdateTanker")]
        public DescriptiveResponse<string> UpdateTanker([FromBody] VehicleDTO permitDto)
        {
            OnActionExecuting();

            var result = _vehicleService.UpdateTanker(permitDto);

            return result;
        }

        [HttpPost]
        [Route("SaveVehicleNWCSettingsBulk")]
        public DescriptiveResponse<Boolean> SaveVehicleNWCSettingsBulk(VehicleNWCSettingsBulkUpdateDTO updateModel)
        {
            OnActionExecuting();

            var result = _vehicleService.SaveVehicleNWCSettingsBulk(updateModel);

            return result;
        }

        [HttpGet]
        [Route("OutForParking")]
        public DescriptiveResponse<Boolean> OutForParking(string vehicleID)
        {
            OnActionExecuting();

            var result = _workOrderVehicleService.OutForParking(new Guid(vehicleID));

            return result;
        }
    }
}
