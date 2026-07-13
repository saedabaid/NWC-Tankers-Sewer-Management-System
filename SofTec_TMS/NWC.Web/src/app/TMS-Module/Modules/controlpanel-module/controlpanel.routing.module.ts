import { Routes, RouterModule } from '@angular/router';

import { NgModule } from '@angular/core';
import { UserStationPermissionComponent } from './user-station-permission/user-station-permission.component';
import { StationSettingsComponent } from './station-settings/station-settings/station-settings.component';
import { VehicleSettingsComponent } from './vehicle-settings/vehicle-settings.component';
import { CitySettingsComponent } from './city-settings/city-settings/city-settings.component';
import { IndexComponent } from './index/index.component';
import { StaffRolesComponent } from './staff-roles/staff-roles.component';
import { UsersListComponent } from './users-list/users-list.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import {ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { RoleCreateComponent } from './role-create/role-create.component';
import { BranchManagementComponent } from './branch-management/branch-management.component';
import { LandmarkTypesComponent } from './landmark-types/landmark-types.component';
import { VehicleBrandComponent } from './vehicle-brand/vehicle-brand.component';
import { VehicleModelComponent } from './vehicle-model/vehicle-model.component';
import { VehicleTypesComponent } from './vehicle-types/vehicle-types.component';
import { InsuranceCompaniesComponent } from './insurance-companies/insurance-companies.component';
import { CreateBranchManagementComponent } from './branch-management/create-branch-management/create-branch-management.component';
import { CreateLandmarkTypesComponent } from './landmark-types/create-landmark-types/create-landmark-types.component';
import { CreateVehicleBrandComponent } from './vehicle-brand/create-vehicle-brand/create-vehicle-brand.component';
import { CreateVehicleModelComponent } from './vehicle-model/create-vehicle-model/create-vehicle-model.component';
import { CreateVehicleTypesComponent } from './vehicle-types/create-vehicle-types/create-vehicle-types.component';
import { CreateInsuranceCompaniesComponent } from './insurance-companies/create-insurance-companies/create-insurance-companies.component';
import { changePasswordService } from '../../Services/change-password.service';
import { NewEditCitySettingsComponent } from './city-settings/new-edit-city-settings/new-edit-city-settings.component';
import { NewEditStationSettingsComponent } from './station-settings/new-edit-station-settings/new-edit-station-settings.component';
import { MyAccountComponent } from './my-account/my-account.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: IndexComponent,
      },
      {
        path: 'user-landmark-permission',
        pathMatch: 'full',
        component: UserStationPermissionComponent,
      },
      {
        path: 'station-settings',
        pathMatch: 'full',
        component: StationSettingsComponent,
      },
      {
        path: 'station-settings/:stationId',
        pathMatch: 'full',
        component: NewEditStationSettingsComponent,
      },
      {
        path: 'vehicle-settings',
        pathMatch: 'full',
        component: VehicleSettingsComponent,
      },
      // {
      //   path: 'city-settings',
      //   pathMatch: 'full',
      //   component: CitySettingsComponent,
      // },
      {
        path: 'branch-settings',
        pathMatch: 'full',
        component: BranchManagementComponent,
      }, 
      {
        path: 'change-password',
        pathMatch: 'full',
        component: ChangePasswordComponent,
      },
      {
        path: 'forgot-password',
        pathMatch: 'full',
        component: ForgotPasswordComponent,
      },
      {
        path: 'city-settings',
        pathMatch: 'full',
        component: CitySettingsComponent,
      },
      {
        path: 'city-settings/:cityId',
        pathMatch: 'full',
        component: NewEditCitySettingsComponent,
      },
      {
        path: 'staff-role',
        pathMatch: 'full',
        component: StaffRolesComponent,
      },
      {
        path: 'users-list',
        pathMatch: 'full',
        component: UsersListComponent,
      },
      {
        path: 'role-create',
        pathMatch: 'full',
        component: RoleCreateComponent,
      },
      {
        path: 'branch-management',
        pathMatch: 'full',
        component: BranchManagementComponent,
      },
      {
        path: 'branch-management/create',
        pathMatch: 'full',
            component: CreateBranchManagementComponent,
      },
      {
        path: 'landmark-types',
        pathMatch: 'full',
        component: LandmarkTypesComponent,
      },
      {
        path: 'landmark-types/create',
        pathMatch: 'full',
            component: CreateLandmarkTypesComponent,
      },
      {
        path: 'vehicle-brand',
        pathMatch: 'full',
        component: VehicleBrandComponent,
      },
      {
        path: 'vehicle-brand/create',
        pathMatch: 'full',
            component: CreateVehicleBrandComponent,
      },
      {
        path: 'vehicle-model',
        pathMatch: 'full',
        component: VehicleModelComponent,
      },
      {
        path: 'vehicle-model/create',
        pathMatch: 'full',
            component: CreateVehicleModelComponent,
      },
      {
        path: 'vehicle-types',
        pathMatch: 'full',
        component: VehicleTypesComponent,
      },
      {
        path: 'vehicle-types/create',
        pathMatch: 'full',
            component: CreateVehicleTypesComponent,
      },
      {
        path: 'insurance-companies',
        pathMatch: 'full',
        component: InsuranceCompaniesComponent,
      },
      {
        path: 'insurance-companies/create',
        pathMatch: 'full',
            component: CreateInsuranceCompaniesComponent,
      },
      {
        path: 'my-account',
        pathMatch: 'full',
        component: MyAccountComponent,
      }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ControlPanelRoutingModule {}
