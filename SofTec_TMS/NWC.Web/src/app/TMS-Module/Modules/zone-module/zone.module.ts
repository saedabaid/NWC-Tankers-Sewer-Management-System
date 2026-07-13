import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ZoneListComponent } from './zoneList/zoneList.component';
import { NewZoneComponent } from './new-zone/new-zone.component';
//import { ZoneComponent } from './zone.component';
import { ZoneRoutingModule } from './zone-routing.module';
//import { SortableModule, BsDropdownModule, ModalModule, CollapseModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { UploadZoneExcelComponent } from './upload-zone-excel/upload-zone-excel.component';
import { ModalModule } from 'ngx-bootstrap/modal';



@NgModule({
  declarations: [
    //ZoneComponent,
    ZoneListComponent,
    NewZoneComponent,
    UploadZoneExcelComponent,
  
  ],
  imports: [
    CommonModule,
    ZoneRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    ModalModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),
  ],
  entryComponents:[
    UploadZoneExcelComponent
  ],
})
export class ZoneModule { }
