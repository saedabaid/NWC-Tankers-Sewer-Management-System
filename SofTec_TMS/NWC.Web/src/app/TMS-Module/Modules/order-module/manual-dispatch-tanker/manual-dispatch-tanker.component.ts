import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { Component, OnInit, ViewEncapsulation, Input, Output, EventEmitter } from '@angular/core';
import { OrderDetailsService } from 'src/app/TMS-Module/Services/order-details.service';
import { StateVeicle } from 'src/app/TMS-Module/Models/state-vehicle';
import { FilterModelVehicle } from 'src/app/TMS-Module/Models/common/filter-model-vehicle';
import { OrderDetails } from 'src/app/TMS-Module/Models/order-details';
import { DispatchWorkOrder } from 'src/app/TMS-Module/Models/events/dispatch-workorder';
import { AccessoryDTO } from 'src/app/TMS-Module/Models/Accessory';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'app-manual-dispatch-tanker',
  templateUrl: './manual-dispatch-tanker.component.html',
  styleUrls: ['./manual-dispatch-tanker.component.scss'],
  encapsulation: ViewEncapsulation.None

})

export class ManualDispatchTankerComponent implements OnInit {
  tankerDispatch: boolean = false;
  DispatchWorkOrder: DispatchWorkOrder = new DispatchWorkOrder();
  AssignableVehicles: StateVeicle[];
  order: OrderDetails = new OrderDetails();
  Filter: FilterModelVehicle = new FilterModelVehicle();
  Accessories: AccessoryDTO[] = [];
  IsNoVehicles : boolean = true;
  @Output() dispatch: EventEmitter<boolean> = new EventEmitter<boolean>();
  constructor(private modalService: BsModalService,
    private modalRef: BsModalRef,
    private orderDetailsService: OrderDetailsService,
    private _alert: alertService,
    private mainloading: LoaderService
    ) {

  }

  ngOnInit() {

    this.order = this.modalService.config.initialState["order"]
    this.Filter.PageFilter = {
      "PageIndex": 1,
      "PageSize": 10
    }
    this.Filter.orderId = this.order.WorkOrderID;

  }
  ngAfterViewInit() {

    this.mainloading.PreloaderIcreaseCount();
    this.orderDetailsService.GetAssignableVehicles(this.Filter).subscribe(res => {
      if(!res.IsErrorState &&  res.Value.Result){
        this.AssignableVehicles = res.Value.Result;
        if ( res.Value.Result.length >0 )
          this.IsNoVehicles = false;
      }
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });

    this.mainloading.PreloaderIcreaseCount();
    this.orderDetailsService.GetWorkOrderAccessory(this.order.WorkOrderID).subscribe(res => {
      if (res != null) {
        this.Accessories = res.Value;
      }
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });

    this.DispatchWorkOrder.EventWorkOrderDTO.WorkOrderID = this.order.WorkOrderID;
    this.DispatchWorkOrder.EventWorkOrderDTO.StatusID = this.order.LastStatusID;
    this.DispatchWorkOrder.EventWorkOrderDTO.ServiceTypeID = this.order.ServiceTypeID;
    this.DispatchWorkOrder.EventWorkOrderDTO.StationID = this.order.AssignedStationID;
    this.DispatchWorkOrder.EventWorkOrderDTO.ScheduledDeliveryTime = this.order.ScheduledDeliveryTime;
    this.DispatchWorkOrder.EventWorkOrderDTO.OrderQuantity = this.order.OrderQuantity;
    this.DispatchWorkOrder.EventWorkOrderDTO.OrderNumber = this.order.OrderNumber;
    this.DispatchWorkOrder.EventWorkOrderDTO.CustomerLocationID = this.order.CustomerLocationID;
    this.DispatchWorkOrder.EventWorkOrderDTO.Accessories = this.Accessories;

  }

  dispatchTanker(vehicle: StateVeicle) {
    this.tankerDispatch = !this.tankerDispatch;
    if (!vehicle.dispatch) {
      this.AssignableVehicles.map(v => v.dispatch = false);
      vehicle.dispatch = true;
    }
    else {
      vehicle.dispatch = false;
    }
    this.DispatchWorkOrder.EventWorkOrderVehicleDTO.VehicleID = vehicle.VehicleID;
    this.DispatchWorkOrder.EventWorkOrderVehicleDTO.DriverID = vehicle.DriverID;

  }

  save() {
    if (this.tankerDispatch) {
      this.mainloading.PreloaderIcreaseCount();
      this.orderDetailsService.AssignWorkOrder(this.DispatchWorkOrder).subscribe(res => {

        if (res.IsErrorState == false) {

          this._alert.showSuccess();
          this.closePopup();
        }
        else {
          this._alert.showError();
        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });
    }
    else{
      this._alert.errorList(['NoVehicleSelected']);
    }
  }

  closePopup(){
    this.dispatch.emit(true);
    this.modalRef.hide();
  }
}
