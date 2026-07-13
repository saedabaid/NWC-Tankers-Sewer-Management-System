import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { ToastrManager } from 'ng6-toastr-notifications';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { AlertmessageComponent } from '../../component/alertmessage/alertmessage.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Injectable({ providedIn: 'root' })
export class alertService {

    toasterOptions: any;

    constructor(
        private toastr: ToastrManager,
        private translateService: TranslateService,
        private modalRef: BsModalRef,
        private modalService: BsModalService
    ) { 
        this.setOptions();
        translateService.onLangChange.subscribe(res => {
            this.setOptions();
        });
    }

    private setOptions() {
        this.toasterOptions = {};
        this.toasterOptions.enableHTML = true;
        this.toasterOptions.toastTimeout= 10000;
        if (this.translateService.currentLang == 'ar') {
            this.toasterOptions.position = 'top-right';
            this.toasterOptions.messageClass = 'toastr-messages-ar';
        }
        else
        {
            this.toasterOptions.position = 'top-left';
            this.toasterOptions.messageClass = 'toastr-messages-en';
        }
    }

    showSuccess() {
        const msg = this.translateService.instant("SavedSuccessed");
        const header = this.translateService.instant("Successed");
        this.toastr.successToastr(msg, header, this.toasterOptions);
    }

    success(messageKey?: string, headerKey?: string) {
        if (isNullOrUndefined(messageKey))
            messageKey = ' ';
        let msg = this.translateService.instant(messageKey);

        if (isNullOrUndefined(headerKey))
            headerKey = ' ';
        let header = this.translateService.instant(headerKey);

        this.toastr.successToastr(msg, header, this.toasterOptions);
    }

    showError() {
        const msg = this.translateService.instant("Error");
        const header = this.translateService.instant("ErrorContact");
        this.toastr.errorToastr(msg, header, this.toasterOptions);
    }

    error(messageKey?: string, headerKey?: string) {
        if (isNullOrUndefined(messageKey))
            messageKey = ' ';
        let msg = this.translateService.instant(messageKey);

        if (isNullOrUndefined(headerKey))
            headerKey = ' ';
        let header = this.translateService.instant(headerKey);

        this.toastr.errorToastr(msg, header, this.toasterOptions);
    }

    warning(messageKey?: string, headerKey?: string) {
        if (isNullOrUndefined(messageKey))
            messageKey = ' ';

        let msg = this.translateService.instant(messageKey);

        if (isNullOrUndefined(headerKey))
            headerKey = ' ';
        let header = this.translateService.instant(headerKey);



        this.toastr.warningToastr(msg, header, this.toasterOptions);
    }

    info(messageKey?: string, headerKey?: string) {
        if (isNullOrUndefined(messageKey))
            messageKey = ' ';
        let msg = this.translateService.instant(messageKey);

        if (isNullOrUndefined(headerKey))
            headerKey = ' ';
        let header = this.translateService.instant(headerKey);

        this.toastr.infoToastr(msg, header, this.toasterOptions);
    }

    errorList(errorMessagesKeys?: string[], headerKey?: string) {
        if (isNullOrUndefined(headerKey))
            headerKey = ' ';
        let errorHeader = this.translateService.instant(headerKey);

        if (isNullOrUndefined(errorMessagesKeys) || errorMessagesKeys.length < 1)
            errorMessagesKeys = ['Error'];

        let errorMsg = "";
        errorMessagesKeys.forEach(a => {
            errorMsg += this.translateService.instant(a) + "</br>";
        })

        //let pos = this.translateService.langs ? "toast-bottom-left" : "toast-top-right";

        this.toastr.errorToastr(errorMsg, errorHeader, this.toasterOptions);
    }

    confirmationMessage(confirmationMessagesKeys?: string) {
        if (isNullOrUndefined(confirmationMessagesKeys)) {
            confirmationMessagesKeys = "ConfirmMessage";
        }

        this.modalRef = this.modalService.show(AlertmessageComponent);
        this.modalRef.content.message = confirmationMessagesKeys;

        this.modalRef.content.confirm.subscribe(() => {
            this.modalRef.hide();
        })

        return this.modalRef.content.confirm as Observable<boolean>;
    }
    
}
