import { Component, OnInit, OnDestroy } from '@angular/core';
import { Lookup } from '../../..//Models/common/lookup';
import { alertService } from '../../../../shared/Services/alert/alert.service';
import { LookupService } from '../../../Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { EventWorkOrder } from '../../../Models/events/event-workorder';
import { OrderDetailsService } from '../../../Services/order-details.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-order-create',
  templateUrl: './order-create.component.html',
  styleUrls: ['./order-create.component.scss']
})
export class OrderCreateComponent implements OnInit, OnDestroy {

  eventOrder: EventWorkOrder;
  customerId: number;

  pagePermission: string = '';
  bulkCreate = false;
  commercialOrder = false;
  BC_StartingTimeString: string;
  OrdersNoPerMonth: number;
  isSewerServiceType = false;

  constructor(
    private _alert: alertService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private orderService: OrderDetailsService,
    private router: Router,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService,
    private activatedsrRoute: ActivatedRoute,

  ) {

    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('orderlist');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

    this.bulkCreate = this.activatedsrRoute.snapshot.routeConfig.path == "bulkCreate";
    if(this.activatedsrRoute.snapshot.routeConfig.path == "commercialOrder"){
      this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('commercialOrder');
      this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

      this.commercialOrder = true;
      this.bulkCreate = true;
    }
  }

  ngOnInit() {
    this.setDefaultContract();
    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });

  }

  loadDDLsGV() {
    console.log(this.bulkCreate, this.commercialOrder);
    this.titleService.setTitle(this._translate.instant((this.bulkCreate ? this.commercialOrder ? 'CommercialOrder' : 'SpectialOrders' : 'createOrder')));

    // load Search DDls values
    //this.getServicesTypes();
    //this.getCustomers('');

    this.getAccessories();

    //this.getTanckerCapacities();

  }

  setDefaultContract() {
    this.eventOrder = new EventWorkOrder();

    let startDate = new Date();
    this.eventOrder.BC_StartingTime = startDate;
    this.BC_StartingTimeString = startDate.toTimeString().substring(0, 5);
    if (this.bulkCreate) {
      this.setDefaultCategory(1);
      this.eventOrder.BC_NoOfOrders = 1;
      this.eventOrder.BC_HoldIntervalMin = 0;
    }
  }

  //#region Drop Down list
  SearchStream: SearchStream = new SearchStream();

  //#region  "Lookups declarations"
  customerNameList: Lookup<number>[] = [];
  //customerLocationsList: Lookup<number>[] = [];
  customerAccountsList: Lookup<number>[] = [];
  lacationStationsList: Lookup<string>[] = [];
  customerServiceList: Lookup<number>[] = [];
  accessoriesList: Lookup<number>[] = [];
  categoriesList: Lookup<number>[] = [];
  TanckerCapacityList: Lookup<number>[] = [];
  //#endregion  "Lookups declarations"

  //#region "LookupsBindingModels"
  //bindingModel_CustomerLocations: Lookup<number>[] = [];
  bindingModel_CustomerAccounts: Lookup<number>[] = [];
  bindingModel_lacationStations: Lookup<string>[] = [];
  bindingModel_TanckerCapacity: Lookup<number>[] = [];
  //#endregion "LookupsBindingModels"
  bindingModel_Category: Lookup<number>[] = [];

  selectMenuOptions = {
    enableSearchFilter: false,    
  };
  selectMenuOptions2 = {
    enableSearchFilter: true,
    singleSelect: true
  };

  selectMenuOptions3 = {
    enableSearchFilter: false,
    singleSelect: true
  };

  Customer_Loading = false;
  CustomerAccount_Loading = false;
  //CustomerAddress_Loading = false;
  Station_Loading = 0;
  Capacity_Loading = false;

  //#region  "get Lookups"

  getCustomers(searchKeyword: string) {
    this.SearchStream.initStream("CustomerDDL_createOrder", (a) => {
      this.Customer_Loading = true;
      let customerObserve = this.commercialOrder ? this.lookupservice.getCommercialCustomers(a) : this.lookupservice.getCustomers(a);
      customerObserve.subscribe(res => {
        if (res.Value)
          this.customerNameList = res.Value;
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }

  getAccessories() {
    this.lookupservice.getAccessories().subscribe(res => {
      if (res.Value)
        this.accessoriesList = res.Value;
    });
  }

  getCategories(serviceType) {
    this.lookupservice.GetWorkOrderCategory().subscribe(res => {
      if (res.Value) {
        var SewerCategories = [11, 12];
        var List = serviceType == 1 ? res.Value.filter(x => !SewerCategories.includes(x.Id)) :  res.Value.filter(x => SewerCategories.includes(x.Id));
        this.categoriesList = List;
        this.setDefaultCategory(serviceType);
      }
    });
  }
 
  setDefaultCategory(serviceType) {
    if (this.categoriesList && this.categoriesList.length) {
      this.eventOrder.CategoryID = serviceType == 3 ? 11 :1;
      this.bindingModel_Category = this.categoriesList.filter(a => a.Id == this.eventOrder.CategoryID);
    }
  }


  getPermittedServicesTypes(serviceType) {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value) {
        this.customerServiceList = res.Value.filter(x => x.Id == serviceType);
        //this.setDefaultCategory();
      }
    });
  }

  //#endregion "get Lookups"

  //#region  "on Lookups change"
  onCustomerDDLChanged(evt) {
    this.customerId = evt.map(m => m.Id)[0];
    //this.eventOrder.CustomerLocationID = null;
    this.eventOrder.CustomerAccountId = null;
    this.eventOrder.StationID = null;
    //this.bindingModel_CustomerLocations = [];
    this.bindingModel_CustomerAccounts = [];
    this.bindingModel_lacationStations = [];
    this.lacationStationsList = [];

    if (!isNullOrUndefined(this.customerId)) {
      this.CustomerAccount_Loading = true;
      let accountObserve = this.commercialOrder ? this.lookupservice.getCustomerCommercialAccountsAddOrderPage(this.customerId) : 
                            this.lookupservice.GetCustomerAccountsAddOrderPage(this.customerId);
      accountObserve.subscribe(res => {
        if (res.Value != null && res.Value.length > 0)
          this.customerAccountsList = res.Value;
        //this.customerLocationsList = res.Value;
        else
          this.customerAccountsList = [];
        //this.customerLocationsList = [];

      }
        , err => {
          this.CustomerAccount_Loading = false;
        }
        , () => {
          this.CustomerAccount_Loading = false;
        });
    }
    else {
      this.customerAccountsList = [];
      //this.customerLocationsList = [];
    }

  }

  onCustomerAccountDDLChanged(evt, IsBulk) {

    this.eventOrder.CustomerAccountId = evt.map(m => m.Id)[0];
    this.eventOrder.StationID = null;
    this.bindingModel_lacationStations = [];
    this.TanckerCapacityList = [];
    this.bindingModel_TanckerCapacity = [];
    this.eventOrder.OrderQuantity = null;

    this.lacationStationsList = [];

    this.getStationsIfValid(IsBulk);

    var ServiceType = evt[0].Name.includes("أشياب") ? 1 : 3;
    console.log("ServiceType:" + ServiceType);
    this.getPermittedServicesTypes(ServiceType);
    this.getCategories(ServiceType);
  }

  getStationsIfValid(IsBulk) {
    if (this.eventOrder.CustomerAccountId)   {
      this.Station_Loading += 1;

      forkJoin(
        [
          this.orderService.IsCustomerBlacklisted(this.eventOrder.CustomerAccountId),
          this.orderService.IsZoneWithoutTankers(this.eventOrder.CustomerAccountId),
          this.orderService.GetNoOfOrdersForThisMonth(this.eventOrder.CustomerAccountId)
        ]
      ).subscribe(([blacklisted, zoneWithoutTanker, noOfOrders]) => {

        let validationMessages: string[] = [];

        if (blacklisted.Value == true ) {
          validationMessages.push("CreateOrderBlacklistedCustomerAccountMsg");
        }
        if ((zoneWithoutTanker.Value == true || zoneWithoutTanker.IsErrorState) && !IsBulk ) {
          validationMessages.push("CreateOrder_ZoneWithoutTankerMsg");
        }

        if (!noOfOrders.IsErrorState && !isNullOrUndefined(noOfOrders.Value)) {
          this.OrdersNoPerMonth = noOfOrders.Value;
        }

        //**************** */
        if (validationMessages.length > 0) {
          this._alert.errorList(validationMessages);
          return false;
        } else {
          this.getStations();
        }

      }
        , err => {
          this.Station_Loading -= 1;
        }
        , () => {
          this.Station_Loading -= 1;
        }

      )

      // this.orderService.IsCustomerBlacklisted(this.eventOrder.CustomerAccountId).subscribe(res => {
      //   // stop only if response is true "blaclisted"
      //   // if exception or service fail to response continue without stop
      //   if (res.IsErrorState || res.Value == false) {
      //     this.getStations();
      //   }
      //   else if (res.Value == true) {
      //     this._alert.error("CreateOrderBlacklistedCustomerAccountMsg");
      //   }

      // }
      //   , err => {
      //     this.Station_Loading = false;
      //   }
      //   , () => {
      //     this.Station_Loading = false;
      //   }

      // )


    }
  }

  getStations() {
    if (!isNullOrUndefined(this.eventOrder.CustomerAccountId)) {
      this.Station_Loading += 1;
      this.lookupservice.getCustomerAccountStations(this.eventOrder.CustomerAccountId).subscribe(res => {
        if (res.Value != null && res.Value.length > 0) {
          this.lacationStationsList = res.Value;

          this.eventOrder.StationID = this.lacationStationsList[0].Id;
          this.bindingModel_lacationStations = this.lacationStationsList.filter(s => s.Id == this.eventOrder.StationID);

          this.getCapacitiesIfNotExceedOrdersCount();

        }
      }
        , err => {
          this.Station_Loading -= 1;
        }
        , () => {
          this.Station_Loading -= 1;
        });
    }
  }

  onlocationStationDDLChanged(evt) {
    this.eventOrder.StationID = evt.map(m => m.Id)[0];
    this.bindingModel_TanckerCapacity = [];
    this.eventOrder.OrderQuantity = null;
    this.TanckerCapacityList = [];

    this.getCapacitiesIfNotExceedOrdersCount();
  }

  getCapacitiesIfNotExceedOrdersCount() {
    if (!isNullOrUndefined(this.eventOrder.StationID) && !isNullOrUndefined(this.customerId)) {
      this.Capacity_Loading = true;
      this.orderService.IsCustomerExceededQuota(this.eventOrder.StationID, this.customerId).subscribe(res => {

        if (!res.IsErrorState && res.Value == false) {
          this.getCapacities();
        }
        else if (res.Value == true) {
          this._alert.error("CreateOrderExceedQuata");
        }

      }
        , err => {
          this.Capacity_Loading = false;
        }
        , () => {
          this.Capacity_Loading = false;
        });
    }
  }

  getCapacities() {
    if (!isNullOrUndefined(this.eventOrder.StationID) && !isNullOrUndefined(this.eventOrder.CustomerAccountId)) {
      this.Capacity_Loading = true;
      this.lookupservice.GetTanckerCapacitiesByStation(this.eventOrder.StationID, this.eventOrder.CustomerAccountId).subscribe(res => {
        if (res.Value != null && res.Value.length > 0)
          this.TanckerCapacityList = res.Value;
        else
          this.TanckerCapacityList = [];

      }
        , err => {
          this.Capacity_Loading = false;
        }
        , () => {
          this.Capacity_Loading = false;
        });
    }
    else {
      this.TanckerCapacityList = [];
    }
  }


  onAccessoriesDDLChanged(evt) {
    this.eventOrder.Accessories = [];
    evt.forEach(element => {
      this.eventOrder.Accessories.push(element);
    });
  }

  onCategoriesDDLChanged(evt) {
    this.eventOrder.CategoryID = evt.map(m => m.Id)[0];
  }

  onServiceTypeDDLChanged(evt) {
    this.eventOrder.ServiceTypeID = evt.map(m => m.Id)[0];
    this.isSewerServiceType = this.eventOrder.ServiceTypeID == 3;
  }

  onTanckerCapacityDDLChanged(evt) {
    //this.SearchCriteria.TanckerCapacityAddIds = evt.map(m => m.Id);
    this.eventOrder.OrderQuantity = evt.map(m => m.Id)[0];
  }

  //#endregion  "on Lookups change"


  //#endregion Drop Down list


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.customerId) || (+this.customerId == 0)) {
      validationMessages.push("ChooseCustomer");
    }
    // if (isNullOrUndefined(this.eventOrder.CustomerLocationID) || (+this.eventOrder.CustomerLocationID == 0)) {
    //   validationMessages.push("ChooseCustomerLocation");
    // }
    if (isNullOrUndefined(this.eventOrder.CustomerAccountId) || (+this.eventOrder.CustomerAccountId == 0)) {
      validationMessages.push("ChooseCustomerAccount");
    }
    if (isNullOrUndefined(this.eventOrder.StationID) || (+this.eventOrder.StationID == 0)) {
      validationMessages.push("ChooseStation");
    }
    if (isNullOrUndefined(this.eventOrder.ServiceTypeID) || (+this.eventOrder.ServiceTypeID == 0)) {
      validationMessages.push("ChooseServiceType");
    }
    if (isNullOrUndefined(this.eventOrder.OrderQuantity) || (+this.eventOrder.OrderQuantity < 1)) {
      validationMessages.push("InvalidQuantity");
    }

    let ex = new RegExp(Configuration.RegExp.ValidMobileNumber);
    if (this.eventOrder.RecieverMobile && !ex.test(this.eventOrder.RecieverMobile)) {
      validationMessages.push("InvalidMobileNumber");
    }

    if (this.bulkCreate) {
      if (!this.eventOrder.BC_NoOfOrders || +this.eventOrder.BC_NoOfOrders < 1) {
        validationMessages.push("PleaseEnterValidNoOfOrders");
      }

      if (isNullOrUndefined(this.eventOrder.BC_HoldIntervalMin) || +this.eventOrder.BC_HoldIntervalMin < 0) {
        validationMessages.push("PleaseEnterValidTimeDiffBetweenEachOrder");
      }

      let today = new Date();
      if (!this.eventOrder.BC_StartingTime
        || this.eventOrder.BC_StartingTime.getFullYear() < today.getFullYear()
        || this.eventOrder.BC_StartingTime.getMonth() < today.getMonth()
        || this.eventOrder.BC_StartingTime.getDate() < today.getDate()
      ) {
        validationMessages.push("PleaseEnterValidStartingDate");
      }

      let startingTime = this.BC_StartingTimeString.split(":");
      if (
        startingTime.length !== 2 ||
        isNullOrUndefined(startingTime[0]) ||
        startingTime[0] === "" ||
        isNullOrUndefined(startingTime[1]) ||
        startingTime[1] === ""
      ) {
        validationMessages.push("PleaseEnterValidStartingTime");
      }


    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  save() {
    if (!this.isValidModel()) return;

    this.eventOrder.SourceApplication = 'TMS';
    
    if (this.bulkCreate) {

      let timeFrom = this.BC_StartingTimeString.split(":");
      this.eventOrder.BC_StartingTime.setHours(+timeFrom[0]);
      this.eventOrder.BC_StartingTime.setMinutes(+timeFrom[1]);

      //alert time zone offset before send
      let clonedObj = Object.assign({}, this.eventOrder);
      clonedObj.BC_StartingTime = new Date(
        this.eventOrder.BC_StartingTime.getTime()
      );
      clonedObj.BC_StartingTime.setMinutes(
        clonedObj.BC_StartingTime.getMinutes() -
        clonedObj.BC_StartingTime.getTimezoneOffset()
      );

      this.mainloading.PreloaderIcreaseCount();
      this.orderService.BulkCreateWorkOrder(clonedObj).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();

          if (response.Value.failed) {
            let msg: string;
            if (this._translate.currentLang == 'ar') {
              msg = `تم تسجيل ${response.Value.success} طلب بينما لم يتم تسجيل ${response.Value.failed} طلب`;
            }
            else {
              msg = `${response.Value.success} records were added successfully , while ${response.Value.failed} records failed.`;
            }
            this._alert.warning(msg);
          }

          this.navigateToList();
        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });





    }
    else {
      if( this.eventOrder.ServiceTypeID == 3)
      { 
        this.eventOrder.Accessories = [];
      }
      this.mainloading.PreloaderIcreaseCount();
      this.orderService.CreateWorkOrder(this.eventOrder).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          this.navigateToList();
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


  cancel() {
    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.navigateToList();
      }
    })
  }

  navigateToList() {
    this.router.navigate(['/tms/order/orderlist']);
  }

  // onQuantityChanged() {
  //   if (isNaN(+this.eventOrder.OrderQuantity) || +this.eventOrder.OrderQuantity < 0) {
  //     this.eventOrder.OrderQuantity = 1;
  //   } else if (+this.eventOrder.OrderQuantity > 50) {
  //     this.eventOrder.OrderQuantity = 50;
  //   } else {
  //     this.eventOrder.OrderQuantity = Math.floor(this.eventOrder.OrderQuantity);
  //   }

  // }

  onNoOfOrdersChanged() {
    if (isNaN(+this.eventOrder.BC_NoOfOrders) || +this.eventOrder.BC_NoOfOrders < 0) {
      this.eventOrder.BC_NoOfOrders = 1;
    } else {
      this.eventOrder.BC_NoOfOrders = Math.floor(this.eventOrder.BC_NoOfOrders);
    }
  }

  onMinutesDiffBetweenEachOrderChanged() {
    if (isNaN(+this.eventOrder.BC_HoldIntervalMin) || +this.eventOrder.BC_HoldIntervalMin < 0) {
      this.eventOrder.BC_HoldIntervalMin = 0;
    } else {
      this.eventOrder.BC_HoldIntervalMin = Math.floor(this.eventOrder.BC_HoldIntervalMin);
    }
  }


  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

}
