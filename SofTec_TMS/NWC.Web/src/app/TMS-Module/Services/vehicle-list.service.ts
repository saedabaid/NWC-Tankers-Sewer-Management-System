import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EditVehicleStatusDTO } from '@tms-models/edit-vehicle-status-dto';
import { Observable } from 'rxjs';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';
import { VehicleSearchCriteriaDTO } from '../Models/search-criteria/vehicle-search-criteria';
import { VehicleDTO } from '../Models/vehicle-dto';
import { VehicleListDTO } from '../Models/vehicle-list-dto';
import { VehicleType } from '../Models/vehicle-type';

@Injectable({ providedIn: 'root' })
export class VehicleListService {
  constructor(private http: HttpClient) { }

  public Search(vehicleSearchCriteriaDTO: VehicleSearchCriteriaDTO): Observable<DescriptiveResponse<SearchResult<VehicleListDTO>>> {
    const url = Configuration.urls.queryEndpoint + Configuration.urls.Vehicles.Search;
    return this.http.post<DescriptiveResponse<SearchResult<VehicleListDTO>>>(url, vehicleSearchCriteriaDTO);
  }

  Add(vehicleDTO: VehicleDTO): Observable<DescriptiveResponse<Boolean>> {
    const url = Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.Add;
    return this.http.post<DescriptiveResponse<Boolean>>(url, vehicleDTO);
  }

  GetOne(vehicleId: string) {
    const url = Configuration.urls.queryEndpoint + Configuration.urls.Vehicles.GetOne + '/' + vehicleId;
    return this.http.get<DescriptiveResponse<VehicleDTO>>(url);
  }

  Update(vehicleDTO: VehicleDTO): Observable<DescriptiveResponse<Boolean>> {
    const url = Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.Update + '/' + vehicleDTO.Id;
    return this.http.put<DescriptiveResponse<Boolean>>(url, vehicleDTO);
  }

  Delete(vehicleId: string): Observable<DescriptiveResponse<Boolean>> {
    const url = Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.Delete + '/' + vehicleId;
    return this.http.delete<DescriptiveResponse<Boolean>>(url);
  }

  AddRange(vehicleDTO: VehicleDTO[]): Observable<DescriptiveResponse<VehicleDTO[]>> {
    const url = Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.AddRange;
    return this.http.post<DescriptiveResponse<VehicleDTO[]>>(url, vehicleDTO);
  }

  UpdateVehicleStatus(dto: EditVehicleStatusDTO){
    const url = Configuration.urls.commandEndpoint + Configuration.urls.Vehicles.UpdateVehicleStatus;
    return this.http.post<DescriptiveResponse<Boolean>>(url, dto);
  }

  searchVehicleType(VehicleSearchCriteriaDTO: VehicleSearchCriteriaDTO): Observable<DescriptiveResponse<SearchResult<VehicleType>>> {
    debugger;
    const url = Configuration.urls.queryEndpoint + Configuration.urls.Vehicles.searchVehicleType;
    return this.http.post<DescriptiveResponse<SearchResult<VehicleType>>>(url, VehicleSearchCriteriaDTO);

  }

}
