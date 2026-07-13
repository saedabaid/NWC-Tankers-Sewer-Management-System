import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { Router } from '@angular/router';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { Contract } from 'src/app/TMS-Module/Models/contract';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { ContractSearchCriteria } from 'src/app/TMS-Module/Models/search-criteria/contract-search-criteria';
import { AttachmentDTO } from 'src/app/shared/datamodels/attachment-dto';
import { LoaderService } from 'src/app/shared/loader.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';

@Component({
  selector: 'contract-details',
  templateUrl: './contract-details.component.html',
  styleUrls: ['./contract-details.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ContractDetailsComponent implements OnInit, OnDestroy {
  ListFiles: AttachmentDTO[] = [];

  contract: Contract;
  editMode = true;

  constructor(
    private _alert: alertService,
    private router: Router,
    private authServer: AuthenticationService,
    private contractService: ContractService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private mainloading: LoaderService
  ) {

  }

  ngOnInit() {

    this.editMode = (!isNullOrUndefined(this.contractService.updateContractId) && this.contractService.updateContractId > 0);

    this.setDefaultContract();
    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });

  }



  loadDDLsGV() {
    // load Search DDls values
    this.getContractsTypes();
    this.getContractsStatuses();
    this.getContractsTerminationReasons();
    this.getContractors('');
  }

  setDefaultContract() {
    this.contract = new Contract;

    if (!isNullOrUndefined(this.contractService.updateContractId) && this.contractService.updateContractId > 0) {
      let filters = new ContractSearchCriteria;
      filters.Id = this.contractService.updateContractId;
      filters.FilterModel.PageFilter.PageSize = 1;
      filters.FilterModel.PageFilter.PageIndex = 1;

      this.mainloading.PreloaderIcreaseCount();
      this.contractService.searchContractList(filters).subscribe(res => {
        if (res.IsErrorState == false && res.Value.Result.length > 0) {
          this.contract = res.Value.Result[0];

          if (this.contract.ContractStatusEnumId == 4) {
            this._alert.error("CanNotEditTerminatedContract");
            this.navigateToList();
          }

          if (this.contract.ContractStatusEnumId == 3) {
            this._alert.error("CanNotEditFinishedContract");
            this.navigateToList();
          }

          this.contract.ContractStartDate = isNullOrUndefined(this.contract.ContractStartDate) ? null : new Date(this.contract.ContractStartDate);
          this.contract.ContractEndDate = isNullOrUndefined(this.contract.ContractEndDate) ? null : new Date(this.contract.ContractEndDate);
          this.contract.TerminatedDate = isNullOrUndefined(this.contract.TerminatedDate) ? null : new Date(this.contract.TerminatedDate);

          if (!isNullOrUndefined(this.contract.ContractorID)) {
            let selectedContractor = this.contractorsList.find(s => s.Id == this.contract.ContractorID);
            if (!isNullOrUndefined(selectedContractor)) {
              this.bindingModel_Contrcators.push(selectedContractor);
            }
            else {
              let newSelectedContractor = new Lookup<number>();
              newSelectedContractor.Id = this.contract.ContractorID;
              newSelectedContractor.Name = this.contract.ContractorFullName;

              this.contractorsList.push(newSelectedContractor);
              this.bindingModel_Contrcators.push(newSelectedContractor);

            }
          }

          if (!isNullOrUndefined(this.contract.ContractTypeID)) {

            let selectedContractorTypeID = this.contractTypesList.find(s => s.Id == this.contract.ContractTypeID);
            if (!isNullOrUndefined(selectedContractorTypeID)) {
              this.bindingModel_ContractTypes.push(selectedContractorTypeID);
            }

          }
          // if (!isNullOrUndefined(this.contract.ContractAttachments) && this.contract.ContractAttachments.length > 0)
          //   this.ListFiles = this.contract.ContractAttachments;

        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

      //Attachments
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.GetContractAttachments(this.contractService.updateContractId).subscribe(res => {
        if (!res.IsErrorState && !isNullOrUndefined(res.Value)) {
          this.ListFiles = res.Value;
        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

    }
    else {
      this.contract.ContractTypeID = null;
      this.contract.ContractStatusID = null;
      //this.contract.TerminationReasonID = null;

    }
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  //#region  "Lookups"
  SearchStream: SearchStream = new SearchStream();


  //#region  "Lookups declarations"
  contractTypesList: Lookup<number>[] = [];
  contractStatusesList: Lookup<number>[] = [];
  contractTerminationReasonsList: Lookup<number>[] = [];
  contractorsList: Lookup<number>[] = [];
  //#endregion  "Lookups declarations"

  //#region "LookupsBindingModels"
  bindingModel_Contrcators: Lookup<number>[] = [];
  bindingModel_ContractTypes: Lookup<number>[] = [];

  //#endregion "LookupsBindingModels"

  Contractor_Loading = false;

  selectMenuOptions = {
    enableSearchFilter: true,
    singleSelect: true
  };
  selectMenuOptionsDisableSearch = {
    enableSearchFilter: false,
    singleSelect: true
  };


  getContractsTypes() {
    this.lookupservice.GetContractTypes().subscribe(res => {
      if (res.Value)
        this.contractTypesList = res.Value;
    });
  }

  getContractsStatuses() {
    this.lookupservice.GetContractStatuses().subscribe(res => {
      if (res.Value)
        this.contractStatusesList = res.Value;
    });
  }

  getContractsTerminationReasons() {
    this.lookupservice.GetContractTerminationReasons().subscribe(res => {
      if (res.Value)
        this.contractTerminationReasonsList = res.Value;
    });
  }

  getContractors(searchKeyword: string) {
    this.SearchStream.initStream("Contractor_contractDetails", (a) => {
      this.Contractor_Loading = true;
      this.lookupservice.SearchContractorNameCode(a).subscribe(res => {
        if (res.Value)
          this.contractorsList = res.Value;
      }
      ,err => {
        this.Contractor_Loading = false;
      }
      ,() => {
        this.Contractor_Loading = false;
      });
    }).next(searchKeyword);
  }


  onContractorDDLChanged(evt) {
    if (!isNullOrUndefined(evt[0]))
      this.contract.ContractorID = evt[0].Id;
  }

  //#endregion  "Lookups"


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.contract.Code)) {
      validationMessages.push("InsertCode");
    }
    if (isNullOrUndefined(this.contract.ContractTypeID) || (this.contract.ContractTypeID == 0)) {
      validationMessages.push("ChooseContractType");
    }
    if (isNullOrUndefined(this.contract.ContractorID) || (this.contract.ContractorID == 0)) {
      validationMessages.push("ChooseContractor");
    }
    if (isNullOrUndefined(this.contract.ContractStartDate)) {
      validationMessages.push("InsertStartDate");
    }
    if (isNullOrUndefined(this.contract.ContractEndDate)) {
      validationMessages.push("InsertEndDate");
    }
    if (!isNullOrUndefined(this.contract.ContractStartDate) && !isNullOrUndefined(this.contract.ContractEndDate)
      && (this.contract.ContractStartDate > this.contract.ContractEndDate)) {
      validationMessages.push("StartDateMustBeAfterEndDate");
    }
    // if (isNullOrUndefined(this.contract.ContractStatusID) || (this.contract.ContractStatusID == 0)) {
    //   validationMessages.push("ChooseContractStatus");
    // }


    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  clear() {
    this.contract = new Contract;
    this.contract.ContractTypeID = null;
    this.contract.ContractStatusID = null;
    this.bindingModel_ContractTypes = [];
    this.bindingModel_Contrcators = [];

    if (!isNullOrUndefined(this.contractService.updateContractId) && this.contractService.updateContractId > 0){
      this.contract.ID = this.contractService.updateContractId;
    }

  }

  close() {
    this.contractService.changeTab$.next("contractlist");
  }

  save(leaveTab: boolean) {
    this.contract.ContractAttachments = this.ListFiles;

    if (!this.isValidModel()) return;

    //alert time zone offset before send
    let modifiedContract = Object.assign({}, this.contract)
    modifiedContract.ContractStartDate = new Date(this.contract.ContractStartDate.getTime());
    modifiedContract.ContractStartDate.setMinutes(modifiedContract.ContractStartDate.getMinutes() - modifiedContract.ContractStartDate.getTimezoneOffset());
    modifiedContract.ContractEndDate = new Date(this.contract.ContractEndDate.getTime());
    modifiedContract.ContractEndDate.setMinutes(modifiedContract.ContractEndDate.getMinutes() - modifiedContract.ContractEndDate.getTimezoneOffset());
    if (!isNullOrUndefined(this.contract.TerminatedDate)) {
      modifiedContract.TerminatedDate = new Date(this.contract.TerminatedDate.getTime());
      modifiedContract.TerminatedDate.setMinutes(modifiedContract.TerminatedDate.getMinutes() - modifiedContract.TerminatedDate.getTimezoneOffset());
    }

    // edit
    if (!isNullOrUndefined(this.contractService.updateContractId)) {
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.EditContract(modifiedContract).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          //this.contract.ContractStatusName = "";
          if (!isNullOrUndefined(leaveTab) && leaveTab == true) {
            this.contractService.changeTab$.next('stations');
          }
          else {
            this.setDefaultContract();
          }

        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

    }
    else // new
    {
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.addContract(modifiedContract).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          let targetLink = '/tms/contract/edit/' + response.Value;
          //let targetLink = '/tms/contract/edit/' + response.Value + '/stations';
          this.router.navigate([targetLink]);
        }

      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

    }


  }

  onContractTypeIDChanged($event) {
    if (!isNullOrUndefined($event[0]))
      this.contract.ContractTypeID = $event[0].Id;
  }

  navigateToList() {
    this.router.navigate(['/tms/contract/contractlist']);
  }

}
