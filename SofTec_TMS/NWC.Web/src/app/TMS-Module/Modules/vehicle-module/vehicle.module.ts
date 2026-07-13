import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { ModalModule } from 'ngx-bootstrap/modal';
import { SharedModule } from 'src/app/shared/shared.module';
import { NewVehicleComponent } from './new-vehicle/new-vehicle.component';
import { UploadVehicleExcelComponent } from './upload-vehicle-excel/upload-vehicle-excel.component';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';
import { VehicleRoutingModule } from './vehicle-routing.module';

@NgModule({
  declarations: [
    VehicleListComponent,
    NewVehicleComponent,
    UploadVehicleExcelComponent,
  ],
  imports: [
    CommonModule,
    VehicleRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    ModalModule,
    NgMultiSelectDropDownModule.forRoot(),
  ],
  entryComponents: [
    UploadVehicleExcelComponent
  ],
})
export class VehicleModule { }
