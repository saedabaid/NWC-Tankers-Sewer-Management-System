import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { TranslateService } from '@ngx-translate/core';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { contractAccessorySC } from 'src/app/TMS-Module/Models/search-criteria/contractAccessory-SC';
import { contractAccessory } from 'src/app/TMS-Module/Models/contract-Accessory';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'accessories-charges-details',
  templateUrl: './accessories-charges-details.component.html',
  styleUrls: ['./accessories-charges-details.component.scss']
})
export class AccessoriesChargesDetailsComponent implements OnInit {

  advancedDiv = false;
  searchCriteria = new contractAccessorySC();
  contractAccessoryList = new SearchResult<contractAccessory>();

  contractStations: Lookup<string>[] = [];
  selectedStations: Lookup<string>[] = [];

  Accessories: Lookup<number>[] = [];
  selectedAccessories: Lookup<number>[] = [];

  charge: number = 0;
  isEditMode: boolean = false;
  contractAccID: number = 0;

  selectMenuOptions = {
    enableSearchFilter: true,    
  };

  constructor(private router: Router,
    private contractService: ContractService,
    private lookupService: LookupService,
    private _translate: TranslateService,
    private alert: alertService,
    private mainloading: LoaderService
    ) {
  }

  ngOnInit() {
    this.getContractStations();
    this.getAccessories();
    this.setDefaultSearchValues();

    this.searchContractAccessories(this.searchCriteria);

    this._translate.onLangChange.subscribe(res => {
      this.setDefaultSearchValues();
      this.searchContractAccessories(this.searchCriteria);
    });
  }

  setDefaultSearchValues() {
    this.searchCriteria.PageFilter = new PageFilter();
    this.searchCriteria.PageFilter.PageIndex = 1;
    this.searchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    //Get InService & Assigned vehicles
    this.searchCriteria.ContractID = this.contractService.updateContractId;

    this.selectedStations = [];
    this.selectedAccessories = [];
    this.charge = 0;
    this.isEditMode = false;
    this.contractAccID = 0;
  }

  searchContractAccessories(contractAccessorySC: contractAccessorySC) {
    this.mainloading.PreloaderIcreaseCount();
    this.contractService.SearchContractAccessories(contractAccessorySC).subscribe(res => {
      if (res.Value)
        this.contractAccessoryList = res.Value;
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });
  }

  getContractStations() {
    //this.mainloading.PreloaderIcreaseCount();
    this.lookupService.getContractStations(this.contractService.updateContractId).subscribe(res => {
      if (res.Value)
        this.contractStations = res.Value;
    }
    ,err => {}
    ,() => {
      //this.mainloading.PreloaderDecreaseCount();
    });
  }

  getAccessories() {
    this.lookupService.getAccessories().subscribe(res => {
      if (res.Value) {
        this.Accessories = res.Value;
      }
    });
  }

  onStationChanged($event) {
    this.selectedStations = $event;
  }

  onAccessoriesChanged($event) {
    this.selectedAccessories = $event;
  }

  onEdit(contractAcc: contractAccessory) {

    if (this.isEditMode == false) {

      this.isEditMode = true;
      this.contractAccID = contractAcc.ID;

      let selectedStation = this.contractStations.find(s => s.Id == contractAcc.StationID);
      if (!isNullOrUndefined(selectedStation)) {
        this.selectedStations = [];
        this.selectedStations.push(selectedStation);
      }

      let selectedAcc = this.Accessories.find(s => s.Id == contractAcc.AccessoryID);
      if (!isNullOrUndefined(selectedAcc)) {
        this.selectedAccessories = [];
        this.selectedAccessories.push(selectedAcc);
      }

      this.charge = contractAcc.Charge;
    }
  }

  onDelete(contractAcc: contractAccessory) {

    if (this.isEditMode == true) {
      this.alert.error("CannotDeleteUntilSaving");
    }
    else {
      this.alert.confirmationMessage("DeleteMsgContractAccessory").subscribe(confirm => {

        if (confirm == true) {
          this.mainloading.PreloaderIcreaseCount();
          this.contractService.DeleteContractAccessory(contractAcc.ID).subscribe(res => {
            if (!res.IsErrorState) {
              this.alert.success("DeletedSuccessed");
            }

            this.searchContractAccessories(this.searchCriteria);
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

  onSave() {
    if (!this.isValidModel()) return;

    let contractAcc = new contractAccessory();

    if (this.isEditMode == true) {
      contractAcc.ID = this.contractAccID;
      contractAcc.ContractID = this.contractService.updateContractId;
      contractAcc.StationID = this.selectedStations[0].Id;
      contractAcc.AccessoryID = this.selectedAccessories[0].Id;
      contractAcc.Charge = this.charge;

      this.mainloading.PreloaderIcreaseCount();
      this.contractService.UpdateContractAccessory(contractAcc).subscribe(res => {
        if (res.IsErrorState == true) {
          this.alert.errorList(res.Errors);
        }

        if (!res.IsErrorState) {
          this.alert.showSuccess();
          this.isEditMode = false;

          this.setDefaultSearchValues();
          this.searchContractAccessories(this.searchCriteria);
        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

      //this.isEditMode = false;
    }
    else {
      contractAcc.ContractID = this.contractService.updateContractId;
      contractAcc.StationIDs = this.selectedStations.map(({ Id }) => Id);
      contractAcc.AccessoryIDs = this.selectedAccessories.map(({ Id }) => Id);
      contractAcc.Charge = this.charge;

      this.mainloading.PreloaderIcreaseCount();
      this.contractService.AddContractAccessories(contractAcc).subscribe(res => {
        if (res.IsErrorState == true) {
          this.alert.errorList(res.Errors);
        }
        else if (!isNullOrUndefined(res.Value) && res.Value.failed > 0) {
          // let msg = `${this._translate.instant("success")}: ${res.Value.success}, ${this._translate.instant("failed")}: ${res.Value.failed}`;
          let msg: string;

          if (this._translate.currentLang == 'ar') {
            msg = `تم تسجيل (${res.Value.success}) سعار استخدام المعدات بينما لم يتم تسجيل (${res.Value.failed}) من الاسعار لان تم تسجيلهم مسبقاً`;
          }
          else {
            msg = `${res.Value.success} records were added successfully , while ${res.Value.failed} records failed since charge already exists.`;
          }

          this.alert.warning(msg);

          this.setDefaultSearchValues();
          this.searchContractAccessories(this.searchCriteria);
        }
        else {
          this.setDefaultSearchValues();
          this.searchContractAccessories(this.searchCriteria);
        }

        // if (!res.IsErrorState) {
        //   this.alert.showSuccess();

        //   this.setDefaultSearchValues();
        //   this.searchContractAccessories(this.searchCriteria);
        // }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });
    }
  }

  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.selectedStations) || (this.selectedStations.length == 0)) {
      validationMessages.push("ChooseStation");
    }

    if (isNullOrUndefined(this.selectedAccessories) || (this.selectedAccessories.length == 0)) {
      validationMessages.push("ChooseAccessory");
    }

    if (isNullOrUndefined(this.charge) || +this.charge < 0) {
      validationMessages.push("InvalidCharge");
    }

    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  onClear() {
    this.isEditMode = false;
    this.setDefaultSearchValues();
  }

  onPageIndexChanged(evt) {
    this.searchCriteria.PageFilter.PageIndex = evt;
    this.searchContractAccessories(this.searchCriteria);
  }

  onPageSizeChanged(evt) {
    this.searchCriteria.PageFilter.PageSize = evt;
    this.searchContractAccessories(this.searchCriteria);
  }

  close() {
    this.contractService.changeTab$.next("contractlist");
  }

  backBtn() {
    this.contractService.changeTab$.next('traiff');
  }

  nextBtn() {
    this.contractService.changeTab$.next('violation');
  }

  onChargeChanged() {
    if (isNaN(+this.charge) || +this.charge < 0) {
      this.charge = 0;
    }
  }

}
