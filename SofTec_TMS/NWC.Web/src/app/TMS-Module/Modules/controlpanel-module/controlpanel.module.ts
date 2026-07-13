import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { UserStationPermissionComponent } from './user-station-permission/user-station-permission.component';
import { ControlPanelRoutingModule } from './controlpanel.routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { StationSettingsComponent } from './station-settings/station-settings/station-settings.component';
import { VehicleSettingsComponent } from './vehicle-settings/vehicle-settings.component';
import { CitySettingsComponent } from './city-settings/city-settings/city-settings.component';
import { IndexComponent } from './index/index.component';
import { UsersListComponent } from './users-list/users-list.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { StaffRolesComponent } from './staff-roles/staff-roles.component';
import { SharedModule } from '@tms-shared/shared.module';
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
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { TreeModule } from 'angular-tree-component';
import { TreeComponent } from './branch-management/tree/tree.component';
import { NewEditCitySettingsComponent } from './city-settings/new-edit-city-settings/new-edit-city-settings.component';
import { NewEditStationSettingsComponent } from './station-settings/new-edit-station-settings/new-edit-station-settings.component';
import { MyAccountComponent } from './my-account/my-account.component';
import { NgbTabsetModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    UserStationPermissionComponent,
    StationSettingsComponent,
    NewEditStationSettingsComponent,
    VehicleSettingsComponent,
    CitySettingsComponent,
    NewEditCitySettingsComponent,
    IndexComponent,
    UsersListComponent,
    ChangePasswordComponent,
    ForgotPasswordComponent,
    StaffRolesComponent,
    RoleCreateComponent,
    BranchManagementComponent,
    LandmarkTypesComponent,
    VehicleBrandComponent,
    VehicleModelComponent,
    VehicleTypesComponent,
    InsuranceCompaniesComponent,
    CreateBranchManagementComponent,
    CreateLandmarkTypesComponent,
    CreateVehicleBrandComponent,
    CreateVehicleModelComponent,
    CreateVehicleTypesComponent,
    CreateInsuranceCompaniesComponent,
    TreeComponent,
    MyAccountComponent 
    ],
  imports: [
    CommonModule,
    ControlPanelRoutingModule,
    FormsModule,
    SharedModule,
    ReactiveFormsModule,
    NgbTabsetModule,
    NgMultiSelectDropDownModule.forRoot(),
    TabsModule.forRoot(),
    TreeModule.forRoot()
  ]
})

export class ControlPanelModule { }
