import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StaffRoutingModule } from './staff-routing.module';
import { StaffListComponent } from './staff-list/staff-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ModalModule } from 'ngx-bootstrap/modal';
import { SharedModule } from 'src/app/shared/shared.module';
import { StaffCreateComponent } from './staff-create/staff-create.component';
import { StaffUpdateComponent } from './staff-update/staff-update.component';
import { UploadStaffExcelComponent } from './upload-staff-excel/upload-staff-excel.component';


@NgModule({
  declarations: [
    StaffListComponent,
    StaffCreateComponent,
    StaffUpdateComponent,
    UploadStaffExcelComponent
  ],
  imports: [
    CommonModule,
    StaffRoutingModule,
    BsDropdownModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    ModalModule,
    CollapseModule,
    NgMultiSelectDropDownModule.forRoot()
  ],
  entryComponents: [
    UploadStaffExcelComponent
  ],
})
export class StaffModule { }
