import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { LoaderService } from 'src/app/shared/loader.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { isNullOrUndefined } from 'src/app/shared/utilities/utilities';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { OrderExcel } from 'src/app/TMS-Module/Models/OrderExcel';
import { ReportSC } from 'src/app/TMS-Module/Models/search-criteria/report-SC.model';
import { contractTariffReport } from 'src/app/TMS-Module/Models/search-criteria/contract-tariff-report';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { ReportService } from 'src/app/TMS-Module/Services/report.service';
import { ContractTariff } from '../../../Models/contract-tariff';

@Component({
  selector: 'app-orders-per-zone',
  templateUrl: './contracts-tariffs-component.html',
  styleUrls: ['./contracts-tariffs-component.scss']
})
export class ContractsTariffsComponent implements OnInit {

  SearchCriteria: contractTariffReport;

  selectMenuOptionsMulti = {
    enableSearchFilter: true,
  };

  selectMenuOptionsSingle= {
    enableSearchFilter: true,
    singleSelect: true
  };
  searchResult = new SearchResult<ContractTariff>();

  ordersexcel: OrderExcel[] = [];
  HoverExcel: boolean = false;
  pagePermission: string = "";

  //TargetStationId: string;

  constructor(
    private authServer: AuthenticationService,
    private translateService: TranslateService,
    //private workOrderService: WorkOrderSearchService,
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
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });

  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant("ReportOrdersPerZone"));

    //this.searchCaller();

    // load Search DDls values
    //this.getServiceTypes();
    //this.getStatuses();
    this.getAreas("");
    this.getCapacities("");
    this.getScheduleStatuses();
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new contractTariffReport();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;



    // redraw DDL selections
    //this.bindingModel_ServiceTypes = [];
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Zones = [];
    this.bindingModel_Stations = [];
    //this.bindingModel_Statuses = [];
  }


  //#region "Drop Down lists" *********************************************************************************
  SearchStream: SearchStream = new SearchStream();

  //customerServiceList: Lookup<number>[] = [];
  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerZoneList: Lookup<number>[] = [];
  customerStationList: Lookup<string>[] = [];
  CapacitiesList: Lookup<number>[] = [];
  tariffStatusList: Lookup<number>[] = [];
  //CustomerStatusList: Lookup<number>[] = [];

  citySearchKeyWord = "";
  zoneSearchKeyWord = "";
  stationSearchKeyword = "";

  Area_loading = false;
  City_Loading = false;
  Zone_Loading = false;
  Station_Loading = false;
  Capacities_Loading = false;

  //bindingModel_ServiceTypes: Lookup<number>[] = [];
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_Capacities: Lookup<number>[] = [];
  //bindingModel_Statuses: Lookup<number>[] = [];

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

  getCapacities(searchKeyword: string) {

    this.SearchStream.initStream("CapacityDDL_createOrder", a => {

      this.Capacities_Loading = true;
      this.lookupservice.GetTanckerCapacities().subscribe(
        res => {
          if (res.Value) this.CapacitiesList = res.Value;

        },
        err => {
          this.Capacities_Loading = false;
        },
        () => {
          this.Capacities_Loading = false;
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

  getScheduleStatuses() {
    this.tariffStatusList = [];

    let item1 = new Lookup<number>();
    item1.Id = 1;
    item1.Name = this.translateService.instant('All');

    let item2 = new Lookup<number>();
    item2.Id = 2;
    item2.Name = this.translateService.instant('OnlyCurrent');

    let item3 = new Lookup<number>();
    item3.Id = 3;
    item3.Name = this.translateService.instant('OnlyFinished');

    this.tariffStatusList.push(item1);
    this.tariffStatusList.push(item2);
    this.tariffStatusList.push(item3);

  }
  //#endregion "get Lookups"

  //#region  "on Lookups change"

  // onServiceDDLChanged(evt) {
  //   this.SearchCriteria.ServiceTypeIDs = evt.map(m => m.Id);
  // }

  // onStatusDDLChanged(evt) {
  //   this.SearchCriteria.StatusIDs = evt.map(m => m.Id);
  // }

  onAreaDDLChanged(evt) {
    this.SearchCriteria.AreaIDs = evt.map(m => m.Id);
    this.getCity(this.citySearchKeyWord);
    this.bindingModel_Cities = [];
    this.SearchCriteria.CityIDs = [];

    this.customerZoneList = [];
    this.bindingModel_Zones = [];
    this.SearchCriteria.ZoneIDs = [];

    this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];

  }

  onCityDDLChanged(evt) {
    this.SearchCriteria.CityIDs = evt.map(m => m.Id);
    this.getZones(this.zoneSearchKeyWord);
    this.bindingModel_Zones = [];
    this.SearchCriteria.ZoneIDs = [];

    this.getStations(this.stationSearchKeyword);
    //this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];


  }

  onZoneDDLChanged(evt) {
    this.SearchCriteria.ZoneIDs = evt.map(m => m.Id);
  }

  onStationDDLChanged(evt) {
    this.SearchCriteria.StationIDs = evt.map(m => m.Id);
  }
  onCapacityDDLChanged(evt) {
    this.SearchCriteria.TankerCapacities = evt.map(m => m.Id);
  }
  onTariffStatusDDLChanged(evt) {
    this.SearchCriteria.TarrifStatus = evt.map(m => m.Id)[0];

  }

  // onTargetStationDDLChanged(evt) {
  //   this.TargetStationId = evt.map(m => m.Id)[0];
  // }

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

    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  searchCaller() {
    if (!this.isValidModel()) return;

    let clonedObj = Object.assign({}, this.SearchCriteria);

    //alert time zone offset before send
    //let modifiedCriteria = Object.assign({}, this.SearchCriteria);



    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.reportService.ContractTariffReport(clonedObj).subscribe(
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
    let clonedObj = Object.assign({}, this.SearchCriteria);
    clonedObj.ExcelFlage = true;




    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.reportService.ContractTariffReport(clonedObj).subscribe(res => {
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
            City: value.CityName,
            Zone: value.ZoneName,
            Station: value.StationName,
            TotalOrdersCount: value.TanckerCapacityId,
            CubicMeterCharge: value.CubicMeterCharge,
            DistanceCharge: value.DistanceCharge,
            AfterFirstKM: value.AfterFirstKM,
            From: value.DateFromHijri,
            To: value.DateToHijri
          }
          return r;
        });

        this._ExcelService.exportAsExcelFile(excelJson, this._translate.instant("ReportOrdersPerZone"));
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
