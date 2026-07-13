import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StationDashboardComponent } from './station-dashboard/station-dashboard.component';


const routes: Routes = [
  {
    path: "",
    pathMatch: "prefix",
    children: [
      {
        path: "",
        pathMatch: "full",
        component: StationDashboardComponent,
      },
      // {
      //   path: "main",
      //   pathMatch: "full",
      //   component: MainDashboardComponent
      // }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StationDashboardModuleRoutingModule { }
