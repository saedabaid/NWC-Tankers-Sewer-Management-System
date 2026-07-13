

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddMeterReadingComponent } from './add-meter-reading/add-meter-reading.component';
import { MeterReadingListComponent } from './meter-reading-list/meter-reading-list.component';

import { TestVPNComponent } from './test-vpn/test-vpn.component';
import { DeviceMeterListComponent } from './device-meter-list/device-meter-list.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    children: [
      {
        path: 'readingslist',
        pathMatch: 'full',
        component: MeterReadingListComponent
      },
      {
        path: 'newReading',
        pathMatch: 'full',
        component: AddMeterReadingComponent
      },
      {
        path: 'devicemeterlist',
        pathMatch: 'full',
        component: DeviceMeterListComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DeviceMeterRoutingModule { }
