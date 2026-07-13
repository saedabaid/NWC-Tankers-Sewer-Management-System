 import { SelectMenuComponent } from './component/select-menu/select-menu.component';
import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// custom module
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AmazingTimePickerModule } from 'amazing-time-picker'; // this line you need
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AlertModule } from 'ngx-bootstrap/alert';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { arLocale, enGbLocale } from 'ngx-bootstrap/locale';
import { NgxQRCodeModule } from '@techiediaries/ngx-qrcode';

defineLocale('ar', arLocale);
defineLocale('en', enGbLocale);
// Pipes
import { MenuLinkPipe } from './pipes/menu-link.pipe';

//services
import { MenuLinkService } from './Services/menu/menu-link.service';
import { CookieService } from 'ngx-cookie-service';
import { AuthorizationService } from './Services/authorization/authorization.service';
import { AuthenticationService } from './Services/authentication/authentication.service';
import {NgxPrintModule} from 'ngx-print';

import { ModalModule, BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

import { AuthGuard } from './Providers/auth-guard.provider';


//translation
import { Ng5SliderModule } from 'ng5-slider';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, '../../assets/i18n/', '.json');
}

// interceptors
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthHttpInterceptor } from './Providers/authHttpInterceptor';
import { OnlyNumber } from './pipes/OnlyNumber';
import { CommonMapComponent } from './component/commonMap/common-map.component';
import { DatepickerComponent } from './component/datepicker/datepicker.component';
import { AmazingTimepickerComponent } from './component/amazing-timepicker/amazing-timepicker.component';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';

import { PaginationsComponent } from './component/pagination/pagination.component';
import { SearchComponentComponent } from './component/searchComponent/search-component.component';
import { PrintTicketComponent } from './component/print-ticket/print-ticket.component';
import { PrintTicketDriverComponent } from './component/print-ticket-driver/print-ticket-driver.component';
import { PrintTicketCustomerComponent } from './component/print-ticket-customer/print-ticket-customer.component';
import { UploudFilesComponent } from './component/uploader/uploader.component';
import { AlertmessageComponent } from './component/alertmessage/alertmessage.component';
import { FileUploadService } from './Services/file-upload.service';
import { NgbModule, NgbCalendarIslamicUmalqura } from '@ng-bootstrap/ng-bootstrap';
import { DatepickerIslamicumalqura } from './component/datePicker-gregorian-hijri/datepicker-islamicumalqura/datepicker-islamicumalqura';
import { DatepickerGregorianHijri } from './component/datePicker-gregorian-hijri/datepicker-gregorian-hijri/datePicker-gregorian-hijri';
import { ExcelService } from './Services/excel/ExcelService';
import { DateComponent } from './component/Date/date';
import { ImportExcelService } from './Services/excel/import-excel.service';
import { StorageServiceModule } from 'ngx-webstorage-service';
import { MinutesToHHMMPipe } from './pipes/minutes-to-HHmm.pipe';
import { DateUtilityService } from './Services/date-Utility.service';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { LoginComponent } from './component/login/login.component';
import { UploudFilesComponent2 } from './component/uploader2/uploader2.component';
import { UploudFilesComponent3 } from './component/uploader3/uploader3.component';
//import { LandingPageComponent } from './component/landing-page/landing-page.component';

let providers = [
    MenuLinkService,
    CookieService,
    // LoaderService,
    AuthorizationService,
    AuthenticationService,
    AuthGuard,
    BsModalRef,
    BsModalService,
    FileUploadService,
    ExcelService,
    ImportExcelService,
    DateUtilityService,
    {
        provide: HTTP_INTERCEPTORS,
        useClass: AuthHttpInterceptor,
        multi: true
    },
    { provide: NgbCalendarIslamicUmalqura, useClass: NgbCalendarIslamicUmalqura }
]

let sharedDeclarations = [
    MenuLinkPipe,
    OnlyNumber,
    MinutesToHHMMPipe,
    PaginationsComponent, CommonMapComponent,
    DatepickerComponent,
    SelectMenuComponent,
    AmazingTimepickerComponent,
    SearchComponentComponent,
    UploudFilesComponent,
    UploudFilesComponent2,
    UploudFilesComponent3,
    AlertmessageComponent,
    DateComponent


];

@NgModule({
    declarations: [
        sharedDeclarations,
        PrintTicketComponent,
        PrintTicketDriverComponent,
        PrintTicketCustomerComponent,
        DatepickerIslamicumalqura,
        DatepickerGregorianHijri,
        LoginComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        AngularMultiSelectModule,
        AmazingTimePickerModule,
        TypeaheadModule,
        TimepickerModule.forRoot(),
        NgxPrintModule,
        NgxQRCodeModule,
        PaginationModule.forRoot(),
        AlertModule.forRoot(),
        ModalModule.forRoot(),
        BsDatepickerModule.forRoot(),
        Ng5SliderModule,
        PaginationModule.forRoot(),
        TranslateModule.forChild({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient]
            }
        }),
        NgbModule,
        StorageServiceModule
    ],
    providers: providers,
    exports: [
        sharedDeclarations,
        AngularMultiSelectModule,
        BsDatepickerModule,
        PaginationModule,
        TranslateModule,
        DatepickerIslamicumalqura,
        DatepickerGregorianHijri
    ],
    entryComponents:[
        AlertmessageComponent
    ],
    bootstrap: [
        DatepickerIslamicumalqura,
        DatepickerGregorianHijri
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders<SharedModule> {
        return {
            ngModule: SharedModule,
            providers: providers
        }
    }
}
