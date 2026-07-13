import { NgModule, LOCALE_ID } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { OrderRoutingModule } from './order-routing.module';
//import { SortableModule, ModalModule, CollapseModule, BsDropdownModule } from 'ngx-bootstrap';
import { ViewListOrdersComponent } from './work-order-list/work-order-list.component';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { ChangesLogComponent } from './order-details/changes-log/changes-log.component';
import { ShipmentDataComponent } from './order-details/shipment-data/shipment-data.component';
import { StatusLogComponent } from './order-details/status-log/status-log.component';
import { ComplainsComponent } from './order-details/complains/complains.component';
import { PaymentsComponent } from './order-details/payments/payments.component';
import { CommentsComponent } from './order-details/comments/comments.component';
import { AttachmentsComponent } from './order-details/attachments/attachments.component';
import { ChangeOrderStatusComponent } from './change-order-status/change-order-status.component';
import { DeassignTankerComponent } from './deassign-tanker/deassign-tanker.component';
import { ManualDispatchTankerComponent } from './manual-dispatch-tanker/manual-dispatch-tanker.component';
import { AddOrderCommentComponent } from './add-order-comment/add-order-comment.component';
import { OrderCreateComponent } from './order-create/order-create.component';
import { WorkOrderSearchService } from '../../Services/work-order-search.service';
import { OrderDetailsService } from '../../Services/order-details.service';
//import { OrderComponent } from './order.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormsModule } from '@angular/forms';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { MassOrderTransferStationComponent } from './mass-order-transfer-station/mass-order-transfer-station.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';



@NgModule({
  declarations: [
    //OrderComponent,
    ViewListOrdersComponent,
    OrderDetailsComponent,
    ChangesLogComponent,
    ShipmentDataComponent,
    StatusLogComponent,
    ComplainsComponent,
    PaymentsComponent,
    CommentsComponent,
    AttachmentsComponent,
    ChangeOrderStatusComponent,
    DeassignTankerComponent,
    ManualDispatchTankerComponent,
    AddOrderCommentComponent,
    OrderCreateComponent,
    MassOrderTransferStationComponent
  ],
  imports: [
    CommonModule,
    OrderRoutingModule,
    // SortableModule.forRoot(),
    BsDropdownModule,
    FormsModule,
    SharedModule,
    ModalModule,
    CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),

  ],
  entryComponents:[
    ChangeOrderStatusComponent,
    DeassignTankerComponent,
    ManualDispatchTankerComponent,
    AddOrderCommentComponent
  ],
  providers: [
    WorkOrderSearchService,
    OrderDetailsService,
    DatePipe,
  ]
})
export class OrderModule { }


