import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { LoginComponent } from '@tms-shared/component/login/login.component';
import { Configuration } from '@tms-shared/configurations/shared.config';
// import { pageNotFound } from './shared/components/pageNotFound/pageNotFound.component';

export const appRoutes: Routes = [

    {
        path: '',
        redirectTo: 'tms',
        pathMatch: 'full'
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: Configuration.modulePrefixes.NWCManagement,
        loadChildren: './TMS-Module/TMS.module#TMSModule'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes, { preloadingStrategy: PreloadAllModules })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
