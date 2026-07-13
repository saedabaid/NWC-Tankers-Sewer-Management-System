
//import { OrderComponent } from './order.component';
import { Routes, RouterModule } from '@angular/router';
import { ViewListOrdersComponent } from './work-order-list/work-order-list.component';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { OrderCreateComponent } from './order-create/order-create.component';
import { NgModule } from '@angular/core';
import { MassOrderTransferStationComponent } from './mass-order-transfer-station/mass-order-transfer-station.component';


const routes: Routes = [
    {
        path: '',
        //component: OrderComponent,
        pathMatch: 'prefix',
        children: [
          
            
            {
                path: 'orderlist',
                component: ViewListOrdersComponent,
                pathMatch: 'full'
            },
            {
                path: 'orderlist/:Filters',
                component: ViewListOrdersComponent,
                pathMatch: 'full'
            },
            {
                path: 'orderdetails/:ID',
                pathMatch: 'full',
                component: OrderDetailsComponent
            },
          
         
            {
                path: 'create',
                component: OrderCreateComponent,
                pathMatch: 'full'
            },
            {
                path: 'bulkCreate',
                component: OrderCreateComponent,
                pathMatch: 'full'
            },
            {
                path: 'commercialOrder',
                component: OrderCreateComponent,
                pathMatch: 'full'
            },
            {
                path: 'massTransfer',
                component: MassOrderTransferStationComponent,
                pathMatch: 'full'
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class OrderRoutingModule { }
