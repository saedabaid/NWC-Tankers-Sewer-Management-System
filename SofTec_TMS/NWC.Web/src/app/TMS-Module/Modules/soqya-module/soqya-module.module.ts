import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SoqyaSchedulingComponent } from './soqya-scheduling/soqya-scheduling.component';
import { SoqyaRoutingModule } from './soqya-module.routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
//import { ModalModule, CollapseModule, SortableModule, BsDropdownModule } from 'ngx-bootstrap';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { FormsModule } from '@angular/forms';
import { SoqyaService } from '../../Services/soqya.service';
import { UploadSoqyaSchedulingExcelComponent } from './upload-soqya-scheduling-excel/upload-soqya-scheduling-excel.component';
import { ModalModule } from 'ngx-bootstrap/modal';



@NgModule({
  declarations: [ SoqyaSchedulingComponent ,
                  UploadSoqyaSchedulingExcelComponent],
  imports: [
    CommonModule ,
    SoqyaRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    ModalModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot()
    
  ],
  providers:[
    SoqyaService
  ],
  entryComponents:[
    UploadSoqyaSchedulingExcelComponent
  ],
})
export class SoqyaModuleModule { }
