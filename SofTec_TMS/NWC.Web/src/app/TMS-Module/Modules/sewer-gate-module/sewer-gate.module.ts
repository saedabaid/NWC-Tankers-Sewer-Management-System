import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { NgMultiSelectDropDownModule } from "ng-multiselect-dropdown";
import { ModalModule } from "ngx-bootstrap/modal";
import { NgxPrintModule } from "ngx-print";
import { SharedModule } from "src/app/shared/shared.module";
import { EnteryGateListComponent } from "./entery-gate-list/entery-gate-list.component";
import { ExitGateListComponent } from "./exit-gate-list/exit-gate-list.component";
import { PrintVehicleTicketComponent } from "./print-vehicle-ticket/print-vehicle-ticket.component";
import { SewerGateRoutingModule } from "./sewer-gate-routing.module";
import { VehicleViolationComponent } from "./vehicle-violation/vehicle-violation.component";

@NgModule({
  declarations: [
    //GateComponent,
    EnteryGateListComponent,
    ExitGateListComponent,
    PrintVehicleTicketComponent,
    VehicleViolationComponent,
  ],
  imports: [
    CommonModule,
    SewerGateRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    ModalModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),
    NgxPrintModule,
  ],
  entryComponents: [VehicleViolationComponent],
})
export class SewerGateModule {}
