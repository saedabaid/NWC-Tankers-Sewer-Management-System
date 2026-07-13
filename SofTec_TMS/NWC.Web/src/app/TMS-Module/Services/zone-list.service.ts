import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { ZoneSearchCriteriaDTO } from '../Models/search-criteria/zone-search-criteria';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { ZoneListDTO } from '../Models/zone-listDTO';
import { Observable } from 'rxjs';
import { ZoneDTO } from '../Models/zoneDTO';
import { SearchResult } from '../Models/common/search-result';

@Injectable({ providedIn: 'root' })
export class zoneListService implements OnInit {
  ngOnInit(): void {

  }

  constructor(private httpClient: HttpClient) {

  }

  public Search(ZoneSearchCriteriaDTO: ZoneSearchCriteriaDTO): Observable<DescriptiveResponse<SearchResult<ZoneListDTO>>> {
    debugger;
    return this.httpClient.post<DescriptiveResponse<SearchResult<ZoneListDTO>>>
      (Configuration.urls.queryEndpoint + Configuration.urls.zone.zoneSearch, ZoneSearchCriteriaDTO);
  }

  Add(ZoneDTO: ZoneDTO): Observable<DescriptiveResponse<Boolean>> {
    let url = Configuration.urls.commandEndpoint + Configuration.urls.zone.Add;
    return this.httpClient.post<DescriptiveResponse<Boolean>>(url, ZoneDTO);
  }

  GetZoneDetails(ZoneId: number) {
    let url = `${Configuration.urls.queryEndpoint + Configuration.urls.zone.GetZoneDetails + '?ZoneId=' + ZoneId}`;
    return this.httpClient.get<DescriptiveResponse<ZoneDTO>>(url);
  }

  Update(ZoneDTO: ZoneDTO): Observable<DescriptiveResponse<Boolean>> {
    let url = Configuration.urls.commandEndpoint + Configuration.urls.zone.Update;
    return this.httpClient.post<DescriptiveResponse<Boolean>>(url, ZoneDTO);
  }

  Delete(ZoneId: number): Observable<DescriptiveResponse<Boolean>> {
    debugger;
    let url = Configuration.urls.commandEndpoint + Configuration.urls.zone.Delete;
    return this.httpClient.post<DescriptiveResponse<Boolean>>(url, ZoneId);
  }

  AddRange(ZoneDTO: ZoneDTO[]): Observable<DescriptiveResponse<ZoneDTO[]>> {
    let url = Configuration.urls.commandEndpoint + Configuration.urls.zone.AddRange;
    return this.httpClient.post<DescriptiveResponse<ZoneDTO[]>>(url, ZoneDTO);
  }

  CallGISService(premiseCoordinates: string, orderNumber: string, sourceApp: string, transactionId: string): Observable<DescriptiveResponse<ZoneDTO>> {
    let url = `${Configuration.urls.queryEndpoint + Configuration.urls.zone.CallGISService}?premiseCoordinates=${premiseCoordinates}&orderNumber=${orderNumber}&sourceApp=${sourceApp}&transactionId=${transactionId}`;
    return this.httpClient.get<DescriptiveResponse<ZoneDTO>>(url);
  }


}
