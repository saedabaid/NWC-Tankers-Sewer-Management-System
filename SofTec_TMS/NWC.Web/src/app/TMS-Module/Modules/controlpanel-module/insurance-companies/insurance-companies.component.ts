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
  selector: 'app-insurance-companies',
  templateUrl: './insurance-companies.component.html',
  styleUrls: ['./insurance-companies.component.scss']
})
export class InsuranceCompaniesComponent implements OnInit {
  pagePermission = '';
  SearchCriteria: StaffSearchCriteria;
  tableLoading = false;

  selectMenuOptions = {
    enableSearchFilter: true,
  };
  selectMenuOptions2 = {
    singleSelect: true,
  };

  Customer_Loading = false;
  dropdownSettings: IDropdownSettings = {};
  advancedDiv = <boolean>false;
  searchResult = new SearchResult<any>();



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
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'Id',
      textField: 'Name',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 1,
      allowSearchFilter: true,
    };


    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe((res) => {
      this.loadDDLsGV();
    });
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new StaffSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
  }



  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant('InsuranceCompanies'));
  }

  searchCaller() {
    this.tableLoading = true;
    this.lookupservice
      .searchStaff(this.SearchCriteria)
      .subscribe((res) => {
        this.searchResult.TotalCount = res.Value.TotalCount;
        this.searchResult.Result = res.Value.Result;
        this.tableLoading = false;
      });
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

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

  editItem(id) {
    this.router.navigate([`create/${id}`])
  }
  deleteItem(id) {
    this.cpService.deleteBranch(id);
  }

}
