import { Component, OnInit } from '@angular/core';
import { ContractTermsViolationsDTo } from 'src/app/TMS-Module/Models/contract-terms-violations.model';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { LoaderService } from 'src/app/shared/loader.service';
import { ContractViolationSC } from 'src/app/TMS-Module/Models/search-criteria/contract-violation-SC.model';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { vw_NWC_ContractTermsDTO } from 'src/app/TMS-Module/Models/vw_NWC_Contract-TermsDTO';
import { Title } from '@angular/platform-browser';
import { AttachmentDTO } from 'src/app/shared/datamodels/attachment-dto';

@Component({
  selector: 'app-contract-violation-addedit',
  templateUrl: './contract-violation-addedit.component.html',
  styleUrls: ['./contract-violation-addedit.component.scss']
})
export class ContractViolationAddeditComponent implements OnInit {

  pagePermission: string;
  updateId: number;
  editMode = false;
  IsArabic = false;
  currentTab = 1;

  ListFiles: AttachmentDTO[] = [];

  violation: ContractTermsViolationsDTo = new ContractTermsViolationsDTo();
  myTerm: vw_NWC_ContractTermsDTO;
  incidentTimeStr: string;
  enablePrint = false;

  constructor(
    private _alert: alertService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private contractService: ContractService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private mainloading: LoaderService,
    private titleService: Title

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsContractTermsViolations');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

    let urlList = this.router.routerState.snapshot.url.split('/');
    if (urlList[4] === "edit") {
      this.updateId = +urlList[5];
      this.editMode = true;
    }
    else {
      this.updateId = null;
      this.editMode = false;
    }

  }

  ngOnInit() {

    this.setDefaultContract();
    this.loadDDLsGV();
    this.IsArabic = (this._translate.currentLang == 'ar');
    this._translate.onLangChange.subscribe(res => {
      this.IsArabic = (res.lang == 'ar');

      this.loadDDLsGV();
    });

  }


  loadDDLsGV() {
    // load Search DDls values
    this.getTermsCategories();
    this.getContracts('');
    this.getInvoiceStatues();
    this.getViolationStatusesList();
    this.getCancelReasonsList();

    this.titleService.setTitle(this._translate.instant((this.editMode ? 'EditViolation' : 'NewViolation')));
  }

  setDefaultContract() {
    //this.violation = new ContractTermsViolationsDTo;

    if (!isNullOrUndefined(this.updateId) && this.updateId > 0) {
      let filters = new ContractViolationSC;
      filters.Id = this.updateId;
      filters.PageFilter.PageSize = 1;
      filters.PageFilter.PageIndex = 1;

      this.mainloading.PreloaderIcreaseCount();
      this.contractService.GetContractViolations(filters).subscribe(res => {
        if (res.Value && res.Value.Result && res.Value.Result.length > 0) {
          this.violation = res.Value.Result[0];

          if (this.violation && this.violation.IssueDate) {
            this.violation.IssueDate = new Date(this.violation.IssueDate);
          }

          // if (this.violation && this.violation.PaymentDueDate) {
          //   this.violation.PaymentDueDate = new Date(this.violation.PaymentDueDate);
          // }
          if (this.violation && this.violation.PaymentStatusId == 1) {
            this.enablePrint = true;
          }

          if (this.violation && this.violation.PaymentStatusDate) {
            this.violation.PaymentStatusDate = new Date(this.violation.PaymentStatusDate);
          }

          if (this.violation && this.violation.IncidentTime) {
            this.violation.IncidentTime = new Date(this.violation.IncidentTime);
            this.incidentTimeStr = this.violation.IncidentTime.toTimeString().substring(0, 5);
          }


          if (this.violation && this.violation.PaymentStatusId && +this.violation.PaymentStatusId > 0) {
            let selectedInvoice = this.InvoiceStatusList.find(s => s.Id == this.violation.PaymentStatusId);
            if (!isNullOrUndefined(selectedInvoice)) {
              this.bindingModel_InvoiceStatues.push(selectedInvoice);
            }
          }

          if (this.violation && this.violation.StatusId && +this.violation.StatusId > 0) {
            let selectedStatus = this.ViolationStatusesList.find(s => s.Id == this.violation.StatusId);
            if (!isNullOrUndefined(selectedStatus)) {
              this.bindingModel_ViolationStatuses.push(selectedStatus);
            }
          }

          if (this.violation && this.violation.CancelReasonId && +this.violation.CancelReasonId > 0) {
            let selectedReason = this.CancelReasonsList.find(s => s.Id == this.violation.CancelReasonId);
            if (!isNullOrUndefined(selectedReason)) {
              this.bindingModel_CancelReasons.push(selectedReason);
            }
          }

          if (this.violation && this.violation.VehicleId && this.violation.VehicleId != "") {
            this.getVehicles(this.violation.VehicleCode, "LoadUpdate");
          }

          if (this.violation && this.violation.DriverId && this.violation.DriverId != "") {
            this.getDrivers(this.violation.DriverName, "LoadUpdate");
          }

          if (this.violation && this.violation.ContractTermId && +this.violation.ContractTermId > 0) {
            this.getTermUnitValue(this.violation.ContractTermId);
          }



        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

      //Attachments
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.GetContractViolationsAttachments(this.updateId).subscribe(res => {
        if (!res.IsErrorState && !isNullOrUndefined(res.Value)) {
          this.ListFiles = res.Value;
        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

    }
    else {
      // this.contract.ContractTypeID = null;
      // this.contract.ContractStatusID = null;

      this.violation.IssueDate = new Date();
      //this.violation.PaymentDueDate = new Date();
      this.violation.PaymentStatusDate = new Date();
      this.violation.IncidentTime = new Date();
      this.incidentTimeStr = this.violation.IncidentTime.toTimeString().substring(0, 5);
      this.violation.AddVehicleToBlacklist = true;

    }
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  //#region  "Lookups"
  SearchStream: SearchStream = new SearchStream();

  ContractList: Lookup<number>[] = [];
  StationList: Lookup<string>[] = [];
  TermCategoryList: Lookup<number>[] = [];
  TermList: Lookup<number>[] = [];
  InvoiceStatusList: Lookup<number>[] = [];
  ViolationStatusesList: Lookup<number>[] = [];
  CancelReasonsList: Lookup<number>[] = [];
  vehicleList: Lookup<string>[] = [];
  driverList: Lookup<string>[] = [];

  bindingModel_Contracts: Lookup<number>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_Categories: Lookup<number>[] = [];
  bindingModel_Terms: Lookup<number>[] = [];
  bindingModel_InvoiceStatues: Lookup<number>[] = [];
  bindingModel_ViolationStatuses: Lookup<number>[] = [];
  bindingModel_CancelReasons: Lookup<number>[] = [];
  bindingModel_Vehicles: Lookup<string>[] = [];
  bindingModel_Drivers: Lookup<string>[] = [];

  Contract_Loading = false;
  Station_Loading = false;
  Term_Loading = false;
  TermUnit_Loading = false;
  Vehicle_loading = false;
  Driver_Loading = false;

  selectMenuOptions = {
    enableSearchFilter: true,
    singleSelect: true
  };

  selectMenuOptionsDisableSearch = {
    enableSearchFilter: false,
    singleSelect: true
  };


  getTermsCategories() {
    this.lookupservice.GetTermsCategory().subscribe(res => {
      if (res.Value)
        this.TermCategoryList = res.Value;
    });
  }

  getInvoiceStatues() {
    this.lookupservice.GetOrderInvoiceStatuses().subscribe(res => {
      if (res.Value) {
        this.InvoiceStatusList = res.Value;

        // on Edit selected invoice
        if (this.updateId && +this.updateId > 0 && this.violation && this.violation.PaymentStatusId && +this.violation.PaymentStatusId > 0) {
          let selectedInvoice = this.InvoiceStatusList.find(s => s.Id == this.violation.PaymentStatusId);
          if (selectedInvoice) {
            this.bindingModel_InvoiceStatues.push(selectedInvoice);
          }
        }

      }
    });
  }

  getViolationStatusesList() {
    this.lookupservice.GetContractTermsViolationStatuses().subscribe(res => {
      if (res.Value) {
        this.ViolationStatusesList = res.Value;

        // on Edit selected Status
        if (this.updateId && +this.updateId > 0 && this.violation && this.violation.StatusId && +this.violation.StatusId > 0) {
          let selectedStatus = this.ViolationStatusesList.find(s => s.Id == this.violation.StatusId);
          if (selectedStatus) {
            this.bindingModel_ViolationStatuses.push(selectedStatus);
          }
        }

      }
    })
  }

  getCancelReasonsList() {
    this.lookupservice.GetContractTermsViolationCancelReasons().subscribe(res => {
      if (res.Value) {
        this.CancelReasonsList = res.Value;

        // on Edit selected cancel Reason
        if (this.updateId && +this.updateId > 0 && this.violation && this.violation.CancelReasonId && +this.violation.CancelReasonId > 0) {
          let selectedReason = this.CancelReasonsList.find(s => s.Id == this.violation.CancelReasonId);
          if (selectedReason) {
            this.bindingModel_CancelReasons.push(selectedReason);
          }
        }

      }
    })
  }

  getContracts(searchKeyword: string) {
    this.SearchStream.initStream("Contractor_contractDetails", (a) => {
      this.Contract_Loading = true;
      this.lookupservice.SearchContractContractors(a).subscribe(res => {
        if (res.Value)
          this.ContractList = res.Value;
      }
        , err => {
          this.Contract_Loading = false;
        }
        , () => {
          this.Contract_Loading = false;
        });
    }).next(searchKeyword);
  }

  getStations(contractId: number) {
    this.Station_Loading = true;
    this.lookupservice.getContractStations(contractId).subscribe(res => {
      if (res.Value)
        this.StationList = res.Value;
    }
      , err => {
        this.Station_Loading = false;
      }
      , () => {
        this.Station_Loading = false;
      });
  }

  getTerms(contractId: number, stationId: string, categoryId: number) {
    this.Term_Loading = true;
    this.lookupservice.GetContractTerm(contractId, stationId, categoryId).subscribe(res => {
      if (res.Value)
        this.TermList = res.Value;
    }
      , err => {
        this.Term_Loading = false;
      }
      , () => {
        this.Term_Loading = false;
      });
  }

  getTermUnitValue(TermId: number) {
    this.TermUnit_Loading = true;
    this.contractService.GetTermValueUnit(TermId).subscribe(res => {
      if (res.Value) {
        this.myTerm = res.Value;

        if (!this.editMode) {
          this.violation.TotalPenalty = this.myTerm.TotalValue;
          this.violation.TermUnitNoOfDays = 1;
        }
      }
    }
      , err => {
        this.TermUnit_Loading = false;
      }
      , () => {
        this.TermUnit_Loading = false;
      });
  }

  getVehicles(searchKeyword: string, source = '') {
    if (!this.violation.StationID) {
      this.vehicleList = [];
      this.bindingModel_Vehicles = [];
      return;
    }

    this.SearchStream.initStream("VehicleDDL_exitGate", (a) => {
      this.Vehicle_loading = true;
      this.lookupservice.searchVehicles(a, [this.violation.StationID]).subscribe(res => {
        if (res.Value) {
          this.vehicleList = res.Value;

          // on LoadUpdate select the vehicle
          if (source == 'LoadUpdate' && this.updateId && +this.updateId > 0 && this.violation && this.violation.VehicleId && this.violation.VehicleId != "") {
            let selectedVehicle = this.vehicleList.find(s => s.Id == this.violation.VehicleId);
            if (selectedVehicle) {
              this.bindingModel_Vehicles.push(selectedVehicle);
            }
          }

        }
      }
        , err => {
          this.Vehicle_loading = false;
        }
        , () => {
          this.Vehicle_loading = false;
        }
      );
    }).next(searchKeyword);
  }

  getDrivers(searchKeyword: string, source = '') {
    if (!this.violation.VehicleId) {
      this.driverList = [];
      this.bindingModel_Drivers = [];
      return;
    }

    this.SearchStream.initStream("DriverDDL_exitGate", (a) => {
      this.Driver_Loading = true;
      this.lookupservice.SearchDriversByTanker(a, [this.violation.VehicleId]).subscribe(res => {
        if (res.Value) {
          this.driverList = res.Value;

          // on LoadUpdate select the Driver
          if (source == 'LoadUpdate' && this.updateId && +this.updateId > 0 && this.violation && this.violation.DriverId && this.violation.DriverId != "") {
            let selectedDriver = this.driverList.find(s => s.Id == this.violation.DriverId);
            if (selectedDriver) {
              this.bindingModel_Drivers.push(selectedDriver);
            }
          }
          else {
            this.violation.DriverId = this.driverList[0].Id;
            let selectedDriver = this.driverList.find(s => s.Id == this.violation.DriverId);
            if (selectedDriver) {
              this.bindingModel_Drivers.push(selectedDriver);
            }
          }

        }
      }
        , err => {
          this.Driver_Loading = false;
        }
        , () => {
          this.Driver_Loading = false;
        });
    }).next(searchKeyword);
  }


  onContractDDLChanged(evt) {
    this.violation.ContractID = evt[0].Id;
    this.StationList = [];
    this.bindingModel_Stations = [];
    this.TermList = [];
    this.bindingModel_Terms = [];
    this.myTerm = null;
    this.getStations(this.violation.ContractID);
  }

  onStationDDLChanged(evt) {
    this.violation.StationID = evt[0].Id;
    this.callGetTerm();
    this.callGetTransporter();
  }

  onCategoryDDLChanged(evt) {
    this.violation.CategoryId = evt[0].Id;
    this.callGetTerm();
  }

  callGetTerm() {
    this.TermList = [];
    this.bindingModel_Terms = [];
    this.myTerm = null;
    if (this.violation && this.violation.ContractID && +this.violation.ContractID > 0
      && this.violation.StationID && this.violation.StationID != ''
      && this.violation.CategoryId && +this.violation.CategoryId > 0) {
      this.getTerms(this.violation.ContractID, this.violation.StationID, this.violation.CategoryId);
    }
  }

  callGetTransporter() {
    this.vehicleList = [];
    this.bindingModel_Vehicles = [];
    this.violation.VehicleId = '';
    this.driverList = [];
    this.bindingModel_Drivers = [];
    this.violation.DriverId = '';
    this.getVehicles('');
  }

  onTermDDLChanged(evt) {
    this.violation.ContractTermId = evt[0].Id;
    this.myTerm = null;
    this.getTermUnitValue(this.violation.ContractTermId);
  }

  onInvoiceStatusDDLChanged(evt) {
    this.violation.PaymentStatusId = evt[0].Id;
  }

  onViolationStatusDDLChanged(evt) {
    this.violation.StatusId = evt.map(m => m.Id)[0];
  }

  onCancelReasonDDLChanged(evt) {
    this.violation.CancelReasonId = evt.map(m => m.Id)[0];
  }

  onVehicleDDLChanged(evt) {
    this.violation.VehicleId = evt[0].Id;
    this.violation.DriverId = null;
    this.driverList = [];
    this.bindingModel_Drivers = [];
    this.getDrivers('');
  }

  onDriverDDLChanged(evt) {
    this.violation.DriverId = evt[0].Id;
  }

  //#endregion  "Lookups"

  onNoOfDaysChanged() {
    if (isNaN(this.violation.TermUnitNoOfDays) || +this.violation.TermUnitNoOfDays < 1)
      this.violation.TermUnitNoOfDays = 1;
    this.violation.TotalPenalty = this.violation.TermUnitNoOfDays * this.myTerm.TotalValue;
  }


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (!this.editMode) {
      if (!this.violation.ContractID) {
        validationMessages.push("ContractIdIsRequired");
      }

      if (!this.violation.StationID) {
        validationMessages.push("StationIsRequired");
      }

      if (!this.violation.CategoryId) {
        validationMessages.push("ChooseCategory");
      }

      if (!this.violation.ContractTermId) {
        validationMessages.push("ContractTermRequired");
      }
    }

    // if (!this.violation.ViolationLocation) {
    //   validationMessages.push("ViolationLocationRequired");
    // }

    if (!this.violation.PaymentStatusId) {
      validationMessages.push("PaymentStatusIdRequired");
    }

    if (!this.violation.StatusId) {
      validationMessages.push("StatusIdRequired");
    }

    if (this.violation.StatusId == 3 && !this.violation.CancelReasonId) {
      validationMessages.push("CancellationReasonRequired");
    }


    let timeInc = this.incidentTimeStr.split(":");
    if (
      timeInc.length !== 2 ||
      isNullOrUndefined(timeInc[0]) ||
      timeInc[0] === "" ||
      isNullOrUndefined(timeInc[1]) ||
      timeInc[1] === ""
    ) {
      validationMessages.push("IncidentTimeRequired");
    }


    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  save() {

    if (!this.isValidModel()) return;

    this.violation.ViolationAttachments = this.ListFiles;

    let timeFrom = this.incidentTimeStr.split(":");
    this.violation.IncidentTime.setHours(+timeFrom[0]);
    this.violation.IncidentTime.setMinutes(+timeFrom[1]);


    //alert time zone offset before send
    let modifiedContract = Object.assign({}, this.violation)
    modifiedContract.IncidentTime = new Date(this.violation.IncidentTime.getTime());
    modifiedContract.IncidentTime.setMinutes(modifiedContract.IncidentTime.getMinutes() - modifiedContract.IncidentTime.getTimezoneOffset());
    modifiedContract.IssueDate = new Date(this.violation.IssueDate.getTime());
    modifiedContract.IssueDate.setMinutes(modifiedContract.IssueDate.getMinutes() - modifiedContract.IssueDate.getTimezoneOffset());
    //modifiedContract.PaymentDueDate = new Date(this.violation.PaymentDueDate.getTime());
    //modifiedContract.PaymentDueDate.setMinutes(modifiedContract.PaymentDueDate.getMinutes() - modifiedContract.PaymentDueDate.getTimezoneOffset());
    if (!isNullOrUndefined(this.violation.PaymentStatusDate)) {
      modifiedContract.PaymentStatusDate = new Date(this.violation.PaymentStatusDate.getTime());
      modifiedContract.PaymentStatusDate.setMinutes(modifiedContract.PaymentStatusDate.getMinutes() - modifiedContract.PaymentStatusDate.getTimezoneOffset());
    }

    // edit
    if (!isNullOrUndefined(this.updateId)) {
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.EditContractViolation(modifiedContract).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          this.navigateToList();

        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

    }
    else // new
    {
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.AddContractViolation(modifiedContract).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          this.navigateToList();
        }

      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

    }


  }


  navigateToList() {
    this.router.navigate(['/tms/contract/violationslist']);
  }

  close() {
    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.navigateToList();
        //this.router.navigate(['/tms/contract/violationslist']);
      }
    })
  }

  closeRedirectToContracts() {
    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.router.navigate(['/tms/contract/contractlist']);
      }
    })
  }

  switchTab(tabName) {
    this.currentTab = tabName;
  }

}
