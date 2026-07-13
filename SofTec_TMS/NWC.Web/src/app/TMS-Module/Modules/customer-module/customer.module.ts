import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
//import { SortableModule, BsDropdownModule, CollapseModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { NewCustomerComponent } from './new-customer/new-customer.component';
import { CustomerRoutingModule } from './customer.routing.module';
import { CustomerListComponent } from './customer-list/customer-list.component';


@NgModule({
  declarations: [NewCustomerComponent, CustomerListComponent],
  imports: [
    CommonModule,
    CustomerRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),
  ]
})

export class CustomerModule { }
