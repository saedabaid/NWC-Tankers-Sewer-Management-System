import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StaffCreateComponent } from './staff-create/staff-create.component';
import { StaffListComponent } from './staff-list/staff-list.component';
import { StaffUpdateComponent } from './staff-update/staff-update.component';


const routes: Routes = [
  {
    path: "",
    pathMatch: "prefix",
    children: [
      {
          path: "",
          pathMatch: "full",
          component: StaffListComponent
      },
      {
        path: "create",
        pathMatch: "full",
        component: StaffCreateComponent
      },
      {
        path: "update/:id",
        pathMatch: "full",
        component: StaffCreateComponent
      }
    ]
}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StaffRoutingModule { }
