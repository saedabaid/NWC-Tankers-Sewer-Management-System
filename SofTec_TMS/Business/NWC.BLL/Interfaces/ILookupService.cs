using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface ILookupService
    {
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationClasses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationCategories();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationPriorities();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetServiceTypes();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetVehicleLogTypes();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> getPermittedServicesTypes();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderStatusesInDeassign();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetAccessories();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetZoneStations(long ZoneId);
        DescriptiveResponse<Guid> GetMainZoneStationByZoneIntID(string zoneIntId);
        DescriptiveResponse<bool> GetStationStatusByZoneIntID(string zoneIntId);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStaffSelectedCategory(Guid? key = null);
        DescriptiveResponse<string> GetStaffSelectedRoleName(Guid? key = null);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffDefaultPage(Guid? key = null);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetUserStations(string searchKeyword);
        DescriptiveResponse<LookUpDTO<Guid>> GetMainZoneStation(long zoneId);

        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerLocations(long CustomerId);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerAccounts(long CustomerId, int[] serviceTypeId = null);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerCommercialAccounts(long CustomerId, int[] serviceTypeId = null);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchAreasByName(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchCitiesByName(List<Guid> areasIds, string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesByName(List<Guid> cityId, string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchStations(List<long> zoneId, string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchVehicles(List<Guid> stationIds, string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchVehicles(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDriversByTanker(List<Guid> transporterIds, string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDrivers(List<Guid> stationIds, string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDrivers(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchStaff(string searchKeyword, short? category);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchCustomers(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchSoqyaCustomers(string searchKeyword);
        // For Commercial Orders
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchCommercialCustomers(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchOrderNumbers(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetNextWorkOrderStatus(int currentStatusId);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetSewerNextWorkOrderStatus(int currentStatusId);
        
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GeReasonsByStatusId(int statusId);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetDeassignReason();
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractCode(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractContractors(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTypes();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTerminationReasons();
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractorNameCode(string searchKeyword);

        DescriptiveResponse<IEnumerable<CityDTO>> GetPermittedCities();

        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> getZonesByNameOrCode();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchUserByName(string searchKeyword);   
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesByNameOrCode(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetAllCities(PageFilter PageFilter);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStationBasedOnCity(string searchKeyword, List<Guid> CityIDs);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetAllStationBasedOnCity(List<Guid> CityIDs);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterTypes();
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchAllZones(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getContractStations(long contractId);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetPersonalIDTypes();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getCustomerLocationStations(long customerLocationId);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getCustomerAccountStations(long customerAccountId);

        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetTermsCategory();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchPermittedStations(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchPermittedSoqyaStations(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchMeterSerial(List<Guid> stationIds, string searchKeyword);

        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTanckerCapacities();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTanckerCapacitiesByStation(Guid stationId, int customerAccountId);
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesBasedOnAssignedStations(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetDeferredOrdersStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTermsValueUnits();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCommingMonthYear();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterBrands();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterGroups();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterProductionYears();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterManufacturer();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTransporterStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTransporterStatusesInDeassign();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetOrderInvoiceStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetContractTerm(long contractId, Guid stationId, int categoryId);
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTermsViolationStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTermsViolationCancelReasons();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStationStatuses();
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderCategory();


        // ------ 
        DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStaffRoleCategories();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffRolesByCategoryID(int staffRoleCategoryID);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetPageList();

        //=====================================

        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffRoles();
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetBranches(string searchKeyword);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetSubBranches(string searchKeyword, List<Guid> parentBranchIds);
        DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetLandmarks(string searchKeyword, List<Guid> branchIds);

    }
}
