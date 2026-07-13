import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';

import { MeterReading } from '../Models/meter-reading';
import { MeterReadingSearchCriteria } from '../Models/search-criteria/meter-reading-SC';
import { DeviceMeterSearchCriteria } from '../Models/search-criteria/device-meter-SC';
import { DeviceMeter } from '../Models/device-meter';



@Injectable({
  providedIn: 'root'
})
export class DeviceMeterService {

  constructor(private http: HttpClient) { }

  SearchReadingList(filter: MeterReadingSearchCriteria): Observable<DescriptiveResponse<SearchResult<MeterReading>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.deviceMeter.SearchReadingList}`;
    return this.http.post<DescriptiveResponse<SearchResult<MeterReading>>>(url, filter);
  }

  AddReading(contractor: MeterReading): Observable<DescriptiveResponse<number>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.deviceMeter.AddReading}`;
    return this.http.post<DescriptiveResponse<number>>(url, contractor);
  }

  DeleteReading(contractId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.deviceMeter.DeleteReading}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractId);
  }
 
  SearchDeviceMeterList(filter: DeviceMeterSearchCriteria): Observable<DescriptiveResponse<SearchResult<DeviceMeter>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.deviceMeter.SearchDeviceMeterList}`;
    return this.http.post<DescriptiveResponse<SearchResult<DeviceMeter>>>(url, filter);
  }
}
