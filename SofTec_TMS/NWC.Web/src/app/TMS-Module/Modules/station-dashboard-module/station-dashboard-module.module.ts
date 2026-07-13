import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StationDashboardComponent } from './station-dashboard/station-dashboard.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@tms-shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { MainVehicleDataComponent } from './main-vehicle-data/main-vehicle-data.component';
import { StationDashboardModuleRoutingModule } from './station-dashboard-module-routing.module';
import { VehicleInsideDataComponent } from './vehicle-inside-data/vehicle-inside-data.component';
import { AmrDataComponent } from './amr-data/amr-data.component';
import { VehicleOutsideDataComponent } from './vehicle-outside-data/vehicle-outside-data.component';
import { OrderDataComponent } from './order-data/order-data.component';
import { TablePriceDataComponent } from './table-price-data/table-price-data.component';
import { TableDayOrderComponent } from './table-day-order/table-day-order.component';


@NgModule({
  declarations: [
    StationDashboardComponent,
    MainVehicleDataComponent,
    VehicleInsideDataComponent,
    AmrDataComponent,
    VehicleOutsideDataComponent,
    OrderDataComponent,
    TablePriceDataComponent,
    TableDayOrderComponent
  ],
  imports: [
    CommonModule,
    StationDashboardModuleRoutingModule,
    FormsModule,
    SharedModule,
    NgMultiSelectDropDownModule.forRoot(),
  ]
})
export class StationDashboardModuleModule { }
