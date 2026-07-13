import { Component, OnInit, ViewEncapsulation, Input } from "@angular/core";
import { OrderDetails } from "../../../../Models/order-details";
import { TranslateService } from "@ngx-translate/core";
import { OrderDetailsService } from "src/app/TMS-Module/Services/order-details.service";
import { Lookup } from "src/app/TMS-Module/Models/common/lookup";
import { EventWorkOrder } from "src/app/TMS-Module/Models/events/event-workorder";
import { AccessoryDTO } from "src/app/TMS-Module/Models/Accessory";
import { LookupService } from "src/app/TMS-Module/Services/lookup.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { LoaderService } from "src/app/shared/loader.service";

@Component({
  selector: "shipment-data",
  templateUrl: "./shipment-data.component.html",
  styleUrls: ["./shipment-data.component.scss"],
  encapsulation: ViewEncapsulation.None
})
export class ShipmentDataComponent implements OnInit {
  isEditmode: boolean = false;
  shipmentData: OrderDetails = new OrderDetails();
  WorkOrder: EventWorkOrder = new EventWorkOrder();

  pagePermission: string = "";

  constructor(
    private trans: TranslateService,
    private lookupService: LookupService,
    private alert: alertService,
    private OrderDetailsService: OrderDetailsService,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName(
      "orderlist"
    );
    //this.authenticationService.checkViewPrivilege(this.pagePermission);
  }

  ngOnInit() {
    this.lookupService.getAccessories().subscribe(res => {
      this.Accessories = res.Value;
    });

    this.lookupService.getPermittedServicesTypes().subscribe(res => {
      if (res.Value != null && res.Value.length > 0)
        this.ServicesTypes = res.Value;
    });

    // this.AccessoryselectedItems = [{Id:1 , Name: 'pump'}];
  }

  @Input("shipmentData") public set data(value: OrderDetails) {
    if (value.WorkOrderID) {
      this.shipmentData = value;
      this.WorkOrder.StationID = value.AssignedStationID;
      this.WorkOrder.CustomerLocationID = value.CustomerLocationID;
      this.WorkOrder.CustomerAccountId = value.CustomerAccountID;
      this.WorkOrder.WorkOrderID = value.WorkOrderID;
      this.WorkOrder.OrderQuantity = value.OrderQuantity;
      this.WorkOrder.ScheduledDeliveryTime = new Date(
        value.ScheduledDeliveryTime
      );
      // this.WorkOrder.CreatedBy = "EB350254-1569-E611-80CF-000D3A23B759";
      this.WorkOrder.ServiceTypeID = value.ServiceTypeID;
      this.OrderDetailsService.GetWorkOrderAccessory(
        this.shipmentData.WorkOrderID
      ).subscribe(res => {
        if (res.Value != null && res.Value.length > 0) {
          this.WorkOrder.Accessories = res.Value;
          this.AccessoryselectedItems = res.Value.map(item => ({
            Id: item.ID,
            Name: item.Name
          } as Lookup<number>));
        }
      });

      this.lookupService
        .GetZoneStations(this.shipmentData.ZoneID)
        .subscribe(res => {
          // console.log("GetZoneStations",res);
          if (res) {
            this.Stations = res.Value;
            //console.log("stations" , this.Stations);
          }
        });

      // this.lookupService
      //   .GetCustomerLocations(this.shipmentData.CustomerID)
      //   .subscribe(res => {
      //     if (res.Value != null && res.Value.length > 0)
      //       this.CustomerLocations = res.Value;
      //   });
      this.lookupService
        .GetCustomerAccountsSameService(this.shipmentData.CustomerID, this.shipmentData.ServiceTypeID)
        .subscribe(res => {
          if (res.Value != null && res.Value.length > 0)
            this.CustomerAccounts = res.Value;
        });

      this.AssignedStation = [
        { Id: value.AssignedStationID, Name: value.StationName } as Lookup<string>
      ];
      // console.log(this.AssignedStation)
      this.AssignedStationName = value.StationName;

      this.selectedServicesType = [
        { Id: value.ServiceTypeID, Name: value.ServiceType } as Lookup<number>
      ];
      this.selectedServicesTypeName = value.ServiceType;

      this.SelectedCustomerAccount = [
        { Id: value.CustomerAccountID, Name: value.CustomerAddress } as Lookup<number>
      ];
      this.SelectedCustomerAccountName = value.CustomerAddress;
      // this.SelectedCustomerLocation = [
      //   { Id: value.CustomerLocationID, Name: value.CustomerAddress } as Lookup<number>
      // ];
      // this.SelectedCustomerLocationName = value.CustomerAddress;

      this.AccessoryNames = value.AccessoryNames;
    }
  }

  onQuantityChange($event) {
    this.WorkOrder.OrderQuantity = $event.target.value;
  }

  onselecteddateChange($event) {
    this.WorkOrder.ScheduledDeliveryTime = new Date($event);
  }

  //#region Drop Down List
  Accessories: Lookup<number>[];
  AccessoryNames: string;
  Stations: Lookup<string>[] = [];
  ServicesTypes: Lookup<number>[];
  //CustomerLocations: Lookup<number>[];
  CustomerAccounts: Lookup<number>[];
  //selected values
  selectedServicesType: Lookup<number>[] = [];
  selectedServicesTypeName: string;
  AssignedStation: Lookup<string>[] = [];
  AssignedStationName: string = "";
  //SelectedCustomerLocationName: string = "";
  SelectedCustomerAccountName: string = "";
  AccessoryselectedItems: Lookup<number>[] = [];
  //SelectedCustomerLocation: Lookup<number>[] = [];
  SelectedCustomerAccount: Lookup<number>[] = [];


  selectMenuOptions = {
    enableSearchFilter: false,
    badgeShowLimit: 1
  };
  selectMenuOptions2 = {
    enableSearchFilter: false,
    singleSelect: true
  };

  Station_loading = false;

  onAssignedStationChanged($event) {
    this.WorkOrder.StationID = $event[0].Id;
  }

  
  // onCustomerLocationChanged($event) {
  //   this.WorkOrder.CustomerLocationID = $event[0].Id;

  //   this.WorkOrder.StationID = null;
  //   this.Stations = [];
  //   this.AssignedStation = [];
  //   this.AccessoryselectedItems = [];

  //   if (!isNullOrUndefined(this.WorkOrder.CustomerLocationID)) {
  //     //this.Station_loading = true;
  //     this.mainloading.PreloaderIcreaseCount();
  //     this.lookupService
  //       .getCustomerLocationStations(this.WorkOrder.CustomerLocationID)
  //       .subscribe(
  //         res => {
  //           if (res.Value != null && res.Value.length > 0)
  //             this.Stations = res.Value;
  //           else this.Stations = [];
  //         },
  //         err => {
  //           this.mainloading.PreloaderDecreaseCount();
  //         },
  //         () => {
  //           //this.Station_loading = false;
  //           this.mainloading.PreloaderDecreaseCount();
  //         }
  //       );
  //   } else {
  //     this.AssignedStation = [];
  //   }
  // }
  onCustomerAccountChanged($event) {
    this.WorkOrder.CustomerAccountId = $event[0].Id;

    this.WorkOrder.StationID = null;
    this.Stations = [];
    this.AssignedStation = [];
    this.AccessoryselectedItems = [];

    if (!isNullOrUndefined(this.WorkOrder.CustomerAccountId)) {
      //this.Station_loading = true;
      this.mainloading.PreloaderIcreaseCount();
      this.lookupService
        .getCustomerAccountStations(this.WorkOrder.CustomerAccountId)
        .subscribe(
          res => {
            if (res.Value != null && res.Value.length > 0)
              this.Stations = res.Value;
            else this.Stations = [];
          },
          err => {
            this.mainloading.PreloaderDecreaseCount();
          },
          () => {
            //this.Station_loading = false;
            this.mainloading.PreloaderDecreaseCount();
          }
        );
    } else {
      this.AssignedStation = [];
    }
  }


  onNecessaryAccessoriesChanged($event) {
    this.WorkOrder.Accessories = $event.map(item => new AccessoryDTO(item));
  }

  onOperationTypeChanged($event) {
    this.WorkOrder.ServiceTypeID = $event[0].Id;
  }

  //#endregion Drop Down List

  allowEdit() {
    if (this.shipmentData.LastStatusID == 2) {
      // on hold
      this.isEditmode = !this.isEditmode;
      //this.selectMenuOptions.disabled = !this.selectMenuOptions.disabled;
    }
  }

  save() {
    let ModifiedOrder = Object.assign({}, this.WorkOrder);
    ModifiedOrder.ScheduledDeliveryTime = new Date(
      this.WorkOrder.ScheduledDeliveryTime.getTime()
    );
    ModifiedOrder.ScheduledDeliveryTime.setMinutes(
      ModifiedOrder.ScheduledDeliveryTime.getMinutes() -
        ModifiedOrder.ScheduledDeliveryTime.getTimezoneOffset()
    );

    this.mainloading.PreloaderIcreaseCount();
    this.OrderDetailsService.UpdateWorkOrderShipment(ModifiedOrder).subscribe(
      res => {
        if (res.IsErrorState == false) {
          this.AssignedStationName = this.AssignedStation[0].Name;
          this.SelectedCustomerAccountName = this.SelectedCustomerAccount[0].Name;
          //this.SelectedCustomerLocationName = this.SelectedCustomerLocation[0].Name;
          this.selectedServicesTypeName = this.selectedServicesType[0].Name;
          this.AccessoryNames = this.AccessoryselectedItems.map(
            s => s.Name
          ).join(", ");

          this.alert.showSuccess();
          this.allowEdit();
        } else {
          this.alert.errorList(res.Errors);
        }
      },
      err => {
        this.mainloading.PreloaderDecreaseCount();
      },
      () => {
        this.mainloading.PreloaderDecreaseCount();
      }
    );
  }
}
