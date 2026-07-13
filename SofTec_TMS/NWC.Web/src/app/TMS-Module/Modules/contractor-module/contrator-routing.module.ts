

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ContractorListComponent } from './contractor-list/contractor-list.component';
import { ContractorCreateComponent } from './contractor-create/contractor-create.component';


const routes: Routes = [
    {
        path: '',
        pathMatch: 'prefix',
        children: [
            {
                path: 'contractorlist',
                pathMatch: 'full',
                component: ContractorListComponent
            },
            {
                path: 'new',
                pathMatch: 'full',
                component: ContractorCreateComponent
            },
            {
                path: 'edit/:Id',
                pathMatch: 'full',
                component: ContractorCreateComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ContractorRoutingModule { }
