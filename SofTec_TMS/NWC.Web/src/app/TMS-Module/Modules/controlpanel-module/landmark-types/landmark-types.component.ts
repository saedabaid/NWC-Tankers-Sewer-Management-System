import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { SearchResult } from '@tms-models/common/search-result';
import { ControlPanelService } from '@tms-services/control-panel.service';
import { LookupService } from '@tms-services/lookup.service';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-landmark-types',
  templateUrl: './landmark-types.component.html',
  styleUrls: ['./landmark-types.component.scss']
})
export class LandmarkTypesComponent implements OnInit {

  pagePermission = '';
  tableLoading = false;


  Customer_Loading = false;
  dropdownSettings: IDropdownSettings = {};
  advancedDiv = <boolean>false;
  searchResult = new SearchResult<any>();

  items = [];



  constructor(
    private authServer: AuthenticationService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private router: Router,
    private titleService: Title,
    private cpService: ControlPanelService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('orderlist');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }


  ngOnInit() {
    this.titleService.setTitle(this._translate.instant('LandMarkTypes'));
    this.getLandMarkTypes();
    this._translate.onLangChange.subscribe((res) => {
      this.titleService.setTitle(this._translate.instant('LandMarkTypes'));
    });

  }

  getLandMarkTypes() {
    this.lookupservice.getLandMarkTypes().subscribe(res => this.items = res.Value.Value);
  }

  deleteItem(id) {
    this.cpService.deleteLandMarkType(id);
  }

}
