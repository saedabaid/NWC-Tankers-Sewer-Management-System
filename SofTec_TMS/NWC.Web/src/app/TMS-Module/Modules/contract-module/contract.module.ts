import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ContractRoutingModule } from './contract-routing.module';
import { NewContractComponent } from './new-contract/new-contract.component';
import { ContractDetailsComponent } from './new-contract/contract-details/contract-details.component';
import { StationsDetailsComponent } from './new-contract/stations-details/stations-details.component';
import { PriceDetailsComponent } from './new-contract/price-details/price-details.component';
import { TariffDetailsComponent } from './new-contract/tariff-details/tariff-details.component';
import { AccessoriesChargesDetailsComponent } from './new-contract/accessories-charges-details/accessories-charges-details.component';
import { ViolationTermsDetailsComponent } from './new-contract/violation-terms-details/violation-terms-details.component';
import { AddEditTariffComponent } from './new-contract/tariff-details/add-edit-tariff/add-edit-tariff.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { ContractListComponent } from './contract-list/contract-list.component';
import { UploadTariffExcelComponent } from './new-contract/tariff-details/upload-tariff-excel/upload-tariff-excel.component';
import { ContractViolationsListComponent } from './contract-violations-list/contract-violations-list.component';
import { ContractViolationAddeditComponent } from './contract-violation-addedit/contract-violation-addedit.component';
import { ContractViolationsLogsListComponent } from './contract-violations-logs-list/contract-violations-logs-list.component';
import {  ContractViolationsApprovalLogsListComponent} from './contract-violations-approval-logs-list/contract-violations-approval-logs-list.component';
import { NewContractViolationApprovalComponent } from './new-contract-violation-approval/new-contract-violation-approval.component';

import { ContractViolationPrintComponent } from './contract-violation-print/contract-violation-print.component';
  import { NgxPrintModule } from 'ngx-print';
import { ModalModule } from 'ngx-bootstrap/modal';


@NgModule({
  declarations: [
    NewContractComponent,
    ContractDetailsComponent,
    StationsDetailsComponent,
    PriceDetailsComponent,
    TariffDetailsComponent,
    AccessoriesChargesDetailsComponent,
    ViolationTermsDetailsComponent,
    AddEditTariffComponent,
    ContractListComponent,
    UploadTariffExcelComponent,
    ContractViolationsListComponent,
    ContractViolationAddeditComponent,
    ContractViolationsLogsListComponent,
    ContractViolationPrintComponent,
    ContractViolationsApprovalLogsListComponent,
    NewContractViolationApprovalComponent
  ],
  imports: [
    CommonModule,
    ContractRoutingModule,
    // SortableModule.forRoot(),
    // BsDropdownModule,
    FormsModule,
    SharedModule,
    ModalModule,
    //CollapseModule,
    NgMultiSelectDropDownModule.forRoot(),
     NgxPrintModule
  ],
  entryComponents: [
    UploadTariffExcelComponent
  ],
  providers: [

    DatePipe

  ]
})
export class ContractModule { }
