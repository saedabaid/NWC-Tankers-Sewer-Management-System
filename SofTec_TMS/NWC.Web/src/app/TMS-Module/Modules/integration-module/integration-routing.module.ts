

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DeferredOrderListComponent } from './deferred-order-list/deferred-order-list.component';
import { DeferredOrderDetailsComponent } from './deferred-order-details/deferred-order-details.component';


const routes: Routes = [
    {
        path: '',
        //component: ContractComponent,
        pathMatch: 'prefix',
        children: [
            {
                path: 'deferredorderslist',
                pathMatch: 'full',
                component: DeferredOrderListComponent
            },
            {
                path: 'deferredorderedit/:Id',
                pathMatch: 'full',
                component: DeferredOrderDetailsComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class IntegrationRoutingModule { }
