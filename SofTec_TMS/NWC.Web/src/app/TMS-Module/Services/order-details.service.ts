import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderDetails } from '../Models/order-details';
import { OrderComment } from '../Models/order-comment';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { Lookup } from '../Models/common/lookup';
import { OrderComplaint } from '../Models/order-compaint';
import { DispatchWorkOrder } from '../Models/events/dispatch-workorder';
import { EventWorkOrder } from '../Models/events/event-workorder';
import { AccessoryDTO } from '../Models/Accessory';
import { OrderPayment } from '../Models/order-payment';
import { SearchResult } from '../Models/common/search-result';
import { StateVeicle } from '../Models/state-vehicle';
import { FilterModelVehicle } from '../Models/common/filter-model-vehicle';
import { WorkOrderSearchCriteria } from '../Models/search-criteria/work-order-search-criteria';
import { ReturnResult } from '../Models/events/ReturnResult';
import { WorkOrderChangeLog } from '../Models/work-order-change-log';
import { DeferredOrder } from '../Models/deferred-order.model';
import { AddItemsResponse } from '../Models/common/AddItemsResponse';

@Injectable({
  providedIn: 'root'
})

export class OrderDetailsService {
  constructor(private http: HttpClient) { }

  getOrderDetails(orderId: number): Observable<DescriptiveResponse<OrderDetails>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.details + '?orderId=' + orderId}`;
    return this.http.get<DescriptiveResponse<OrderDetails>>(url)
  }

  getOrderComments(orderId: number): Observable<DescriptiveResponse<OrderComment[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetWorkOrderComments + '?orderId=' + orderId}`;
    return this.http.get<DescriptiveResponse<OrderComment[]>>(url)
  }

  getWorkOrderPayments(workOrderID: number): Observable<DescriptiveResponse<OrderPayment[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetWorkOrderPayments + '?workOrderID=' + workOrderID}`;
    return this.http.get<DescriptiveResponse<OrderPayment[]>>(url);
  }

  addComment(comment: OrderComment): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.AddOrderComments}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, comment)
  }

  getWorkOrdersNextStatuses(currentStatusId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetNextWorkOrderStatuses}?currentStatusId=${currentStatusId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url)
  }
  GetSewerNextWorkOrderStatus(currentStatusId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GetSewerNextWorkOrderStatus}?currentStatusId=${currentStatusId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url)
  }
  GeReasonsByStatusId(statusId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.lookups.GeReasonsByStatusId}?statusId=${statusId}`;
    return this.http.get<DescriptiveResponse<Lookup<number>[]>>(url)
  }

  AssignWorkOrder(myDispatch: DispatchWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.AssignWorkOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, myDispatch);
  }

  //DeassignWorkOrder(myDispatch: DispatchWorkOrder) {
  // const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.DeassignWorkOrder}`;
  //  return this.http.post<DescriptiveResponse<Lookup<OrderDetails>[]>>(url, myDispatch);
  //}

  OutForDeliveryWorkOrder(myDispatch: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.OutForDeliveryWorkOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, myDispatch);
  }

  ArrivedWorkOrder(workOrder: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.ArrivedWorkOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, workOrder);
  }
  SewerConfirmWorkOrder(workOrder: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.SewerConfirmWorkOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, workOrder);
  }
  SewerCompleteWorkOrder(workOrder: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.SewerCompleteWorkOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, workOrder);
  }

  DeliveredWorkOrder(myDispatch: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.DeliveredWorkOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, myDispatch);
  }

  CancelWorkOrder(myDispatch: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.CancelWorkOrder}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, myDispatch);
  }

  FailedToDeliver(workOrder: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.FailedToDeliver}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, workOrder);
  }

  OnHold(myDispatch: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.OnHold}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, myDispatch);
  }

  NotAssigned(workOrder: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.NotAssigned}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, workOrder);
  }

  getOrderComplaints(orderId: number): Observable<DescriptiveResponse<OrderComplaint[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetWorkOrderComplaints + '?orderId=' + orderId}`;
    return this.http.get<DescriptiveResponse<OrderComplaint[]>>(url)
  }

  GetWorkOrderAccessory(orderId: number): Observable<DescriptiveResponse<AccessoryDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetWorkOrderAccessory + '?workOrderId=' + orderId}`;
    return this.http.get<DescriptiveResponse<AccessoryDTO[]>>(url)
  }

  UpdateWorkOrderShipment(orderEvent: EventWorkOrder): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.UpdateWorkOrderShipment}`;
    return this.http.put<DescriptiveResponse<boolean>>(url, orderEvent)
  }
  GetAssignableVehicles(FilterVehicle: FilterModelVehicle): Observable<DescriptiveResponse<SearchResult<StateVeicle>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Vehicles.GetAssignableVehicles}`;
    return this.http.post<DescriptiveResponse<SearchResult<StateVeicle>>>(url, FilterVehicle)
  }
 
  DispatchWorkOrder(DispatchWorkOrder: DispatchWorkOrder): Observable<DescriptiveResponse<Boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.AssignWorkOrder}`;
    return this.http.post<DescriptiveResponse<Boolean>>(url, DispatchWorkOrder);
  }

  GetAssignableWorkOrders(searchCriteria: WorkOrderSearchCriteria): Observable<DescriptiveResponse<SearchResult<OrderDetails>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetAssignableWorkOrders}`;
    return this.http.post<DescriptiveResponse<SearchResult<OrderDetails>>>(url, searchCriteria)
  }

  DeassignWorkOrder(request: DispatchWorkOrder, eventWorkOrderDTO: EventWorkOrder): Observable<DescriptiveResponse<Boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.DeassignWorkOrder}`;
    const DeassignDTO = { request: request, eventWorkOrderDTO: eventWorkOrderDTO }
    return this.http.post<DescriptiveResponse<Boolean>>(url, DeassignDTO);
  }

  GetChangesLogs(workorderId) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetWorkOrderChangeLogs}?workOrderId=${workorderId}`;
    return this.http.get<DescriptiveResponse<WorkOrderChangeLog[]>>(url);
  }

  CreateWorkOrder(eventOrder: EventWorkOrder): Observable<DescriptiveResponse<EventWorkOrder>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.CreateWorkOrder}`;
    return this.http.post<DescriptiveResponse<EventWorkOrder>>(url, eventOrder)
  }

  BulkCreateWorkOrder(eventOrder: EventWorkOrder): Observable<DescriptiveResponse<AddItemsResponse>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.BulkCreateWorkOrder}`;
    return this.http.post<DescriptiveResponse<AddItemsResponse>>(url, eventOrder)
  }

  IsCustomerBlacklisted(accountID: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.IsCustomerBlacklisted + '?accountID=' + accountID}`;
    return this.http.get<DescriptiveResponse<boolean>>(url)
  }

  IsCustomerExceededQuota(stationID: string, customerID: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.IsCustomerExceededQuota}?stationID=${stationID}&customerID=${customerID}`;
    return this.http.get<DescriptiveResponse<boolean>>(url)
  }

  IsZoneWithoutTankers(customerAccountID: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.IsZoneWithoutTankers + '?customerAccountID=' + customerAccountID}`;
    return this.http.get<DescriptiveResponse<boolean>>(url)
  }

  GetNoOfOrdersForThisMonth(customerAccountID: number): Observable<DescriptiveResponse<number>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.orders.GetNoOfOrdersForThisMonth + '?customerAccountID=' + customerAccountID}`;
    return this.http.get<DescriptiveResponse<number>>(url)
  }

}
