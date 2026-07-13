

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
//import { ContractComponent } from './contract.component';
import { ContractListComponent } from './contract-list/contract-list.component';
import { NewContractComponent } from './new-contract/new-contract.component';
import { ContractViolationsListComponent } from './contract-violations-list/contract-violations-list.component';
import { ContractViolationAddeditComponent } from './contract-violation-addedit/contract-violation-addedit.component';
import { ContractViolationPrintComponent } from './contract-violation-print/contract-violation-print.component';
import { NewContractViolationApprovalComponent } from './new-contract-violation-approval/new-contract-violation-approval.component';

const routes: Routes = [
  {
    path: '',
    //component: ContractComponent,
    pathMatch: 'prefix',
    children: [
      {
        path: 'contractlist',
        pathMatch: 'full',
        component: ContractListComponent
      },
      {
        path: 'new',
        pathMatch: 'full',
        component: NewContractComponent
      },
      {
        path: 'violationapproval/add',
        pathMatch: 'full',
        component: NewContractViolationApprovalComponent
      },
      {
        path: 'edit/:Id',
        pathMatch: 'full',
        component: NewContractComponent
      },
      {
        path: 'violationslist',
        pathMatch: 'full',
        component: ContractViolationsListComponent
      },
      {
        path: 'violation/add',
        pathMatch: 'full',
        component: ContractViolationAddeditComponent
      },
      {
        path: 'violation/edit/:Id',
        pathMatch: 'full',
        component: ContractViolationAddeditComponent
      },
      {
        path: 'violation/print/:Id',
        pathMatch: 'full',
        component: ContractViolationPrintComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ContractRoutingModule { }
