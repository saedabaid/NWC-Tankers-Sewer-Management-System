import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
// import {
//   SortableModule,
//   BsDropdownModule,
//   CollapseModule
// } from "ngx-bootstrap";
import { FormsModule } from "@angular/forms";
import { SharedModule } from "src/app/shared/shared.module";
import { NgMultiSelectDropDownModule } from "ng-multiselect-dropdown";
import { DashboardRoutingModule } from "./dashboard.routing.module";
import { MainDashboardComponent } from "./main-dashboard/main-dashboard.component";
import { DashboardTilesCardsComponent } from "./dashboard-tiles-cards/dashboard-tiles-cards.component";
import { OrdersDayHoursChartComponent } from "./orders-day-hours-chart/orders-day-hours-chart.component";
import { OrdersZonesChartComponent } from "./orders-zones-chart/orders-zones-chart.component";
import { OrdersStatusesChartComponent } from "./orders-statuses-chart/orders-statuses-chart.component";
import { OrdersDateChartComponent } from "./orders-date-chart/orders-date-chart.component";
import { OrdersExecuteTimeChartComponent } from "./orders-execute-time-chart/orders-execute-time-chart.component";
import { DashboardVehicleCardsComponent } from "./dashboard-vehicle-cards/dashboard-vehicle-cards.component";
import { StationDashboardComponent } from "./station-dashboard/station-dashboard.component";
@NgModule({
  declarations: [
    MainDashboardComponent,
    DashboardTilesCardsComponent,
    OrdersDayHoursChartComponent,
    OrdersZonesChartComponent,
    OrdersStatusesChartComponent,
    OrdersDateChartComponent,
    OrdersExecuteTimeChartComponent,
    DashboardVehicleCardsComponent,
    StationDashboardComponent,
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),
  ],
})
export class DashboardModule {}
