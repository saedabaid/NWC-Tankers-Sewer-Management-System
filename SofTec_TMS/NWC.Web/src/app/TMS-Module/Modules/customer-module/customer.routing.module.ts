import { Routes, RouterModule } from '@angular/router';

import { NgModule } from '@angular/core';
import { NewCustomerComponent } from './new-customer/new-customer.component';
import { CustomerListComponent } from './customer-list/customer-list.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'prefix',
        children: [
            {
                path: 'customerlist',
                pathMatch: 'full',
                component: CustomerListComponent
            },
            {
                path: 'create',
                pathMatch: 'full',
                component: NewCustomerComponent
            },
            {
                path: 'edit/:Id',
                pathMatch: 'full',
                component: NewCustomerComponent
            },

        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CustomerRoutingModule { }
