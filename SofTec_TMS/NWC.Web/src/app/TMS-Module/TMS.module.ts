
// import { BsDropdownModule, CollapseModule,  SortableModule } from 'ngx-bootstrap';
import { NgModule, Component } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TMSRoutingModule } from './TMS-routing.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { SharedModule } from '@tms-shared/shared.module';
import { NgxPrintModule } from 'ngx-print';
// import { BsDropdownModule } from 'ngx-bootstrap/dropdown/bs-dropdown.module';


@NgModule({
  imports: [
    BsDropdownModule,
    CommonModule,
    FormsModule,
    SharedModule,
    TMSRoutingModule,
    ModalModule,
    ModalModule.forRoot(),
    // CollapseModule,
     NgMultiSelectDropDownModule.forRoot(),
    // SortableModule.forRoot(),
    NgxPrintModule
  ],
  declarations: [
    // TMSHomeComponent,
    LandingPageComponent  

  ],
  entryComponents: [],
  providers: [
    DatePipe
  ]
})
export class TMSModule { }
