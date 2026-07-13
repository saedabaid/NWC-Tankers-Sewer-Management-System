import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

//import { SortableModule, BsDropdownModule, ModalModule, CollapseModule } from 'ngx-bootstrap';

import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { DeviceMeterRoutingModule } from './device-meter-routing.module';
import { MeterReadingListComponent } from './meter-reading-list/meter-reading-list.component';
import { AddMeterReadingComponent } from './add-meter-reading/add-meter-reading.component';
import { TestVPNComponent } from './test-vpn/test-vpn.component';
import { StorageServiceModule } from 'ngx-webstorage-service';
import { ModalModule } from 'ngx-bootstrap/modal';
import { DeviceMeterListComponent } from './device-meter-list/device-meter-list.component';

// import { ContractorListComponent } from './contractor-list/contractor-list.component';
// import { ContractorCreateComponent } from './contractor-create/contractor-create.component';
// import { ContractorRoutingModule } from './contrator-routing.module';


@NgModule({
    declarations: [
        MeterReadingListComponent,
        AddMeterReadingComponent,
        TestVPNComponent,
        DeviceMeterListComponent
    ],
    imports: [
        CommonModule,
        DeviceMeterRoutingModule,
        // SortableModule.forRoot(),
        // BsDropdownModule,
        FormsModule,
        SharedModule,
        ModalModule,
        //CollapseModule,
        NgMultiSelectDropDownModule.forRoot(),
        StorageServiceModule
    ],
    entryComponents: [

    ],
    providers: [

        DatePipe

    ]
})
export class DeviceMeterModule { }
