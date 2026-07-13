import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
//import { SortableModule, BsDropdownModule, ModalModule, CollapseModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

import { DeferredOrderDetailsComponent } from './deferred-order-details/deferred-order-details.component';
import { IntegrationRoutingModule } from './integration-routing.module';
import { DeferredOrderListComponent } from './deferred-order-list/deferred-order-list.component';
import { ModalModule } from 'ngx-bootstrap/modal';


@NgModule({
  declarations: [
    DeferredOrderListComponent,
    DeferredOrderDetailsComponent
  ],
  imports: [
    CommonModule,
    IntegrationRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    ModalModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),

  ],
  entryComponents:[
  ],
  providers: [

    DatePipe

  ]
})
export class IntegrationModule { }
