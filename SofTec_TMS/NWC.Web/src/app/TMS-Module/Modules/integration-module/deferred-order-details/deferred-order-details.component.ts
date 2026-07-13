import { Component, OnInit, OnDestroy } from '@angular/core';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { Router } from '@angular/router';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { WorkOrderSearchService } from 'src/app/TMS-Module/Services/work-order-search.service';
import { DeferredOrderSC } from 'src/app/TMS-Module/Models/search-criteria/deferred-order-SC.model';
import { DeferredOrder } from 'src/app/TMS-Module/Models/deferred-order.model';
import { forkJoin } from 'rxjs';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { zoneListService } from 'src/app/TMS-Module/Services/zone-list.service';
import { ZoneDTO } from 'src/app/TMS-Module/Models/zoneDTO';
import { MapService } from 'src/app/shared/Services/mapService/map-service';

@Component({
  selector: 'app-deferred-order-details',
  templateUrl: './deferred-order-details.component.html',
  styleUrls: ['./deferred-order-details.component.scss']
})
export class DeferredOrderDetailsComponent implements OnInit, OnDestroy {

  DeferredOrder = new DeferredOrder();

  DeferredOrderId: number;
  pagePermission: string = '';

  gisLoading = false;
  gisReturningZone: ZoneDTO;
  showZoneDDL = false;
  scheduleTimeStr: string;
  CustomerIconSrc: string = "assets/TMSBranding/styles/img/maps-and-flags.png";



  constructor(
    private router: Router,
    private workOrderService: WorkOrderSearchService,
    private _alert: alertService,
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private mainloading: LoaderService,
    private zoneService: zoneListService,
    private mapAPI: MapService
  ) {

    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsDeferredOrders');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

    let urlList = this.router.routerState.snapshot.url.split('/');
    if (urlList[3] === "deferredorderedit") {
      this.DeferredOrderId = +urlList[4];
      // this.updateMode = true;
    }
    else {
      // this.updateContractorId = null;
      // this.updateMode = false;
    }

  }

  ngOnInit() {

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  load() {
    this.titleService.setTitle(this.translateService.instant('EditDeferredOrder'));

    this.mainloading.PreloaderIcreaseCount();
    forkJoin(
      [
      this.lookupservice.getCustomerClasses(),
      this.lookupservice.getPermittedServicesTypes(),
      this.lookupservice.GetTanckerCapacities(),
      this.lookupservice.GetPersonalIDTypes()
      ]
    ).subscribe(([customerClasses, serviceTypes, TanckerCapacities, PersonalIdTypes]) => {

      if (customerClasses.Value)
        this.customerClassList = customerClasses.Value;

      if (serviceTypes.Value)
        this.customerServiceList = serviceTypes.Value;


      if (TanckerCapacities.Value)
        this.TanckerCapacityList = TanckerCapacities.Value;

      if (TanckerCapacities.Value)
        this.PersonalIDList = PersonalIdTypes.Value;

      this.setDefaultContract();

    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });


  }


  //#region Drop Down list

  customerServiceList: Lookup<number>[] = [];
  TanckerCapacityList: Lookup<number>[] = [];
  customerClassList: Lookup<number>[] = [];
  PersonalIDList: Lookup<number>[] = [];
  zoneList: Lookup<number>[] = [];

  bindingModel_CustomerService: Lookup<number>[] = [];
  bindingModel_TanckerCapacities: Lookup<number>[] = [];
  bindingModel_Classes: Lookup<number>[] = [];
  bindingModel_PersonalIdType: Lookup<number>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];

  Zone_Loading = false;
  SearchStream: SearchStream = new SearchStream();

  selectMenuOptions = {
    enableSearchFilter: false,
    singleSelect: true
  };

  selectMenuOptionsZone = {
    enableSearchFilter: true,
    singleSelect: true,
    //text: "Zone"
  };

  getZones(searchKeyword: string) {
    this.SearchStream.initStream("ZoneDDL_addCustomer", (a) => {
      this.Zone_Loading = true;
      this.lookupservice.SearchZonesBasedOnAssignedStations(a).subscribe(res => {
        if (res.Value)
          this.zoneList = res.Value;
      }
        , err => {
          this.Zone_Loading = false;
        }
        , () => {
          this.Zone_Loading = false;
        });
    }).next(searchKeyword);
  }

  onServiceDDLChanged(evt) {
    this.DeferredOrder.helper_ServiceTypeId = evt.map(m => m.Id)[0];
    this.DeferredOrder.SERVICETYPE = evt.map(m => m.IntegrationId)[0];
  }

  onTanckerCapacityDDLChanged(evt) {
    this.DeferredOrder.TANKERSIZE = evt.map(m => m.Id)[0];
  }

  onClassDDLChanged(evt) {
    this.DeferredOrder.helper_CustomerClassId = evt.map(m => m.Id)[0];
    this.DeferredOrder.CUSTOMERCLASS = evt.map(m => m.IntegrationId)[0];
  }

  onIdtypeDDLChanged(evt) {
    this.DeferredOrder.helper_PersonIdTypeID = evt.map(m => m.Id)[0];
    this.DeferredOrder.PERSONIDTYPE = evt.map(m => m.IntegrationId)[0];
  }

  onZonesDDLChanged(evt) {
    this.DeferredOrder.helper_ZoneId = evt.map(m => m.Id)[0];
    this.DeferredOrder.helper_ZoneName = evt.map(m => m.Name)[0];
    this.gisReturningZone = null;
  }

  //#endregion Drop Down list





  setDefaultContract() {

    let filters = new DeferredOrderSC;
    filters.Id = this.DeferredOrderId;
    filters.PageFilter.PageSize = 1;
    filters.PageFilter.PageIndex = 1;

    this.mainloading.PreloaderIcreaseCount();
    this.workOrderService.SearchDeferredWorkOrders(filters).subscribe(res => {
      if (res.IsErrorState == false && res.Value.Result.length > 0) {
        this.DeferredOrder = res.Value.Result[0];

        //choose customer class
        if (!isNullOrUndefined(this.DeferredOrder.CUSTOMERCLASS)) {
          this.bindingModel_Classes = this.customerClassList.filter(s => s.IntegrationId == this.DeferredOrder.CUSTOMERCLASS);
          this.DeferredOrder.helper_CustomerClassId = this.bindingModel_Classes[0] ? this.bindingModel_Classes[0].Id : 0;
        }

        //choose Service type
        if (!isNullOrUndefined(this.DeferredOrder.SERVICETYPE)) {
          this.bindingModel_CustomerService = this.customerServiceList.filter(s => s.IntegrationId == this.DeferredOrder.SERVICETYPE);
          this.DeferredOrder.helper_ServiceTypeId = this.bindingModel_CustomerService[0] ? this.bindingModel_CustomerService[0].Id : 0;
        }

        //choose Tancker Capacity
        if (!isNullOrUndefined(this.DeferredOrder.TANKERSIZE)) {
          this.bindingModel_TanckerCapacities = this.TanckerCapacityList.filter(s => s.Id == +this.DeferredOrder.TANKERSIZE);
          if (!this.bindingModel_TanckerCapacities) {
            this.DeferredOrder.TANKERSIZE = '';
          }
        }

        //choose person Id type
        if (!isNullOrUndefined(this.DeferredOrder.PERSONIDTYPE)) {
          this.bindingModel_PersonalIdType = this.PersonalIDList.filter(s => s.IntegrationId == this.DeferredOrder.PERSONIDTYPE);
          this.DeferredOrder.helper_PersonIdTypeID = this.bindingModel_PersonalIdType[0] ? this.bindingModel_PersonalIdType[0].Id : 0;
        }

        let longlat = this.DeferredOrder.XYCOORDINATESGF.split(' ');
        this.DeferredOrder.helper_longitude = +longlat[0];
        this.DeferredOrder.helper_latitude = +longlat[1];

    this.mapAPI.centerMap(this.DeferredOrder.helper_longitude, this.DeferredOrder.helper_latitude, 12);
    let layer = this.mapAPI.createGraphicLayer();
    this.mapAPI.addFeatureOnMap("customer", 'POINT(' + this.DeferredOrder.helper_longitude + ' ' + this.DeferredOrder.helper_latitude + ')', layer.getSource(), 'white', null, null, this.CustomerIconSrc);



        let sdate = this.DeferredOrder.SCHEDDTTM.substring(0, 10).split('/');
        let sTime = this.DeferredOrder.SCHEDDTTM.substring(11).split(':');
        this.DeferredOrder.helper_scheduleTime = new Date(+sdate[2], +sdate[1] - 1 , +sdate[0], +sTime[0], +sTime[1], +sTime[2]);
        this.scheduleTimeStr = this.DeferredOrder.helper_scheduleTime.toTimeString().substring(0, 5);


      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });

  }


  GetZoneByGIS() {

    let coordinates = `${this.DeferredOrder.helper_longitude} ${this.DeferredOrder.helper_latitude}`;

    this.gisLoading = true;
    this.zoneService.CallGISService(coordinates, this.DeferredOrder.ORDERNUMBER, this.DeferredOrder.SOURCEAPPLICATION, this.DeferredOrder.TRANSACTIONID)
      .subscribe(res => {

        if (res.IsErrorState) {
          this._alert.error(res.ErrorDescription);
          this.gisReturningZone = null;
          this.showZoneDDL = true;
        }
        else {
          this.gisReturningZone = res.Value;

          this.DeferredOrder.helper_ZoneId = this.gisReturningZone.ID;
          this.DeferredOrder.helper_ZoneName = this.gisReturningZone.Name;
        }

      }
        , err => {
          this.gisLoading = false;
        }
        , () => {
          this.gisLoading = false;
        });

  }


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    // if (isNullOrUndefined(this.contractorDTO.ContractorFullName)) {
    //   validationMessages.push("ContractorNameRequired");
    // }
    // if (isNullOrUndefined(this.contractorDTO.Code)) {
    //   validationMessages.push("ContractorCodeRequired");
    // }
    // if (isNullOrUndefined(this.contractorDTO.CommercialIDNumber)) {
    //   validationMessages.push("CommercialIDNumberRequired");
    // }
    // if (isNullOrUndefined(this.contractorDTO.TaxNumber)) {
    //   validationMessages.push("TaxNumberRequired");
    // }
    // if (isNullOrUndefined(this.contractorDTO.MOI)) {
    //   validationMessages.push("MOIRequired");
    // }

    // if (isNullOrUndefined(this.contractorDTO.AreaId)) {
    //   validationMessages.push("ChooseArea");
    // }
    // if (isNullOrUndefined(this.contractorDTO.CompanyAddressCityID)) {
    //   validationMessages.push("ChooseCity");
    // }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }


  save() {

    if (!this.isValidModel()) return;

    let scheduleTime = this.scheduleTimeStr.split(":");
    this.DeferredOrder.helper_scheduleTime.setHours(+scheduleTime[0]);
    this.DeferredOrder.helper_scheduleTime.setMinutes(+scheduleTime[1]);


    //alert time zone offset before send
    let modifiedCriteria = Object.assign({}, this.DeferredOrder);
    modifiedCriteria.helper_scheduleTime = new Date(this.DeferredOrder.helper_scheduleTime.getTime());
    modifiedCriteria.helper_scheduleTime.setMinutes(
      modifiedCriteria.helper_scheduleTime.getMinutes() - modifiedCriteria.helper_scheduleTime.getTimezoneOffset());

    this.mainloading.PreloaderIcreaseCount();
    this.workOrderService.EditDeferredOrder(modifiedCriteria).subscribe(response => {
      if (response.IsErrorState === true) {
        this._alert.errorList(response.Errors);
      } else {
        this._alert.showSuccess();
        //this.navigateContractorList();
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });




  }





  cancel() {
    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.navigateToList();
      }
    })
  }

  navigateToList() {
    this.router.navigate(['/tms/integration/deferredorderslist']);
  }


}
