import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { LoaderService } from 'src/app/shared/loader.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { ConvertDateToExcel, isNullOrUndefined } from 'src/app/shared/utilities/utilities';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { OrderExcel } from 'src/app/TMS-Module/Models/OrderExcel';
import { Report_TankersPermissionsStatus } from 'src/app/TMS-Module/Models/report-tanker-P-S.model';
import { ReportTankersPermissionsStatusSC } from 'src/app/TMS-Module/Models/search-criteria/report-tankers-P-S-SC.model';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { ReportService } from 'src/app/TMS-Module/Services/report.service';

@Component({
  selector: 'app-tankers-permissions-status',
  templateUrl: './tankers-permissions-status.component.html',
  styleUrls: ['./tankers-permissions-status.component.scss']
})
export class TankersPermissionsStatusComponent implements OnInit {

  SearchCriteria: ReportTankersPermissionsStatusSC;

  selectMenuOptions2 = {
    //singleSelect: true
  };

  selectMenuOptions3 = {
    singleSelect: true
  };

  searchResult = new SearchResult<Report_TankersPermissionsStatus>();

  ordersexcel: OrderExcel[] = [];
  HoverExcel: boolean = false;
  pagePermission: string = "";
  IsArabic = false;

  //TargetStationId: string;

  constructor(
    private authServer: AuthenticationService,
    private lookupservice: LookupService,
    private alert: alertService,
    private _ExcelService: ExcelService,
    private _translate: TranslateService,
    private titleService: Title,
    private mainloading: LoaderService,
    private reportService: ReportService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName("tmsReports");
    this.authServer.checkFullControlPrivilege(this.pagePermission, true);

  }

  ngOnInit() {

    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this.IsArabic = (this._translate.currentLang == 'ar');
    this._translate.onLangChange.subscribe(res => {
      this.IsArabic = (res.lang == 'ar');
      this.loadDDLsGV();
    });

  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant("ReportTankersPermissionsStatus"));

    //this.searchCaller();

    // load Search DDls values
    //this.getServiceTypes();
    //this.getStatuses();
    this.getAreas("");
    this.getExpiryStatuses();

  }

  setDefaultSearchValues() {
    this.SearchCriteria = new ReportTankersPermissionsStatusSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    // this.SearchCriteria.LicenseExpiryDateFrom = new Date();
    // this.SearchCriteria.LicenseExpiryDateTo = new Date();

    // this.SearchCriteria.PermissionExpiryDateFrom = new Date();
    // this.SearchCriteria.PermissionExpiryDateTo = new Date();

    // redraw DDL selections
    //this.bindingModel_ServiceTypes = [];
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    //this.bindingModel_Zones = [];
    this.bindingModel_Stations = [];
    //this.bindingModel_Statuses = [];
    this.bindingModel_PermissionStatus = [];
    this.bindingModel_LicenseStatus = [];
  }


  //#region "Drop Down lists" *********************************************************************************
  SearchStream: SearchStream = new SearchStream();

  //customerServiceList: Lookup<number>[] = [];
  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  //customerZoneList: Lookup<number>[] = [];
  customerStationList: Lookup<string>[] = [];
  //CustomerStatusList: Lookup<number>[] = [];
  expiryStatusList: Lookup<number>[] = [];

  citySearchKeyWord = "";
  //zoneSearchKeyWord = "";
  stationSearchKeyword = "";

  Area_loading = false;
  City_Loading = false;
  //Zone_Loading = false;
  Station_Loading = false;

  //bindingModel_ServiceTypes: Lookup<number>[] = [];
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  //bindingModel_Zones: Lookup<number>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  //bindingModel_Statuses: Lookup<number>[] = [];
  bindingModel_PermissionStatus: Lookup<number>[] = [];
  bindingModel_LicenseStatus: Lookup<number>[] = [];


  //#region  "get Lookups"
  // getServiceTypes() {
  //   this.lookupservice.getPermittedServicesTypes().subscribe(res => {
  //     if (res.Value) {
  //       this.customerServiceList = res.Value;
  //     }

  //   });
  // }

  // getStatuses() {
  //   this.lookupservice.getWorkOrdersStatues().subscribe(res => {
  //     if (res.Value) this.CustomerStatusList = res.Value.filter(a => a.Id == 1 || a.Id == 2);

  //   });
  // }

  getExpiryStatuses() {
    this.expiryStatusList = [];

    let item1 = new Lookup<number>();
    item1.Id = 1;
    item1.Name = this._translate.instant('Valid');

    let item2 = new Lookup<number>();
    item2.Id = 2;
    item2.Name = this._translate.instant('Expired');

    this.expiryStatusList.push(item1);
    this.expiryStatusList.push(item2);
  }

  getAreas(searchKeyword: string) {
    this.SearchStream.initStream("AreaDDL_createOrder", a => {
      this.Area_loading = true;
      this.lookupservice.getAreasName(a).subscribe(
        res => {
          if (res.Value) this.customerAreaList = res.Value;
        },
        err => {
          this.Area_loading = false;
        },
        () => {
          this.Area_loading = false;
        }
      );
    }).next(searchKeyword);
  }

  //depends on areaIds
  getCity(searchKeyword: string) {
    this.citySearchKeyWord = searchKeyword;
    this.SearchStream.initStream("CityDDL_createOrder", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.AreaIDs) &&
        this.SearchCriteria.AreaIDs.length > 0
      ) {
        this.City_Loading = true;
        this.lookupservice
          .getCityName(a, this.SearchCriteria.AreaIDs)
          .subscribe(
            res => {
              if (res.Value) this.customerCityList = res.Value;
            },
            err => {
              this.City_Loading = false;
            },
            () => {
              this.City_Loading = false;
            }
          );
      } else {
        this.customerCityList = [];
      }
    }).next(searchKeyword);
  }

  //depends on cityIds
  // getZones(searchKeyword: string) {
  //   this.zoneSearchKeyWord = searchKeyword;
  //   this.SearchStream.initStream("ZoneDDL_createOrder", a => {
  //     if (
  //       !isNullOrUndefined(this.SearchCriteria.CityIDs) &&
  //       this.SearchCriteria.CityIDs.length > 0
  //     ) {
  //       this.Zone_Loading = true;
  //       this.lookupservice
  //         .getZoneName(a, this.SearchCriteria.CityIDs)
  //         .subscribe(
  //           res => {
  //             if (res.Value) this.customerZoneList = res.Value;
  //           },
  //           err => {
  //             this.Zone_Loading = false;
  //           },
  //           () => {
  //             this.Zone_Loading = false;
  //           }
  //         );
  //     } else {
  //       this.customerZoneList = [];
  //     }
  //   }).next(searchKeyword);
  // }

  //depends on cityIds
  getStations(searchKeyword: string) {
    this.stationSearchKeyword = searchKeyword;
    this.SearchStream.initStream("StationDDL_createOrder", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.CityIDs) && this.SearchCriteria.CityIDs.length > 0
        //!isNullOrUndefined(this.SearchCriteria.ZoneIDs) && this.SearchCriteria.ZoneIDs.length > 0
      ) {
        this.Station_Loading = true;
        this.lookupservice
          .GetStationBasedOnCity(a, this.SearchCriteria.CityIDs)
          //.getStationName(name, this.SearchCriteria.ZoneIDs)
          .subscribe(
            res => {
              if (res.Value) this.customerStationList = res.Value;
            },
            err => {
              this.Station_Loading = false;
            },
            () => {
              this.Station_Loading = false;
            }
          );
      } else {
        this.customerStationList = [];
      }
    }).next(searchKeyword);
  }


  //#endregion "get Lookups"

  //#region  "on Lookups change"

  // onServiceDDLChanged(evt) {
  //   this.SearchCriteria.ServiceTypeIDs = evt.map(m => m.Id);
  // }

  // onStatusDDLChanged(evt) {
  //   this.SearchCriteria.StatusIDs = evt.map(m => m.Id);
  // }

  onPermissionStatusDDLChanged(evt) {
    this.SearchCriteria.PermissionStatus = evt.map(m => m.Id)[0];
  }

  onLicenseStatusDDLChanged(evt) {
    this.SearchCriteria.LicenseStatus = evt.map(m => m.Id)[0];
  }

  onAreaDDLChanged(evt) {
    this.SearchCriteria.AreaIDs = evt.map(m => m.Id);
    this.getCity(this.citySearchKeyWord);
    this.bindingModel_Cities = [];
    this.SearchCriteria.CityIDs = [];

    // this.customerZoneList = [];
    // this.bindingModel_Zones = [];
    // this.SearchCriteria.ZoneIDs = [];

    this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];

  }

  onCityDDLChanged(evt) {
    this.SearchCriteria.CityIDs = evt.map(m => m.Id);
    // this.getZones(this.zoneSearchKeyWord);
    // this.bindingModel_Zones = [];
    // this.SearchCriteria.ZoneIDs = [];

    this.getStations(this.stationSearchKeyword);
    //this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];


  }

  // onZoneDDLChanged(evt) {
  //   this.SearchCriteria.ZoneIDs = evt.map(m => m.Id);
  // }

  onStationDDLChanged(evt) {
    this.SearchCriteria.StationIDs = evt.map(m => m.Id);
  }

  //#endregion "on Lookups change"

  //#endregion "Drop Down lists"

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

    if (this.SearchCriteria.AreaIDs && this.SearchCriteria.AreaIDs.length) {

      if (!this.SearchCriteria.CityIDs || !this.SearchCriteria.CityIDs.length) {
        validationMessages.push("ChooseCity");
      }

    }

    if (this.SearchCriteria.PermissionExpiryDateFrom && this.SearchCriteria.PermissionExpiryDateTo
      && this.SearchCriteria.PermissionExpiryDateFrom > this.SearchCriteria.PermissionExpiryDateTo) {
      validationMessages.push("PermissionExpiryDateFromMoreTo");
    }

    if (this.SearchCriteria.LicenseExpiryDateFrom && this.SearchCriteria.LicenseExpiryDateTo
      && this.SearchCriteria.LicenseExpiryDateFrom > this.SearchCriteria.LicenseExpiryDateTo) {
      validationMessages.push("LicenseExpiryDateFromMoreTo");
    }


    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  searchCaller() {
    if (!this.isValidModel()) return;

    let clonedObj = this.cloneSearchCriteriaObjectWithoutTimeZone();

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.reportService.GetTankerPermissionStatus(clonedObj).subscribe(
      res => {
        if (res.Value != null) {
          this.searchResult = res.Value;
        } else {
          this.searchResult.Result = [];
          this.searchResult.TotalCount = 0;
        }
      },
      err => {
        this.mainloading.PreloaderDecreaseCount();
      },
      () => {
        //this.tableLoading = false;
        this.mainloading.PreloaderDecreaseCount();
      }
    );
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"





  Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";

  onexportOrders() {
    if (!this.isValidModel()) return;

    let clonedObj = this.cloneSearchCriteriaObjectWithoutTimeZone();
    clonedObj.ExcelFlage = true;

    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.reportService.GetTankerPermissionStatus(clonedObj).subscribe(res => {
      if (res.IsErrorState == false) {
        if (
          isNullOrUndefined(res.Value.Result) ||
          res.Value.Result.length == 0
        ) {
          this.alert.error("NoDataFound");
          return;
        }

        let excelJson = res.Value.Result.map(value => {
          let r = {
            Vehicle: value.Code + ' | ' + value.PlateNo,
            Station: value.StationName,
            City: value.CityName,
            PermissionExpiryDate: ConvertDateToExcel(value.PermissionExpiryDate, false),
            PermissionStatus: value.PermissionStatus ? this._translate.instant(value.PermissionStatus) : '',
            LicenseExpiryDate: ConvertDateToExcel(value.licenseExpiryDate, false),
            LicenseStatus: value.LicenseStatus ? this._translate.instant(value.LicenseStatus) : ''
          }
          return r;
        });

        this._ExcelService.exportAsExcelFile(excelJson, this._translate.instant("ReportTankersPermissionsStatus"));
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




  cloneSearchCriteriaObjectWithoutTimeZone() {
    let clonedObj = Object.assign({}, this.SearchCriteria);

    //alert time zone offset before send
    if (this.SearchCriteria.LicenseExpiryDateFrom) {
      clonedObj.LicenseExpiryDateFrom = new Date(this.SearchCriteria.LicenseExpiryDateFrom.getTime());
      clonedObj.LicenseExpiryDateFrom.setMinutes(
        clonedObj.LicenseExpiryDateFrom.getMinutes() - clonedObj.LicenseExpiryDateFrom.getTimezoneOffset());
    }
    if (this.SearchCriteria.LicenseExpiryDateTo) {
      clonedObj.LicenseExpiryDateTo = new Date(this.SearchCriteria.LicenseExpiryDateTo.getTime());
      clonedObj.LicenseExpiryDateTo.setMinutes(
        clonedObj.LicenseExpiryDateTo.getMinutes() - clonedObj.LicenseExpiryDateTo.getTimezoneOffset());
    }

    if (this.SearchCriteria.PermissionExpiryDateFrom) {
      clonedObj.PermissionExpiryDateFrom = new Date(this.SearchCriteria.PermissionExpiryDateFrom.getTime());
      clonedObj.PermissionExpiryDateFrom.setMinutes(
        clonedObj.PermissionExpiryDateFrom.getMinutes() - clonedObj.PermissionExpiryDateFrom.getTimezoneOffset());
    }
    if (this.SearchCriteria.PermissionExpiryDateTo) {
      clonedObj.PermissionExpiryDateTo = new Date(this.SearchCriteria.PermissionExpiryDateTo.getTime());
      clonedObj.PermissionExpiryDateTo.setMinutes(
        clonedObj.PermissionExpiryDateTo.getMinutes() - clonedObj.PermissionExpiryDateTo.getTimezoneOffset());
    }
    return clonedObj;
  }


}
