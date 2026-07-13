import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { MainDashboardComponent } from "./main-dashboard/main-dashboard.component";
import { StationDashboardComponent } from "./station-dashboard/station-dashboard.component";

const routes: Routes = [
  {
    path: "",
    pathMatch: "prefix",
    children: [
      {
        path: "",
        pathMatch: "full",
        component: MainDashboardComponent,
      },
      // {
      //   path: "main",
      //   pathMatch: "full",
      //   component: MainDashboardComponent
      // }
    ],
  },
  {
    path: "station",
    pathMatch: "full",
    component: StationDashboardComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DashboardRoutingModule {}
