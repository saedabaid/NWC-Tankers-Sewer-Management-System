import { Component, OnInit } from '@angular/core';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { GateService } from 'src/app/TMS-Module/Services/gate.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { VehicleDataReportSC } from 'src/app/TMS-Module/Models/search-criteria/vehicle-data-report-SC.model';
import { VehicleData } from 'src/app/TMS-Module/Models/vehicle-data.model';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";

@Component({
  selector: 'app-vehicle-data',
  templateUrl: './vehicle-data.component.html',
  styleUrls: ['./vehicle-data.component.scss']
})
export class VehicleDataComponent implements OnInit {

  advancedDiv = false;
  pagePermission: string = '';
  tableLoading = false;

  SearchCriteria: VehicleDataReportSC;
  searchResult = new SearchResult<VehicleData>();
  IsArabic = false;

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private _alertservice: alertService,
    private mainloading: LoaderService,
    private _ExcelService: ExcelService,
    private gateService: GateService

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsReports');
    this.authenticationService.checkFullControlPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.setDefaultSearchValues();

    this.getAreas();
    this.searchCaller();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.getServiceTypes();
    this.GetTransporterStatuses();

    this.IsArabic = (this.translateService.currentLang == 'ar');
    this.titleService.setTitle(this.translateService.instant('TankerDataReport'));
  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }

  //#region  "For search"
  SearchStream: SearchStream = new SearchStream();

  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerStationList: Lookup<string>[] = [];
  customerServiceList: Lookup<number>[] = [];
  TransporterStatuses: Lookup<number>[] = [];

  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_ServiceTypes: Lookup<number>[] = [];
  bindingModel_TransporterStatuses: Lookup<number>[] = [];

  citySearchKeyWord = '';
  stationSearchKeyword = '';

  // timeFromStr: string;
  // timeToStr: string;

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
    this.SearchCriteria = new VehicleDataReportSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    // this.SearchCriteria.DateTimeFrom = new Date();
    // this.SearchCriteria.DateTimeTo = new Date();

    // let startDate = new Date();
    // startDate.setDate(startDate.getDate() - 1);
    // this.SearchCriteria.DateTimeFrom = startDate;
    // this.timeFromStr = startDate.toTimeString().substring(0, 5);

    // let endDate = new Date();
    // endDate.setDate(endDate.getDate() + 1);
    // endDate.setHours(0);
    // endDate.setMinutes(0);
    // endDate.setSeconds(0);
    // this.SearchCriteria.DateTimeTo = endDate;
    // this.timeToStr = endDate.toTimeString().substring(0, 5);

    // redraw DDL selections
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Stations = [];
    this.bindingModel_ServiceTypes = [];
    this.bindingModel_TransporterStatuses = [];

    this.customerCityList = [];
    this.customerStationList = [];

  }

  getAreas(searchKeyword: string = '') {
    this.SearchStream.initStream("AreaDDL_ReportVehicleData", (a) => {
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
    this.SearchStream.initStream("CityDDL_ReportVehicleData", (a) => {
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
    this.SearchStream.initStream("StationDDL_ReportVehicleData", (a) => {
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

  GetTransporterStatuses() {
    this.lookupservice.GetTransporterStatuses().subscribe(res => {
      if (res.Value) this.TransporterStatuses = res.Value;
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

  onTransporterStatusDDLChanged(evt) {
    this.SearchCriteria.StatusIDs = evt.map(m => m.Id);
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

    // if (isNullOrUndefined(this.SearchCriteria.DateTimeFrom)) {
    //   validationMessages.push("DateFromRequired");
    // }

    // if (isNullOrUndefined(this.SearchCriteria.DateTimeTo)) {
    //   validationMessages.push("DateToRequired");
    // }

    // let timeFrom = this.timeFromStr.split(":");
    // if (
    //   timeFrom.length !== 2 ||
    //   isNullOrUndefined(timeFrom[0]) || timeFrom[0] === "" ||
    //   isNullOrUndefined(timeFrom[1]) || timeFrom[1] === ""
    // ) {
    //   validationMessages.push("TimeFromRequired");
    // }

    // let timeTo = this.timeToStr.split(":");
    // if (
    //   timeTo.length !== 2 ||
    //   isNullOrUndefined(timeTo[0]) || timeTo[0] === "" ||
    //   isNullOrUndefined(timeFrom[1]) || timeTo[1] === ""
    // ) {
    //   validationMessages.push("TimeToRequired");
    // }

    // if (
    //   !isNullOrUndefined(this.SearchCriteria.DateTimeFrom) &&
    //   !isNullOrUndefined(this.SearchCriteria.DateTimeTo) &&
    //   this.SearchCriteria.DateTimeTo < this.SearchCriteria.DateTimeFrom
    // ) {
    //   validationMessages.push("DateTimeFromMustBeBeforDateTimeTo");
    // }


    // if (isNullOrUndefined(this.SearchCriteria.AreaIDs) || this.SearchCriteria.AreaIDs.length < 1) {
    //   // validationMessages.push("DashboardSelectArea");
    // } else if (isNullOrUndefined(this.SearchCriteria.CityIDs) || this.SearchCriteria.CityIDs.length < 1) {
    //   validationMessages.push("ChooseCity");
    // }
    // else if (isNullOrUndefined(this.SearchCriteria.StationIDs) || this.SearchCriteria.StationIDs.length < 1) {
    //   validationMessages.push("StationIsRequired");
    // }

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
    //if (!this.isValidModel()) return;

    // let timeFrom = this.timeFromStr.split(":");
    // this.SearchCriteria.DateTimeFrom.setHours(+timeFrom[0]);
    // this.SearchCriteria.DateTimeFrom.setMinutes(+timeFrom[1]);

    // let timeTo = this.timeToStr.split(":");
    // this.SearchCriteria.DateTimeTo.setHours(+timeTo[0]);
    // this.SearchCriteria.DateTimeTo.setMinutes(+timeTo[1]);

    //alert time zone offset before send
    // let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    // modifiedCriteria.DateTimeFrom = new Date(this.SearchCriteria.DateTimeFrom.getTime());
    // modifiedCriteria.DateTimeFrom.setMinutes(
    //   modifiedCriteria.DateTimeFrom.getMinutes() - modifiedCriteria.DateTimeFrom.getTimezoneOffset());
    // modifiedCriteria.DateTimeTo = new Date(this.SearchCriteria.DateTimeTo.getTime());
    // modifiedCriteria.DateTimeTo.setMinutes(
    //   modifiedCriteria.DateTimeTo.getMinutes() - modifiedCriteria.DateTimeTo.getTimezoneOffset()
    // );

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.gateService.GetVehicleDataReport(this.SearchCriteria).subscribe(res => {
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

    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.gateService.GetVehicleDataReport(clonedObj)
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
              code: value.Code,
              VehiclePlateNo: value.PlateNo,
              CapacityM: value.Capacity,
              Area: value.AreaName,
              City: value.CityName,
              Station: value.StationName,
              ServiceType: (this.IsArabic ? value.ServiceTypeAR : value.ServiceTypeEN),

              Driver: value.DriverName,
              DriverCode: value.DriverCode,
              DriverMobile: value.DriverMobile,
              DriverPersonalId: value.DriverPersonalId,

              Manufacturer: (this.IsArabic ? value.ManufacturerNameAr : value.ManufacturerName),
              Brand: (this.IsArabic ? value.BrandNameAr : value.BrandName),
              Type: (this.IsArabic ? value.TypeNameAr : value.TypeName),
              ProductionYear: value.ProductionYearName,

              PermissionExpiryDate: this.ConvertDateToExcel(value.PermissionExpiryDate),
              ChassisNo: value.chassisNo,
              LicenseExpiryDate: this.ConvertDateToExcel(value.licenseExpiryDate),
              Status: (this.IsArabic ? value.StatusNameAr : value.StatusName),
              Class: (this.IsArabic ? value.ClassesAr : value.ClassesEn)
            }
            return r;
          });

          this._ExcelService.exportAsExcelFile(excelJson, this.translateService.instant("TankerDataReport"));
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

  ConvertDateToExcel(input: Date): string {
    if (!isNullOrUndefined(input)) {
      let date = input.toString().substring(0, 10);
      let time = input.toString().substring(11, 16);
      return date + ' ' + time;
    }
    return '';
  }

}
