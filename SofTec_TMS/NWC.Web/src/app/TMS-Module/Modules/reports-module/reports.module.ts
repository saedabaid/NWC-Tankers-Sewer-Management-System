import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
// import {
//   SortableModule,
//   BsDropdownModule,
//   CollapseModule
// } from "ngx-bootstrap";
import { FormsModule } from "@angular/forms";
import { SharedModule } from "src/app/shared/shared.module";
import { NgMultiSelectDropDownModule } from "ng-multiselect-dropdown";
import { ReportsRoutingModule } from './reports.routing.module';
import { MainReportsComponent } from './main-reports/main-reports.component';
import { DailyOrdersSummaryComponent } from './daily-orders-summary/daily-orders-summary.component';
import { DailyOrderDetailsComponent } from './daily-order-details/daily-order-details.component';
import { VehicleLogComponent } from './vehicle-log/vehicle-log.component';
import { VehiclePerformanceComponent } from './vehicle-performance/vehicle-performance.component';
import { SoqyaScheduleReportComponent } from './soqya-schedule-report/soqya-schedule-report.component';
import { VehicleDataComponent } from './vehicle-data/vehicle-data.component';
import { OrdersPerZoneComponent } from './orders-per-zone/orders-per-zone.component';
import { StationOrderCapacityComponent } from './station-order-capacity/station-order-capacity.component';
import { StationServiceTimeComponent } from './station-service-time/station-service-time.component';
import { TankersPermissionsStatusComponent } from './tankers-permissions-status/tankers-permissions-status.component';
import { SoqyaScheduledPerDayComponent } from './soqya-scheduled-per-day/soqya-scheduled-per-day.component';
import { OrderReassignmentComponent } from "./order-reassignment/order-reassignment.component";

import { ContractsTariffsComponent } from './contracts-tariffs/contracts-tariffs-component';
@NgModule({
  declarations:
    [
      MainReportsComponent,
      DailyOrdersSummaryComponent,
      DailyOrderDetailsComponent,
      VehicleLogComponent,
      VehiclePerformanceComponent,
      SoqyaScheduleReportComponent,
      VehicleDataComponent,
      OrdersPerZoneComponent,
      StationOrderCapacityComponent,
      StationServiceTimeComponent,
      TankersPermissionsStatusComponent,
      SoqyaScheduledPerDayComponent,
      OrderReassignmentComponent,
      ContractsTariffsComponent
    ],
  imports: [
    CommonModule,
    ReportsRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot()
  ]
})
export class ReportsModule { }
