import { Routes, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { MainReportsComponent } from "./main-reports/main-reports.component";
import { DailyOrdersSummaryComponent } from "./daily-orders-summary/daily-orders-summary.component";
import { DailyOrderDetailsComponent } from "./daily-order-details/daily-order-details.component";
import { VehicleLogComponent } from "./vehicle-log/vehicle-log.component";
import { VehiclePerformanceComponent } from "./vehicle-performance/vehicle-performance.component";
import { SoqyaScheduleReportComponent } from "./soqya-schedule-report/soqya-schedule-report.component";
import { VehicleDataComponent } from "./vehicle-data/vehicle-data.component";
import { OrdersPerZoneComponent } from "./orders-per-zone/orders-per-zone.component";
import { StationOrderCapacityComponent } from "./station-order-capacity/station-order-capacity.component";
import { StationServiceTimeComponent } from "./station-service-time/station-service-time.component";
import { TankersPermissionsStatusComponent } from "./tankers-permissions-status/tankers-permissions-status.component";
import { SoqyaScheduledPerDayComponent } from "./soqya-scheduled-per-day/soqya-scheduled-per-day.component";
import { OrderReassignmentComponent } from "./order-reassignment/order-reassignment.component";
import { ContractsTariffsComponent } from "./contracts-tariffs/contracts-tariffs-component";

const routes: Routes = [
    {
        path: "",
        pathMatch: "prefix",
        children: [
            {
                path: "",
                pathMatch: "full",
                component: MainReportsComponent
            },
            {
                path: "DailyOrdersSummary",
                pathMatch: "full",
                component: DailyOrdersSummaryComponent
            },
            {
                path: "DailyOrdersDetails",
                pathMatch: "full",
                component: DailyOrderDetailsComponent
            },
            {
                path: "VehicleLogs",
                pathMatch: "full",
                component: VehicleLogComponent
            },
            {
                path: "VehicleData",
                pathMatch: "full",
                component: VehicleDataComponent
            },
            {
                path: "SoqyaCustomerBalance",
                pathMatch: "full",
                component: SoqyaScheduleReportComponent
            },
            {
                path: "OrdersPerZone",
                pathMatch: "full",
                component: OrdersPerZoneComponent
            },
            {
                path: "StationOrderCapacity",
                pathMatch: "full",
                component: StationOrderCapacityComponent
            },
            {
                path: "StationServiceTime",
                pathMatch: "full",
                component: StationServiceTimeComponent
            },
            {
                path: "TankersPermissionsStatus",
                pathMatch: "full",
                component: TankersPermissionsStatusComponent
            },
            {
                path: "SoqyaScheduledPerDay",
                pathMatch: "full",
                component: SoqyaScheduledPerDayComponent
            },
            {
                path: "VehiclePerformance",
                pathMatch: "full",
                component: VehiclePerformanceComponent
            },
            {
                path: "OrderReassignment",
                pathMatch: "full",
                component: OrderReassignmentComponent
            },
            {
                path: "ContractsTariffs",
                pathMatch: "full",
                component: ContractsTariffsComponent
            },
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReportsRoutingModule { }
