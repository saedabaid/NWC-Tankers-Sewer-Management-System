import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GateRoutingModule } from './gate-routing.module';
//import { SortableModule, BsDropdownModule, ModalModule, CollapseModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
//import { GateComponent } from './gate.component';
import { EnteryGateListComponent } from './entery-gate-list/entery-gate-list.component';
import { ExitGateListComponent } from './exit-gate-list/exit-gate-list.component';
import { PrintVehicleTicketComponent } from './print-vehicle-ticket/print-vehicle-ticket.component';
import { NgxPrintModule } from 'ngx-print';
import { VehicleViolationComponent } from './vehicle-violation/vehicle-violation.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgxQRCodeModule } from '@techiediaries/ngx-qrcode';


@NgModule({
  declarations: [
    //GateComponent,
    EnteryGateListComponent,
    ExitGateListComponent,
    PrintVehicleTicketComponent,
    VehicleViolationComponent,

  ],
  imports: [
    CommonModule ,
    GateRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    NgxQRCodeModule,
    FormsModule,
    SharedModule,
    ModalModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),
    NgxPrintModule
  ],
  entryComponents: [
    VehicleViolationComponent,

  ]
})
export class GateModule { }
