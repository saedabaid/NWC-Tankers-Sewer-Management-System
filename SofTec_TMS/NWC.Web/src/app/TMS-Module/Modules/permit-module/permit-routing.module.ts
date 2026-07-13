import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddPermitComponent } from './add-permit/add-permit.component';
import { ListComponent } from './list/list.component';
import { PermitListComponent } from './permit-list/permit-list.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'prefix',
        children: [            
            {
                path: 'new',
                pathMatch: 'full',
                component: AddPermitComponent
            },
            {
                path: 'permit-list',
                pathMatch: 'full',
                component: PermitListComponent,
              },
              {
                path: 'edit/:Id',
                pathMatch: 'full',
                component: AddPermitComponent
            },
              {
                path: 'list',
                pathMatch: 'full',
                component: ListComponent,
              },
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PermitRoutingModule { }
