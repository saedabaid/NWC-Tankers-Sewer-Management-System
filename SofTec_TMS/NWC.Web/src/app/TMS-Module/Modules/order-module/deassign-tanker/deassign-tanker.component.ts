import { Component, OnInit, ViewEncapsulation, Output, EventEmitter } from '@angular/core';
//import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { LookupService } from '../../../Services/lookup.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { OrderDetails } from 'src/app/TMS-Module/Models/order-details';
import { OrderDetailsService } from 'src/app/TMS-Module/Services/order-details.service';
import { WorkOrderSearchCriteria } from 'src/app/TMS-Module/Models/search-criteria/work-order-search-criteria';
import { DispatchWorkOrder } from 'src/app/TMS-Module/Models/events/dispatch-workorder';
import { EventWorkOrder } from 'src/app/TMS-Module/Models/events/event-workorder';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-deassign-tanker',
  templateUrl: './deassign-tanker.component.html',
  styleUrls: ['./deassign-tanker.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class DeassignTankerComponent implements OnInit {

  @Output() deassignWorkorder: EventEmitter<boolean> = new EventEmitter<boolean>();
  order: OrderDetails = new OrderDetails();

  DispatchWorkOrder: DispatchWorkOrder = new DispatchWorkOrder();
  EventWorkOrderDTO: EventWorkOrder = new EventWorkOrder();

  IsNoOrders: boolean = true;

  constructor(
    private modalService: BsModalService, private modalRef: BsModalRef,
    private LookupService: LookupService,
    private orderServie: OrderDetailsService,
    private _alert: alertService,
    private mainloading: LoaderService) { }

  ngOnInit() {
    this.order = this.modalService.config.initialState["order"];

    //this.getDeassignReason();
    this.getTankerStatuses();
    this.getOrderStatuses();
    this.getOrderAvailableList();
  }

  DeassignReasonList: Lookup<number>[] = [];
  TankerStatuses: Lookup<number>[] = [];
  OrderStatuses: Lookup<number>[] = [];
  orderList: OrderDetails[];

  Loading_Reasons = false;
  loading_TankerStatus = false;
  Loading_OrderStatus = false;
  Loading_OrderList = false;

  SelectedTankerStatusId: number;
  SelectedOrderStatusId: number;

  selectMenuOptions2 = {
    enableSearchFilter: false,
    singleSelect: true
  };

  getDeassignReason() {
    if (this.SelectedOrderStatusId == 8 || this.SelectedOrderStatusId == 3) {
      this.Loading_Reasons = true;
      this.orderServie.GeReasonsByStatusId(this.SelectedOrderStatusId).subscribe(res => {
        if (res.Value != null) {
          this.DeassignReasonList = res.Value;
        }
      }
        , err => {
          this.Loading_Reasons = false;
        }
        , () => {
          this.Loading_Reasons = false;
        });

    } else {
      this.DeassignReasonList = [];
    }
  }

  getTankerStatuses() {
    this.loading_TankerStatus = true;
    this.LookupService.GetTransporterStatusesInDeassign().subscribe(res => {
      if (res.Value != null) {
        this.TankerStatuses = res.Value;
      }
    }
      , err => {
        this.loading_TankerStatus = false;
      }
      , () => {
        this.loading_TankerStatus = false;
      });
  }

  getOrderStatuses() {
    this.Loading_OrderStatus = true;
    this.LookupService.GetWorkOrderStatusesInDeassign().subscribe(res => {
      if (res.Value != null) {
        this.OrderStatuses = res.Value;
      }
    }
      , err => {
        this.Loading_OrderStatus = false;
      }
      , () => {
        this.Loading_OrderStatus = false;
      });
  }

  onReasonChanged($event) {
    this.DispatchWorkOrder.EventWorkOrderDTO.StatusReasonID = $event[0].Id;
  }

  onTankerStatusDDLChanged(evt) {
    this.SelectedTankerStatusId = (evt.length > 0) ? evt[0].Id : null;
  }

  onOrderStatusDDLChanged(evt) {
    this.SelectedOrderStatusId = (evt.length > 0) ? evt[0].Id : null;
    this.getDeassignReason();
  }

  getOrderAvailableList() {
    let orderSC = new WorkOrderSearchCriteria()
    // orderSC.FilterModel = new FilterModel<string>();
    orderSC.FilterModel.PageFilter = null;
    orderSC.VehicleID = this.order.AssignedVehicleID;

    this.Loading_OrderList = true;
    this.orderServie.GetAssignableWorkOrders(orderSC).subscribe(res => {
      if (res.Value.Result != null) {
        this.orderList = res.Value.Result;
        if (res.Value.Result.length > 0)
          this.IsNoOrders = false;
      }
    }
      , err => {
        this.Loading_OrderList = false;
      }
      , () => {
        this.Loading_OrderList = false;
      });
  }


  selectOrder(itemOrder: OrderDetails) {
    if (itemOrder.IsAssigned) {
      itemOrder.IsAssigned = false;

      this.EventWorkOrderDTO = new EventWorkOrder();
    }
    else {
      this.orderList.map(v => v.IsAssigned = false);
      itemOrder.IsAssigned = true;

      this.EventWorkOrderDTO.WorkOrderID = itemOrder.WorkOrderID;
      this.EventWorkOrderDTO.StatusID = itemOrder.LastStatusID;
      this.EventWorkOrderDTO.ServiceTypeID = itemOrder.ServiceTypeID;
      this.EventWorkOrderDTO.StationID = itemOrder.AssignedStationID;
      this.EventWorkOrderDTO.ScheduledDeliveryTime = itemOrder.ScheduledDeliveryTime;
      this.EventWorkOrderDTO.OrderQuantity = itemOrder.OrderQuantity;
      this.EventWorkOrderDTO.OrderNumber = itemOrder.OrderNumber;
      this.EventWorkOrderDTO.CustomerLocationID = itemOrder.CustomerLocationID;
    }
  }


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if ((this.SelectedOrderStatusId == 8 || this.SelectedOrderStatusId == 3)
      && this.DispatchWorkOrder.EventWorkOrderDTO.StatusReasonID == null) {
      validationMessages.push("SelectReason");
    }

    if (isNullOrUndefined(this.SelectedTankerStatusId) || this.SelectedTankerStatusId < 0) {
      validationMessages.push("SelectTankerStatus");
    }

    if (!this.SelectedOrderStatusId || this.SelectedOrderStatusId < 1) {
      validationMessages.push("SelectOrderStatus");
    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }


  deassign() {
    // validation
    if (!this.isValidModel()) return;

    this.DispatchWorkOrder.EventWorkOrderVehicleDTO.VehicleID = this.order.AssignedVehicleID;
    this.DispatchWorkOrder.EventWorkOrderVehicleDTO.DriverID = this.order.AssignedDriverID;
    this.DispatchWorkOrder.EventWorkOrderVehicleDTO.StatusID = this.SelectedTankerStatusId;
    this.DispatchWorkOrder.EventWorkOrderDTO.WorkOrderID = this.order.WorkOrderID;
    //this.DispatchWorkOrder.EventWorkOrderDTO.StatusID = this.order.LastStatusID;
    this.DispatchWorkOrder.EventWorkOrderDTO.StatusID = this.SelectedOrderStatusId;
    this.DispatchWorkOrder.EventWorkOrderDTO.ServiceTypeID = this.order.ServiceTypeID;
    this.DispatchWorkOrder.EventWorkOrderDTO.StationID = this.order.AssignedStationID;
    this.DispatchWorkOrder.EventWorkOrderDTO.ScheduledDeliveryTime = this.order.ScheduledDeliveryTime;
    this.DispatchWorkOrder.EventWorkOrderDTO.OrderQuantity = this.order.OrderQuantity;
    this.DispatchWorkOrder.EventWorkOrderDTO.OrderNumber = this.order.OrderNumber;
    this.DispatchWorkOrder.EventWorkOrderDTO.CustomerLocationID = this.order.CustomerLocationID;

    this.mainloading.PreloaderIcreaseCount();
    this.orderServie.DeassignWorkOrder(this.DispatchWorkOrder, this.EventWorkOrderDTO).subscribe(res => {

      if (res.Value == true) {
        this._alert.showSuccess();
        this.closePopup();
      }
      else {
        this._alert.showError();
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });

  }

  closePopup() {
    this.deassignWorkorder.emit(true);
    this.modalRef.hide();
  }

}
