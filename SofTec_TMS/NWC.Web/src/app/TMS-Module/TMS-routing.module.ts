
import { RouterModule, Routes } from "@angular/router";
import { NgModule } from "@angular/core";

import { AuthGuard } from '../shared/Providers/auth-guard.provider';
import { LandingPageComponent } from "./landing-page/landing-page.component";




const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        component: LandingPageComponent
    },
    {
        path: 'defaultpage',
        pathMatch: 'full',
        component: LandingPageComponent
    },
    {
        path: 'defaultpage/:token',
        pathMatch: 'full',
        component: LandingPageComponent
    },
    {
        path: 'defaultpage/:token/:culture',
        pathMatch: 'full',
        component: LandingPageComponent
    },
    {
        path: 'order',
        loadChildren: './Modules/order-module/order.module#OrderModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'contract',
        loadChildren: './Modules/contract-module/contract.module#ContractModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'zone',
        loadChildren: './Modules/zone-module/zone.module#ZoneModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'gate',
        loadChildren: './Modules/gate-module/gate.module#GateModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'contractor',
        loadChildren: './Modules/contractor-module/contractor.module#ContractorModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'devicemeter',
        loadChildren: './Modules/device-meter-module/device-meter.module#DeviceMeterModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'controlpanel',
        loadChildren: './Modules/controlpanel-module/controlpanel.module#ControlPanelModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'dashboard',
        loadChildren: './Modules/dashboard-module/dashboard.module#DashboardModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'reports',
        loadChildren: './Modules/reports-module/reports.module#ReportsModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'customer',
        loadChildren: './Modules/customer-module/customer.module#CustomerModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'integration',
        loadChildren: './Modules/integration-module/integration.module#IntegrationModule',
        canActivate: [AuthGuard]
    },
    {

        path: 'soqya',
        loadChildren: './Modules/soqya-module/soqya-module.module#SoqyaModuleModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'vehicle',
        loadChildren: './Modules/vehicle-module/vehicle.module#VehicleModule',
        canActivate: [AuthGuard]
    },
    {

        path: 'staff',
        loadChildren: './Modules/staff/staff.module#StaffModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'sewer-gate',
        loadChildren: './Modules/sewer-gate-module/sewer-gate.module#SewerGateModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'permits',
        loadChildren: './Modules/permit-module/permit.module#PermitModule',
        canActivate: [AuthGuard]
    },
    {
        path: 'station-dashboard',
        loadChildren: './Modules/station-dashboard-module/station-dashboard-module.module#StationDashboardModuleModule',
        canActivate: [AuthGuard]
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class TMSRoutingModule { }
