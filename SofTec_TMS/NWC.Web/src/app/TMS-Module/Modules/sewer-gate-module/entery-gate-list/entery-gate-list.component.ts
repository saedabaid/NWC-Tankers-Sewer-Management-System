import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { GateService } from '../../../Services/gate.service';
import { StateVeicle } from '../../../Models/state-vehicle';
import { SearchResult } from '../../../Models/common/search-result';
import { VehicleSC } from '../../../Models/search-criteria/vehicle-sc';
import { PageFilter } from '../../../Models/common/page-fillter-model';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { WorkOrderVehicleSC } from '../../../Models/search-criteria/workorder-vehicle-sc';
import { Lookup } from '../../../Models/common/lookup';
import { StateWorkOrderVeicle } from '../../../Models/state-workorder-vehicle';
import { LookupService } from '../../../Services/lookup.service';
import { WOVArrivedStation } from '../../../Models/events/WOVArrivedStation';
import { alertService } from '../../../../shared/Services/alert/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';
//import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap';
import { VehicleViolationComponent } from '../vehicle-violation/vehicle-violation.component';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
  selector: 'entery-gate-list',
  templateUrl: './entery-gate-list.component.html',
  styleUrls: ['./entery-gate-list.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class EnteryGateListComponent implements OnInit, OnDestroy {

  customerClassList: Lookup<number>[] = [];

  vehicleList: Lookup<string>[] = [];
  driverList: Lookup<string>[] = [];

  bindingModel_Vehicles: Lookup<string>[] = [];
  bindingModel_Drivers: Lookup<string>[] = [];

  vehicleSearchText = '';
  driverSearchText = '';

  selectMenuOptions = {
    enableSearchFilter: true,

  };

  selectMenuOptions2 = {
    enableSearchFilter: true,
    badgeShowLimit: 3
    //singleSelect: true
  };

  stateWOVeicleSearchResult = new SearchResult<StateWorkOrderVeicle>();
  workOrderVehicleSC = new WorkOrderVehicleSC();

  stateVeicleSearchResult = new SearchResult<StateVeicle>();
  vehicleSC = new VehicleSC();

  pagePermission: string = '';
  intervalAutoRefresh: any;
  SearchStream: SearchStream = new SearchStream();

  Vehicle_loading = false;
  Driver_Loading = false;
  IsArabic = false;
  modalRef: BsModalRef;


  constructor(private gateService: GateService,
    private lookupService: LookupService,
    private _alert: alertService,
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService,
    private modalService: BsModalService,

  ) {

    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('entrygate');
    this.authenticationService.checkViewPrivilege(this.pagePermission, true);

  }

  ngOnInit() {
    this.load();
    this.IsArabic = (this.translateService.currentLang == 'ar');
    this.translateService.onLangChange.subscribe(res => {
      this.load();
      this.IsArabic = (res.lang == 'ar');
    })
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalAutoRefresh);
    this.SearchStream.DestroyStreams();
  }

  load() {
    this.titleService.setTitle(this.translateService.instant('entry gate'));

    this.mainloading.PreloaderIcreaseCount();
    this.setDefaultSearchValues();
    this.onSearchSubmit();
    this.mainloading.PreloaderDecreaseCount();
  }

  searchWorkOrderVehicles() {
    this.mainloading.PreloaderIcreaseCount();
    this.gateService.getWorkOrderVehicles(this.workOrderVehicleSC).subscribe(res => {
      if (res.Value != null) {
        this.stateWOVeicleSearchResult = res.Value;

        if (this.stateWOVeicleSearchResult.Result && this.stateWOVeicleSearchResult.Result.length > 0) {
          this.stateWOVeicleSearchResult.Result.forEach(m => {
            //m.CustLocationClasses = this.customerClassList.filter(a => m.VehicleCustomerLocationClassId == a.Id);
            if (m.VehicleCustomerLocationClassesIds && m.VehicleCustomerLocationClassesIds.length > 0) {
              m.CustLocationClasses = this.customerClassList
                .filter(a => m.VehicleCustomerLocationClassesIds.includes(a.Id));
            }
          });
        }

      }
      else {
        this.stateWOVeicleSearchResult.Result = [];
        this.stateWOVeicleSearchResult.TotalCount = 0
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });
  }
  setDefaultSearchValues() {
    this.vehicleSC.PageFilter.PageIndex = 1;
    this.workOrderVehicleSC.PageFilter.PageIndex = 1;
    this.vehicleSC.PageFilter.PageIndex = 1;

    //set default values for WorkOrderVehicleSC
    this.workOrderVehicleSC.PageFilter = new PageFilter();
    this.workOrderVehicleSC.PageFilter.PageIndex = 1;
    this.workOrderVehicleSC.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.workOrderVehicleSC.WorkOrderNumber = "";
    this.workOrderVehicleSC.DriverIDs = [];
    this.workOrderVehicleSC.VehicleIDs = [];
    this.workOrderVehicleSC.ServiceTypeID = 3;
    this.workOrderVehicleSC.WorkOrderStatusIDs = [3, 4];
    //set default values for VehicleSC
    this.vehicleSC.ServiceTypeID = 3;
    this.vehicleSC.PageFilter = new PageFilter();
    this.vehicleSC.PageFilter.PageIndex = 1;
    this.vehicleSC.PageFilter.PageSize = 10; //Configuration.GridSetting.Pagesize;

    this.vehicleSC.StatusIDList = [13]; // Parking or BlackList or InTheWayToStation
    this.vehicleSC.VehicleIDs = [];
    this.vehicleSC.DriverIDs = [];

    this.vehicleSearchText = '';
    this.driverSearchText = '';

    // clear DDL selections
    this.bindingModel_Vehicles = [];
    this.bindingModel_Drivers = [];
  }

  onSearchSubmit() {
    this.vehicleSC.VechilePlateNumberOrCode = this.vehicleSearchText;
    this.vehicleSC.Driver = this.driverSearchText;
    this.searchWorkOrderVehicles();
    this.searchVehicle();
  }

  searchVehicle() {
    this.mainloading.PreloaderIcreaseCount();
    this.gateService.getStateVehicles(this.vehicleSC).subscribe(res => {
      if (res.Value != null) {
        this.stateVeicleSearchResult = res.Value;

        if (this.stateVeicleSearchResult.Result && this.stateVeicleSearchResult.Result.length > 0) {
          this.stateVeicleSearchResult.Result.forEach(m => {
            if (m.VehicleCustomerLocationClassesIds && m.VehicleCustomerLocationClassesIds.length > 0) {
              m.CustLocationClasses = this.customerClassList
                .filter(a => m.VehicleCustomerLocationClassesIds.includes(a.Id));
            }
          });
        }

      }
      else {
        this.stateVeicleSearchResult.Result = [];
        this.stateVeicleSearchResult.TotalCount = 0
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
    this.SearchStream.initStream("VehicleDDL_entryGate", (a) => {
      this.Vehicle_loading = true;
      this.lookupService.getVehicles(a).subscribe(res => {
        if (res.Value)
          this.vehicleList = res.Value;
      }
        , err => {
          this.Vehicle_loading = false;
        }
        , () => {
          this.Vehicle_loading = false;
        });
    }).next(searchKeyword);
  }

  getDrivers(searchKeyword: string) {
    this.SearchStream.initStream("DriverDDL_entryGate", (a) => {
      this.Driver_Loading = true;
      this.lookupService.getDrivers(a).subscribe(res => {
        if (res.Value)
          this.driverList = res.Value;
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
    this.workOrderVehicleSC.WorkOrderNumber = isNullOrUndefined(evt.Name) ? evt : evt.Name;
  }

  onVehicleDDLChanged(evt) {
    this.workOrderVehicleSC.VehicleIDs = evt.map(m => m.Id);
    this.vehicleSC.VehicleIDs = evt.map(m => m.Id);
  }

  onDriverDDLChanged(evt) {
    this.workOrderVehicleSC.DriverIDs = evt.map(m => m.Id);
    this.vehicleSC.DriverIDs = evt.map(m => m.Id);
  }

  onArrivedVehicleToStation(item: StateVeicle) {
    this.mainloading.PreloaderIcreaseCount();
    this.gateService.ArriveSewerVehicleWithOutOrderToStation(item.VehicleID).subscribe(res => {
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

  onArrivedStation(item: StateWorkOrderVeicle) {
    let wovArrivedStationDTO = new WOVArrivedStation;
    wovArrivedStationDTO.WorkOrderID = item.WorkOrderID;
    wovArrivedStationDTO.VehicleID = item.VehicleID;
    wovArrivedStationDTO.IsPaid = item.IsPaid;
    wovArrivedStationDTO.ConfirmationCode = item.ConfirmationCode;


    this.mainloading.PreloaderIcreaseCount();
    this.gateService.SewerArrivedStation(wovArrivedStationDTO).subscribe(res => {
      if (!res.IsErrorState) {
        this.onSearchSubmit();
      }
      else {
        this._alert.error(res.ErrorDescription);
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

  viewVehicleViolation(vehicleID: string) {
    let modelProp: ModalOptions = {
      class:
        this.translateService.currentLang == "ar"
          ? "vehicle-violation-modal rtl"
          : "vehicle-violation-modal",
      initialState: vehicleID
    };
    this.modalRef = this.modalService.show(
      VehicleViolationComponent,
      modelProp
    );
  }
}
