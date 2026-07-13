import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { ContractService } from '../../../Services/contract.service';
import { alertService } from '../../../../shared/Services/alert/alert.service';
import { Subscription } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from '../../../../shared/Services/authentication/authentication.service';

@Component({
    selector: 'app-new-contract',
    templateUrl: './new-contract.component.html',
    styleUrls: ['./new-contract.component.scss']
})
export class NewContractComponent implements OnInit, OnDestroy {

    activeTab: string = "contract";
    changeTabSubscripe: Subscription;
    updateMode = false;

    pagePermission: string = '';

    constructor(
        private router: Router,
        private contractService: ContractService,
        private _alert: alertService,
        private translateService: TranslateService,
        private titleService: Title,
        private authenticationService: AuthenticationService
    ) {

        this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('contractlist');
        this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

        let urlList = this.router.routerState.snapshot.url.split('/');
        if (urlList[3] === "edit") {
            this.contractService.updateContractId = +urlList[4];
            this.updateMode = true;
            // if (!isNullOrUndefined(urlList[5]) && urlList[5] === "stations") {
            //     this.changeTab("stations");
            // }
            //this.changeTab("traiff");
            //this.changeTab("violation");
        }
        else {
            this.contractService.updateContractId = null;
            this.updateMode = false;
        }

    }

    ngOnInit() {

        this.changeTabSubscripe = this.contractService.changeTab$.subscribe(res => {
            if (res == "contractlist") {
                this.close();
            }
            else {
                this.changeTab(res);
            }
        })

        this.load();
        this.translateService.onLangChange.subscribe(res => {
            this.load();
        });
    }


    load() {
        this.titleService.setTitle(this.translateService.instant((this.updateMode ? 'UpdateContract' : 'NewContract')));
    }

    close() {
        this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
            if (confirm === true) {
                this.router.navigate(['/tms/contract/contractlist']);
            }
        });
    }

    ngOnDestroy(): void {
        this.changeTabSubscripe.unsubscribe();
    }

    changeTab(tab: string) {
        this.activeTab = tab;
    }

}
