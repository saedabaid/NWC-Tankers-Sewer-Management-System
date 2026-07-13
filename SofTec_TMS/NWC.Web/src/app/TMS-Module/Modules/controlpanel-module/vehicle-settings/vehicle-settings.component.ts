import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { ControlPanelService } from 'src/app/TMS-Module/Services/control-panel.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { VehicleSettingsSC } from 'src/app/TMS-Module/Models/search-criteria/vehicle-settings-SC.model';
import { VehicleSettingsNWC } from 'src/app/TMS-Module/Models/vehicle-settings-nwc.model';
import { forkJoin } from 'rxjs';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';
import { VehicleSettingsBulkUpdate } from 'src/app/TMS-Module/Models/vehicle-settings-bulk-update.model';

@Component({
  selector: 'app-vehicle-settings',
  templateUrl: './vehicle-settings.component.html',
  styleUrls: ['./vehicle-settings.component.scss']
})
export class VehicleSettingsComponent implements OnInit, OnDestroy {

  bulkUpdate = false;
  pagePermission: string = '';
  tableLoading: boolean;
  IsArabic = false;

  customerClassList: Lookup<number>[] = [];
  customerServiceList: Lookup<number>[] = [];
  accessoriesList: Lookup<number>[] = [];

  SearchCriteria: VehicleSettingsSC; //= new VehicleSettingsSC();
  searchResult = new SearchResult<VehicleSettingsNWC>();
  bulkUpdateModel = new VehicleSettingsBulkUpdate();

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private controlPanelService: ControlPanelService,
    private alertservice: alertService,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('nwc_VehicleSettings');
    this.authenticationService.checkFullControlPrivilege(this.pagePermission, true);
  }



  ngOnInit() {
    this.setDefaultSearchValues();
    this.getVehicles();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {

    this.getAreas();
    this.getVehicleTypes();

    forkJoin(
      [
        this.lookupservice.getCustomerClasses(),
        this.lookupservice.getServicesTypes(),
        this.lookupservice.getAccessories()
      ]
    ).subscribe(([customerClasses, serviceTypes, accessories]) => {

      if (customerClasses.Value)
        this.customerClassList = customerClasses.Value;

      if (serviceTypes.Value)
        this.customerServiceList = serviceTypes.Value;

      if (accessories.Value)
        this.accessoriesList = accessories.Value;

      this.searchCaller();
    });

    this.IsArabic = (this.translateService.currentLang == 'ar');
    this.titleService.setTitle(this.translateService.instant('VehiclesSettings'));
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  onAccessoryDDLChange(evt, item: VehicleSettingsNWC) {
    item.AccessoryIDs = evt.map(m => m.Id);
  }

  onClassDDLChange(evt, item: VehicleSettingsNWC) {
    item.CustLocationClassIDs = evt.map(m => m.Id);
  }

  onServiceDDLChange(evt, item: VehicleSettingsNWC) {
    item.ServiceTypeID = evt.map(m => m.Id)[0];
  }



  //#region  "For search"
  selectMenuOptions = {
    enableSearchFilter: false,
    singleSelect: true
  };

  selectMenuOptions2 = {
    enableSearchFilter: true,
  };

  SearchStream: SearchStream = new SearchStream();

  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerStationList: Lookup<string>[] = [];
  vehicleList: Lookup<string>[] = [];
  vehicleTypeList: Lookup<string>[] = [];

  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_Vehicles: Lookup<string>[] = [];
  bindingModel_vehicleType: Lookup<string>[] = [];

  citySearchKeyWord = '';
  stationSearchKeyword = '';

  Area_loading = false;
  City_Loading = false;
  Station_Loading = false;
  Vehicle_Loading = false;

  setDefaultSearchValues() {
    this.SearchCriteria = new VehicleSettingsSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = 5; //Configuration.GridSetting.Pagesize;

    // redraw DDL selections
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Stations = [];
    this.bindingModel_Vehicles = [];
    this.bindingModel_vehicleType = [];

    this.customerCityList = [];
    this.customerStationList = [];

  }

  getAreas(searchKeyword: string = '') {

    this.SearchStream.initStream("AreaDDL_vehicleSettings", (a) => {
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
    this.SearchStream.initStream("CityDDL_vehicleSettings", (a) => {
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
    this.SearchStream.initStream("StationDDL_vehicleSettings", (a) => {
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

  getVehicles(searchKeyword: string = '') {
    this.SearchStream.initStream("VehicleDDL_vehicleSettings", (a) => {
      this.Vehicle_Loading = true;
      this.lookupservice.getVehicles(a).subscribe(res => {
        if (res.Value)
          this.vehicleList = res.Value;
      }, err => {
        this.Vehicle_Loading = false;
      }
        , () => {
          this.Vehicle_Loading = false;
        });
    }).next(searchKeyword);
  }

  getVehicleTypes() {
    this.lookupservice.GetTransporterTypes().subscribe(res => {
      if (res.Value)
        this.vehicleTypeList = res.Value;
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

  onVehicleDDLChanged(evt) {
    this.SearchCriteria.VehicleIDs = evt.map(m => m.Id);
  }

  onVehicleTypeDDLChanged(evt) {
    this.SearchCriteria.VehicleTypeIDs = evt.map(m => m.Id);
  }
  //#endregion "For Search"


  //#region "Bulk Update"
  bulkUpdate_StationList: Lookup<string>[] = [];
  
  bindingModel_BulkUpdate_CustomerClasses: Lookup<number>[] = [];
  bindingModel_BulkUpdate_Accessories: Lookup<number>[] = [];
  bindingModel_BulkUpdate_ServiceTypes: Lookup<number>[] = [];
  bindingModel_BulkUpdate_Station: Lookup<string>[] = [];
  
  bulkUpdate_Station_Loading = false;

  SearchPermittedStations(searchKeyword: string) {
    this.SearchStream.initStream("Station_AddReading", (a) => {
      this.bulkUpdate_Station_Loading = true;
      this.lookupservice.SearchPermittedStations(a).subscribe(res => {
        if (res.Value) {
          this.bulkUpdate_StationList = res.Value;
        }
      }
        , err => {
          this.bulkUpdate_Station_Loading = false;
        }
        , () => {
          this.bulkUpdate_Station_Loading = false;
        });

    }).next(searchKeyword);
  }

  onBulkUpdate_ClassDDLChange(evt) {
    this.bulkUpdateModel.CustLocationClassIDs = evt.map(m => m.Id);
  }

  onBulkUpdate_AccessoryDDLChange(evt) {
    this.bulkUpdateModel.AccessoryIDs = evt.map(m => m.Id);
  }

  onBulkUpdate_ServiceTypeDDLChange(evt) {
    this.bulkUpdateModel.ServiceTypeID = evt.map(m => m.Id)[0];
  }

  onBulkUpdate_StationDDLChange(evt) {
    this.bulkUpdateModel.StationID = evt.map(m => m.Id)[0];
  }

  onBulkUpdateSubmit() {

    let selectedVehicleIds = this.searchResult.Result.filter(a => a.CheckToSave).map(s => s.VehicleID);
    this.bulkUpdateModel.ApplyVehicleIds = selectedVehicleIds;
    this.bulkUpdateModel.VehicleSettingsSCModel = this.SearchCriteria;
    
    this.mainloading.PreloaderIcreaseCount();
    this.controlPanelService.SaveVehicleNWCSettingsBulk(this.bulkUpdateModel).subscribe(res => {
      if (res.IsErrorState === false) {
        this.alertservice.success("SavedSuccessed");
        this.clearBulkUpdateSearch();
        this.searchCaller();
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });

  }

  clearBulkUpdateSearch() {
    this.bulkUpdateModel = new VehicleSettingsBulkUpdate;

    this.bindingModel_BulkUpdate_CustomerClasses = [];
    this.bindingModel_BulkUpdate_Accessories = [];
    this.bindingModel_BulkUpdate_ServiceTypes = [];
    this.bindingModel_BulkUpdate_Station = [];
    
  }

  //#endregion "Bulk Update"


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

  searchCaller() {
    //if (!this.isValidModel()) return;

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.controlPanelService.GetVehicleNWCSettings(this.SearchCriteria).subscribe(res => {
      if (res.Value != null) {
        this.searchResult = res.Value;

        this.searchResult.Result.forEach(m => {
          m.ServiceTypes = this.customerServiceList.filter(a => m.ServiceTypeID == a.Id);
          m.CustLocationClasses = this.customerClassList.filter(a => m.CustLocationClassIDs.includes(a.Id));
          m.Accessories = this.accessoriesList.filter(a => m.AccessoryIDs.includes(a.Id));
        });

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
      });

  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"

  save() {

    let send = this.searchResult.Result.filter(a => a.CheckToSave);
    this.mainloading.PreloaderIcreaseCount();

    this.controlPanelService.SaveVehicleNWCSettings(send).subscribe(res => {
      if (res.IsErrorState === false)
        this.alertservice.success("SavedSuccessed");
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });

  }




}
