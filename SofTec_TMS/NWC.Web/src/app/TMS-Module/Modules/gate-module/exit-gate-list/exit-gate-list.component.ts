import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { GateService } from '../../../Services/gate.service';
import { StateWorkOrderVeicle } from '../../../Models/state-workorder-vehicle';
import { SearchResult } from '../../../Models/common/search-result';
import { WorkOrderVehicleSC } from '../../../Models/search-criteria/workorder-vehicle-sc';
import { PageFilter } from '../../../Models/common/page-fillter-model';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { WorkOrderSearchService } from '../../../Services/work-order-search.service';
import { isNullOrUndefined } from 'src/app/shared/utilities/utilities';
import { Lookup } from '../../../Models/common/lookup';
import { DispatchWorkOrder } from '../../../Models/events/dispatch-workorder';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from '../../../Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { Router } from '@angular/router';
import { LoaderService } from 'src/app/shared/loader.service';
import { VehicleSC } from 'src/app/TMS-Module/Models/search-criteria/vehicle-sc';
import { StateVeicle } from 'src/app/TMS-Module/Models/state-vehicle';
import { Item } from 'angular2-multiselect-dropdown';

@Component({
  selector: 'app-exit-gate-list',
  templateUrl: './exit-gate-list.component.html',
  styleUrls: ['./exit-gate-list.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ExitGateListComponent implements OnInit, OnDestroy {
  vehicleList: Lookup<string>[] = [];
  driverList: Lookup<string>[] = [];

  bindingModel_Vehicles: Lookup<string>[] = [];
  bindingModel_Drivers: Lookup<string>[] = [];

  selectMenuOptions = {
    enableSearchFilter: true,

  };

  searchResult = new SearchResult<StateWorkOrderVeicle>();
  searchCriteria = new WorkOrderVehicleSC();
  vehicleSC = new VehicleSC();

  vehicleSearchText = '';
  driverSearchText = '';

  pagePermission = '';
  intervalAutoRefresh: any;
  SearchStream: SearchStream = new SearchStream();

  Vehicle_loading = false;
  Driver_Loading = false;

  AvailableVehicles = new SearchResult<StateVeicle>();
  IsArabic = false;

  constructor(private gateService: GateService,
    private authServer: AuthenticationService,
    private lookupService: LookupService,
    private translateService: TranslateService,
    private titleService: Title,
    private router: Router,
    private mainloading: LoaderService

    ) {

      this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('exitgate');
      this.authServer.checkViewPrivilege(this.pagePermission, true);
    }

  ngOnInit() {
    this.IsArabic = (this.translateService.currentLang == 'ar');

    // this.getVehicles('');
    // this.getDrivers('');

    this.setDefaultSearchValues();
    this.onSearchSubmit();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
      this.IsArabic = (res.lang == 'ar');
    });

    // this.intervalAutoRefresh = setInterval(() => {
    //   this.searchVehicle();
    //   this.getAvailableVehicles();

    // }, Configuration.AutoRefresh.milliseconds);

  }

  ngOnDestroy(): void {
    clearInterval(this.intervalAutoRefresh);
    this.SearchStream.DestroyStreams();
  }

  load() {
    this.titleService.setTitle( this.translateService.instant('exit gate'));
  }

  setDefaultSearchValues() {
    this.searchCriteria.PageFilter = new PageFilter();
    this.searchCriteria.PageFilter.PageIndex = 1;
    this.searchCriteria.PageFilter.PageSize = 10; // Configuration.GridSetting.Pagesize;
    this.searchCriteria.WorkOrderNumber = '';

    // Get InService & Assigned vehicles
    this.searchCriteria.WorkOrderStatusIDs = [5]; // Assigned


    // set default values for VehicleSC
    this.vehicleSC.PageFilter.PageIndex = 1;
    this.vehicleSC.ServiceTypeID = 1;
    this.vehicleSC.PageFilter = new PageFilter();
    this.vehicleSC.PageFilter.PageIndex = 1;
    this.vehicleSC.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    this.vehicleSC.StatusIDList = [0]; // Available
    this.vehicleSC.VehicleIDs = [];
    this.vehicleSC.DriverIDs = [];

    this.vehicleSearchText = '';
    this.driverSearchText = '';


    // clear DDL selections
    this.bindingModel_Vehicles = [];
    this.bindingModel_Drivers = [];
    this.searchCriteria.VehicleIDs = [];
    this.searchCriteria.DriverIDs = [];

  }

  onSearchSubmit() {
    this.searchCriteria.PageFilter.PageIndex = 1;

    this.searchCriteria.VechilePlateNumberOrCode = this.vehicleSearchText;
    this.searchCriteria.Driver = this.driverSearchText;
    this.vehicleSC.VechilePlateNumberOrCode = this.vehicleSearchText;
    this.vehicleSC.Driver = this.driverSearchText;


    this.searchVehicle();
    this.getAvailableVehicles();
  }

  searchVehicle() {
    this.mainloading.PreloaderIcreaseCount();
    this.gateService.getWorkOrderVehicles(this.searchCriteria).subscribe(res => {

      if (res.Value != null) {
        this.searchResult = res.Value;
      } else {
        this.searchResult.Result = [];
        this.searchResult.TotalCount = 0;
      }
    }
    , err => {
      this.mainloading.PreloaderDecreaseCount();
    }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });
  }

  getVehicles(searchKeyword: string) {
    this.SearchStream.initStream('VehicleDDL_exitGate', (a) => {
      this.Vehicle_loading = true;
      this.lookupService.getVehicles(a).subscribe(res => {
        if (res.Value) {
          this.vehicleList = res.Value;
        }
      }
      , err => {
        this.Vehicle_loading = false;
      }
      , () => {
        this.Vehicle_loading = false;
      }
      );
    }).next(searchKeyword);
  }

  getDrivers(searchKeyword: string) {
    this.SearchStream.initStream('DriverDDL_exitGate', (a) => {
      this.Driver_Loading = true;
      this.lookupService.getDrivers(a).subscribe(res => {
        if (res.Value) {
        this.driverList = res.Value;
        }
      }
      , err => {
        this.Driver_Loading = false;
      }
      , () => {
        this.Driver_Loading = false;
      });
    }).next(searchKeyword);
  }

  getWorkOrderNumbers(name) {
    return this.lookupService.SearchOrderNumbers(name);
  }

  onWorkOrderDDLChanged(evt) {
    this.searchCriteria.WorkOrderNumber = isNullOrUndefined(evt.Name) ? evt : evt.Name;
  }

  onWorkOrderDDLClicked(evt) {
    // this.searchCriteria.WorkOrderNumber = isNullOrUndefined(evt.Name) ? evt : evt.Name;
    // this.searchVehicle();
  }

  onVehicleDDLChanged(evt) {
    this.searchCriteria.VehicleIDs = evt.map(m => m.Id);
    this.vehicleSC.VehicleIDs = evt.map(m => m.Id);
  }

  onDriverDDLChanged(evt) {
    this.searchCriteria.DriverIDs = evt.map(m => m.Id);
    this.vehicleSC.DriverIDs = evt.map(m => m.Id);
  }

  onOutForDelivery(workOrder: StateWorkOrderVeicle) {

    const dispatch = new DispatchWorkOrder;
    dispatch.EventWorkOrderDTO.WorkOrderID = workOrder.WorkOrderID;
    // dispatch.EventWorkOrderDTO.CreatedBy = this.authServer.getCurrentStaffId();
    // dispatch.EventWorkOrderVehicleDTO.CreatedBy = this.authServer.getCurrentStaffId();

    this.mainloading.PreloaderIcreaseCount();
    this.gateService.OutForDeliveryWorkOrder(dispatch.EventWorkOrderDTO).subscribe(res => {
      if (res.Value) {

        this.onSearchSubmit();
      }

      if (workOrder.CitySettings_ShowInvoice) {
        const targetLink = `/tms/gate/print/${workOrder.VehicleID}/${workOrder.WorkOrderID}`;
        const url = this.router.serializeUrl(this.router.createUrlTree([targetLink]));
        //window.open(url, '_blank', 'noreferrer').opener = null; 
        this.router.navigate([targetLink]);
      }

    }
    , err => {
      this.mainloading.PreloaderDecreaseCount();
    }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.onSearchSubmit();
  }



  getAvailableVehicles() {
    this.mainloading.PreloaderIcreaseCount();
    this.gateService.getStateVehicles(this.vehicleSC).subscribe(res => {
      if (res.Value != null) {
        this.AvailableVehicles = res.Value;
      } else {
        this.AvailableVehicles.Result = [];
        this.AvailableVehicles.TotalCount = 0;
      }
    }
    , err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    , () => {
      this.mainloading.PreloaderDecreaseCount();
    });
  }


  onOutForParking(vehicleID: string) {

    this.mainloading.PreloaderIcreaseCount();
    this.gateService.OutForParking(vehicleID).subscribe(res => {
      if (res.Value) {
        this.onSearchSubmit();
      }
    }
    , err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    , () => {
      this.mainloading.PreloaderDecreaseCount();
    });

  }

}
