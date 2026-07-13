import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { StaffRolesSearchCriteria } from '../../../Models/search-criteria/StaffRolesSearchCriteria';
import { StaffRolesDTO } from '../../../Models/StaffRolesDTO';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { StaffService } from '../../../Services/staff-service';
import { Router, ActivatedRoute } from "@angular/router";
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { PagesDTO } from '../../../Models/PagesDTO';

@Component({
  selector: 'app-staff-roles',
  templateUrl: './staff-roles.component.html',
  styleUrls: ['./staff-roles.component.scss']
})
export class StaffRolesComponent implements OnInit {
  pagePermission: string = '';
  staffRoleList: StaffRolesDTO[];
  staffSecondRoleList: StaffRolesDTO[];
  PagesDTO: PagesDTO[];
  TotalCount: number;
  advancedDiv: false;
  tableLoading: false;
  constructor(
    private _translate: TranslateService,
    private titleService: Title,
    private authServer: AuthenticationService,
    private mainloading: LoaderService,
    private staffService: StaffService, private _router: Router,
    private _alert: alertService

  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('orderlist');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.titleService.setTitle(this._translate.instant('StaffRolesList'));
    this.Search();
  }
 
  Search() {
    this.mainloading.PreloaderIcreaseCount();
    this.staffService.getStaffRoles().subscribe(res => {
      if (res.Value != null) {
        this.staffRoleList = res.Value.FirstResult;
        this.staffSecondRoleList = res.Value.SecondResult;
        this.TotalCount = res.Value.TotalCount;
      }
      else {
        this.staffRoleList = [];
        this.staffSecondRoleList = [];
        this.TotalCount = 0;
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        //this.tableLoading= false;
        this.mainloading.PreloaderDecreaseCount();
      })
  }
  customRoute(routeTo: string, value: string) {
    if (routeTo && routeTo !== "") {
      const targetLink = `/tms/${routeTo}`;
      const url = this._router.serializeUrl(
        this._router.createUrlTree([targetLink])
      );
      console.log(routeTo, url);
      this._router.navigate([url], { queryParams: { key: value } });
      // window.open(url, '_blank');
    }
  }
  deleteRoleItem() {
    console.log('deleteRoleItem');
  }
}
