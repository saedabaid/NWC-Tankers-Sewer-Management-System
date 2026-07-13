import { Component, OnInit } from '@angular/core';
import { ContractService } from '../../../../Services/contract.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { vw_NWC_ContractTermsDTO } from 'src/app/TMS-Module/Models/vw_NWC_Contract-TermsDTO';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';

import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { isNullOrUndefined } from 'src/app/shared/utilities/utilities';
import { TranslateService } from '@ngx-translate/core';
import { searchCriteriaContractDTO } from 'src/app/TMS-Module/Models/search-criteria/search-Criteria-Contract-DTO';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { ContractTermDTO } from 'src/app/TMS-Module/Models/contract-termsDTO';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'violation-terms-details',
  templateUrl: './violation-terms-details.component.html',
  styleUrls: ['./violation-terms-details.component.scss']
})
export class ViolationTermsDetailsComponent implements OnInit {

  advancedDiv = false;
  TermsCategoryList: Lookup<number>[] = [];
  TermsValueUnits: Lookup<number>[] = [];
  contractStations: Lookup<string>[] = [];

  selectedStations: Lookup<string>[] = [];
  selectedTermsCategory: Lookup<number>[] = [];
  selectedValueUnit: Lookup<number>[] = [];
  validationMessages: string[] = [];

  IsEditMood = false;
  searchCriteriaContractDTO: searchCriteriaContractDTO = new searchCriteriaContractDTO();
  ContractTermDTO: ContractTermDTO = new ContractTermDTO();
  ContractTermsList: SearchResult<vw_NWC_ContractTermsDTO> = new SearchResult<vw_NWC_ContractTermsDTO>();

  selectMenuOptions = {
    enableSearchFilter: false,
  };
  selectMenuOptions2 = {
    enableSearchFilter: false,
    singleSelect: true
  };

  constructor(private translateService: TranslateService, private contractService: ContractService, private LookupService: LookupService, private _alert: alertService,
    private mainloading: LoaderService
  ) { }

  ngOnInit() {


    this.searchCriteriaContractDTO.PageFilter.PageIndex = 1;
    this.searchCriteriaContractDTO.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.ContractTermDTO.ContractID = this.searchCriteriaContractDTO.ContractID = this.contractService.updateContractId;
    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });

  }

  load() {
    this.getContractStations();
    this.GetTermsCategory();
    this.GetTermsValueUnits();
    this.GetContractTermsList();
  }

  getContractStations() {
    this.LookupService.getContractStations(this.contractService.updateContractId).subscribe(res => {
      if (res.Value) {
        this.contractStations = res.Value;
      }
    });
  }

  GetTermsCategory() {
    this.LookupService.GetTermsCategory().subscribe(res => {
      if (res.IsErrorState == false) {
        this.TermsCategoryList = res.Value;
      }
    });
  }

  GetTermsValueUnits() {
    this.LookupService.GetTermsValueUnits().subscribe(res => {
      if (res.IsErrorState == false) {
        this.TermsValueUnits = res.Value;
      }
    });
  }



  GetContractTermsList() {
    this.mainloading.PreloaderIcreaseCount();
    this.contractService.GetContractTermsList(this.searchCriteriaContractDTO).subscribe(res => {
      // console.log(res);
      if (res.IsErrorState == false) {
        this.ContractTermsList = res.Value;
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });
  }


  onStationChanged($event) {
    this.selectedStations = $event;
  }
  onTermsChanged($event) {
    this.selectedTermsCategory = $event;
  }

  save() {

    this.ContractTermDTO.StationIDs = this.selectedStations.map(s => s.Id);
    this.ContractTermDTO.TermsCategoryID = this.selectedTermsCategory.map(s => s.Id)[0];
    this.ContractTermDTO.TotalValueUnitId = this.selectedValueUnit.map(s => s.Id)[0];

    if (this.isValidModel()) {
      if (this.IsEditMood == false) {
        this.AddItem();
      }
      else {
        this.EditItem();
      }

    }

  }

  edit(term: vw_NWC_ContractTermsDTO) {
    if (this.IsEditMood != true) {
      this.IsEditMood = true;
      this.ContractTermDTO.ID = term.ID;
      this.ContractTermDTO.Description = term.Description;
      this.ContractTermDTO.ContractTermName = term.ContractTermName;
      this.ContractTermDTO.ContractTermCode = term.ContractTermCode;
      this.selectedStations = [{ Name: term.stationName, Id: term.StationID } as Lookup<string>];
      this.selectedTermsCategory = [{ Name: term.Category, Id: term.TermsCategoryID } as Lookup<number>];
      this.ContractTermDTO.TotalValue = term.TotalValue;
      this.selectedValueUnit = [{ Name: term.TotalValueUnitName, Id: term.TotalValueUnitId } as Lookup<number>];


    }

  }

  delete(term: vw_NWC_ContractTermsDTO) {
    if (this.IsEditMood) {
      this._alert.error('CannotDeleteUntilSaving');
    } else {
      this._alert.confirmationMessage('ConfirmDelete').subscribe(confirm => {
        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractService.DeleteTerm(term.ID).subscribe(res => {
            if (res.IsErrorState == false) {
              this._alert.success('DeletedSuccessed');
              this.clear();
              this.GetContractTermsList();
            } else {
              this._alert.showError();
            }
          }
            , err => {
              this.mainloading.PreloaderDecreaseCount();
            }
            , () => {
              this.mainloading.PreloaderDecreaseCount();
            });
        }
      });
    }

  }

  AddItem() {
    this.mainloading.PreloaderIcreaseCount();
    this.contractService.AddTerm(this.ContractTermDTO).subscribe(res => {

      if (res.IsErrorState == false) {
        this.clear();
        this.GetContractTermsList();
        if (res.Value.failed > 0) {
          let msg = this.translateService.instant(res.Value.failed.toString()) + ':' + this.translateService.instant('failed')
            + this.translateService.instant(res.Value.success.toString()) + ':' + this.translateService.instant('success');
          res.Value.message.forEach(element => {
            msg += ',' + this.translateService.instant(element);
          });

          this._alert.warning(msg);
        } else {
          this._alert.showSuccess();
        }

      } else {
        this._alert.errorList(res.Errors);
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });
  }

  EditItem() {
    this.mainloading.PreloaderIcreaseCount();
    this.contractService.UpdateTerm(this.ContractTermDTO).subscribe(res => {

      if (res.IsErrorState == false) {

        this._alert.showSuccess();
        this.clear();
        this.GetContractTermsList();
      } else {
        this._alert.errorList(res.Errors);
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });
  }

  clear() {
    this.ContractTermDTO = new ContractTermDTO();
    this.ContractTermDTO.ContractID = this.contractService.updateContractId;
    this.selectedTermsCategory = [];
    this.selectedValueUnit = [];
    this.selectedStations = [];
    this.IsEditMood = false;
  }

  isValidModel(): boolean {

    this.validationMessages = [];

    if (this.ContractTermDTO.StationIDs.length <= 0) {
      this.validationMessages.push('ChooseStation');
    }
    if (isNullOrUndefined(this.ContractTermDTO.TermsCategoryID)) {
      this.validationMessages.push('ChooseCategory');
    }
    if (isNullOrUndefined(this.ContractTermDTO.ContractTermCode)) {
      this.validationMessages.push('ContractTermCodeMissed');
    }
    if (isNullOrUndefined(this.ContractTermDTO.ContractTermName)) {
      this.validationMessages.push('ContractNameMissed');
    }

    if (this.validationMessages.length > 0) {
      this._alert.errorList(this.validationMessages);
      return false;
    }
    return true;
  }

  onPageIndexChanged(evt) {
    this.searchCriteriaContractDTO.PageFilter.PageIndex = evt;
    this.GetContractTermsList();
  }

  onPageSizeChanged(evt) {
    this.searchCriteriaContractDTO.PageFilter.PageSize = evt;
    this.GetContractTermsList();
  }
  close() {
    this.contractService.changeTab$.next('contractlist');
  }

  backBtn() {
    this.contractService.changeTab$.next('accessories');
  }

}
