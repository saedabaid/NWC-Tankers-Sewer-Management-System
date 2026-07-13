import { Component, OnInit } from '@angular/core';
import { SearchResult } from '../../../Models/common/search-result';
import { Contractor } from '../../../Models/contractor';
import { ContractorSearchCriteria } from '../../../Models/search-criteria/contractor-search-criteria';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from '../../../Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { Title } from '@angular/platform-browser';
import { ContractorService } from '../../../Services/contractor.service';
import { FilterModel } from '../../../Models/common/filter-model';
import { PageFilter } from '../../../Models/common/page-fillter-model';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { LoaderService } from 'src/app/shared/loader.service';


@Component({
  selector: 'app-contractor-list',
  templateUrl: './contractor-list.component.html',
  styleUrls: ['./contractor-list.component.scss']
})
export class ContractorListComponent implements OnInit {
  advancedDiv = <boolean>false;
  SearchCriteria: ContractorSearchCriteria;
  searchResult = new SearchResult<Contractor>();

  pagePermission: string = '';
  tableLoading= false;

  constructor(
    private authenticationService: AuthenticationService,
    //private router: Router,
    private contractorService: ContractorService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private _alert: alertService,
    private titleService: Title,
    private mainloading: LoaderService
  ) {

    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('contractorlist');
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
    this.titleService.setTitle(this._translate.instant('ContractorList'));
    this.searchCaller();
    // load Search DDls values
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new ContractorSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }


  //#region  "get Lookups"
  searchContractorsNames(name) {
    return this.lookupservice.SearchContractorNameCode(name);
  }
  //#endregion  "get Lookups"


  //#region  "on Lookups change"
  onContractorNameDDLChanged(evt) {
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
    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.contractorService.searchContractorList(this.SearchCriteria).subscribe(res => {
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
    })
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"


  ActivateDeactivateContractor(contractor: Contractor) {
    if (!this.authenticationService.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }

    if (!contractor.IsActive) {

      this._alert.confirmationMessage("ConfirmActivateContractor").subscribe(confirm => {
        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractorService.ActivateContractor(contractor.ID).subscribe(res => {
            if (!res.IsErrorState) {
              this._alert.success("ActivatedSuccessed");
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

    } else {

      this._alert.confirmationMessage("ConfirmDeActivateContractor").subscribe(confirm => {
        if (confirm == true) {

          this.mainloading.PreloaderIcreaseCount();
          this.contractorService.DeActivateContractor(contractor.ID).subscribe(res => {
            if (!res.IsErrorState) {
              if (res.Value == true) {
                this._alert.success("DeActivatedSuccessed");
                this.onSearchSubmit();
              } else {
                this._alert.error("DactivateContractorWithActiveContractError");
              }
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

  }

  AddRemoveBlackListed(contractor: Contractor) {
    if (!this.authenticationService.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }

    if (!contractor.IsBlackListed) {

      this._alert.confirmationMessage("ConfirmAddContractorToBlackList").subscribe(confirm => {
        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractorService.AddContractorToBlackListed(contractor.ID).subscribe(res => {
            if (!res.IsErrorState) {
              if (res.Value == true) {
                this._alert.success("AddedToBlackListedSuccessed");
                this.onSearchSubmit();
              }
              else {
                this._alert.error("AddContractorToBlacklistedWithActiveContractError");
              }

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

    } else {

      this._alert.confirmationMessage("ConfirmRemoveContractorToBlackList").subscribe(confirm => {
        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractorService.RemoveContractorFromBlackListed(contractor.ID).subscribe(res => {
            if (!res.IsErrorState) {
              if (res.Value == true) {
                this._alert.success("RemovedfromBlackListedSuccessed");
                this.onSearchSubmit();
              }
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
  }

}
