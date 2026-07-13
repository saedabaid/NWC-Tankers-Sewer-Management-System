import { Component, OnInit, OnDestroy } from '@angular/core';
import { LoaderService } from 'src/app/shared/loader.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { DailyOrderReportSC } from 'src/app/TMS-Module/Models/search-criteria/daily-order-report-SC.model';
import { DailyOrderSummary, DailyOrderSummaryExcel } from 'src/app/TMS-Module/Models/daily-order-summary.model';
import { WorkOrderSearchService } from 'src/app/TMS-Module/Services/work-order-search.service';
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';

@Component({
  selector: 'app-daily-orders-summary',
  templateUrl: './daily-orders-summary.component.html',
  styleUrls: ['./daily-orders-summary.component.scss']
})
export class DailyOrdersSummaryComponent implements OnInit, OnDestroy {

  advancedDiv = false;
  pagePermission: string = '';
  tableLoading = false;

  SearchCriteria: DailyOrderReportSC;
  searchResult = new SearchResult<DailyOrderSummary>();


  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private workOrderService: WorkOrderSearchService,
    private _alertservice: alertService,
    private mainloading: LoaderService,
    private _ExcelService: ExcelService

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsReports');
    this.authenticationService.checkFullControlPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.setDefaultSearchValues();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.getAreas();
    this.getServiceTypes();

    this.titleService.setTitle(this.translateService.instant('DailyOrdersSummary'));
  }


  //#region  "For search"
  SearchStream: SearchStream = new SearchStream();

  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerStationList: Lookup<string>[] = [];
  customerServiceList: Lookup<number>[] = [];

  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_ServiceTypes: Lookup<number>[] = [];

  citySearchKeyWord = '';
  stationSearchKeyword = '';

  Area_loading = false;
  City_Loading = false;
  Station_Loading = false;

  selectMenuOptions = {
    enableSearchFilter: true,
  };

  setDefaultSearchValues() {
    this.SearchCriteria = new DailyOrderReportSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    this.SearchCriteria.DateFrom = new Date();
    this.SearchCriteria.DateTo = new Date();

    // redraw DDL selections
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Stations = [];
    this.bindingModel_ServiceTypes = [];

    this.customerCityList = [];
    this.customerStationList = [];

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

  getStations(searchKeyword: string) {
    this.stationSearchKeyword = searchKeyword;
    this.SearchStream.initStream("StationDDL_stationSettings", (a) => {
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

  getServiceTypes() {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value) this.customerServiceList = res.Value;
    });
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

  onServiceTypeDDLChanged(evt) {
    this.SearchCriteria.ServiceTypeIDs = evt.map(m => m.Id);
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
    else if (isNullOrUndefined(this.SearchCriteria.StationIDs) || this.SearchCriteria.StationIDs.length < 1) {
      validationMessages.push("StationIsRequired");
    }

    // if (isNullOrUndefined(this.SearchCriteria.ServiceTypeID) || +this.SearchCriteria.ServiceTypeID < 1) {
    //   validationMessages.push("DashboardSelectServiceType");
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
    this.workOrderService.GetDailyOrderSummaryReport(modifiedCriteria).subscribe(res => {
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
    this.workOrderService.GetDailyOrderSummaryReport(clonedObj)
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
            let r = new DailyOrderSummaryExcel();
            r.Station = value.StationName;
            r.ServiceType = value.ServiceTypeName;
            r.Date = value.CreateDate.toString().substring(0, 10);
            r.TotalOrdersCount = value.TotalCount;
            r.TotalOrdersSum = value.TotalSum;
            r.DeliveredCount = value.DeliveredCount;
            r.DeliveredSum = value.DeliveredSum;
            r.FailedToDeliverCount = value.FailedToDeliverCount;
            r.FailedToDeliverSum = value.FailedToDeliverSum;
            r.CancelledCount = value.CancelledCount;
            return r;
          });

          this._ExcelService.exportAsExcelFile(excelJson, this.translateService.instant("DailyOrdersSummary"));
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
