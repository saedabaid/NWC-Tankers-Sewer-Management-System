import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';

import { AppComponent } from './app.component';
import { SharedModule, HttpLoaderFactory } from './shared/shared.module';
import { CookieService } from 'ngx-cookie-service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppHeader } from './component/header.component';
import { Sidemenu } from './component/side_menu.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app-routing.module';
import { HttpClientModule, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { AmazingTimePickerModule } from 'amazing-time-picker'; // this line you need
import { ToastrModule } from 'ng6-toastr-notifications';
// import { SortableModule, BsDropdownModule, CollapseModule, ModalModule, TypeaheadModule, BsModalRef } from 'ngx-bootstrap';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { registerLocaleData } from '@angular/common';
import localeAr from '@angular/common/locales/Ar';
import { BsModalRef, ModalModule } from 'ngx-bootstrap/modal';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { HeaderInterceptor } from './shared/Services/interceptors/header-interceptor';
import { FormsModule } from '@angular/forms';
import { MapService } from '@tms-shared/Services/mapService/map-service';

registerLocaleData(localeAr);
@NgModule({
  declarations: [
    AppComponent,
    AppHeader,
    Sidemenu
  ],
  imports: [
    HttpClientModule,
    SharedModule,
    FormsModule,
    BrowserModule,
    AmazingTimePickerModule,
    CollapseModule.forRoot(),
    BsDropdownModule.forRoot(),

    ModalModule.forRoot(),
    ToastrModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    // SortableModule.forRoot(),
    TypeaheadModule.forRoot(),
    NgbModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    BrowserAnimationsModule,
    NgbModule,
  ],
  entryComponents: [
  ],
  providers: [MapService, CookieService, BsModalRef,
    {provide: HTTP_INTERCEPTORS, useClass: HeaderInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
