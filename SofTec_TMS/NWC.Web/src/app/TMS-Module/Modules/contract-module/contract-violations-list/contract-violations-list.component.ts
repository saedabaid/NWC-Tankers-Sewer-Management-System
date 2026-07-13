import { Component, OnInit } from '@angular/core';
import { ContractViolationSC } from 'src/app/TMS-Module/Models/search-criteria/contract-violation-SC.model';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { ContractTermsViolationsDTo } from 'src/app/TMS-Module/Models/contract-terms-violations.model';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';

import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { Router } from '@angular/router';
import { DateUtilityService } from 'src/app/shared/Services/date-Utility.service';

@Component({
  selector: 'app-contract-violations-list',
  templateUrl: './contract-violations-list.component.html',
  styleUrls: ['./contract-violations-list.component.scss']
})
export class ContractViolationsListComponent implements OnInit {

  advancedDiv = false;
  pagePermission: string = '';
  tableLoading = false;

  SearchCriteria: ContractViolationSC;
  searchResult = new SearchResult<ContractTermsViolationsDTo>();
  IsArabic = false;

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,

    private _alertservice: alertService,
    private mainloading: LoaderService,
    private _ExcelService: ExcelService,
    private contractService: ContractService,
    private router: Router,
    private _DateUtilityService: DateUtilityService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsContractTermsViolations');
    this.authenticationService.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.setDefaultSearchValues();
    this.searchCaller();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.getAreas();
    // this.getActionLogTypes();
    this.getTermsCategories();
    this.getInvoiceStatusesList();
    this.getViolationStatusesList();
    //this.searchCaller();

    this.IsArabic = (this.translateService.currentLang == 'ar');
    this.titleService.setTitle(this.translateService.instant('TMS_contractTermsViolationslist'));
  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }

  //#region  "For search"
  SearchStream: SearchStream = new SearchStream();

  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerStationList: Lookup<string>[] = [];
  // ActionLogTypes: Lookup<number>[] = [];
  TermsCategoriesList: Lookup<number>[] = [];
  InvoiceStatusesList: Lookup<number>[] = [];
  ViolationStatusesList: Lookup<number>[] = [];

  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  // bindingModel_ActionLogTypes: Lookup<number>[] = [];
  bindingModel_TermsCategories: Lookup<number>[] = [];
  bindingModel_InvoiceStatuses: Lookup<number>[] = [];
  bindingModel_ViolationStatuses: Lookup<number>[] = [];

  citySearchKeyWord = '';
  stationSearchKeyword = '';
  show: boolean;
  timeFromStr: string;
  timeToStr: string;

  Area_loading = false;
  City_Loading = false;
  Station_Loading = false;

  selectMenuOptions = {
    enableSearchFilter: true,
  };

  selectMenuOptions2 = {
    singleSelect: true
  };

  setDefaultSearchValues() {
    this.SearchCriteria = new ContractViolationSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    // this.SearchCriteria.ViolationDateFrom = new Date();
    // this.SearchCriteria.ViolationDateTo = new Date();

    let startDate = new Date();
    startDate.setDate(startDate.getDate() - 1);
    this.SearchCriteria.ViolationDateFrom = startDate;
    this.timeFromStr = startDate.toTimeString().substring(0, 5);

    let endDate = new Date();
    endDate.setDate(endDate.getDate() + 1);
    endDate.setHours(0);
    endDate.setMinutes(0);
    endDate.setSeconds(0);
    this.SearchCriteria.ViolationDateTo = endDate;
    this.timeToStr = endDate.toTimeString().substring(0, 5);

    // redraw DDL selections
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Stations = [];
    // this.bindingModel_ActionLogTypes = [];
    this.bindingModel_TermsCategories = [];
    this.bindingModel_InvoiceStatuses = [];
    this.bindingModel_ViolationStatuses = [];

    this.customerCityList = [];
    this.customerStationList = [];

  }

  getAreas(searchKeyword: string = '') {
    this.SearchStream.initStream("AreaDDL_TankerMovementReport", (a) => {
      this.Area_loading = true;
      this.lookupservice.getAreasName(a).subscribe(res => {
        if (res.Value)
          this.customerAreaList = res.Value;
      }
        , err => {
          this.Area_loading = false;
        }
        , () => {
          this.Area_loading = false;
        });
    }).next(searchKeyword);
  };

  getCity(searchKeyword: string) {
    this.citySearchKeyWord = searchKeyword;
    this.SearchStream.initStream("CityDDL_TankerMovementReport", (a) => {
      if (!isNullOrUndefined(this.SearchCriteria.AreaIDs) && this.SearchCriteria.AreaIDs.length > 0) {
        this.City_Loading = true;
        this.lookupservice.getCityName(a, this.SearchCriteria.AreaIDs).subscribe(res => {
          if (res.Value)
            this.customerCityList = res.Value;
        }
          , err => {
            this.City_Loading = false;
          }
          , () => {
            this.City_Loading = false;
          });
      }
      else {
        this.customerCityList = [];
      }
    }).next(searchKeyword);
  };

  getStations(searchKeyword: string) {
    this.stationSearchKeyword = searchKeyword;
    this.SearchStream.initStream("StationDDL_TankerMovementReport", (a) => {
      if (!isNullOrUndefined(this.SearchCriteria.CityIDs) && this.SearchCriteria.CityIDs.length > 0) {
        this.Station_Loading = true;
        this.lookupservice.GetStationBasedOnCity(a, this.SearchCriteria.CityIDs).subscribe(res => {
          if (res.Value)
            this.customerStationList = res.Value;
        }
          , err => {
            this.Station_Loading = false;
          }
          , () => {
            this.Station_Loading = false;
          });
      }
      else {
        this.customerStationList = [];
      }
    }).next(searchKeyword);
  }

  // getStations(searchKeyword: string) {
  //   this.stationSearchKeyword = searchKeyword;
  //   this.SearchStream.initStream("StationDDL_TankerMovementReport", (a) => {
  //     //if (!isNullOrUndefined(this.SearchCriteria.CityIDs) && this.SearchCriteria.CityIDs.length > 0) {
  //     this.Station_Loading = true;
  //     this.lookupservice.SearchPermittedStations(a).subscribe(res => {
  //       if (res.Value)
  //         this.customerStationList = res.Value;
  //     }
  //       , err => {
  //         this.Station_Loading = false;
  //       }
  //       , () => {
  //         this.Station_Loading = false;
  //       });
  //     // }
  //     // else {
  //     //   this.customerStationList = [];
  //     // }
  //   }).next(searchKeyword);
  // }

  getTermsCategories() {
    this.lookupservice.GetTermsCategory().subscribe(res => {
      if (res.Value) this.TermsCategoriesList = res.Value;
    })
  }
  getInvoiceStatusesList() {
    this.lookupservice.GetOrderInvoiceStatuses().subscribe(res => {
      if (res.Value) this.InvoiceStatusesList = res.Value;
    })
  }

  getViolationStatusesList() {
    this.lookupservice.GetContractTermsViolationStatuses().subscribe(res => {
      if (res.Value) this.ViolationStatusesList = res.Value;
    })
  }

  onAreaDDLChanged(evt) {
    this.SearchCriteria.AreaIDs = evt.map(m => m.Id);
    this.getCity(this.citySearchKeyWord);
    this.bindingModel_Cities = [];
    this.SearchCriteria.CityIDs = [];

    this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];
  }

  onCityDDLChanged(evt) {
    this.SearchCriteria.CityIDs = evt.map(m => m.Id);
    this.getStations(this.stationSearchKeyword);

    this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];
  }

  onStationDDLChanged(evt) {
    this.SearchCriteria.StationIDs = evt.map(m => m.Id);
  }

  // onActionLogTypeDDLChanged(evt) {
  //   this.SearchCriteria.LogType = evt.map(m => m.Id)[0];
  // }
  onTermsCategoryDDLChanged(evt) {
    this.SearchCriteria.CategoryIds = evt.map(m => m.Id);
  }

  onInvoiceStatusDDLChanged(evt) {
    this.SearchCriteria.PaymentIds = evt.map(m => m.Id);
  }

  onViolationStatusDDLChanged(evt) {
    this.SearchCriteria.StatusIds = evt.map(m => m.Id);
  }

  //#endregion "For Search"


  //#region "table Pagination and Search"
  onSearchSubmit() {
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.PageFilter.PageIndex = evt;
    this.searchCaller();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.PageFilter.PageSize = evt;
    this.searchCaller();
  }

  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.SearchCriteria.AreaIDs) || this.SearchCriteria.AreaIDs.length < 1) {
      // validationMessages.push("DashboardSelectArea");
    } else if (isNullOrUndefined(this.SearchCriteria.CityIDs) || this.SearchCriteria.CityIDs.length < 1) {
      validationMessages.push("ChooseCity");
    }
    else if (isNullOrUndefined(this.SearchCriteria.StationIDs) || this.SearchCriteria.StationIDs.length < 1) {
      validationMessages.push("StationIsRequired");
    }

    if (isNullOrUndefined(this.SearchCriteria.ViolationDateFrom)) {
      validationMessages.push("DateFromRequired");
    }

    if (isNullOrUndefined(this.SearchCriteria.ViolationDateTo)) {
      validationMessages.push("DateToRequired");
    }

    let timeFrom = this.timeFromStr.split(":");
    if (
      timeFrom.length !== 2 ||
      isNullOrUndefined(timeFrom[0]) || timeFrom[0] === "" ||
      isNullOrUndefined(timeFrom[1]) || timeFrom[1] === ""
    ) {
      validationMessages.push("TimeFromRequired");
    }

    let timeTo = this.timeToStr.split(":");
    if (
      timeTo.length !== 2 ||
      isNullOrUndefined(timeTo[0]) || timeTo[0] === "" ||
      isNullOrUndefined(timeFrom[1]) || timeTo[1] === ""
    ) {
      validationMessages.push("TimeToRequired");
    }

    if (
      !isNullOrUndefined(this.SearchCriteria.ViolationDateFrom) &&
      !isNullOrUndefined(this.SearchCriteria.ViolationDateTo) &&
      this.SearchCriteria.ViolationDateTo < this.SearchCriteria.ViolationDateFrom
    ) {
      validationMessages.push("ViolationDateFromMustBeBeforViolationDateTo");
    }

    if (validationMessages.length > 0) {
      this._alertservice.errorList(validationMessages);
      return false;
    }
    return true;
  }


  searchCaller() {
    if (!this.isValidModel()) return;

    let timeFrom = this.timeFromStr.split(":");
    this.SearchCriteria.ViolationDateFrom.setHours(+timeFrom[0]);
    this.SearchCriteria.ViolationDateFrom.setMinutes(+timeFrom[1]);

    let timeTo = this.timeToStr.split(":");
    this.SearchCriteria.ViolationDateTo.setHours(+timeTo[0]);
    this.SearchCriteria.ViolationDateTo.setMinutes(+timeTo[1]);

    //alert time zone offset before send
    let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    modifiedCriteria.ViolationDateFrom = new Date(this.SearchCriteria.ViolationDateFrom.getTime());
    modifiedCriteria.ViolationDateFrom.setMinutes(
      modifiedCriteria.ViolationDateFrom.getMinutes() - modifiedCriteria.ViolationDateFrom.getTimezoneOffset());
    modifiedCriteria.ViolationDateTo = new Date(this.SearchCriteria.ViolationDateTo.getTime());
    modifiedCriteria.ViolationDateTo.setMinutes(
      modifiedCriteria.ViolationDateTo.getMinutes() - modifiedCriteria.ViolationDateTo.getTimezoneOffset()
    );

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.contractService.GetContractViolations(modifiedCriteria).subscribe(res => {
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
        this.mainloading.PreloaderDecreaseCount();
        //this.tableLoading = false;
      })
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"


  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }


  Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";
  HoverExcel: boolean = false;

  onexportOrders() {

    let timeFrom = this.timeFromStr.split(":");
    this.SearchCriteria.ViolationDateFrom.setHours(+timeFrom[0]);
    this.SearchCriteria.ViolationDateFrom.setMinutes(+timeFrom[1]);

    let timeTo = this.timeToStr.split(":");
    this.SearchCriteria.ViolationDateTo.setHours(+timeTo[0]);
    this.SearchCriteria.ViolationDateTo.setMinutes(+timeTo[1]);

    let clonedObj = Object.assign({}, this.SearchCriteria);

    clonedObj.ExcelFlage = true;
    //alert time zone offset before send
    //let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    clonedObj.ViolationDateFrom = new Date(this.SearchCriteria.ViolationDateFrom.getTime());
    clonedObj.ViolationDateFrom.setMinutes(
      clonedObj.ViolationDateFrom.getMinutes() - clonedObj.ViolationDateFrom.getTimezoneOffset());
    clonedObj.ViolationDateTo = new Date(this.SearchCriteria.ViolationDateTo.getTime());
    clonedObj.ViolationDateTo.setMinutes(
      clonedObj.ViolationDateTo.getMinutes() - clonedObj.ViolationDateTo.getTimezoneOffset()
    );

    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.contractService.GetContractViolations(clonedObj)
      .subscribe(res => {
        if (!res || res.IsErrorState == false) {
          if (
            isNullOrUndefined(res.Value.Result) ||
            res.Value.Result.length == 0
          ) {
            this._alertservice.error("NoDataFound");
            return;
          }

          let excelJson = res.Value.Result.map(value => {
            let r = {
              ViolationTicketNumber: value.Id,
              //ViolationTicketNumber: value.ViolationTicketNumber,
              contract: value.ContractCode,
              Station: value.StationName,
              violationCategory: (this.IsArabic ? value.CategoryAr : value.CategoryEn),
              termsNO: value.ContractTermCode,
              termsName: value.ContractTermName,
              Vehicle: value.VehicleCode ? `${value.VehicleCode} | ${value.VehiclePlateNo}` : '',
              Driver: value.DriverName,
              Total: value.TotalPenalty,
              IssueDate: this._DateUtilityService.getDateWithMonthName(value.IssueDate),
              IssueDateHijri: this._DateUtilityService.getIslamicUmalquraDate(value.IssueDate, false),
              //DueDate: this.ConvertDateToExcel(value.PaymentDueDate),
              PaymentStatus: (this.IsArabic ? value.PaymentStatusAr : value.PaymentStatusEn),
              PaidOn: this._DateUtilityService.getDateWithMonthName(value.PaymentStatusDate),
              PaidOnHijri: this._DateUtilityService.getIslamicUmalquraDate(value.PaymentStatusDate, false),
              Status: (this.IsArabic ? value.StatusNameAr : value.StatusNameEn),
              CancellationReason: (this.IsArabic ? value.CancelReasonAr : value.CancelReasonEn)

            }
            return r;
          });

          this._ExcelService.exportAsExcelFile(excelJson, this.translateService.instant("TMS_contractTermsViolationslist"));
        }
      }
        , err => {
          this.Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";
        }
        , () => {
          this.Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";
        });
  }

  mouseEnterExcel() {
    this.HoverExcel = true;
  }
  mouseLeaveExcel() {
    this.HoverExcel = false;
  }


  editViolation(violation: ContractTermsViolationsDTo) {
    if (this.authenticationService.checkAddEditPrivilege(this.pagePermission)) {
      this.router.navigate(['/tms/contract/violation/edit/' + violation.Id]);
    }
  }
  AddViolationDecision(violationId: number, Approval: Boolean)
  {
    this._alertservice.confirmationMessage("AddDecision").subscribe(confirm => {
      if (confirm == true) {
        this.mainloading.PreloaderIcreaseCount();

        this.contractService.AddViolationDecision(violationId, Approval).subscribe(res => {
          if (!res.IsErrorState) {
            this._alertservice.success("DecisionSuccessed");
            this.onSearchSubmit();
          }
          else {
            this._alertservice.errorList(res.Errors);
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
  deleteContract(violation: ContractTermsViolationsDTo) {
    if (!this.authenticationService.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }

    this._alertservice.confirmationMessage("ConfirmDelete").subscribe(confirm => {
      if (confirm == true) {
        this.mainloading.PreloaderIcreaseCount();
        this.contractService.DeleteContractViolation(violation.Id).subscribe(res => {
          if (!res.IsErrorState) {
            this._alertservice.success("DeletedSuccessed");
            this.onSearchSubmit();
          }
          else {
            this._alertservice.errorList(res.Errors);
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
