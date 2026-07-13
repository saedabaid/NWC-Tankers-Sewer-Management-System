import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { WorkOrderSearchCriteria } from '../Models/search-criteria/work-order-search-criteria';
//import { Observable } from 'openlayers';
import { Lookup } from '../Models/common/lookup';
import { Observable } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';
import { OrderDetails } from '../Models/order-details';
import { DailyOrderReportSC } from '../Models/search-criteria/daily-order-report-SC.model';
import { DailyOrderSummary } from '../Models/daily-order-summary.model';
import { DeferredOrder } from '../Models/deferred-order.model';
import { DeferredOrderSC } from '../Models/search-criteria/deferred-order-SC.model';
import { AddItemsResponse } from '../Models/common/AddItemsResponse';

@Injectable({
  providedIn: 'root'
})
export class WorkOrderSearchService {
  constructor(private http: HttpClient) { }

  searchOrdersList(filter: WorkOrderSearchCriteria ): Observable<DescriptiveResponse<SearchResult<OrderDetails>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.orderSearch}`;
    return this.http.post<DescriptiveResponse<SearchResult<OrderDetails>>>(url, filter);
  }

  GetDailyOrderSummaryReport(filter: DailyOrderReportSC): Observable<DescriptiveResponse<SearchResult<DailyOrderSummary>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetDailyOrderSummaryReport}`;
    return this.http.post<DescriptiveResponse<SearchResult<DailyOrderSummary>>>(url, filter);
  }

  GetDailyOrderDetailsReport(filter: WorkOrderSearchCriteria ): Observable<DescriptiveResponse<SearchResult<OrderDetails>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetDailyOrderDetailsReport}`;
    return this.http.post<DescriptiveResponse<SearchResult<OrderDetails>>>(url, filter);
  }

  SearchDeferredWorkOrders(filter: DeferredOrderSC ): Observable<DescriptiveResponse<SearchResult<DeferredOrder>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.SearchDeferredWorkOrders}`;
    return this.http.post<DescriptiveResponse<SearchResult<DeferredOrder>>>(url, filter);
  }
  
  EditDeferredOrder(deferredOrder: DeferredOrder): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.Integration.EditDeferredOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, deferredOrder)
  }

  CancelDeferredOrder(orderId: number): Observable<DescriptiveResponse<Boolean>> {
    let url = Configuration.urls.commandEndpoint + Configuration.urls.Integration.CancelDeferredOrder;
    return this.http.post<DescriptiveResponse<Boolean>>(url, orderId);
  }

  UpdateWorkOrdersStation(stationID: string, filter: WorkOrderSearchCriteria):Observable<DescriptiveResponse<AddItemsResponse>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.UpdateWorkOrdersStation}?stationID=${stationID}`;
    return this.http.post<DescriptiveResponse<AddItemsResponse>>(url, filter)
  }
  
  
}
