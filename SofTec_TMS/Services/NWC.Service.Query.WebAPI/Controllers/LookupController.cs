using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC.Service.Query.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NWC.Service.Query.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Lookup")]
    public class LookupController : ApiControllerBase
    {
        private ILookupService _LookupService;

        public LookupController()
        {
            _LookupService = new LookupService(loggedInService);
        }

        [Route("GetCustomerLocationClasses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationClasses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetCustomerLocationClasses());
        }

        [Route("GetCustomerLocationCategories")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationCategories()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetCustomerLocationCategories());
        }

        [Route("GetCustomerLocationPriorities")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationPriorities()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetCustomerLocationPriorities());
        }

        [Route("GetCustomerLocationStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetCustomerLocationStatuses());
        }

        [Route("GetServiceTypes")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetServiceTypes()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetServiceTypes());
        }

        [Route("GetVehicleLogTypes")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetVehicleLogTypes()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetVehicleLogTypes());
        }


        [Route("getPermittedServicesTypes")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> getPermittedServicesTypes()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.getPermittedServicesTypes());
        }


        [Route("GetWorkOrderStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                 .Try(() => _LookupService.GetWorkOrderStatuses());
        }

        [Route("GetWorkOrderStatusesInDeassign")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderStatusesInDeassign()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                 .Try(() => _LookupService.GetWorkOrderStatusesInDeassign());
        }

        [Route("GetAccessories")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetAccessories()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetAccessories());
        }

        [Route("GetZoneStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetZoneStations(long ZoneId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetZoneStations(ZoneId));
        }

        [Route("GetUserStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetUserStations(string searchKeyword = "")
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetUserStations(searchKeyword));
        }

        [Route("GetMainZoneStation")]
        [HttpGet]
        public DescriptiveResponse<LookUpDTO<Guid>> GetMainZoneStation(long ZoneId)
        {
            OnActionExecuting();

            return DescriptiveResponse<LookUpDTO<Guid>>
                .Try(() => _LookupService.GetMainZoneStation(ZoneId));
        }

        [Route("GetMainZoneStationByZoneIntID")]
        [HttpGet]
        public DescriptiveResponse<Guid> GetMainZoneStationByZoneIntID(string zoneIntId)
        {
            OnActionExecuting();

            return DescriptiveResponse<Guid>
                .Try(() => _LookupService.GetMainZoneStationByZoneIntID(zoneIntId));
        }

        [Route("GetMainStationStatusByZoneIntID")]
        [HttpGet]
        public DescriptiveResponse<bool> GetStationStatusByZoneIntID(string zoneIntId)
        {
            OnActionExecuting();

            return DescriptiveResponse<bool>
                .Try(() => _LookupService.GetStationStatusByZoneIntID(zoneIntId));
        }


        [Route("GetCustomerLocations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerLocations(long CustomerId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetCustomerLocations(CustomerId));
        }

        [Route("GetCustomerAccounts")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerAccounts(long CustomerId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetCustomerAccounts(CustomerId));
        }

        [Route("GetCustomerAccountsSoqya")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerAccountsSoqya(long CustomerId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetCustomerAccounts(CustomerId, new int[] { 2 }));
        }

        [Route("GetCustomerAccountsSameService")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerAccountsSameService(long CustomerId, int serviceId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetCustomerAccounts(CustomerId, new int[] { serviceId }));
        }

        [Route("GetCustomerAccountsAddOrderPage")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerAccountsAddOrderPage(long CustomerId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetCustomerAccounts(CustomerId, new int[] { 1, 3 }));
        }

        [Route("GetCustomerCommercialAccounts")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerCommercialAccounts(long CustomerId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetCustomerCommercialAccounts(CustomerId, new int[] { 1, 3 }));
        }

        [Route("SearchAreasByName")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchAreasByName(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchAreasByName(searchKeyword));
        }

        [Route("SearchCitiesByName")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchCitiesByName([FromUri] string searchKeyword, [FromBody] List<Guid> areasIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchCitiesByName(areasIds, searchKeyword));
        }

        [Route("GetPermittedCities")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<CityDTO>> GetPermittedCities()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<CityDTO>>
                .Try(() => _LookupService.GetPermittedCities());
        }

        [Route("SearchZonesByName")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesByName([FromUri] string searchKeyword, [FromBody] List<Guid> cityIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchZonesByName(cityIds, searchKeyword));
        }

        [Route("SearchStations")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchStations([FromUri] string searchKeyword, [FromBody] List<long> zoneIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchStations(zoneIds, searchKeyword));
        }

        [Route("SearchVehicles")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchVehicles([FromUri] string searchKeyword, [FromBody] List<Guid> stationIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchVehicles(stationIds, searchKeyword));
        }

        [Route("GetVehicles")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetVehicles([FromUri] string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchVehicles(searchKeyword));
        }

        [Route("SearchDrivers")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDrivers([FromUri] string searchKeyword, [FromBody] List<Guid> stationIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchDrivers(stationIds, searchKeyword));
        }

        [Route("SearchDriversByTanker")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDriversByTanker([FromUri] string searchKeyword, [FromBody] List<Guid> transporterIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchDriversByTanker(transporterIds, searchKeyword));
        }

        [Route("GetDrivers")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetDrivers([FromUri] string searchKeyword)
        {
            OnActionExecuting();

            return _LookupService.SearchDrivers(searchKeyword);
        }

        [Route("SearchStaff")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchStaff([FromUri] string searchKeyword, [FromUri] short? category)
        {
            OnActionExecuting();

            return _LookupService.SearchStaff(searchKeyword, category);
        }

        [Route("SearchCustomers")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchCustomers([FromUri] string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchCustomers(searchKeyword));
        }

        [Route("SearchSoqyaCustomers")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchSoqyaCustomers([FromUri] string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchSoqyaCustomers(searchKeyword));
        }

        [Route("SearchCommercialCustomers")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchCommercialCustomers([FromUri] string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchCommercialCustomers(searchKeyword));
        }

        [Route("SearchOrderNumbers")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchOrderNumbers(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchOrderNumbers(searchKeyword));
        }

        [Route("GetNextWorkOrderStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetNextWorkOrderStatuses(int currentStatusId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetNextWorkOrderStatus(currentStatusId));
        }


        [Route("GetSewerNextWorkOrderStatus")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetSewerNextWorkOrderStatus(int currentStatusId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetSewerNextWorkOrderStatus(currentStatusId));
        }

        [Route("GeReasonsByStatusId")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GeReasonsByStatusId(int statusId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GeReasonsByStatusId(statusId));
        }

        [Route("GetDeassignReason")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetDeassignReason()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetDeassignReason());
        }

        [Route("SearchContractCode")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractCode(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchContractCode(searchKeyword));
        }

        [Route("SearchContractContractors")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractContractors(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchContractContractors(searchKeyword));
        }

        [Route("GetContractTypes")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTypes()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetContractTypes());
        }

        [Route("GetContractStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetContractStatuses());
        }

        [Route("GetContractTerminationReasons")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTerminationReasons()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetContractTerminationReasons());
        }

        [Route("SearchContractorNameCode")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractorNameCode(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchContractorNameCode(searchKeyword));
        }

        [Route("getZonesByNameOrCode")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> getZonesByNameOrCode()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.getZonesByNameOrCode());

        }

        [Route("searchZonesByNameOrCode")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesByNameOrCode(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                           .Try(() => _LookupService.SearchZonesByNameOrCode(searchKeyword));
        }

        [Route("GetAllCities")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetAllCities(PageFilter PageFilter)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetAllCities(PageFilter));
        }

        [Route("GetStationBasedOnCity")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStationBasedOnCity([FromUri] string searchKeyword, [FromBody] List<Guid> CityIDs)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetStationBasedOnCity(searchKeyword, CityIDs));
        }

        [Route("GetAllStationBasedOnCity")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetAllStationBasedOnCity([FromBody] List<Guid> CityIDs)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetAllStationBasedOnCity(CityIDs));
        }

        [Route("GetTransporterTypes")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterTypes()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetTransporterTypes());
        }

        [Route("SearchAllZones")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchAllZones(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchAllZones(searchKeyword));
        }

        [Route("getContractStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetContractStations(long contractId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.getContractStations(contractId));
        }

        [Route("GetPersonalIDTypes")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetPersonalIDTypes()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetPersonalIDTypes());
        }

        [Route("getCustomerLocationStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getCustomerLocationStations(long customerLocationId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.getCustomerLocationStations(customerLocationId));
        }

        [Route("getCustomerAccountStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getCustomerAccountStations(long customerAccountId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.getCustomerAccountStations(customerAccountId));
        }


        [Route("GetTermsCategory")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetTermsCategory()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetTermsCategory());
        }

        [Route("SearchPermittedStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchPermittedStations(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchPermittedStations(searchKeyword));
        }

        [Route("SearchPermittedSoqyaStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchPermittedSoqyaStations(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.SearchPermittedSoqyaStations(searchKeyword));
        }

        [Route("SearchMeterSerial")]
        [HttpPost]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchMeterSerial([FromUri] string searchKeyword, [FromBody] List<Guid> stationIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchMeterSerial(stationIds, searchKeyword));
        }

        [Route("GetTanckerCapacities")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTanckerCapacities()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetTanckerCapacities());
        }

        [Route("GetTanckerCapacitiesByStation")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTanckerCapacitiesByStation(Guid stationId, int customerAccountId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetTanckerCapacitiesByStation(stationId, customerAccountId));
        }

        [Route("GetDeferredOrdersStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetDeferredOrdersStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetDeferredOrdersStatuses());
        }

        [Route("GetTermsValueUnits")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTermsValueUnits()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetTermsValueUnits());
        }

        [Route("SearchZonesBasedOnAssignedStations")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesBasedOnAssignedStations(string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.SearchZonesBasedOnAssignedStations(searchKeyword));
        }

        [Route("GetCommingMonthYear")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCommingMonthYear()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetCommingMonthYear());
        }

        [Route("GetTransporterBrands")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterBrands()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetTransporterBrands());
        }

        [Route("GetTransporterGroups")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterGroups()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetTransporterGroups());
        }

        [Route("GetTransporterProductionYears")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterProductionYears()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetTransporterProductionYears());
        }

        [Route("GetTransporterManufacturer")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterManufacturer()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetTransporterManufacturer());
        }

        [Route("GetTransporterStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTransporterStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetTransporterStatuses());
        }

        [Route("GetTransporterStatusesInDeassign")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTransporterStatusesInDeassign()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetTransporterStatusesInDeassign());
        }

        [Route("GetOrderInvoiceStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetOrderInvoiceStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetOrderInvoiceStatuses());
        }

        [Route("GetContractTerm")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetContractTerm(long contractId, Guid stationId, int categoryId)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>
                .Try(() => _LookupService.GetContractTerm(contractId, stationId, categoryId));
        }

        [Route("GetContractTermsViolationStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTermsViolationStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetContractTermsViolationStatuses());
        }

        [Route("GetContractTermsViolationCancelReasons")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTermsViolationCancelReasons()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetContractTermsViolationCancelReasons());
        }

        [Route("GetStationStatuses")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStationStatuses()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetStationStatuses());
        }

        [Route("GetWorkOrderCategory")]
        [HttpGet]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderCategory()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetWorkOrderCategory());
        }


        /// --------
        [HttpGet]
        [Route("GetStaffRoleCategories")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStaffRoleCategories()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetStaffRoleCategories());
        }

        [HttpGet]
        [Route("GetStaffRolesByCategoryID")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffRolesByCategoryID(int staffRoleCategoryID)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetStaffRolesByCategoryID(staffRoleCategoryID));
        }
        [HttpGet]
        [Route("GetPageList")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetPageList()
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetPageList());
        }

        

        [HttpGet]
        [Route("GetBranches")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetBranches([FromUri] string searchKeyword)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetBranches(searchKeyword));
        }

        [HttpGet]
        [Route("GetSubBranches")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetSubBranches([FromUri] string searchKeyword, [FromUri] List<Guid> parentBranchIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetSubBranches(searchKeyword, parentBranchIds));
        }

        [HttpGet]
        [Route("GetLandmarks")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetLandmarks([FromUri] string searchKeyword, [FromUri] List<Guid> branchIds)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetLandmarks(searchKeyword, branchIds));
        }
        [HttpGet]
        [Route("GetStaffSelectedCategory")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStaffSelectedCategory([FromUri] Guid? key = null)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>
                .Try(() => _LookupService.GetStaffSelectedCategory(key));
        }
        [HttpGet]
        [Route("GetStaffSelectedRoleName")]
        public DescriptiveResponse<string>GetStaffSelectedRoleName([FromUri] Guid? key = null)
        {
            OnActionExecuting();

            return DescriptiveResponse<string>
                .Try(() => _LookupService.GetStaffSelectedRoleName(key));
        }
        [HttpGet]
        [Route("GetStaffDefaultPage")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffDefaultPage([FromUri] Guid? key = null)
        {
            OnActionExecuting();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>
                .Try(() => _LookupService.GetStaffDefaultPage(key));
        }
    }
}