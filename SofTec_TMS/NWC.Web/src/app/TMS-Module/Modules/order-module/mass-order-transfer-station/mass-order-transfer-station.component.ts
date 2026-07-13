import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { LoaderService } from 'src/app/shared/loader.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { isNullOrUndefined } from 'src/app/shared/utilities/utilities';
import { FilterModel } from 'src/app/TMS-Module/Models/common/filter-model';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { OrderDetails } from 'src/app/TMS-Module/Models/order-details';
import { OrderExcel } from 'src/app/TMS-Module/Models/OrderExcel';
import { WorkOrderSearchCriteria } from 'src/app/TMS-Module/Models/search-criteria/work-order-search-criteria';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { WorkOrderSearchService } from 'src/app/TMS-Module/Services/work-order-search.service';

@Component({
  selector: 'app-mass-order-transfer-station',
  templateUrl: './mass-order-transfer-station.component.html',
  styleUrls: ['./mass-order-transfer-station.component.scss']
})
export class MassOrderTransferStationComponent implements OnInit {

  SearchCriteria: WorkOrderSearchCriteria;

  selectMenuOptions2 = {
    singleSelect: true,
    enableSearchFilter: true
  };

  selectMenuOptionsZone = {
    enableSearchFilter: true
  };
  searchResult = new SearchResult<OrderDetails>();

  ordersexcel: OrderExcel[] = [];
  HoverExcel: boolean = false;
  pagePermission: string = "";

  TargetStationId: string;

  constructor(
    private authServer: AuthenticationService,
    private workOrderService: WorkOrderSearchService,
    private lookupservice: LookupService,
    private alert: alertService,
    private ExcelService: ExcelService,
    private _translate: TranslateService,
    private titleService: Title,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName("tmsMassOrderTransfer");
    this.authServer.checkViewPrivilege(this.pagePermission, true);

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
    this.titleService.setTitle(this._translate.instant("MassOrderTransferStation"));

    //this.searchCaller();

    // load Search DDls values
    this.getServiceTypes();
    this.getStatuses();
    this.getAreas("");

  }

  setDefaultSearchValues() {
    this.SearchCriteria = new WorkOrderSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    // redraw DDL selections
    this.bindingModel_ServiceTypes = [];
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Zones = [];
    this.bindingModel_Stations = [];
    this.bindingModel_Statuses = [];
  }


  //#region "Drop Down lists" *********************************************************************************
  SearchStream: SearchStream = new SearchStream();

  customerServiceList: Lookup<number>[] = [];
  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerZoneList: Lookup<number>[] = [];
  customerStationList: Lookup<string>[] = [];
  CustomerStatusList: Lookup<number>[] = [];
  TargetStationList: Lookup<string>[] = [];

  citySearchKeyWord = "";
  zoneSearchKeyWord = "";
  stationSearchKeyword = "";
  targetStationSearchKeyword = "";

  Area_loading = false;
  City_Loading = false;
  Zone_Loading = false;
  Station_Loading = false;
  TargetStation_Loading = false;

  bindingModel_ServiceTypes: Lookup<number>[] = [];
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_Statuses: Lookup<number>[] = [];
  bindingModel_TargetStations: Lookup<string>[] = [];

  //#region  "get Lookups"
  getServiceTypes() {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value) {
        this.customerServiceList = res.Value;
      }

    });
  }

  getStatuses() {
    this.lookupservice.getWorkOrdersStatues().subscribe(res => {
      if (res.Value) this.CustomerStatusList = res.Value.filter(a => a.Id == 1 || a.Id == 2);

    });
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

  //depends on cityIds
  getTragetStations(searchKeyword: string) {
    this.targetStationSearchKeyword = searchKeyword;
    this.SearchStream.initStream("TargetStationDDL_createOrder", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.CityIDs) && this.SearchCriteria.CityIDs.length > 0
        //!isNullOrUndefined(this.SearchCriteria.ZoneIDs) && this.SearchCriteria.ZoneIDs.length > 0
      ) {
        this.TargetStation_Loading = true;
        this.lookupservice
          .GetStationBasedOnCity(a, this.SearchCriteria.CityIDs)
          //.getStationName(name, this.SearchCriteria.ZoneIDs)
          .subscribe(
            res => {
              if (res.Value) this.TargetStationList = res.Value;
            },
            err => {
              this.TargetStation_Loading = false;
            },
            () => {
              this.TargetStation_Loading = false;
            }
          );
      } else {
        this.TargetStationList = [];
      }
    }).next(searchKeyword);
  }


  //#endregion "get Lookups"

  //#region  "on Lookups change"

  onServiceDDLChanged(evt) {
    this.SearchCriteria.ServiceTypeIDs = evt.map(m => m.Id);
  }

  onStatusDDLChanged(evt) {
    this.SearchCriteria.StatusIDs = evt.map(m => m.Id);
  }

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

    this.TargetStationList = [];
    this.bindingModel_TargetStations = [];
    this.TargetStationId = "";

  }

  onCityDDLChanged(evt) {
    this.SearchCriteria.CityIDs = evt.map(m => m.Id);
    this.getZones(this.zoneSearchKeyWord);
    this.bindingModel_Zones = [];
    this.SearchCriteria.ZoneIDs = [];

    this.getStations(this.stationSearchKeyword);
    this.getTragetStations(this.targetStationSearchKeyword);
    //this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];

    this.bindingModel_TargetStations = [];
    this.TargetStationId = "";

  }

  onZoneDDLChanged(evt) {
    this.SearchCriteria.ZoneIDs = evt.map(m => m.Id);
  }

  onStationDDLChanged(evt) {
    this.SearchCriteria.StationIDs = evt.map(m => m.Id);
  }

  onTargetStationDDLChanged(evt) {
    this.TargetStationId = evt.map(m => m.Id)[0];
  }

  //#endregion "on Lookups change"

  //#endregion "Drop Down lists"

  //#region "table Pagination and Search"
  onSearchSubmit() {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = evt;
    this.searchCaller();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageSize = evt;
    this.searchCaller();
  }

  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (!this.SearchCriteria.AreaIDs || !this.SearchCriteria.AreaIDs.length) {
      validationMessages.push("ChooseArea");
    }

    if (!this.SearchCriteria.CityIDs || !this.SearchCriteria.CityIDs.length) {
      validationMessages.push("ChooseCity");
    }

    if (!this.SearchCriteria.StationIDs || !this.SearchCriteria.StationIDs.length) {
      validationMessages.push("ChooseStation");
    }

    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  searchCaller() {
    if (!this.isValidModel()) return;

    this.SearchCriteria.StatusIDs = this.SearchCriteria.StatusIDs
      ? this.SearchCriteria.StatusIDs.filter(a => a == 1 || a == 2) : [1, 2];

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.workOrderService.searchOrdersList(this.SearchCriteria).subscribe(
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
    this.searchResult = new SearchResult<OrderDetails>();
    //this.searchCaller();
  }

  //#endregion "table Pagination and Search"


  transferOrders() {

    if (!this.isValidModel()) return;

    if(!this.TargetStationId) {
      this.alert.error("TargetStationRequired");
      return;
    }

    this.SearchCriteria.StatusIDs = this.SearchCriteria.StatusIDs
      ? this.SearchCriteria.StatusIDs.filter(a => a == 1 || a == 2) : [1, 2];

    this.mainloading.PreloaderIcreaseCount();
    this.workOrderService.UpdateWorkOrdersStation(this.TargetStationId, this.SearchCriteria).subscribe(response => {

      if (response.IsErrorState === true) {
        this.alert.errorList(response.Errors);
      } else {

        if (response.Value.success && response.Value.failed) {
          this.alert.warning("NoOrdersChanged");
        }
        else {
          this.alert.showSuccess();
        }

        if (response.Value.failed) {
          let msg: string;
          if (this._translate.currentLang == 'ar') {
            msg = `تم تسجيل ${response.Value.success} طلب بينما لم يتم تسجيل ${response.Value.failed} طلب`;
          }
          else {
            msg = `${response.Value.success} records were added successfully , while ${response.Value.failed} records failed.`;
          }
          this.alert.warning(msg);
        }

        this.searchCaller();

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



  Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";

  onexportOrders() {
    //this.SearchCriteria.excelFlage = true;
    let clonedObj = Object.assign({}, this.SearchCriteria);
    clonedObj.excelFlage = true;
    //alert time zone offset before send
    //let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    clonedObj.DateTimeFrom = new Date(
      this.SearchCriteria.DateTimeFrom.getTime()
    );
    clonedObj.DateTimeFrom.setMinutes(
      clonedObj.DateTimeFrom.getMinutes() -
      clonedObj.DateTimeFrom.getTimezoneOffset()
    );
    clonedObj.DateTimeTo = new Date(
      this.SearchCriteria.DateTimeTo.getTime()
    );
    clonedObj.DateTimeTo.setMinutes(
      clonedObj.DateTimeTo.getMinutes() -
      clonedObj.DateTimeTo.getTimezoneOffset()
    );


    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.workOrderService
      .searchOrdersList(clonedObj)
      .subscribe(res => {
        if (res.IsErrorState == false) {
          if (
            isNullOrUndefined(res.Value.Result) ||
            res.Value.Result.length == 0
          ) {
            this.alert.error("NoDataFound");
            return;
          }

          this.ordersexcel = res.Value.Result.map(order => {
            return new OrderExcel(order);
          });
          this.ordersexcel.forEach(entry => {
            if (entry.RequestedAt != null || entry.RequestedAt != undefined) {
              let date,
                time,
                new_date,
                copydate,
                old_date = entry.RequestedAt;
              copydate = old_date.toString().split(/T/);
              date = copydate[0]
                .split("-")
                .reverse()
                .join("/");
              time = copydate[1];
              if (time.split(/:/)[0] < 12) {
                time = time + " AM";
              } else {
                time = time + " PM";
              }
              new_date = date + " " + time;
              entry.RequestedAt = new_date;
            }
            if (
              entry.LastStatusModificationDate != null ||
              entry.LastStatusModificationDate != undefined
            ) {
              let date,
                time,
                new_date,
                copydate,
                old_date = entry.LastStatusModificationDate;
              copydate = old_date.toString().split(/T/);
              date = copydate[0]
                .split("-")
                .reverse()
                .join("/");
              time = copydate[1];
              if (time.split(/:/)[0] < 12) {
                time = time + " AM";
              } else {
                time = time + " PM";
              }
              new_date = date + " " + time;
              entry.LastStatusModificationDate = new_date;
            }
            if (
              entry.ScheduledTime != null ||
              entry.ScheduledTime != undefined
            ) {
              let date,
                time,
                new_date,
                copydate,
                old_date = entry.ScheduledTime;
              copydate = old_date.toString().split(/T/);
              date = copydate[0]
                .split("-")
                .reverse()
                .join("/");
              time = copydate[1];
              if (time.split(/:/)[0] < 12) {
                time = time + " AM";
              } else {
                time = time + " PM";
              }
              new_date = date + " " + time;
              entry.ScheduledTime = new_date;
            }
          });

          this.ExcelService.exportAsExcelFile(this.ordersexcel, "orderlist");
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
