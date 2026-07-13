import { Component, OnInit } from '@angular/core';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { SoqyaScheduleReportDTO } from 'src/app/TMS-Module/Models/soqya-schedule-report.model';
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
import { Operator, SoqyaScheduleReportSC } from 'src/app/TMS-Module/Models/search-criteria/soqya-schecule-report-SC.model';
import { SoqyaService } from 'src/app/TMS-Module/Services/soqya.service';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-soqya-schedule-report',
  templateUrl: './soqya-schedule-report.component.html',
  styleUrls: ['./soqya-schedule-report.component.scss']
})
export class SoqyaScheduleReportComponent implements OnInit {

  advancedDiv = false;
  pagePermission: string = '';

  SearchCriteria: SoqyaScheduleReportSC;
  searchResult = new SearchResult<SoqyaScheduleReportDTO>();


  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private soqyaService: SoqyaService,
    private _alertservice: alertService,
    private mainloading: LoaderService,
    private _ExcelService: ExcelService,
    private _translate: TranslateService,
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsReports');
    this.authenticationService.checkFullControlPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.setDefaultSearchValues();
    this.getAreas();
    this.getBalanceOperator();
    this.getNotScheduledOperator();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });

  }

  load() {
    this.getScheduleStatuses();

    this.titleService.setTitle(this.translateService.instant('SoqyaCustomerBalanceReport'));
  }


  //#region  "For search"
  SearchStream: SearchStream = new SearchStream();

  customerNameList: Lookup<number>[] = [];
  customerAccountsList: Lookup<number>[] = [];
  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerZoneList: Lookup<number>[] = [];
  scheduleStatusList: Lookup<number>[] = [];
  NSQ_OperatorList : Lookup<number>[] = [];
  Balance_OperatorList : Lookup<number>[] = [];

  bindingModel_CustomerNames: Lookup<number>[] = [];
  bindingModel_CustomerAccounts: Lookup<number>[] = [];
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];
  bindingModel_scheduleStatuses: Lookup<number>[] = [];

  citySearchKeyWord = '';
  zoneSearchKeyWord = '';

  Customer_Loading = false;
  CustomerAccount_Loading = false;
  Area_loading = false;
  City_Loading = false;
  Zone_Loading = false;

  selectMenuOptions = {
    enableSearchFilter: true,
  };

  selectMenuOptions2 = {
    enableSearchFilter: true,
    singleSelect: true
  };

  setDefaultSearchValues() {
    this.SearchCriteria = new SoqyaScheduleReportSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    this.SearchCriteria.DateFrom = new Date();
    this.SearchCriteria.DateFrom.setDate(1);
    this.SearchCriteria.DateFrom.setMonth(this.SearchCriteria.DateFrom.getMonth() - 1);
    this.SearchCriteria.DateTo = new Date();
    this.SearchCriteria.DateTo.setDate(2);

    // redraw DDL selections
    this.bindingModel_CustomerNames = [];
    this.bindingModel_CustomerAccounts = [];
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Zones = [];

    this.customerNameList = [];
    this.customerAccountsList = [];
    this.customerCityList = [];
    this.customerZoneList = [];
    this.bindingModel_scheduleStatuses = [];

  }

  getCustomers(searchKeyword: string) {
    this.SearchStream.initStream("CustomerDDL_createOrder", (a) => {
      this.SearchCriteria.CustomerId = null;
      this.SearchCriteria.CustomerAccountIDs = [];
      this.customerAccountsList = [];
      this.bindingModel_CustomerAccounts = [];

      this.Customer_Loading = true;
      this.lookupservice.SearchSoqyaCustomers(a).subscribe(res => {
        if (res.Value)
          this.customerNameList = res.Value;
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }

  getScheduleStatuses() {
    this.scheduleStatusList = [];

    let item1 = new Lookup<number>();
    item1.Id = 1;
    item1.Name = this.translateService.instant('FullScheduled');

    let item2 = new Lookup<number>();
    item2.Id = 2;
    item2.Name = this.translateService.instant('PartiallyScheduled');

    let item3 = new Lookup<number>();
    item3.Id = 3;
    item3.Name = this.translateService.instant('NotScheduled');

    this.scheduleStatusList.push(item1);
    this.scheduleStatusList.push(item2);
    this.scheduleStatusList.push(item3);
  }

  getAreas(searchKeyword: string = '') {
    this.SearchStream.initStream("AreaDDL_stationSettings", (a) => {
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
    this.SearchStream.initStream("CityDDL_stationSettings", (a) => {
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

  getZones(searchKeyword: string) {
    this.zoneSearchKeyWord = searchKeyword;
    this.SearchStream.initStream("ZoneDDL_createOrder", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.CityIDs) &&
        this.SearchCriteria.CityIDs.length > 0
      ) {
        this.Zone_Loading = true;
        this.lookupservice
          .getZoneName(a, this.SearchCriteria.CityIDs)
          .subscribe(
            res => {
              if (res.Value) this.customerZoneList = res.Value;
            },
            err => {
              this.Zone_Loading = false;
            },
            () => {
              this.Zone_Loading = false;
            }
          );
      } else {
        this.customerZoneList = [];
      }
    }).next(searchKeyword);
  }

  getOperators() {
    let OperatorList : Lookup<number>[] = [];

    let item1 =  new Lookup<number>();
    item1.Id = Operator.Equal;
    item1.Name = this._translate.instant('Equal');

    let item2 =  new Lookup<number>();
    item2.Id = Operator.LessThan;
    item2.Name = this._translate.instant('LessThan');

    let item3 =  new Lookup<number>();
    item3.Id = Operator.MoreThan;
    item3.Name = this._translate.instant('MoreThan');

    OperatorList.push(item1);
    OperatorList.push(item2);
    OperatorList.push(item3);
    return OperatorList;
  }

  getBalanceOperator(){
    this.Balance_OperatorList = this.getOperators();
  }

  getNotScheduledOperator(){
    this.NSQ_OperatorList = this.getOperators();
  }

  onCustomerDDLChanged(evt) {
    this.SearchCriteria.CustomerId = evt.map(m => m.Id)[0];
    this.SearchCriteria.CustomerAccountIDs = [];
    this.bindingModel_CustomerAccounts = [];

    if (!isNullOrUndefined(this.SearchCriteria.CustomerId)) {
      this.CustomerAccount_Loading = true;
      this.lookupservice.GetCustomerAccounts(this.SearchCriteria.CustomerId).subscribe(res => {
        if (res.Value != null && res.Value.length > 0)
          this.customerAccountsList = res.Value;
        else
          this.customerAccountsList = [];

      }
        , err => {
          this.CustomerAccount_Loading = false;
        }
        , () => {
          this.CustomerAccount_Loading = false;
        });
    }
    else {
      this.customerAccountsList = [];
    }

  }

  onCustomerAccountDDLChanged(evt) {
    this.SearchCriteria.CustomerAccountIDs = evt.map(m => m.Id);
  }

  onAreaDDLChanged(evt) {
    this.SearchCriteria.AreaIDs = evt.map(m => m.Id);
    this.getCity(this.citySearchKeyWord);
    this.bindingModel_Cities = [];
    this.SearchCriteria.CityIDs = [];

    this.customerZoneList = [];
    this.bindingModel_Zones = [];
    this.SearchCriteria.ZoneIDs = [];
  }

  onCityDDLChanged(evt) {
    this.SearchCriteria.CityIDs = evt.map(m => m.Id);

    this.getZones(this.zoneSearchKeyWord);
    this.customerZoneList = [];
    this.bindingModel_Zones = [];
    this.SearchCriteria.ZoneIDs = [];
  }

  onZoneDDLChanged(evt) {
    this.SearchCriteria.ZoneIDs = evt.map(m => m.Id);
  }

  onScheduleStatusDDLChanged(evt) {
    this.SearchCriteria.ScheduleStatus = evt.map(m => m.Id);
  }

  onBalanceOperatorDDLChanged(evt){
    this.SearchCriteria.B_Operator = evt.map(m => m.Id)[0];
  }

  onNSQOperatorDDLChanged(evt){
    this.SearchCriteria.NotScheduled_Operator = evt.map(m => m.Id)[0];
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

    if (isNullOrUndefined(this.SearchCriteria.DateFrom)) {
      validationMessages.push("DateFromRequired");
    }

    if (isNullOrUndefined(this.SearchCriteria.DateTo)) {
      validationMessages.push("DateToRequired");
    }

    if (
      !isNullOrUndefined(this.SearchCriteria.DateFrom) &&
      !isNullOrUndefined(this.SearchCriteria.DateTo) &&
      this.SearchCriteria.DateTo < this.SearchCriteria.DateFrom
    ) {
      validationMessages.push("DateTimeFromMustBeBeforDateTimeTo");
    }


    if (isNullOrUndefined(this.SearchCriteria.AreaIDs) || this.SearchCriteria.AreaIDs.length < 1) {
      // validationMessages.push("DashboardSelectArea");
    } else if (isNullOrUndefined(this.SearchCriteria.CityIDs) || this.SearchCriteria.CityIDs.length < 1) {
      validationMessages.push("ChooseCity");
    }
    // else if (isNullOrUndefined(this.SearchCriteria.StationIDs) || this.SearchCriteria.StationIDs.length < 1) {
    //   validationMessages.push("StationIsRequired");
    // }


    if (validationMessages.length > 0) {
      this._alertservice.errorList(validationMessages);
      return false;
    }
    return true;
  }


  searchCaller() {
    if (!this.isValidModel()) return;

    //alert time zone offset before send
    let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    modifiedCriteria.DateFrom = new Date(this.SearchCriteria.DateFrom.getTime());
    modifiedCriteria.DateFrom.setMinutes(
      modifiedCriteria.DateFrom.getMinutes() - modifiedCriteria.DateFrom.getTimezoneOffset());
    modifiedCriteria.DateTo = new Date(this.SearchCriteria.DateTo.getTime());
    modifiedCriteria.DateTo.setMinutes(
      modifiedCriteria.DateTo.getMinutes() - modifiedCriteria.DateTo.getTimezoneOffset()
    );

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.soqyaService.GetSoqyaSchedulesReport(modifiedCriteria).subscribe(res => {
      if (res.Value != null) {
        this.searchResult = res.Value;

        if (this.searchResult.Result && this.searchResult.Result.length > 0) {
          this.searchResult.Result.forEach(x => {
            if (x.Year != null && x.Year > 0) {
              x.helper_scheduleDate = new Date();
              x.helper_scheduleDate.setFullYear(x.Year);
              x.helper_scheduleDate.setMonth(x.Month - 1);
            }
          })
        }

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

    let clonedObj = Object.assign({}, this.SearchCriteria);

    clonedObj.ExcelFlage = true;
    //alert time zone offset before send
    //let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    clonedObj.DateFrom = new Date(this.SearchCriteria.DateFrom.getTime());
    clonedObj.DateFrom.setMinutes(
      clonedObj.DateFrom.getMinutes() - clonedObj.DateFrom.getTimezoneOffset());
    clonedObj.DateTo = new Date(this.SearchCriteria.DateTo.getTime());
    clonedObj.DateTo.setMinutes(
      clonedObj.DateTo.getMinutes() - clonedObj.DateTo.getTimezoneOffset()
    );

    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.soqyaService.GetSoqyaSchedulesReport(clonedObj)
      .subscribe(res => {
        if (res.IsErrorState == false) {
          if (
            isNullOrUndefined(res.Value.Result) ||
            res.Value.Result.length == 0
          ) {
            this._alertservice.error("NoDataFound");
            return;
          }

          let excelJson = res.Value.Result.map(value => {
            let r = {
              customer: value.CustomerName,
              City: value.CityName,
              Zone: value.ZoneName,
              Address: value.CustomerLocationAddress,
              "Month/Year": `${value.Year}-${value.Month}`,
              BalanceM3: value.SoqyaBalance,
              ScheduledQTYM3: value.ScheduledSum,
              DeliveredQTYM3: value.DeliveredSum,
              FailedToDeliveredQTYM3: value.NotDeliveredSum,
              CanceledQTYM3: value.CancelledSum,
              RemainingM3: value.RemainingQty
            }
            return r;
          });

          this._ExcelService.exportAsExcelFile(excelJson, this.translateService.instant("SoqyaCustomerBalanceReport"));
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



}
