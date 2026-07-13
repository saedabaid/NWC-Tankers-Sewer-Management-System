import { Component, OnInit } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
//import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap';
import { WorkOrderSearchCriteria, DateperiodEnum } from 'src/app/TMS-Module/Models/search-criteria/work-order-search-criteria';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { OrderDetails } from 'src/app/TMS-Module/Models/order-details';
import { OrderExcel } from 'src/app/TMS-Module/Models/OrderExcel';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { WorkOrderSearchService } from 'src/app/TMS-Module/Services/work-order-search.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { LoaderService } from 'src/app/shared/loader.service';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { FilterModel } from 'src/app/TMS-Module/Models/common/filter-model';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { ChangeOrderStatusComponent } from '../../order-module/change-order-status/change-order-status.component';
import { DeassignTankerComponent } from '../../order-module/deassign-tanker/deassign-tanker.component';
import { ManualDispatchTankerComponent } from '../../order-module/manual-dispatch-tanker/manual-dispatch-tanker.component';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-daily-order-details',
  templateUrl: './daily-order-details.component.html',
  styleUrls: ['./daily-order-details.component.scss']
})
export class DailyOrderDetailsComponent implements OnInit {

  dropdownSettings: IDropdownSettings = {};
  advancedDiv = <boolean>false;
  modalRef: BsModalRef;
  closeBtnName: string;
  SearchCriteria: WorkOrderSearchCriteria;
  tableLoading = false;

  timeFromStr: string;
  timeToStr: string;
  selectMenuOptions = {
    enableSearchFilter: true,
  };

  searchResult = new SearchResult<OrderDetails>();

  ordersexcel: OrderExcel[] = [];
  HoverExcel: boolean = false;
  pagePermission: string = "";

  constructor(
    private router: Router,
    private authServer: AuthenticationService,
    private workOrderService: WorkOrderSearchService,
    private lookupservice: LookupService,
    private alert: alertService,
    private ExcelService: ExcelService,
    private _translate: TranslateService,
    private titleService: Title,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName("tmsReports");
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.dropdownSettings = {
      singleSelection: false,
      idField: "Id",
      textField: "Name",
      selectAllText: "Select All",
      unSelectAllText: "UnSelect All",
      itemsShowLimit: 1,
      allowSearchFilter: true
    };

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
    this.titleService.setTitle(this._translate.instant("DailyOrdersDetails"));

    this.searchCaller();

    // load Search DDls values
    this.getClass();
    this.getPriorities();
    this.getServiceTypes();
    this.getStatuses();
    this.getCustomers("");
    this.getAreas("");
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new WorkOrderSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize =
      Configuration.GridSetting.Pagesize;

    this.SearchCriteria.DatePeriod = DateperiodEnum.ScheduleDate;

    let startDate = new Date();
    startDate.setDate(startDate.getDate() - 1);
    this.SearchCriteria.DateTimeFrom = startDate;
    this.timeFromStr = startDate.toTimeString().substring(0, 5);

    let endDate = new Date();
    endDate.setDate(endDate.getDate() + 1);
    endDate.setHours(0);
    endDate.setMinutes(0);
    endDate.setSeconds(0);
    this.SearchCriteria.DateTimeTo = endDate;
    this.timeToStr = endDate.toTimeString().substring(0, 5);

    // redraw DDL selections
    this.bindingModel_Customers = [];
    this.bindingModel_Classes = [];
    this.bindingModel_Priorities = [];
    this.bindingModel_ServiceTypes = [];
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Zones = [];
    this.bindingModel_Stations = [];
    this.bindingModel_Statuses = [];
    this.bindingModel_Vehicles = [];
    this.bindingModel_Drivers = [];
  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }

  //#region "Drop Down lists" *********************************************************************************
  SearchStream: SearchStream = new SearchStream();

  //#region  "Lookups declarations"
  customerNameList: Lookup<number>[] = [];
  customerClassList: Lookup<number>[] = [];
  customerPrioritiesList: Lookup<number>[] = [];
  customerServiceList: Lookup<number>[] = [];
  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerZoneList: Lookup<number>[] = [];
  customerStationList: Lookup<string>[] = [];
  CustomerStatusList: Lookup<number>[] = [];
  customerVehicleList: Lookup<string>[] = [];
  CustomerDriverList: Lookup<string>[] = [];
  OrderNumberList: Lookup<number>[] = [];
  //#endregion "Lookups declarations"
  citySearchKeyWord = "";
  zoneSearchKeyWord = "";
  stationSearchKeyword = "";
  vehicleSearchKeyword = "";
  driverSearchKeyword = "";

  Customer_Loading = false;
  Area_loading = false;
  City_Loading = false;
  Zone_Loading = false;
  Station_Loading = false;
  Vehicle_Loading = false;
  Driver_Loading = false;

  //#region "LookupsBindingModels"
  bindingModel_Customers: Lookup<number>[] = [];
  bindingModel_Classes: Lookup<number>[] = [];
  bindingModel_Priorities: Lookup<number>[] = [];
  bindingModel_ServiceTypes: Lookup<number>[] = [];
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_Statuses: Lookup<number>[] = [];
  bindingModel_Vehicles: Lookup<string>[] = [];
  bindingModel_Drivers: Lookup<string>[] = [];
  //#endregion "LookupsBindingModels"

  //#region  "get Lookups"
  getClass() {
    this.lookupservice.getCustomerClasses().subscribe(res => {
      if (res.Value) this.customerClassList = res.Value;
    });
  }

  getPriorities() {
    this.lookupservice.getCustomerPriorities().subscribe(res => {
      if (res.Value) this.customerPrioritiesList = res.Value;
    });
  }

  getServiceTypes() {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value) this.customerServiceList = res.Value;
    });
  }

  getStatuses() {
    this.lookupservice.getWorkOrdersStatues().subscribe(res => {
      if (res.Value) this.CustomerStatusList = res.Value;
    });
  }

  getWorkOrderNumbers(name) {
    return this.lookupservice.SearchOrderNumbers(name);
  }

  getCustomers(searchKeyword: string) {
    this.SearchStream.initStream("CustomerDDL_createOrder", a => {
      this.Customer_Loading = true;
      this.lookupservice.getCustomers(a).subscribe(
        res => {
          if (res.Value) this.customerNameList = res.Value;
        },
        err => {
          this.Customer_Loading = false;
        },
        () => {
          this.Customer_Loading = false;
        }
      );
    }).next(searchKeyword);
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

  //depends on cityIds //ZoneIds
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

  //depends on stationIds
  getVehicles(searchKeyword: string) {
    this.vehicleSearchKeyword = searchKeyword;
    this.SearchStream.initStream("VehicleDDL_createOrder", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.StationIDs) &&
        this.SearchCriteria.StationIDs.length > 0
      ) {
        this.Vehicle_Loading = true;
        this.lookupservice
          .searchVehicles(a, this.SearchCriteria.StationIDs)
          .subscribe(
            res => {
              if (res.Value) this.customerVehicleList = res.Value;
            },
            err => {
              this.Vehicle_Loading = false;
            },
            () => {
              this.Vehicle_Loading = false;
            }
          );
      } else {
        this.customerVehicleList = [];
      }
    }).next(searchKeyword);
  }

  //depends on stationIds
  getDerivers(searchKeyword) {
    this.driverSearchKeyword = searchKeyword;
    this.SearchStream.initStream("DriverDDL_createOrder", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.StationIDs) &&
        this.SearchCriteria.StationIDs.length > 0
      ) {
        this.Driver_Loading = true;
        this.lookupservice
          .searchDrivers(a, this.SearchCriteria.StationIDs)
          .subscribe(
            res => {
              if (res.Value) this.CustomerDriverList = res.Value;
            },
            err => {
              this.Driver_Loading = false;
            },
            () => {
              this.Driver_Loading = false;
            }
          );
      } else {
        this.CustomerDriverList = [];
      }
    }).next(searchKeyword);
  }

  //#endregion "get Lookups"

  //#region  "on Lookups change"
  onWorkOrderDDLChanged(evt) {
    this.SearchCriteria.FilterModel.SearchKeyword = isNullOrUndefined(evt.Name)
      ? evt
      : evt.Name;
  }

  onWorkOrderDDLClicked(evt) {
    // this.SearchCriteria.FilterModel.SearchKeyword = isNullOrUndefined(evt.Name)
    //   ? evt
    //   : evt.Name;
    // this.searchCaller();
  }

  onClassDDLChanged(evt) {
    this.SearchCriteria.ClassIDs = evt.map(m => m.Id);
  }

  onPriorityDDLChanged(evt) {
    this.SearchCriteria.PriorityIDs = evt.map(m => m.Id);
  }

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

    this.customerVehicleList = [];
    this.bindingModel_Vehicles = [];
    this.SearchCriteria.VehicleIDs = [];

    this.CustomerDriverList = [];
    this.bindingModel_Drivers = [];
    this.SearchCriteria.DriverIDs = [];
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

    this.customerVehicleList = [];
    this.bindingModel_Vehicles = [];
    this.SearchCriteria.VehicleIDs = [];

    this.CustomerDriverList = [];
    this.bindingModel_Drivers = [];
    this.SearchCriteria.DriverIDs = [];
  }

  onZoneDDLChanged(evt) {
    this.SearchCriteria.ZoneIDs = evt.map(m => m.Id);
    // this.getStations(this.stationSearchKeyword);
    // this.bindingModel_Stations = [];
    // this.SearchCriteria.StationIDs = [];

    // this.customerVehicleList = [];
    // this.bindingModel_Vehicles = [];
    // this.SearchCriteria.VehicleIDs = [];

    // this.CustomerDriverList = [];
    // this.bindingModel_Drivers = [];
    // this.SearchCriteria.DriverIDs = [];
  }

  onStationDDLChanged(evt) {
    this.SearchCriteria.StationIDs = evt.map(m => m.Id);
    this.getVehicles(this.vehicleSearchKeyword);
    this.getDerivers(this.driverSearchKeyword);

    this.bindingModel_Vehicles = [];
    this.SearchCriteria.VehicleIDs = [];

    this.bindingModel_Drivers = [];
    this.SearchCriteria.DriverIDs = [];
  }

  onVehicleDDLChanged(evt) {
    this.SearchCriteria.VehicleIDs = evt.map(m => m.Id);
  }

  onDeriverDDLChanged(evt) {
    this.SearchCriteria.DriverIDs = evt.map(m => m.Id);
  }

  onCustomerDDLChanged(evt) {
    this.SearchCriteria.CustomerIDs = evt.map(m => m.Id);
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

    if (isNullOrUndefined(this.SearchCriteria.DateTimeFrom)) {
      validationMessages.push("DateFromRequired");
    }

    if (isNullOrUndefined(this.SearchCriteria.DateTimeTo)) {
      validationMessages.push("DateToRequired");
    }

    let timeFrom = this.timeFromStr.split(":");
    if (
      timeFrom.length !== 2 ||
      isNullOrUndefined(timeFrom[0]) ||
      timeFrom[0] === "" ||
      isNullOrUndefined(timeFrom[1]) ||
      timeFrom[1] === ""
    ) {
      validationMessages.push("TimeFromRequired");
    }
    else {
      this.SearchCriteria.DateTimeFrom.setHours(+timeFrom[0]);
      this.SearchCriteria.DateTimeFrom.setMinutes(+timeFrom[1]);
    }

    let timeTo = this.timeToStr.split(":");
    if (
      timeTo.length !== 2 ||
      isNullOrUndefined(timeTo[0]) ||
      timeTo[0] === "" ||
      isNullOrUndefined(timeFrom[1]) ||
      timeTo[1] === ""
    ) {
      validationMessages.push("TimeToRequired");
    }
    else {
      this.SearchCriteria.DateTimeTo.setHours(+timeTo[0]);
      this.SearchCriteria.DateTimeTo.setMinutes(+timeTo[1]);
    }

    if (
      !isNullOrUndefined(this.SearchCriteria.DateTimeFrom) &&
      !isNullOrUndefined(this.SearchCriteria.DateTimeTo) &&
      this.SearchCriteria.DateTimeTo <= this.SearchCriteria.DateTimeFrom
    ) {
      validationMessages.push("DateTimeFromMustBeBeforDateTimeTo");
    }

    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  searchCaller() {
    if (!this.isValidModel()) return;

    let timeFrom = this.timeFromStr.split(":");
    this.SearchCriteria.DateTimeFrom.setHours(+timeFrom[0]);
    this.SearchCriteria.DateTimeFrom.setMinutes(+timeFrom[1]);

    let timeTo = this.timeToStr.split(":");
    this.SearchCriteria.DateTimeTo.setHours(+timeTo[0]);
    this.SearchCriteria.DateTimeTo.setMinutes(+timeTo[1]);

    //alert time zone offset before send
    let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    modifiedCriteria.DateTimeFrom = new Date(
      this.SearchCriteria.DateTimeFrom.getTime()
    );
    modifiedCriteria.DateTimeFrom.setMinutes(
      modifiedCriteria.DateTimeFrom.getMinutes() -
        modifiedCriteria.DateTimeFrom.getTimezoneOffset()
    );
    modifiedCriteria.DateTimeTo = new Date(
      this.SearchCriteria.DateTimeTo.getTime()
    );
    modifiedCriteria.DateTimeTo.setMinutes(
      modifiedCriteria.DateTimeTo.getMinutes() -
        modifiedCriteria.DateTimeTo.getTimezoneOffset()
    );

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.workOrderService.GetDailyOrderDetailsReport(modifiedCriteria).subscribe(
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
    //this.SearchCriteria.excelFlage = true;
    let clonedObj = Object.assign({}, this.SearchCriteria);
    clonedObj.excelFlage = true;
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
      .GetDailyOrderDetailsReport(clonedObj)
      .subscribe(res => {
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
              OrderNo: value.OrderNumber,
              Status: value.LastStatusName,
              VehiclePlateNo: value.VehicleCodePlateNo,
              AssignedDriverName: value.DriverName,
              CustomerName: value.CustomerName,
              CustomerClass: value.ClassName,
              CustomerZone: value.ZoneName,
              ZoneWithoutTanker: value.ZoneWithOutTankers,
              CustomerCity: value.CityName,
              Station: value.StationName,
              ServiceType: value.ServiceType,
              Quantity: value.OrderQuantity,
              CreateToDeliveredTime: value.CreateToDeliveredTime,
              CreateToOutTime: value.CreateToOutTime,
              OutToDeliveredTime: value.OutToDeliveredTime,
              SourceApplication: value.SourceApplication,

              ScheduledTime: this.ConvertDateToExcel(value.ScheduledDeliveryTime),
              AssignedTime: this.ConvertDateToExcel(value.AssignedOn),
              OutForDeliveryTime: this.ConvertDateToExcel(value.OutForDeliveryOn),
              ArrivedTime: this.ConvertDateToExcel(value.ArrivedOn),
              DeliveryTime: this.ConvertDateToExcel(value.DeliverOn),
              FailedToDeliverTime: this.ConvertDateToExcel(value.FailedToDeliverOn),
              CancelledTime: this.ConvertDateToExcel(value.CancelledOn),

              TotalCoast: value.TotalCost,
              Payment: value.InvoiceStatusName,
              LastUpdate: value.LastStatusByCategory,
              InvoicNo: value.InvoiceNo,
              OrderCategory: value.CategoryName,
              RequestedAt: this.ConvertDateToExcel(value.RequestTime),
              ClosedByCode: value.ClosedByCode,
              CancelReason: value.LastStatusReason,
            };
            return r;
          });

          this.ExcelService.exportAsExcelFile(excelJson, "DailyOrdersDetails");
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
      let date =  input.toString().substring(0, 10);
      let time = input.toString().substring(11, 16);
      return date + ' ' + time;
    }
    return '';
  }

}
