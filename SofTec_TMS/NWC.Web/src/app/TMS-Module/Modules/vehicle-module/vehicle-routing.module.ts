import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NewVehicleComponent } from './new-vehicle/new-vehicle.component';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'prefix',
        children: [
            {
                path: 'vehiclelist',
                pathMatch: 'full',
                component: VehicleListComponent

            },
            {
                path: 'edit/:ID',
                pathMatch: 'full',
                component: NewVehicleComponent
            },
            {
                path: 'new',
                pathMatch: 'full',
                component: NewVehicleComponent
            },
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class VehicleRoutingModule { }
