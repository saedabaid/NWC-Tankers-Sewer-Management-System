import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PrintTicketComponent } from "src/app/shared/component/print-ticket/print-ticket.component";
import { EnteryGateListComponent } from "./entery-gate-list/entery-gate-list.component";
import { ExitGateListComponent } from "./exit-gate-list/exit-gate-list.component";
import { PrintVehicleTicketComponent } from "./print-vehicle-ticket/print-vehicle-ticket.component";

const routes: Routes = [
  {
    path: "",
    //component: GateComponent,
    pathMatch: "prefix",
    children: [
      {
        path: "entry",
        pathMatch: "full",
        component: EnteryGateListComponent,
      },
      {
        path: "exit",
        pathMatch: "full",
        component: ExitGateListComponent,
      },
      {
        path: "print/:VehicleID/:WorkOrderID",
        pathMatch: "full",
        component: PrintTicketComponent,
      },
      {
        path: "printVehicle/:VehicleID",
        pathMatch: "full",
        component: PrintVehicleTicketComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SewerGateRoutingModule {}
