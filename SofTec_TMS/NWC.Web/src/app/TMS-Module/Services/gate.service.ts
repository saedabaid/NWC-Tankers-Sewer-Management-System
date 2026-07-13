import { Injectable } from '@angular/core';
import { Configuration } from './../../shared/configurations/shared.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { StateVeicle } from '../Models/state-vehicle';
import { StateWorkOrderVeicle } from '../Models/state-workorder-vehicle';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { VehicleSC } from '../Models/search-criteria/vehicle-sc';
import { WorkOrderVehicleSC } from '../Models/search-criteria/workorder-vehicle-sc';
import { SearchResult } from '../Models/common/search-result';
import { Lookup } from '../Models/common/lookup';
import { PrintDTO } from '../Models/common/PrintDTO';
import { DispatchWorkOrder } from '../Models/events/dispatch-workorder';
import { OrderDetails } from '../Models/order-details';
import { EventWorkOrder } from '../Models/events/event-workorder';
import { ReturnResult } from '../Models/events/ReturnResult';
import { WOVArrivedStation } from '../Models/events/WOVArrivedStation';
import { PrintCustomerInvoiceDTO } from '../Models/print-customer-invoice';
import { PrintDriverInvoiceDTO } from '../Models/print -driver-invoice';
import { VehicleLog } from '../Models/vehicle-log.model';
import { VehicleLogReportSC } from '../Models/search-criteria/vehicle-log-report-SC.model';
import { VehicleDataReportSC } from '../Models/search-criteria/vehicle-data-report-SC.model';
import { VehicleData } from '../Models/vehicle-data.model';
import { PrintVehicleInvoice } from '../Models/print-vehicle-invoice.model';
import { VehiclePerformanceReportSC } from '@tms-models/search-criteria/vehicle-performance-report-SC.model';
import { VehiclePerformance } from '@tms-models/vehicle-performance.model';
import { OrderReassignmentReportSC } from '@tms-models/search-criteria/order-reassignment.model';
import { OrderReassignment } from '@tms-models/order-reassignment.model';

@Injectable({
  providedIn: 'root'
})
export class GateService {

  constructor(private http: HttpClient) { }

  getStateVehicles(searchCriteria: VehicleSC): Observable<DescriptiveResponse<SearchResult<StateVeicle>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.getStateVehicles}`;
    return this.http.post<DescriptiveResponse<SearchResult<StateVeicle>>>(url, searchCriteria);
  }

  getWorkOrderVehicles(searchCriteria: WorkOrderVehicleSC): Observable<DescriptiveResponse<SearchResult<StateWorkOrderVeicle>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.getWorkOrderVehicles}`;
    return this.http.post<DescriptiveResponse<SearchResult<StateWorkOrderVeicle>>>(url, searchCriteria);
  }

  OutForDeliveryWorkOrder(dispatch: EventWorkOrder) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.OutForDeliveryWorkOrder}`;
    return this.http.post<DescriptiveResponse<Lookup<OrderDetails>[]>>(url, dispatch);
  }

  ArrivedStation(wovArrivedStationDTO: WOVArrivedStation) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.orders.ArrivedStation}`;
    return this.http.post<DescriptiveResponse<ReturnResult>>(url, wovArrivedStationDTO);
  }

  GetPrintCustomerInvoice(PrintDTO: PrintDTO): Observable<DescriptiveResponse<PrintCustomerInvoiceDTO>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.GetPrintCustomerInvoice}`;
    return this.http.post<DescriptiveResponse<PrintCustomerInvoiceDTO>>(url, PrintDTO);
  }

  GetPrintDriverInvoice(PrintDTO: PrintDTO): Observable<DescriptiveResponse<PrintDriverInvoiceDTO>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.GetPrintDriverInvoice}`;
    return this.http.post<DescriptiveResponse<PrintDriverInvoiceDTO>>(url, PrintDTO);
  }

  GetPrintVehicleInvoice(PrintDTO: PrintDTO): Observable<DescriptiveResponse<PrintVehicleInvoice>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.GetPrintVehicleInvoice}`;
    return this.http.post<DescriptiveResponse<PrintVehicleInvoice>>(url, PrintDTO);
  }

  //ArriveVehicleToStation(vehicleID: string, customerClassId: number) :Observable <DescriptiveResponse<boolean>> {
  ArriveVehicleToStation(vehicleID: string, customerClassesIds: number[]): Observable<DescriptiveResponse<boolean>> {
    //const url = `${Configuration.urls.commandEndpoint + Configuration.urls.gate.ArriveVehicleToStation}?vehicleID=${vehicleID}&customerClassId=${customerClassId}`;
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.gate.ArriveVehicleToStation}?vehicleID=${vehicleID}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, customerClassesIds);
  }

  OutForParking(vehicleID: string): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.gate.OutForParking + '?vehicleID=' + vehicleID}`;
    return this.http.get<DescriptiveResponse<boolean>>(url);
  }

  GetVehicleLogReport(searchCriteria: VehicleLogReportSC): Observable<DescriptiveResponse<SearchResult<VehicleLog>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.GetVehicleLogReport}`;
    return this.http.post<DescriptiveResponse<SearchResult<VehicleLog>>>(url, searchCriteria);
  }

  GetVehiclePerformanceReport(searchCriteria: VehiclePerformanceReportSC): Observable<DescriptiveResponse<SearchResult<VehiclePerformance>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.GetVehiclePerformanceReport}`;
    return this.http.post<DescriptiveResponse<SearchResult<VehicleLog>>>(url, searchCriteria);
  }

  GetVehicleDataReport(searchCriteria: VehicleDataReportSC): Observable<DescriptiveResponse<SearchResult<VehicleData>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.GetVehicleDataReport}`;
    return this.http.post<DescriptiveResponse<SearchResult<VehicleData>>>(url, searchCriteria);
  }

  //Sewer
  SewerOutForWork(vehicleID: string): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.sewer.OutForWork + '?vehicleID=' + vehicleID}`;
    return this.http.get<DescriptiveResponse<boolean>>(url);
  }

  ArriveSewerVehicleWithOutOrderToStation(vehicleID: string): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.sewer.ArriveSewerVehicleWithOutOrderToStation + '?vehicleID=' + vehicleID}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, null);
  }
  SewerArrivedStation(wovArrivedStationDTO: WOVArrivedStation) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.sewer.SewerVehicleArrivedStation}`;
    return this.http.post<DescriptiveResponse<ReturnResult>>(url, wovArrivedStationDTO);
  }

  GetOrderReassignmentReport(searchCriteria: OrderReassignmentReportSC): Observable<DescriptiveResponse<SearchResult<OrderReassignment>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.gate.GetOrderReassignmentReport}`;
    return this.http.post<DescriptiveResponse<SearchResult<VehicleLog>>>(url, searchCriteria);
  }
}
