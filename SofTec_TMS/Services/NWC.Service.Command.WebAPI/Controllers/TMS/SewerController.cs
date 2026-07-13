using NWC.BL.Denormalizer.Denormalizers;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using System;
using System.Linq;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers.TMS
{
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Sewer")]
    public class SewerController : ApiControllerBase
    {
        private ISewerService _SewerService;
        private IDenormalizer _denormalizer;

        public SewerController()
        {
            _SewerService = new SewerService(loggedInService);
            _denormalizer = new Denormalizer();
        }



        [HttpPost]
        [Route("SewerVehicleArrivedStation")]
        public DescriptiveResponse<Boolean> SewerVehicleArrivedStation(WOVArrivedStationDTO request)
        {
            OnActionExecuting();

            var response = _SewerService.SewerVehicleArrivedStation(request);
           
            if (!response.IsErrorState && response.Value.Any())
            {
                //this used for 
                if(response.Value.Count == 1 && response.Value.FirstOrDefault() == 0)
                {
                    return DescriptiveResponse<bool>.Success(true);
                }
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }

            return DescriptiveResponse<bool>.Error(response.ErrorDescription);
        }


 
        [HttpPost]
        [Route("ArriveSewerVehicleWithOutOrderToStation")]
        public DescriptiveResponse<Boolean> ArriveSewerVehicleWithOutOrderToStation(string vehicleId)
        {
            OnActionExecuting();

            try
            {
                var response = _SewerService.ArriveSewerVehicleWithOutOrderToStation(new Guid(vehicleId));
                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<Boolean>.Error(ex.Message);
            }
        }

        [HttpGet]
        [Route("OutForWork")]
        public DescriptiveResponse<Boolean> OutForWork(string vehicleID)
        {
            OnActionExecuting();

            var result = _SewerService.OutForWork(new Guid(vehicleID));

            return result;
        }
    }
}
