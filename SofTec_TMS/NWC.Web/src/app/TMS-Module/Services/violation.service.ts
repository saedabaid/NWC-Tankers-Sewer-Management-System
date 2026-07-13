import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';

import { VehicleViolation } from '../Models/vehicle-violation.model';

@Injectable({
  providedIn: 'root'
})
export class ViolationService {

  constructor(private http: HttpClient) { }

  GetVehicleViolations(vehicleID: string) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Violation.GetVehicleViolations}?vehicleID=${vehicleID}`;
    return this.http.get<DescriptiveResponse<SearchResult<VehicleViolation>>>(url);
  }
 
}
