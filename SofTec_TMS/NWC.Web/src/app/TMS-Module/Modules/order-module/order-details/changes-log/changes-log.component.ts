import { Component, ViewEncapsulation, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { WorkOrderChangeLog } from 'src/app/TMS-Module/Models/work-order-change-log';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { ActivatedRoute } from '@angular/router';
import { OrderDetailsService } from 'src/app/TMS-Module/Services/order-details.service';
import { EventTypeEnum } from 'src/app/TMS-Module/Models/enums/event-type';
import { TranslateService } from '@ngx-translate/core';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'changes-log',
  templateUrl: './changes-log.component.html',
  styleUrls: ['./changes-log.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ChangesLogComponent implements OnInit {
  isCollapsed: boolean[] = [];
  @Input() Logs: WorkOrderChangeLog[];
  @Output() LogsChanged = new EventEmitter<WorkOrderChangeLog[]>();
  @Input() OrderNo: string;
  dateFormat = ' d MMM y, h:mm a';

  eventTypesEnums = EventTypeEnum;
  commonEventTypes = [1, 11, 10, 3, 5, 6, 7, 9, 8, 4];
  // commonEventTypes: number[] =
  //   [
  //     EventTypeEnum.WorkOrder_Create,
  //     EventTypeEnum.WorkOrder_NotAssigned,
  //     EventTypeEnum.WorkOrder_OnHold,
  //     EventTypeEnum.WO_Vehicle_Assign,
  //     EventTypeEnum.WorkOrder_OutForDelivery,
  //     EventTypeEnum.WorkOrder_Arrived,
  //     EventTypeEnum.WorkOrder_Delivered,
  //     EventTypeEnum.WorkOrder_FailedToDeliver,
  //     EventTypeEnum.WorkOrder_Cancelled,
  //     EventTypeEnum.WO_Vehicle_Deassign
  //   ];


    changesEventTypes: number[] = [11, 10, 3, 5, 6, 7, 9, 8 ];
  // changesEventTypes: number[] =
  //   [
  //     EventTypeEnum.WorkOrder_NotAssigned,
  //     EventTypeEnum.WorkOrder_OnHold,
  //     EventTypeEnum.WO_Vehicle_Assign,
  //     EventTypeEnum.WorkOrder_OutForDelivery,
  //     EventTypeEnum.WorkOrder_Arrived,
  //     EventTypeEnum.WorkOrder_Delivered,
  //     EventTypeEnum.WorkOrder_FailedToDeliver,
  //     EventTypeEnum.WorkOrder_Cancelled,
  //   ];

  vehicleEventTypes: number[] = [3, 4, 5, 6, 7, 8, 9, 11];
  // vehicleEventTypes: number[] =
  //   [
  //     EventTypeEnum.WorkOrder_OnHold,
  //     EventTypeEnum.WO_Vehicle_Assign,
  //     EventTypeEnum.WorkOrder_OutForDelivery,
  //     EventTypeEnum.WorkOrder_Arrived,
  //     EventTypeEnum.WorkOrder_Delivered,
  //     EventTypeEnum.WorkOrder_FailedToDeliver,
  //     EventTypeEnum.WorkOrder_Cancelled,
  //     EventTypeEnum.WO_Vehicle_Deassign
  //   ];

    reasonEventTypes = [9, 8, 4];
  // reasonEventTypes: number[] =
  //   [
  //     EventTypeEnum.WorkOrder_FailedToDeliver,
  //     EventTypeEnum.WorkOrder_Cancelled,
  //     EventTypeEnum.WO_Vehicle_Deassign
  //   ];


    placedUpdateTypes = [1, 2];
  // placedUpdateTypes: number[] =
  //   [
  //     EventTypeEnum.WorkOrder_Create,
  //     EventTypeEnum.WorkOrder_Update
  //   ];


  constructor(private workOrderService: OrderDetailsService,
    private router: ActivatedRoute,
    private _translate: TranslateService,
    private mainloading: LoaderService) {
  }

  ngOnInit() {
    if (isNullOrUndefined(this.Logs) || this.Logs.length === 0) {
      this.updateLogs();
    }

    this._translate.onLangChange.subscribe(res => {
      this.updateLogs();
    });
  }

  updateLogs() {
    this.mainloading.PreloaderIcreaseCount();
    this.workOrderService.GetChangesLogs(+this.router.snapshot.params["ID"]).subscribe(res => {
      this.Logs = res.Value;
      this.LogsChanged.emit(this.Logs);
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });
  }

}
