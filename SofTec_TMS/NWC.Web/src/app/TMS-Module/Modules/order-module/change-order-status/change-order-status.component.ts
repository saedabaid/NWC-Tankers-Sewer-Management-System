import { Component, OnInit, ViewEncapsulation, Output, EventEmitter, Input } from '@angular/core';
//import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { Lookup } from '../../../Models/common/lookup';
import { OrderDetailsService } from 'src/app/TMS-Module/Services/order-details.service';
import { DispatchWorkOrder } from 'src/app/TMS-Module/Models/events/dispatch-workorder';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { OrderDetails } from '../../../Models/order-details';
import { EventWorkOrder } from '../../../Models/events/event-workorder';
import { LoaderService } from 'src/app/shared/loader.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ServerUtilitiesService } from 'src/app/TMS-Module/Services/server-utilities.service';


@Component({
  selector: 'change-order-status',
  templateUrl: './change-order-status.component.html',
  styleUrls: ['./change-order-status.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ChangeOrderStatusComponent implements OnInit {

  currentWorkOrder: OrderDetails;
  StatusDDL: Lookup<number>[];
  reasonsDDL: Lookup<number>[];

  targetStatusId = 0;
  serverTime: Date;
  selectedDate: Date;
  selectedTime = "";
  comment = "";
  confirmationCode = "";
  reasonId = 0;

  cancelMode = false;
  isVirtualModel = false;
  orderStatus = -1;

  @Input() closeBtnName: string;
  @Output() StatusChanged: EventEmitter<boolean> = new EventEmitter<boolean>();


  constructor(private modalService: BsModalService,
    private modalRef: BsModalRef,
    private orderDetailsService: OrderDetailsService,
    private authServer: AuthenticationService,
    private alert: alertService,
    private mainloading: LoaderService,
    private _ServerUtilitiesService: ServerUtilitiesService
  ) {
    let canDeliverd = [6, 7,14];
    this.currentWorkOrder = modalService.config.initialState as OrderDetails;
    var service = this.currentWorkOrder.ServiceTypeID == 1 ? this.orderDetailsService.getWorkOrdersNextStatuses(this.currentWorkOrder.LastStatusID) : this.orderDetailsService.GetSewerNextWorkOrderStatus(this.currentWorkOrder.LastStatusID)
    service.subscribe(res => {
      if (this.cancelMode) {
        this.StatusDDL = res.Value.filter(x => x.Id === 8);
        this.targetStatusId = 8;
        this.onStatusDDLChange();
      }
      else if (this.isVirtualModel)
      {
         // TODO: Add deliverd status to list when order in VirtualStation
         // TODO: Add deliverd status to list when order has out-for-delivred status
        this.StatusDDL = res.Value.filter(x => x.Id !== 5 );
      }
      else {
        canDeliverd.includes(this.orderStatus) ? this.StatusDDL = res.Value.filter(x => x.Id !== 5) :
          this.StatusDDL = res.Value.filter(x => x.Id !== 5 && x.Id !== 4);
      }
      
     
    });

  }

  ngOnInit() {

    this._ServerUtilitiesService.GetDateTimeNow().subscribe(res => {
      if (!res.IsErrorState && res.Value) {
        this.serverTime = new Date(res.Value);
        this.selectedDate = new Date(res.Value);
        this.selectedTime = this.selectedDate.toTimeString().substring(0, 5);
      }

    });

  }

  onStatusDDLChange() {
    if (this.targetStatusId == 8 || this.targetStatusId == 3) {
      this.mainloading.PreloaderIcreaseCount();
      this.orderDetailsService.GeReasonsByStatusId(this.targetStatusId).subscribe(res => {
        this.reasonsDDL = res.Value;
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });
    } else {
      this.reasonsDDL = [];
      this.reasonId = 0;
    }
  }


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (this.targetStatusId < 1) {
      validationMessages.push("SelectStatus");
    }
    if ([8, 3].includes(+this.targetStatusId) && (isNullOrUndefined(this.reasonId) || this.reasonId < 1)) {
      validationMessages.push("SelectReason");
    }
    let time = this.selectedTime.split(':');
    if (time.length < 2 || isNullOrUndefined(time[0]) || time[0] === '' || isNullOrUndefined(time[1]) || time[1] === '') {
      validationMessages.push("EnterTime");
    }
    else {
      this.selectedDate.setHours(+time[0]);
      this.selectedDate.setMinutes(+time[1]);

      let statusDate = new Date(this.currentWorkOrder.LastStatusTime);
      if (this.selectedDate < statusDate) {
        validationMessages.push("SelectedDateTimeMustBeAfterLastStatusDateTime");
      }

      let validDate = new Date(this.serverTime.getTime() + (30 * 60000));
      if (this.selectedDate > validDate) {
        validationMessages.push("SelectedDateShouldnotbeFuture");
      }

    }

    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  save() {
    if (!this.isValidModel()) {
      return;
    }
    else {

      let time = this.selectedTime.split(':');
      this.selectedDate.setHours(+time[0]);
      this.selectedDate.setMinutes(+time[1]);

      // let statusDate = new Date(this.currentWorkOrder.LastStatusTime);
      // if (this.selectedDate < statusDate) {
      //   this.alert.error("SelectedDateTimeMustBeAfterLastStatusDateTime");
      //   return;
      // }

      let dispatch = new EventWorkOrder;
      dispatch.WorkOrderID = this.currentWorkOrder.WorkOrderID;

      //alert time zone offset before send
      //dispatch.StatusTime = this.selectedDate;
      dispatch.StatusTime = new Date(this.selectedDate.getTime());
      dispatch.StatusTime.setMinutes(
        dispatch.StatusTime.getMinutes() - dispatch.StatusTime.getTimezoneOffset()
      );

      dispatch.StatusReasonID = this.reasonId;
      dispatch.StatusComment = this.comment;
      dispatch.ConfirmationCode = this.confirmationCode;

      dispatch.DriverID = this.currentWorkOrder.AssignedDriverID;
      dispatch.VehicleID = this.currentWorkOrder.AssignedVehicleID;
      dispatch.ServiceTypeID = this.currentWorkOrder.ServiceTypeID;
      switch (+this.targetStatusId) {
        case 6: //Out for delivery
          this.mainloading.PreloaderIcreaseCount();
          console.log("Awies ServiceTypeID:" + dispatch.ServiceTypeID);
          var Service
          if (dispatch.ServiceTypeID != 3) { 
            Service = this.orderDetailsService.OutForDeliveryWorkOrder(dispatch);
          }
          else {
            Service = this.orderDetailsService.SewerConfirmWorkOrder(dispatch);
          }

          Service.subscribe(res => {

            if (res.IsErrorState == false) {
              this.closePopup();
              this.alert.showSuccess();
            }
            else {
              this.alert.showError();
            }

          }
            , err => {
              this.mainloading.PreloaderDecreaseCount();
            }
            , () => {
              this.mainloading.PreloaderDecreaseCount();
            }
          );
          break;
        case 4: //Delivered
          //if( !isNullOrUndefined( this.confirmationCode )  && this.confirmationCode.length > 0 ){
          if (true) {
            this.mainloading.PreloaderIcreaseCount();
            var Service
            if (dispatch.ServiceTypeID != 3) {
              Service = this.orderDetailsService.DeliveredWorkOrder(dispatch);
            }
            else {
              Service = this.orderDetailsService.SewerCompleteWorkOrder(dispatch);
            }
            Service.subscribe(res => {
              console.log(res.ErrorDescription);
              if (res.IsErrorState == false) {
                this.closePopup();
                this.alert.showSuccess();
              }
              else {
                console.log(res.ErrorDescription);
                this.alert.error(res.ErrorDescription);
              }
            }
              , err => {
                this.mainloading.PreloaderDecreaseCount();
              }
              , () => {
                this.mainloading.PreloaderDecreaseCount();
              });
          }
          else {
            this.alert.error("EnterConfirmationCode");
          }

          break;
        case 8: //Cancelled
          this.mainloading.PreloaderIcreaseCount();
          this.orderDetailsService.CancelWorkOrder(dispatch).subscribe(res => {
            if (res.IsErrorState == false) {
              this.closePopup();
              this.alert.showSuccess();
            }
            else {
              this.alert.showError();
            }
          }
            , err => {
              this.mainloading.PreloaderDecreaseCount();
            }
            , () => {
              this.mainloading.PreloaderDecreaseCount();
            });
          break;
        case 7: //Arrived
          this.mainloading.PreloaderIcreaseCount();
          var Service
          if (dispatch.ServiceTypeID = 1) {
            Service = this.orderDetailsService.ArrivedWorkOrder(dispatch);
          }
          else if (dispatch.ServiceTypeID = 3) {
            Service = this.orderDetailsService.SewerConfirmWorkOrder(dispatch);
          }
          Service.subscribe(res => {
            if (res.IsErrorState == false) {
              this.closePopup();
              this.alert.showSuccess();
            }
            else {
              this.alert.showError();
            }
          }
            , err => {
              this.mainloading.PreloaderDecreaseCount();
            }
            , () => {
              this.mainloading.PreloaderDecreaseCount();
            });
          break;
        case 3: //Failed to deliver
          this.mainloading.PreloaderIcreaseCount();
          this.orderDetailsService.FailedToDeliver(dispatch).subscribe(res => {
            if (res.IsErrorState == false) {
              this.closePopup();
              this.alert.showSuccess();
            }
            else {
              this.alert.showError();
            }
          }
            , err => {
              this.mainloading.PreloaderDecreaseCount();
            }
            , () => {
              this.mainloading.PreloaderDecreaseCount();
            });
          break;
        case 2: //OnHold
          this.mainloading.PreloaderIcreaseCount();
          this.orderDetailsService.OnHold(dispatch).subscribe(res => {
            if (res.IsErrorState == false) {
              this.closePopup();
              this.alert.showSuccess();
            }
            else {
              this.alert.showError();
            }
          }
            , err => {
              this.mainloading.PreloaderDecreaseCount();
            }
            , () => {
              this.mainloading.PreloaderDecreaseCount();
            });
          break;
        case 1: //Not Assigned, new
          this.mainloading.PreloaderIcreaseCount();
          this.orderDetailsService.NotAssigned(dispatch).subscribe(res => {
            if (res.IsErrorState == false) {
              this.closePopup();
              this.alert.showSuccess();
            }
            else {
              this.alert.showError();
            }
          }
            , err => {
              this.mainloading.PreloaderDecreaseCount();
            }
            , () => {
              this.mainloading.PreloaderDecreaseCount();
            });
          break;
      }
    }
  }

  closePopup() {
    this.StatusChanged.emit(true);
    this.modalRef.hide();
  }
}
