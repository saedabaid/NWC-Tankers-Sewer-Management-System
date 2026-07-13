import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { ModalModule } from 'ngx-bootstrap/modal';
import { SharedModule } from 'src/app/shared/shared.module';
import { AddPermitComponent } from './add-permit/add-permit.component';
import { PermitListComponent } from './permit-list/permit-list.component';
import { PermitRoutingModule } from './permit-routing.module';
import {MatTabsModule} from '@angular/material/tabs';
import { ListComponent } from './list/list.component';
import { NgxPrintModule } from 'ngx-print';
import { NewEditPermitListComponent } from './new-edit-permit-list/new-edit-permit-list.component';

@NgModule({
  declarations: [
    AddPermitComponent,
    PermitListComponent,
    ListComponent,
    NewEditPermitListComponent
    
  ],
  imports: [
    CommonModule,
    PermitRoutingModule,    
    FormsModule,
    SharedModule,
    ModalModule,
    NgMultiSelectDropDownModule.forRoot(),
    MatTabsModule, NgxPrintModule
  ],
  entryComponents: [
    
  ],
})
export class PermitModule { }
