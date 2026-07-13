import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { FilterModel } from 'src/app/TMS-Module/Models/common/filter-model';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { ContractTariff } from 'src/app/TMS-Module/Models/contract-tariff';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { TranslateService } from '@ngx-translate/core';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Subscription } from 'rxjs';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'tariff-details',
  templateUrl: './tariff-details.component.html',
  styleUrls: ['./tariff-details.component.scss']
})
export class TariffDetailsComponent implements OnInit, OnDestroy {

  advancedDiv = false;
  filter: FilterModel<number>;
  searchResult = new SearchResult<ContractTariff>();
  //isEditMode: boolean = false;


  refreshTariffSubscripe: Subscription;

  constructor(private router: Router,
    private contractService: ContractService,
    private _translate: TranslateService,
    private _alert: alertService,
    private mainloading: LoaderService
    ) {
  }


  ngOnInit() {

    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(() => {
      this.loadDDLsGV();
    });


    this.refreshTariffSubscripe = this.contractService.refreshTariffGV$.subscribe(() => {
      this.onSearchSubmit();
    });

  }

  ngOnDestroy(): void {
    this.refreshTariffSubscripe.unsubscribe();
  }

  loadDDLsGV() {
    this.searchCaller();

    // load Search DDls values

  }

  setDefaultSearchValues() {
    this.filter = new FilterModel<number>();
    this.filter.SearchKeyword = this.contractService.updateContractId;
    this.filter.PageFilter = new PageFilter();
    this.filter.PageFilter.PageIndex = 1;
    this.filter.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.contractService.isTariffEditMode = false;
  }


  //#region "table Pagination and Search"
  onSearchSubmit() {
    this.filter.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  onPageIndexChanged(evt) {
    this.filter.PageFilter.PageIndex = evt;
    this.searchCaller();
  }

  onPageSizeChanged(evt) {
    this.filter.PageFilter.PageSize = evt;
    this.searchCaller();
  }

  searchCaller() {
    this.mainloading.PreloaderIcreaseCount();
    this.contractService.SearchTariffList(this.filter).subscribe(res => {
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
  //#endregion "table Pagination and Search"


  editTariff(tariff: ContractTariff) {
    if (this.contractService.isTariffEditMode == false) {
      if (isNullOrUndefined(tariff.ZoneID) || isNullOrUndefined(tariff.TanckerCapacityId)) {
        this._alert.error("NotAllowed");
        return;
      }

      this.contractService.isTariffEditMode = true;
      this.contractService.updateTariffModel$.next(tariff);
    }
  }

  deleteTariff(tariff: ContractTariff) {
    if (this.contractService.isTariffEditMode == true) {
      this._alert.error("CannotDeleteUntilSaving");
    }
    else {

      this._alert.confirmationMessage("ConfirmDeleteTariff").subscribe(confirm => {

        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractService.DeleteTariff(tariff.ID).subscribe(res => {

            if (!res.IsErrorState) {
              this._alert.success("DeletedSuccessed");
              this.onSearchSubmit();
            }
            else {
              this._alert.showError();
            }
          }
          ,err => {
            this.mainloading.PreloaderDecreaseCount();
          }
          ,() => {
            this.mainloading.PreloaderDecreaseCount();
          });
      }

      })
    }
  }


  //#region  "navigations"
  close() {
    this.contractService.changeTab$.next("contractlist");
  }

  backBtn() {
    this.contractService.changeTab$.next('price');
  }

  nextBtn() {
    this.contractService.changeTab$.next('accessories');
  }
  //#endregion  "navigations"



}
