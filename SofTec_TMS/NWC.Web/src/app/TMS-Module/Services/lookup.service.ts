import { Injectable } from '@angular/core';
import { Configuration } from './../../shared/configurations/shared.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { Lookup } from '../Models/common/lookup';
import { PageFilter } from '../Models/common/page-fillter-model';
import { StaffSearchCriteria } from 'src/app/TMS-Module/Models/search-criteria/staff-search-criteria';
import { VehicleType } from '@tms-models/vehicle-type';
import { System } from '@amcharts/amcharts4/core';


@Injectable({
  providedIn: 'root'
})
export class LookupService {

  constructor(private http: HttpClient) { }

  getDrivers(driverName: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetDriversLookups}?searchKeyword=${driverName}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getVehicleTypes(): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetVehicleTypes}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  getVehicleBrand(): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetVehicleBrand}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  getLandMarkTypes(): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetLandMarkTypes}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  getVehicleBrandById(id): Observable<DescriptiveResponse<Lookup<VehicleType>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetVehicleBrandById}?id=${id}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  getVehicleTypeById(id): Observable<DescriptiveResponse<Lookup<VehicleType>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetVehicleTypes}?id=${id}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getVehicles(vehicleName: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetVehiclesLookups}?searchKeyword=${vehicleName}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  searchVehicles(vehicleName: string, stationIds: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchVehiclesLookups}?searchKeyword=${vehicleName}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, stationIds);
  }

  SearchDriversByTanker(driverName: string, transporterIds: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchDriversByTanker}?searchKeyword=${driverName}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, transporterIds);
  }

  searchDrivers(driverName: string, stationIds: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchDriversLookups}?searchKeyword=${driverName}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, stationIds);
  }


  SearchOrderNumbers(orderNumber: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchOrderNumberLookups}?searchKeyword=${orderNumber}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getCustomerClasses(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.customerClassLookups}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getCustomerPriorities(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.customerPrioritiesLookups}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getServicesTypes(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.servicesTypeLookups}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getPermittedServicesTypes(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.getPermittedServicesTypes}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url );
  }

  GetStaffSelectedCategory(param:string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetStaffSelectedCategory}?key=${param}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }
  GetStaffSelectedRoleName(param: string) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetStaffSelectedRoleName}?key=${param}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  GetStaffDefaultPage(param: string) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetStaffDefaultPage}?key=${param}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  
  GetVehicleLogTypes(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetVehicleLogTypes}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url );
  }

  GetZoneStations(ZoneId: number): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetZoneStations}?ZoneId=${ZoneId}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  GetCustomerLocations(CustomerId: number): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetCustomerLocations}?CustomerId=${CustomerId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }
  getAreasName(areaName): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchAreasNameLookups}?searchKeyword=${areaName}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getCityName(cityName: string, areaIds: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchCitiesNameLookups}?searchKeyword=${cityName}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, areaIds);
  }

  getZoneName(zoneName: string, cityIds: string[]): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchZonesNameLookups}?searchKeyword=${zoneName}`;
    return this.http.post<DescriptiveResponse<Lookup<number>[]>>(url, cityIds);
  }

  getStationName(stationName: string, zoneIds: number[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStationsLookups}?searchKeyword=${stationName}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, zoneIds);
  }

  getWorkOrdersStatues(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.WorkOrderStatusesLookups}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }


  GetWorkOrderStatusesInDeassign(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetWorkOrderStatusesInDeassign}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  // getVehicles(vehicleName: string, stationIds: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
  //   const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchVehiclesLookups}?searchKeyword=${vehicleName}`;
  //   return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, stationIds)
  // }

  // getDrivers(driverName: string, stationIds: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
  //   const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchDriversLookups}?searchKeyword=${driverName}`;
  //   return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, stationIds)
  // }

  getCustomers(customerName: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchCustomersLookups}?searchKeyword=${customerName}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  SearchSoqyaCustomers(customerName: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchSoqyaCustomers}?searchKeyword=${customerName}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getCommercialCustomers(customerName: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchCommercialCustomers}?searchKeyword=${customerName}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getDeassignReason(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetDeassignReason}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  public getZonesByNameOrCode(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.getZonesByNameOrCode}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  public SearchUserByName(searchKeyword: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.searchUserByName}?searchKeyword=${searchKeyword}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  searchContractsCodes(contractCode: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.searchContractsCodes}?searchKeyword=${contractCode}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetContractTypes(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetContractTypes}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetContractStatuses(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetContractStatuses}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetContractTerminationReasons(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetContractTerminationReasons}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  SearchContractorNameCode(contractorNameCode: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchContractorNameCode}?searchKeyword=${contractorNameCode}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }


  public SearchZonesByNameOrCode(searchKeyword: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchZonesByNameOrCode}?searchKeyword=${searchKeyword}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetAllCities(PageFilter: PageFilter): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetAllCities}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url , PageFilter);
  }

  GetStationBasedOnCity( name: string , CityIDs: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetStationBasedOnCity}?searchKeyword=${name}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url , CityIDs );
  }

  GetUserStations(name: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetUserStations}?searchKeyword=${name}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  GetAllStationBasedOnCity(CityIDs: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetAllStationBasedOnCity}`;
    console.log('ids', CityIDs );
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url , CityIDs );
  }

  GetTransporterTypes(): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTransporterTypes}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  SearchAllZones(name: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchAllZones}?searchKeyword=${name}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  SearchZonesBasedOnAssignedStations(name: string): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchZonesBasedOnAssignedStations}?searchKeyword=${name}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getContractStations(contractId: number): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.getContractStations}?contractId=${contractId}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  GetPersonalIDTypes(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetPersonalIDTypes}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getAccessories(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetAccessories}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetWorkOrderCategory(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetWorkOrderCategory}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getCustomerLocationStations(customerLocationId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.getCustomerLocationStations}?customerLocationId=${customerLocationId}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  GetTermsCategory(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTermsCategory}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetTermsValueUnits(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTermsValueUnits}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  SearchPermittedStations(name: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchPermittedStations}?searchKeyword=${name}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  SearchPermittedSoqyaStations(name: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchPermittedSoqyaStations}?searchKeyword=${name}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);

  }

  SearchMeterSerial( name: string , stationIds: string[]): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchMeterSerial}?searchKeyword=${name}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url , stationIds);
  }

  GetTanckerCapacities(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTanckerCapacities}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetTanckerCapacitiesByStation(stationId: string, customerAccountId: number): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTanckerCapacitiesByStation}?stationId=${stationId}&customerAccountId=${customerAccountId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  SearchStaff(staffName: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaff}?searchKeyword=${staffName}&category`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  SearchStaffByCategory(staffName: string,category?: number): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaff}?searchKeyword=${staffName?staffName:""}&category=${category}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  GetDeferredOrdersStatuses(): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetDeferredOrdersStatuses}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getCustomerAccountStations(customerAccountId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.getCustomerAccountStations}?customerAccountId=${customerAccountId}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  GetCustomerAccounts(CustomerId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetCustomerAccounts}?CustomerId=${CustomerId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetCustomerAccountsSameService(CustomerId: number, serviceId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetCustomerAccountsSameService}?CustomerId=${CustomerId}&serviceId=${serviceId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetCustomerAccountsSoqya(CustomerId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetCustomerAccountsSoqya}?CustomerId=${CustomerId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetCustomerAccountsAddOrderPage(CustomerId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetCustomerAccountsAddOrderPage}?CustomerId=${CustomerId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  getCustomerCommercialAccountsAddOrderPage(CustomerId: number){
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetCustomerCommercialAccountsAddOrderPage}?CustomerId=${CustomerId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetCommingMonthYear() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetCommingMonthYear}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetTransporterBrands() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTransporterBrands}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetTransporterGroups() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTransporterGroups}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetTransporterProductionYears() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTransporterProductionYears}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetTransporterManufacturer() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTransporterManufacturer}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }


  GetTransporterStatuses() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTransporterStatuses}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetTransporterStatusesInDeassign() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetTransporterStatusesInDeassign}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetOrderInvoiceStatuses() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetOrderInvoiceStatuses}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  SearchContractContractors(contractSearch: string) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchContractContractors}?searchKeyword=${contractSearch}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetContractTerm(contractId: number, stationId: string, categoryId: number): Observable<DescriptiveResponse<Lookup<number>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetContractTerm}?contractId=${contractId}&stationId=${stationId}&categoryId=${categoryId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetContractTermsViolationStatuses() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetContractTermsViolationStatuses}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  
  GetContractTermsViolationCancelReasons() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetContractTermsViolationCancelReasons}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }

  GetStationStatuses() {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetStationStatuses}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url);
  }







  // Staff
  searchStaff(staffSearchCriteria: StaffSearchCriteria): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.staff.StaffSearch}`;
    //const body = staffSearchCriteria;
    const body = {
      SearchKeyword: staffSearchCriteria.FilterModel.SearchKeyword,
      Id: staffSearchCriteria.Id,
      Name: staffSearchCriteria.Name,
      branchId: staffSearchCriteria.branchId,
      RoleId: staffSearchCriteria.RoleId,
      stationId: staffSearchCriteria.stationId
    };

    console.log('aa', body);
    console.log(url);
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, body);
  }

  getStaffRoles(roleName: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaffRolesLookups}?searchKeyword=${roleName}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  getStaffRoleCategory(): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetStaffRoleCategories}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
  getStaffRolesByCategoryID(id: number): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetStaffRolesByCategoryID}?staffRoleCategoryID=${id}`;
    console.log('url', url);
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getStaffBranch(staffSearchCriteria: StaffSearchCriteria): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaffBranchLookups}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, staffSearchCriteria);
  }

  getStaffSubBranch(roleName): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaffBranchLookups}?searchKeyword=${roleName}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getStaffLandmark(roleName): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaffLandmarkLookups}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, roleName);
  }

  getStaffPermittedBranch(id): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaffLandmarkLookups}`;
    return this.http.post<DescriptiveResponse<Lookup<string>[]>>(url, id);
  }

  getStaffPermittedSubBranch(roleName): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.SearchStaffLandmarkLookups}?searchKeyword=${roleName}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getBranches(searchKeyword?: string): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetBranches}?searchKeyword=${searchKeyword}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getSubBranches(searchKeyword?: string, parentBranchIds?: Array<string>): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const parentBranchIdsString = (parentBranchIds || []).reduce((pv, cv) => pv + `&parentBranchIds=${cv}`, "");
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetSubBranches}?searchKeyword=${searchKeyword}${parentBranchIdsString}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  getLandmarks(searchKeyword?: string, branchIds?: Array<string>): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const branchIdsString = (branchIds || []).reduce((pv, cv) => pv + `&branchIds=${cv}`, "");
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetLandmarks}?searchKeyword=${searchKeyword}${branchIdsString}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }

  // Roles
  getPagesList(): Observable<DescriptiveResponse<Lookup<string>[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetpagesList}`;
    return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  }
}


