import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { TranslateService } from '@ngx-translate/core';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { DeviceMeter } from 'src/app/TMS-Module/Models/device-meter';
import { DeviceMeterService } from 'src/app/TMS-Module/Services/device-meter.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { DeviceMeterSearchCriteria } from '../../../Models/search-criteria/device-meter-SC'
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { Title } from '@angular/platform-browser';
import { LoaderService } from 'src/app/shared/loader.service';
import { FilterModel } from 'src/app/TMS-Module/Models/common/filter-model';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { Configuration } from 'src/app/shared/configurations/shared.config';

@Component({
  selector: 'app-device-meter-list',
  templateUrl: './device-meter-list.component.html',
  styleUrls: ['./device-meter-list.component.scss']
})
export class DeviceMeterListComponent implements OnInit {
  advancedDiv = <boolean>false;
  SearchCriteria: DeviceMeterSearchCriteria;
  searchResult = new SearchResult<DeviceMeter>();

  pagePermission: string = '';
  tableLoading = false;
  DeviceLoading = false;

  DeviceMeterList: Lookup<string>[] = [];
  deviceMeterSearchKeyWord = '';
  bindingModel_DeviceMeters: Lookup<string>[] = [];

  selectMenuOptions = {
    enableSearchFilter: true,
    badgeShowLimit: 2
  };

  constructor(
    private authenticationService: AuthenticationService,
    private deviceMeterService: DeviceMeterService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private _alert: alertService,
    private titleService: Title,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('deviceMeterReading');
    this.authenticationService.checkViewPrivilege(this.pagePermission, true);

  }

 

  ngOnInit() {
    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });
  }

  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant('DeviceMeter'));
    this.searchCaller();
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new DeviceMeterSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
  }

  searchContractorsNames(name) {
    return this.lookupservice.SearchPermittedStations(name);
  }

  //#region "table Pagination and Search"
  onSearchSubmit() {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = evt;
    this.searchCaller();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageSize = evt;
    this.searchCaller();
  }

  searchCaller() {
    let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    
    this.mainloading.PreloaderIcreaseCount();

    this.deviceMeterService.SearchDeviceMeterList(modifiedCriteria).subscribe(res => {
      if (res.Value != null) {
        this.searchResult = res.Value;
      }
      else {
        this.searchResult.Result = [];
        this.searchResult.TotalCount = 0
      }
    }
    , err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    , () => {
      //this.tableLoading = false;
      this.mainloading.PreloaderDecreaseCount();
    });
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"

  onContractorNameDDLChanged($event)
  {}
}
