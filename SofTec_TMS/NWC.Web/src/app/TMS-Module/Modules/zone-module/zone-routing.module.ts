
//import { ZoneComponent } from './zone.component';
import { Routes, RouterModule } from '@angular/router';

import { NgModule } from '@angular/core';
import { ZoneListComponent } from './zoneList/zoneList.component';
import { NewZoneComponent } from './new-zone/new-zone.component';


const routes: Routes = [
    {
        path: '',
        //component: ZoneComponent,
        pathMatch: 'prefix',
        children: [
          
            {
                path: 'zonelist',
                pathMatch: 'full',
                component: ZoneListComponent

            },
            {
                path: 'edit/:ID',
                pathMatch: 'full',
                component: NewZoneComponent
            },
            {
                path: 'new',
                pathMatch: 'full',
                component: NewZoneComponent
            },
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ZoneRoutingModule { }
