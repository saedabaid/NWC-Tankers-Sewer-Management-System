import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { SearchResult } from '../../../Models/common/search-result';
import { Contract } from '../../../Models/contract';
import { LookupService } from '../../../Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { ContractService } from '../../../Services/contract.service';
import { ContractSearchCriteria } from '../../../Models/search-criteria/contract-search-criteria';
import { FilterModel } from '../../../Models/common/filter-model';
import { PageFilter } from '../../../Models/common/page-fillter-model';
import { Configuration } from '../../../../shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Router } from '@angular/router';
import { alertService } from '../../../../shared/Services/alert/alert.service';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from '../../../../shared/Services/authentication/authentication.service';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'app-contract-list',
  templateUrl: './contract-list.component.html',
  styleUrls: ['./contract-list.component.scss'],
  encapsulation: ViewEncapsulation.None

})
export class ContractListComponent implements OnInit {

  advancedDiv = <boolean>false;
  SearchCriteria: ContractSearchCriteria;
  searchResult = new SearchResult<Contract>();

  pagePermission: string = '';

  constructor(
    private authenticationService: AuthenticationService,
    private router: Router,
    private contractService: ContractService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private _alert: alertService,
    private titleService: Title,
    private mainloading: LoaderService
    ) {

      this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('contractlist');
      this.authenticationService.checkViewPrivilege(this.pagePermission, true);
    }

  ngOnInit() {
    // this.dropdownSettings = {
    //   singleSelection: false,
    //   idField: 'Id',
    //   textField: 'Name',
    //   selectAllText: 'Select All',
    //   unSelectAllText: 'UnSelect All',
    //   itemsShowLimit: 1,
    //   allowSearchFilter: true
    // };



    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });


  }

  loadDDLsGV() {
    this.titleService.setTitle( this._translate.instant('TMS_contractlist'));

    this.searchCaller();

    // load Search DDls values

  }

  setDefaultSearchValues() {
    this.SearchCriteria = new ContractSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    //this.SearchCriteria.DatePeriod = DateperiodEnum.ScheduleDate;

    // let startDate = new Date();
    // startDate.setDate(startDate.getDate() - 1);
    // this.SearchCriteria.DateTimeFrom = startDate;
    // this.timeFromStr = startDate.toTimeString().substring(0, 5);

    // let endDate = new Date();
    // endDate.setDate(endDate.getDate() + 1);
    // endDate.setHours(0);
    // endDate.setMinutes(0);
    // endDate.setSeconds(0);
    // this.SearchCriteria.DateTimeTo = endDate;
    // this.timeToStr = endDate.toTimeString().substring(0, 5);

    // redraw DDL selections
    // this.bindingModel_Customers = [];
    // this.bindingModel_Classes = [];
    // this.bindingModel_Priorities = [];
    // this.bindingModel_ServiceTypes = [];
    // this.bindingModel_Areas = [];
    // this.bindingModel_Cities = [];
    // this.bindingModel_Zones = [];
    // this.bindingModel_Stations = [];
    // this.bindingModel_Statuses = [];
    // this.bindingModel_Vehicles = [];
    // this.bindingModel_Drivers = [];

    // this.dateIsVaild = true;
  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }

  //#region  "get Lookups"
  searchContractsCodes(name) {
    return this.lookupservice.searchContractsCodes(name);
  }

  //#endregion  "get Lookups"



  //#region  "on Lookups change"
  onContractCodeDDLChanged(evt) {
    this.SearchCriteria.FilterModel.SearchKeyword = isNullOrUndefined(evt.Name) ? evt : evt.Name;
  }

  //#endregion "on Lookups change"

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
    // let timeFrom = this.timeFromStr.split(':');
    // if (timeFrom.length !== 2 || isNullOrUndefined(timeFrom[0]) || timeFrom[0] === '' || isNullOrUndefined(timeFrom[1]) || timeFrom[1] === '') {
    //   this.alert.error('please enter time');
    //   return;
    // }
    // this.SearchCriteria.DateTimeFrom.setHours(+timeFrom[0]);
    // this.SearchCriteria.DateTimeFrom.setMinutes(+timeFrom[1]);

    // let timeTo = this.timeToStr.split(':');
    // if (timeTo.length !== 2 || isNullOrUndefined(timeTo[0]) || timeTo[0] === '' || isNullOrUndefined(timeFrom[1]) || timeTo[1] === '') {
    //   this.alert.error('please enter time');
    //   return;
    // }
    // this.SearchCriteria.DateTimeTo.setHours(+timeFrom[0]);
    // this.SearchCriteria.DateTimeTo.setMinutes(+timeFrom[1]);

    // if (this.SearchCriteria.DateTimeTo <= this.SearchCriteria.DateTimeFrom ) {
    //   this.dateIsVaild = false;
    //   return;
    // } else{
    //   this.dateIsVaild = true;
    // }

    this.mainloading.PreloaderIcreaseCount();
    this.contractService.searchContractList(this.SearchCriteria).subscribe(res => {
      if (res.Value != null) {
        this.searchResult = res.Value;
      }
      else {
        this.searchResult.Result = [];
        this.searchResult.TotalCount = 0
      }
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"


  editContract(contract: Contract) {
    if (this.authenticationService.checkAddEditPrivilege(this.pagePermission)) {
      if (this.canUpdate(contract)) {
        this.router.navigate(['/tms/contract/edit/' + contract.ID]);
      }
    }
  }

  deleteContract(contract: Contract) {
    if (!this.authenticationService.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }

    if (this.canUpdate(contract)) {

      this._alert.confirmationMessage("ConfirmDelete").subscribe(confirm => {
        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractService.DeleteContract(contract.ID).subscribe(res => {
            if (!res.IsErrorState) {
              this._alert.success("DeletedSuccessed");
              this.onSearchSubmit();
            }
            else {
              this._alert.errorList(res.Errors);
            }
          }
          ,err => {
            this.mainloading.PreloaderDecreaseCount();
          }
          ,() => {
            this.mainloading.PreloaderDecreaseCount();
          });
        }

      });

    }
    else {
      //this._alert.error("NotAllowed");
    }

  }

  TerminateContract(contract: Contract) {
    if (this.pagePermission !== 'Full Control') {
      return;
    }

    if (this.canUpdate(contract)) {

      this._alert.confirmationMessage("ConfirmTerminateContract").subscribe(confirm => {
        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractService.TerminateContract(contract.ID).subscribe(res => {
            if (!res.IsErrorState) {
              this._alert.success("TerminatedSuccessfully");
              this.onSearchSubmit();
            }
            else {
              this._alert.errorList(res.Errors);
            }
          }
          ,err => {
            this.mainloading.PreloaderDecreaseCount();
          }
          ,() => {
            this.mainloading.PreloaderDecreaseCount();
          });
        }

      });

    }
    else {
      //this._alert.error("NotAllowed");
    }

  }

  canUpdate(contract: Contract): boolean {
    if ([3, 4].includes(contract.ContractStatusEnumId) ) {
      //this._alert.error("NotAllowed");
      return false;
    }
    return true;
  }

}
