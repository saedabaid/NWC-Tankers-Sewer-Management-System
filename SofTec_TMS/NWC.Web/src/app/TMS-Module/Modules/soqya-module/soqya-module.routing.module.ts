import { Routes, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { SoqyaSchedulingComponent } from "./soqya-scheduling/soqya-scheduling.component";

const routes: Routes = [
    {
        path: "",
        pathMatch: "prefix",
        children: [
            {
                path: "",
                pathMatch: "full",
                component: SoqyaSchedulingComponent
            }   ,
            {
                path: "SoqyaScheduling",
                pathMatch: "full",
                component: SoqyaSchedulingComponent
            } 
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SoqyaRoutingModule { }
