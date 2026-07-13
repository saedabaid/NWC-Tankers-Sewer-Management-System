import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

//import { SortableModule, BsDropdownModule, ModalModule, CollapseModule } from 'ngx-bootstrap';

import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

import { ContractorListComponent } from './contractor-list/contractor-list.component';
import { ContractorCreateComponent } from './contractor-create/contractor-create.component';
import { ContractorRoutingModule } from './contrator-routing.module';
import { ModalModule } from 'ngx-bootstrap/modal';


@NgModule({
  declarations: [

    ContractorListComponent,
    ContractorCreateComponent,
  ],
  imports: [
    CommonModule,
    ContractorRoutingModule,
    //SortableModule.forRoot(),
    //BsDropdownModule,
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
export class ContractorModule { }
