import { Component, OnInit, OnDestroy } from '@angular/core';
import { Lookup } from '../../../../../Models/common/lookup';
import { LookupService } from '../../../../../Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { ContractTariff } from '../../../../../Models/contract-tariff';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { ContractService } from '../../../../../Services/contract.service';
import { alertService } from '../../../../../../shared/Services/alert/alert.service';
import { ContractSearchCriteria } from '../../../../../Models/search-criteria/contract-search-criteria';
import { Contract } from '../../../../../Models/contract';
import { Subscription } from 'rxjs';
import { LoaderService } from 'src/app/shared/loader.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { NgbCalendarIslamicUmalqura } from '@ng-bootstrap/ng-bootstrap';
import { UploadTariffExcelComponent } from '../upload-tariff-excel/upload-tariff-excel.component';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'add-edit-tariff',
  templateUrl: './add-edit-tariff.component.html',
  styleUrls: ['./add-edit-tariff.component.scss']
})
export class AddEditTariffComponent implements OnInit, OnDestroy {

  tariff: ContractTariff;
  updateMode = false;
  contract: Contract;

  updateTariffSubscripe: Subscription;



  constructor(
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private contractService: ContractService,
    private _alert: alertService,
    private mainloading: LoaderService,
    private _NgbCalendarIslamicUmalqura: NgbCalendarIslamicUmalqura,
    private modalRef: BsModalRef,
    private modalService: BsModalService

  ) {

  }

  ngOnInit() {
    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });

    this.updateTariffSubscripe = this.contractService.updateTariffModel$.subscribe(a => {
      this.setDefaultSearchValues();

      this.tariff = Object.assign({}, a);
      this.tariff.DateFrom = new Date(this.tariff.DateFrom);
      this.tariff.DateTo = new Date(this.tariff.DateTo);
      this.updateMode = true;

      let selectedZone = this.zoneList.find(s => s.Id == this.tariff.ZoneID);
      if (!isNullOrUndefined(selectedZone)) {
        this.bindingModel_Zones.push(selectedZone);
      }
      else {
        let newSelectedZone = new Lookup<number>();
        newSelectedZone.Id = this.tariff.ZoneID;
        newSelectedZone.Name = this.tariff.ZoneName;

        this.zoneList.push(newSelectedZone);
        this.bindingModel_Zones.push(newSelectedZone);

      }

    });


  }

  ngOnDestroy(): void {
    this.updateTariffSubscripe.unsubscribe();
    this.SearchStream.DestroyStreams();
  }

  loadDDLsGV() {

    // load Search DDls values
    this.getClass();
    this.getServiceTypes();
    this.getContractStations();
    this.getZones('');
    this.getTanckerCapacities();
  }


  //#region Drop Down list
  SearchStream: SearchStream = new SearchStream();


  //#region  "Lookups declarations"
  customerClassList: Lookup<number>[] = [];
  customerServiceList: Lookup<number>[] = [];
  contractStationsList: Lookup<string>[] = [];
  zoneList: Lookup<number>[] = [];
  TanckerCapacityList: Lookup<number>[] = [];
  //#endregion  "Lookups declarations"

  //#region "LookupsBindingModels"
  bindingModel_Classes: Lookup<number>[] = [];
  bindingModel_ServiceTypes: Lookup<number>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_TanckerCapacities: Lookup<number>[] = [];
  //#endregion "LookupsBindingModels"

  selectMenuOptions = {
    enableSearchFilter: true,
  };
  selectMenuOptions2 = {
    enableSearchFilter: true,
    singleSelect: true
  };

  Zone_Loading = false;


  //#region  "get Lookups"
  getClass() {
    this.lookupservice.getCustomerClasses().subscribe(res => {
      if (res.Value)
        this.customerClassList = res.Value;
    });
  }

  getServiceTypes() {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value)
        this.customerServiceList = res.Value;
    });
  };

  getContractStations() {
    this.lookupservice.getContractStations(this.contractService.updateContractId).subscribe(res => {
      if (res.Value)
        this.contractStationsList = res.Value;
    });
  };

  getZones(searchKeyword: string) {
    this.SearchStream.initStream("ZoneDDL_contractTariff", (a) => {
      this.Zone_Loading = true;
      this.lookupservice.SearchAllZones(a).subscribe(res => {
        if (res.Value)
          this.zoneList = res.Value;
      }
        , err => {
          this.Zone_Loading = false;
        }
        , () => {
          this.Zone_Loading = false;
        });
    }).next(searchKeyword);
  }

  getTanckerCapacities() {
    this.lookupservice.GetTanckerCapacities().subscribe(res => {
      if (res.Value)
        this.TanckerCapacityList = res.Value;
    });
  }

  //#endregion  "get Lookups"


  //#region  "on Lookups change"
  onStationsDDLChanged(evt) {
    this.tariff.StationsAddIds = evt.map(m => m.Id);
  }

  onZonesDDLChanged(evt) {
    this.tariff.ZoneAddIds = evt.map(m => m.Id);
    this.tariff.ZoneID = this.tariff.ZoneAddIds[0];
  }

  onClassDDLChanged(evt) {
    this.tariff.CustomerLocationClassAddIds = evt.map(m => m.Id);
  }

  onServiceDDLChanged(evt) {
    this.tariff.ServiceTypeAddIds = evt.map(m => m.Id);
  }

  onTanckerCapacityDDLChanged(evt) {
    this.tariff.TanckerCapacityAddIds = evt.map(m => m.Id);
  }
  //#endregion  "on Lookups change"


  //#endregion Drop Down list

  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.tariff.CubicMeterCharge) || +this.tariff.CubicMeterCharge < 0) {
      validationMessages.push("CubicMeterChargeIsRequired");
    }
    if (isNullOrUndefined(this.tariff.DistanceCharge) || +this.tariff.DistanceCharge < 0) {
      validationMessages.push("DistanceChargeIsRequired");
    }
    if (isNullOrUndefined(this.tariff.AfterFirstKM) || +this.tariff.AfterFirstKM < 0) {
      validationMessages.push("AfterFirstKMIsRequired");
    }

    if (isNullOrUndefined(this.tariff.DateFromHijri)) {
      validationMessages.push("InsertStartDate");
    }
    if (isNullOrUndefined(this.tariff.DateToHijri)) {
      validationMessages.push("InsertEndDate");
    }
    if (!isNullOrUndefined(this.tariff.DateFromHijri) && !isNullOrUndefined(this.tariff.DateToHijri)
      && this.tariff.DateFromHijri >= this.tariff.DateToHijri) {
      validationMessages.push("StartDateMustBeAfterEndDate");
    }

    // if (isNullOrUndefined(this.tariff.DateFrom)) {
    //   validationMessages.push("InsertStartDate");
    // }
    // if (isNullOrUndefined(this.tariff.DateTo)) {
    //   validationMessages.push("InsertEndDate");
    // }
    // if (!isNullOrUndefined(this.tariff.DateFrom) && !isNullOrUndefined(this.tariff.DateTo)
    //   && (this.tariff.DateFrom > this.tariff.DateTo)) {
    //   validationMessages.push("StartDateMustBeAfterEndDate");
    // }

    if (this.updateMode) {
      if (isNullOrUndefined(this.tariff.StationID) || (this.tariff.StationID == '')) {
        validationMessages.push("ChooseStation");
      }
      if (isNullOrUndefined(this.tariff.ZoneID) || (this.tariff.ZoneID == 0)) {
        validationMessages.push("ChooseZone");
      }
      if (isNullOrUndefined(this.tariff.CustomerLocationClassID) || (this.tariff.CustomerLocationClassID == 0)) {
        validationMessages.push("ChooseCustomerClass");
      }
      if (isNullOrUndefined(this.tariff.ServiceTypeID) || (this.tariff.ServiceTypeID == 0)) {
        validationMessages.push("ChooseServiceType");
      }
      if (isNullOrUndefined(this.tariff.TanckerCapacityId) || (this.tariff.TanckerCapacityId == 0)) {
        validationMessages.push("ChooseTanckerCapacity");
      }
    }
    else {
      if (isNullOrUndefined(this.tariff.StationsAddIds) || (this.tariff.StationsAddIds.length <= 0)) {
        validationMessages.push("ChooseStation");
      }
      // if (isNullOrUndefined(this.tariff.ZoneAddIds) || (this.tariff.ZoneAddIds.length <= 0)) {
      //   validationMessages.push("ChooseZone");
      // }
      if (isNullOrUndefined(this.tariff.CustomerLocationClassAddIds) || (this.tariff.CustomerLocationClassAddIds.length <= 0)) {
        validationMessages.push("ChooseCustomerClass");
      }
      if (isNullOrUndefined(this.tariff.ServiceTypeAddIds) || (this.tariff.ServiceTypeAddIds.length <= 0)) {
        validationMessages.push("ChooseServiceType");
      }

    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }



  saveTariff() {
    this.tariff.ContractID = this.contractService.updateContractId;

    if (!this.isValidModel()) return;

    //alert time zone offset before send
    // let modifiedTariff = Object.assign({}, this.tariff)
    // modifiedTariff.DateFrom = new Date(this.tariff.DateFrom.getTime());
    // modifiedTariff.DateFrom.setMinutes(modifiedTariff.DateFrom.getMinutes() - modifiedTariff.DateFrom.getTimezoneOffset());
    // modifiedTariff.DateTo = new Date(this.tariff.DateTo.getTime());
    // modifiedTariff.DateTo.setMinutes(modifiedTariff.DateTo.getMinutes() - modifiedTariff.DateTo.getTimezoneOffset());

    // add
    if (isNullOrUndefined(this.tariff.ID) || this.tariff.ID == 0) {
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.AddTariff(this.tariff).subscribe(res => {
        if (res.IsErrorState == true) {
          this._alert.errorList(res.Errors);
        }
        else if (!isNullOrUndefined(res.Value) && res.Value.failed > 0) {
          // let msg = `${this._translate.instant("success")}: ${res.Value.success}, ${this._translate.instant("failed")}: ${res.Value.failed}`;
          let msg: string;

          if (this._translate.currentLang == 'ar') {
            msg = `تم تسجيل )${res.Value.success}( اسعار للبيع بينما لم يتم تسجيل )${res.Value.failed}( من الاسعار`;
          }
          else {
            msg = `${res.Value.success} records were added successfully , while ${res.Value.failed} records failed since tariff already exists.`;
          }

          this._alert.warning(msg);

          this.cancel();
          this.contractService.refreshTariffGV$.next();
        }
        else {
          this.cancel();
          this.contractService.refreshTariffGV$.next();
        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });
    }
    else //edit
    {
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.EditTariff(this.tariff).subscribe(res => {
        if (res.IsErrorState == true) {
          this._alert.errorList(res.Errors);
        }
        else {
          this.cancel();
          this.contractService.refreshTariffGV$.next();
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


  cancel() {
    this.contractService.isTariffEditMode = false;
    this.setDefaultSearchValues();
  }


  setDefaultSearchValues() {
    this.tariff = new ContractTariff();

    this.tariff.CubicMeterCharge = 0;
    this.tariff.DistanceCharge = 0;
    this.tariff.AfterFirstKM = 0;

    // redraw DDL selections
    this.bindingModel_Stations = [];
    this.bindingModel_Classes = [];
    this.bindingModel_Zones = [];
    this.bindingModel_ServiceTypes = [];
    this.bindingModel_TanckerCapacities = [];

    this.updateMode = false;

    if (isNullOrUndefined(this.contract) &&
      !isNullOrUndefined(this.contractService.updateContractId) && this.contractService.updateContractId > 0) {
      let filters = new ContractSearchCriteria;
      filters.Id = this.contractService.updateContractId;
      filters.FilterModel.PageFilter.PageSize = 1;
      filters.FilterModel.PageFilter.PageIndex = 1;

      this.mainloading.PreloaderIcreaseCount();
      this.contractService.searchContractList(filters).subscribe(res => {
        if (res.IsErrorState == false && res.Value.Result.length > 0) {
          this.contract = res.Value.Result[0];

          this.contract.ContractStartDate = isNullOrUndefined(this.contract.ContractStartDate) ? null : new Date(this.contract.ContractStartDate);
          this.contract.ContractEndDate = isNullOrUndefined(this.contract.ContractEndDate) ? null : new Date(this.contract.ContractEndDate);

          this.setDefaultHijriDates();

          // let fromHijri = this._NgbCalendarIslamicUmalqura.fromGregorian(this.contract.ContractStartDate);
          // let toHijri = this._NgbCalendarIslamicUmalqura.fromGregorian(this.contract.ContractStartDate);
          // this.tariff.DateFromHijri = (fromHijri.year * 10000) + (fromHijri.month * 100) + fromHijri.day;
          // this.tariff.DateToHijri = (toHijri.year * 10000) + (toHijri.month * 100) + toHijri.day;


          //this.hijriModel = this._NgbCalendarIslamicUmalqura.fromGregorian(inputDate as Date);

          // this.tariff.DateFrom = this.contract.ContractStartDate;
          // this.tariff.DateTo = this.contract.ContractEndDate;
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
      this.setDefaultHijriDates();
      // this.tariff.DateFrom = this.contract.ContractStartDate;
      // this.tariff.DateTo = this.contract.ContractEndDate;
    }

  }

  setDefaultHijriDates() {
    let fromHijri = this._NgbCalendarIslamicUmalqura.fromGregorian(this.contract.ContractStartDate);
    let toHijri = this._NgbCalendarIslamicUmalqura.fromGregorian(this.contract.ContractEndDate);
    this.tariff.DateFromHijri = (fromHijri.year * 10000) + (fromHijri.month * 100) + fromHijri.day;
    this.tariff.DateToHijri = (toHijri.year * 10000) + (toHijri.month * 100) + toHijri.day;
  }


  onCubicMeterChargeChanged() {
    if (isNaN(+this.tariff.CubicMeterCharge) || +this.tariff.CubicMeterCharge < 0) {
      this.tariff.CubicMeterCharge = 0;
    }
  }

  onDistanceChargeChanged() {
    if (isNaN(+this.tariff.DistanceCharge) || +this.tariff.DistanceCharge < 0) {
      this.tariff.DistanceCharge = 0;
    }
  }

  onAfterFirstKMChanged() {
    if (isNaN(+this.tariff.AfterFirstKM) || +this.tariff.AfterFirstKM < 0) {
      this.tariff.AfterFirstKM = 0;
    }
  }



  uploadExcel() {

    this.modalRef = this.modalService.show(UploadTariffExcelComponent);

    // this.modalRef.content.confirm.subscribe(() => {
    //   this.modalRef.hide();
    // })

  }

  


}
