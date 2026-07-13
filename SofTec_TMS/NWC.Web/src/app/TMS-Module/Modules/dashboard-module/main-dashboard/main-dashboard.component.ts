import { Component, OnInit, OnDestroy } from "@angular/core";
import { DashboardSC } from "src/app/TMS-Module/Models/search-criteria/dashboard-SC.model";
import { TranslateService } from "@ngx-translate/core";
import { Title } from "@angular/platform-browser";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { LookupService } from "src/app/TMS-Module/Services/lookup.service";
import { ControlPanelService } from "src/app/TMS-Module/Services/control-panel.service";
import { LoaderService } from "src/app/shared/loader.service";
import { SearchStream } from "src/app/TMS-Module/Models/common/search-stream-object.model";
import { Lookup } from "src/app/TMS-Module/Models/common/lookup";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Configuration } from "src/app/shared/configurations/shared.config";
import { DashboardService } from "src/app/TMS-Module/Services/dashboard.service";

@Component({
  selector: "app-main-dashboard",
  templateUrl: "./main-dashboard.component.html",
  styleUrls: ["./main-dashboard.component.scss"]
})
export class MainDashboardComponent implements OnInit, OnDestroy {
  pagePermission: string = "";

  SearchCriteria: DashboardSC;

  timeFromStr: string;
  timeToStr: string;

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private dashboardService: DashboardService,
    private alertservice: alertService,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName("tmsDashboard");
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.setDefaultSearchValues();
    this.getAreas();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
      //this.searchCaller();
    });
  }

  load() {
    this.getServiceTypes();
    this.searchCaller();

    this.titleService.setTitle(this.translateService.instant("NWC_Dashboard"));
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
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

  citySearchKeyWord = "";
  stationSearchKeyword = "";

  Area_loading = false;
  City_Loading = false;
  Station_Loading = false;

  selectMenuOptions = {
    enableSearchFilter: true,    
  };

  selectMenuOptions2 = {
    enableSearchFilter: false,
    singleSelect: true
  };

  //*************************************************
  setDefaultSearchValues() {
    this.SearchCriteria = new DashboardSC();
    // this.SearchCriteria.PageFilter.PageIndex = 1;
    // this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    let startDate = new Date();
    //startDate.setDate(startDate.getDate() - 1);
    this.SearchCriteria.DateFrom = startDate;
    this.timeFromStr = startDate.toTimeString().substring(0, 5);

    let endDate = new Date();
    // endDate.setDate(endDate.getDate() + 1);
    // endDate.setHours(0);
    // endDate.setMinutes(0);
    // endDate.setSeconds(0);
    this.SearchCriteria.DateTo = endDate;
    this.timeToStr = endDate.toTimeString().substring(0, 5);

    // redraw DDL selections
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Stations = [];

    this.customerCityList = [];
    this.customerStationList = [];

    this.setDefaultServiceType();
  }

  setDefaultServiceType() {
    if (this.customerServiceList && this.customerServiceList.length > 0) {
      let serviceId = +Configuration.Dashboard.DefaultServiceTypeId;
      this.bindingModel_ServiceTypes = this.customerServiceList.filter(
        a => a.Id == serviceId
      );
      this.SearchCriteria.ServiceTypeID = serviceId;
    }
  }

  //*******************************************************
  getAreas(searchKeyword: string = "") {
    this.SearchStream.initStream("AreaDDL_stationSettings", a => {
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

  getCity(searchKeyword: string) {
    this.citySearchKeyWord = searchKeyword;
    this.SearchStream.initStream("CityDDL_stationSettings", a => {
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

  getStations(searchKeyword: string) {
    this.stationSearchKeyword = searchKeyword;
    this.SearchStream.initStream("StationDDL_stationSettings", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.CityIDs) &&
        this.SearchCriteria.CityIDs.length > 0
      ) {
        this.Station_Loading = true;
        this.lookupservice
          .GetStationBasedOnCity(a, this.SearchCriteria.CityIDs)
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

  getServiceTypes() {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value) {
        this.customerServiceList = res.Value;
        this.setDefaultServiceType();
      }
    });
  }

  //***************************************************************
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
    this.SearchCriteria.ServiceTypeID = evt.map(m => m.Id)[0];
  }

  //#endregion "For Search"


  onSearchSubmit() {
    //this.SearchCriteria.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  clearSearch() {
    this.setDefaultSearchValues();
    //this.searchCaller();
    this.dashboardService.mainSearchClicked$.next(null);

  }

  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.SearchCriteria.DateFrom)) {
      validationMessages.push("DateFromRequired");
    }

    if (isNullOrUndefined(this.SearchCriteria.DateTo)) {
      validationMessages.push("DateToRequired");
    }

    // let timeFrom = this.timeFromStr.split(":");
    // if (
    //   timeFrom.length !== 2 ||
    //   isNullOrUndefined(timeFrom[0]) ||
    //   timeFrom[0] === "" ||
    //   isNullOrUndefined(timeFrom[1]) ||
    //   timeFrom[1] === ""
    // ) {
    //   validationMessages.push("TimeFromRequired");
    // }

    // let timeTo = this.timeToStr.split(":");
    // if (
    //   timeTo.length !== 2 ||
    //   isNullOrUndefined(timeTo[0]) ||
    //   timeTo[0] === "" ||
    //   isNullOrUndefined(timeTo[1]) ||
    //   timeTo[1] === ""
    // ) {
    //   validationMessages.push("TimeToRequired");
    // }

    if (
      !isNullOrUndefined(this.SearchCriteria.DateFrom) &&
      !isNullOrUndefined(this.SearchCriteria.DateTo) &&
      this.SearchCriteria.DateTo < this.SearchCriteria.DateFrom
    ) {
      validationMessages.push("DateTimeFromMustBeBeforDateTimeTo");
    }

    if (this.SearchCriteria.AreaIDs && this.SearchCriteria.AreaIDs.length) {
      
      if (!this.SearchCriteria.CityIDs || !this.SearchCriteria.CityIDs.length) {
        validationMessages.push("DashboardSelectCity");
      }

      if (!this.SearchCriteria.StationIDs || !this.SearchCriteria.StationIDs.length) {
        validationMessages.push("DashboardSelectStation");
      }
    }


    // if (isNullOrUndefined(this.SearchCriteria.AreaIDs) || this.SearchCriteria.AreaIDs.length < 1) {
    //   validationMessages.push("DashboardSelectArea");
    // }

    // if (isNullOrUndefined(this.SearchCriteria.CityIDs) || this.SearchCriteria.CityIDs.length < 1) {
    //   validationMessages.push("DashboardSelectCity");
    // }

    // if (isNullOrUndefined(this.SearchCriteria.StationIDs) || this.SearchCriteria.StationIDs.length < 1) {
    //   validationMessages.push("DashboardSelectStation");
    // }

    if (isNullOrUndefined(this.SearchCriteria.ServiceTypeID) || +this.SearchCriteria.ServiceTypeID < 1) {
      validationMessages.push("DashboardSelectServiceType");
    }

    if (validationMessages.length > 0) {
      this.alertservice.errorList(validationMessages);
      return false;
    }
    return true;
  }

  searchCaller() {

    if (!this.isValidModel()) return;

    // let timeFrom = this.timeFromStr.split(":");
    // this.SearchCriteria.DateFrom.setHours(+timeFrom[0]);
    // this.SearchCriteria.DateFrom.setMinutes(+timeFrom[1]);

    // let timeTo = this.timeToStr.split(":");
    // this.SearchCriteria.DateTo.setHours(+timeTo[0]);
    // this.SearchCriteria.DateTo.setMinutes(+timeTo[1]);

    //alert time zone offset before send
    let modifiedCriteria = Object.assign({}, this.SearchCriteria);

    modifiedCriteria.DateFrom = new Date(this.SearchCriteria.DateFrom.getTime());
    modifiedCriteria.DateFrom.setMinutes(
      modifiedCriteria.DateFrom.getMinutes() - modifiedCriteria.DateFrom.getTimezoneOffset()
    );
    modifiedCriteria.DateTo = new Date(this.SearchCriteria.DateTo.getTime());
    modifiedCriteria.DateTo.setMinutes(
      modifiedCriteria.DateTo.getMinutes() - modifiedCriteria.DateTo.getTimezoneOffset()
    );

    this.dashboardService.mainSearchClicked$.next(modifiedCriteria);
  }


}
