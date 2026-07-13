import { Injectable } from '@angular/core';
import { DescriptiveResponse } from '@tms-models/common/descriptive-response';
import { StaffModel } from '@tms-models/staff-model';
import { StaffRolesDTO } from '../Models/StaffRolesDTO';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { Observable } from 'rxjs';
import { SearchResult } from '../Models/common/search-result';
import { HttpClient, HttpParams } from '@angular/common/http';
import { StaffRolesSearchCriteria } from '../Models/search-criteria/StaffRolesSearchCriteria';
import { PagesDTO, RoleVM } from '../Models/PagesDTO';
import { List } from '@amcharts/amcharts4/core';
import { StaffDTO } from '../Models/staff-dto';

@Injectable({
  providedIn: 'root'
})

export class StaffService {
  constructor(private http: HttpClient) { }

  createNewStaffMember(staffMember: StaffModel) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.staff.StaffAdd}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, staffMember);
  }
  getStaffMemberByID(id: string) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.staff.GetStaffById}?id=${id}`;
    return this.http.get<DescriptiveResponse<boolean>>(url);
  }
  updateStaffMember(staffMember: StaffModel) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.staff.StaffUpdate}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, staffMember);
  }
  deleteStaffMember(id: string) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.staff.StaffDelete}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, { id });
  }
  getStaffRoles(): Observable<DescriptiveResponse<SearchResult<StaffRolesDTO>>> {
    return this.http.get<DescriptiveResponse<StaffRolesDTO>>
      (Configuration.urls.queryEndpoint + Configuration.urls.staff.StaffRole);
  }
  getPages(key: string): Observable<DescriptiveResponse<List<PagesDTO>>> {
    if (key == undefined)
      key = null
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.staff.StaffPages}?key=${key}`;
    return this.http.get<DescriptiveResponse<List<PagesDTO>>>(url);


    //return this.http.get<DescriptiveResponse<List<PagesDTO>>> 
    //  (Configuration.urls.queryEndpoint + Configuration.urls.staff.StaffPages);
  }
  AddRange(StaffDTO: StaffDTO[]): Observable<DescriptiveResponse<StaffDTO[]>> {
    const url = Configuration.urls.commandEndpoint + Configuration.urls.staff.AddRange;
    return this.http.post<DescriptiveResponse<StaffDTO[]>>(url, StaffDTO);
  }
  //getContractStations(contractId: number): Observable<DescriptiveResponse<Lookup<string>[]>> {
  //  const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.getContractStations}?contractId=${contractId}`;
  //  return this.http.get<DescriptiveResponse<Lookup<string>[]>>(url);
  //}

  GetAllRoles(): Observable<DescriptiveResponse<List<PagesDTO>>> {
    return this.http.get<DescriptiveResponse<List<PagesDTO>>>
      (Configuration.urls.queryEndpoint + Configuration.urls.staff.GetAllRoles);
  }

  changePermission(userDetails: any, roleModel: any, param: string) {

    if (param == undefined)
      param = null
    const Body = { userDetails: userDetails, roleModel: roleModel }
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.staff.changePermssion}?key=${param}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, Body);



  }



}
