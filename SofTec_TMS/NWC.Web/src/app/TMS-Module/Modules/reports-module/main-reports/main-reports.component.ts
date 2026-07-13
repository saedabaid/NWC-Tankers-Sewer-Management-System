import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-main-reports',
  templateUrl: './main-reports.component.html',
  styleUrls: ['./main-reports.component.scss']
})
export class MainReportsComponent implements OnInit {

  pagePermission: string;

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsReports');
    this.authenticationService.checkFullControlPrivilege(this.pagePermission, true);
   }

  ngOnInit() {
    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.titleService.setTitle(this.translateService.instant('TMS_Reports'));
  }

}
