import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DescriptiveResponse } from '@tms-models/common/descriptive-response';
import { SearchResult } from '@tms-models/common/search-result';
import { PermitDTO } from '@tms-models/PermitDTO';
import { PermitListSC } from '@tms-models/search-criteria/Permit-list-SC';
import { PermitSC } from '@tms-models/search-criteria/Permit-search-criteria';
import { StaffModel } from '@tms-models/staff-model';
import { VehicleDTO, VehiclePermitDTO } from '@tms-models/vehicle-dto';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PermitService {

  constructor(private http: HttpClient) { }

  SearchDriver(DriverIDSearchText: string): Observable<DescriptiveResponse<StaffModel>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.staff.GetStaffByPersonalId}`;
    return this.http.get<DescriptiveResponse<StaffModel>>(url + '?personalId=' + DriverIDSearchText)
  }
  SearchTanker(TankerNumberSearchText: string): Observable<DescriptiveResponse<VehicleDTO>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Vehicles.GetTransporterByNumber}`;
    return this.http.get<DescriptiveResponse<VehicleDTO>>(url + '?transporterNo=' + TankerNumberSearchText)
  }
  AddPermit(permit: PermitDTO): Observable<DescriptiveResponse<string>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.AddPermit}`;
    return this.http.post<DescriptiveResponse<string>>(url, permit);
  }
  UpdateTanker(VehicleDTO: VehiclePermitDTO): Observable<DescriptiveResponse<string>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.UpdateTanker}`;
    return this.http.post<DescriptiveResponse<string>>(url, VehicleDTO);
  }
  UpdateDriver(StaffDTO: StaffModel): Observable<DescriptiveResponse<string>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.UpdateDriver}`;
    return this.http.post<DescriptiveResponse<string>>(url, StaffDTO);
  }
  GetList(searchCriteria: PermitListSC): Observable<DescriptiveResponse<SearchResult<PermitListSC>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Vehicles.GetPermitList}`;
    return this.http.post<DescriptiveResponse<SearchResult<PermitDTO>>>(url, searchCriteria);
  }
  GetPermit(updateId: string): Observable<DescriptiveResponse<PermitDTO>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Vehicles.GetPermit}`;
    return this.http.get<DescriptiveResponse<PermitDTO>>(url + '?updateId=' + updateId)
  }
}
