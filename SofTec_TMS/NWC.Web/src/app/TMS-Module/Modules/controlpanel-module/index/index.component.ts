import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LoaderService } from '@tms-shared/loader.service';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {

  pagePermission: string;

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsDashboard');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
  }

  ngOnInit() {

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.titleService.setTitle(this.translateService.instant('ControlPanel'));
  }

}
