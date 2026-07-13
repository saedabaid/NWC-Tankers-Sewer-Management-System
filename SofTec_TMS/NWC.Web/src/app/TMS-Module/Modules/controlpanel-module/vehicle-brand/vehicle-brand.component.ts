import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { FilterModel } from '@tms-models/common/filter-model';
import { PageFilter } from '@tms-models/common/page-fillter-model';
import { SearchResult } from '@tms-models/common/search-result';
import { StaffSearchCriteria } from '@tms-models/search-criteria/staff-search-criteria';
import { ControlPanelService } from '@tms-services/control-panel.service';
import { LookupService } from '@tms-services/lookup.service';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-vehicle-brand',
  templateUrl: './vehicle-brand.component.html',
  styleUrls: ['./vehicle-brand.component.scss']
})
export class VehicleBrandComponent implements OnInit {

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
    this.titleService.setTitle(this._translate.instant('VehicleBrands'));
    this.getVehicleBrand();
    this._translate.onLangChange.subscribe((res) => {
      this.titleService.setTitle(this._translate.instant('VehicleBrands'));
    });

  }

  getVehicleBrand() {
    this.lookupservice.getVehicleBrand().subscribe(res => this.items = res.Value.Value);
  }

  deleteItem(id) {
    this.cpService.deleteVehicleType(id);
  }
}
