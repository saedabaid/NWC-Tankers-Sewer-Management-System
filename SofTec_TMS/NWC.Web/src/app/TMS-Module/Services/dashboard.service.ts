import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';
import { VehicleDataDTO } from '../Models/VehicleDataDTO';
import { MeterReading } from '../Models/meter-reading';
import { MeterReadingSearchCriteria } from '../Models/search-criteria/meter-reading-SC';
import { DashboardSC } from '../Models/search-criteria/dashboard-SC.model';
import { DashboardXYChartDTO } from '../Models/common/DashboardXYChart.model';
import { ZonePriceSCDTO } from '@tms-models/search-criteria/zone-price-search-criteria';
import { ZonePriceListDTO } from '@tms-models/zone-price-listDTO';
import { ZoneListDTO } from '@tms-models/zone-list';



@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  mainSearchClicked$ = new BehaviorSubject<DashboardSC>(null);

  constructor(private http: HttpClient) { }

  GetWorkOrdersCountPerStatus(filter: DashboardSC): Observable<DescriptiveResponse<number>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetWorkOrdersCountPerStatus}`;
    return this.http.post<DescriptiveResponse<number>>(url, filter);
  }

  GetOrdersCountGroupByDayHours(filter: DashboardSC): Observable<DescriptiveResponse<DashboardXYChartDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetOrdersCountGroupByDayHours}`;
    return this.http.post<DescriptiveResponse<DashboardXYChartDTO[]>>(url, filter);
  }

  GetOrdersCountGroupByTop10Zones(filter: DashboardSC): Observable<DescriptiveResponse<DashboardXYChartDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetOrdersCountGroupByTop10Zones}`;
    return this.http.post<DescriptiveResponse<DashboardXYChartDTO[]>>(url, filter);
  }

  GetOrdersCountGroupByStatus(filter: DashboardSC): Observable<DescriptiveResponse<DashboardXYChartDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetOrdersCountGroupByStatus}`;
    return this.http.post<DescriptiveResponse<DashboardXYChartDTO[]>>(url, filter);
  }

  GetOrdersCountGroupByDate(filter: DashboardSC): Observable<DescriptiveResponse<DashboardXYChartDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetOrdersCountGroupByDate}`;
    return this.http.post<DescriptiveResponse<DashboardXYChartDTO[]>>(url, filter);
  }

  GetOrdersCountGroupByExecuteTime(filter: DashboardSC): Observable<DescriptiveResponse<DashboardXYChartDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetOrdersCountGroupByExecuteTime}`;
    return this.http.post<DescriptiveResponse<DashboardXYChartDTO[]>>(url, filter);
  }

  GetVehiclesCountGroupByStatus(filter: DashboardSC): Observable<DescriptiveResponse<DashboardXYChartDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetVehicleDataReportByStatus}`;
    return this.http.post<DescriptiveResponse<DashboardXYChartDTO[]>>(url, filter);
  }

  GetAreasWithNoPrices(filter: ZonePriceSCDTO): Observable<DescriptiveResponse<ZonePriceListDTO[]>>{
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Dashboard.GetAreasWithNoPrices}`;
    return this.http.post<DescriptiveResponse<ZoneListDTO[]>>(url, filter);
  }

}
