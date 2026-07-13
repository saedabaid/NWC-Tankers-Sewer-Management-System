import { OrderComplaint } from 'src/app/TMS-Module/Models/order-compaint';
import { Component, OnInit, ViewEncapsulation, Inject, LOCALE_ID } from '@angular/core';
import { ChangeOrderStatusComponent } from '../change-order-status/change-order-status.component';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { OrderDetailsService } from 'src/app/TMS-Module/Services/order-details.service';
import { OrderDetails } from 'src/app/TMS-Module/Models/order-details';
import { OrderComment } from 'src/app/TMS-Module/Models/order-comment';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { MapService } from 'src/app/shared/Services/mapService/map-service';
import { RouteObject } from 'src/app/shared/datamodels/RouteObject';
import { WorkOrderPlannedRoutDTO } from 'src/app/TMS-Module/Models/work-order-planned-rout';
import { AddOrderCommentComponent } from '../add-order-comment/add-order-comment.component';
import { ManualDispatchTankerComponent } from '../manual-dispatch-tanker/manual-dispatch-tanker.component';
import { DeassignTankerComponent } from '../deassign-tanker/deassign-tanker.component';
import { WorkOrderChangeLog } from 'src/app/TMS-Module/Models/work-order-change-log';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss'],
  encapsulation: ViewEncapsulation.None

})

export class OrderDetailsComponent implements OnInit {
  currentTab = 1;
  modalRef: BsModalRef;
  closeBtnName: string;
  WorkOrderID: string;
  orderPlacedOnDate: Date;
  OrderBasicDetails: OrderDetails = new OrderDetails();
  OrderComments: OrderComment[];
  OrderComplaints: OrderComplaint[];
  CustomerIconSrc: string = "assets/TMSBranding/styles/img/start-ico.png";
  StationIconSrc: string = "assets/TMSBranding/styles/img/end-ico.png";
  public ChangesLogs: WorkOrderChangeLog[];

  //StatusReason: string;

  pagePermission: string = '';

  constructor(private router: Router, private route: ActivatedRoute,
    private mapAPI: MapService, private modalService: BsModalService,
    private authServer: AuthenticationService,
    private OrderDetailsService: OrderDetailsService,
    private translate: TranslateService, private AuthenticationService: AuthenticationService,
    private titleService: Title,
    private mainloading: LoaderService) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('orderlist');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.WorkOrderID = this.route.snapshot.paramMap.get("ID");
    this.load();
    this.translate.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {

    this.titleService.setTitle(this.translate.instant('OrderDetails'));


    if (this.WorkOrderID) {
      this.mainloading.PreloaderIcreaseCount();
      this.OrderDetailsService.getOrderComments(parseInt(this.WorkOrderID)).subscribe(res => {

        if (res.Value != null) {
          this.OrderComments = res.Value;
        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

      this.mainloading.PreloaderIcreaseCount();
      this.OrderDetailsService.getOrderComplaints(parseInt(this.WorkOrderID)).subscribe(res => {
        if (res.Value != null) {
          this.OrderComplaints = res.Value;
        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

      this.mainloading.PreloaderIcreaseCount();
      this.OrderDetailsService.getOrderDetails(parseInt(this.WorkOrderID)).subscribe(res => {

        if (res.Value != null) {
          //debugger
          this.orderPlacedOnDate = res.Value.orderPlacedOn;
          this.OrderBasicDetails = res.Value;
          this.OrderBasicDetails.ScheduledDeliveryTime = new Date(res.Value.ScheduledDeliveryTime);
          let source = { "Latitude": this.OrderBasicDetails.customerLocationLat, "Longitude": this.OrderBasicDetails.customerLocationLng };
          let desc = { "Latitude": this.OrderBasicDetails.StationLat, "Longitude": this.OrderBasicDetails.StationLng };
          if (this.OrderBasicDetails.RouteLatLngString == null) {
            this.mapAPI.calculateRoute(source, desc);
          }
          else {
            this.drawRoute(this.OrderBasicDetails.RouteLatLngString);
          }

          // let myStatus = this.OrderBasicDetails.WorkOrderStatusLogs.find(m => m.ActionLogTypeID == 8);
          // this.StatusReason = myStatus ? myStatus.StatusReason : '';

        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });
    }

    this.mapAPI.RoutingResultEvent.subscribe(routeData => {
      var WorkOrderPlannedRout = Object.assign(new WorkOrderPlannedRoutDTO(), {
        WorkOrderID: this.OrderBasicDetails.WorkOrderID,
        CreatedTime: new Date(),
        IsDeleted: false,
        RouteLatLngString: routeData.route,
        DrivingTime: routeData.duration,
        Distance: routeData.distance,
      });

      this.drawRoute(routeData.route);
      this.mapAPI.SaveOrderRoute(WorkOrderPlannedRout);
    });
  }
  switchTab(tabName) {
    this.currentTab = tabName;
  }

  drawRoute(routeData: string) {
    var currentRoute = ' {   "RouteLine" : { "LineGeometry": "' + routeData + '"}}';

    let router: RouteObject = new RouteObject();
    router.RouteName = currentRoute;
    this.mapAPI.drawRouteInMap(router, null);
    this.mapAPI.centerMap(parseFloat(this.OrderBasicDetails.customerLocationLng), parseFloat(this.OrderBasicDetails.customerLocationLat), 12);

    let layer = this.mapAPI.createGraphicLayer();
    this.mapAPI.addFeatureOnMap("customer", 'POINT(' + this.OrderBasicDetails.customerLocationLng + ' ' + this.OrderBasicDetails.customerLocationLat + ')', layer.getSource(), '#fff', null, null, this.CustomerIconSrc);
    this.mapAPI.addFeatureOnMap("station", 'POINT(' + this.OrderBasicDetails.StationLng + ' ' + this.OrderBasicDetails.StationLat + ')', layer.getSource(), '#fff', null, null, this.StationIconSrc);
  }

  addComment() {
    let modelProp = {

      class: this.translate.currentLang == 'ar' ? 'change-order-modal rtl armodal' : 'change-order-modal ',
      initialState: {
        name: "status"
      }
    }

    this.modalRef = this.modalService.show(AddOrderCommentComponent, modelProp);
    this.modalRef.content.closeBtnName = 'Cancel';
    this.modalRef.content.orderId = this.WorkOrderID;
    this.modalRef.content.OrderComments.subscribe(res => {
      this.OrderComments = res;
      this.modalRef.hide();
    });
  }

  changeOrderStatus() {
    let modelProp: ModalOptions = {

      initialState: this.OrderBasicDetails,
      class: this.translate.currentLang == 'ar' ? "change-order-modal rtl" : "change-order-modal"
    }
    this.modalRef = this.modalService.show(ChangeOrderStatusComponent, modelProp);
    this.modalRef.content.isVirtualModel = this.OrderBasicDetails.IsVirtualStation;
    this.modalRef.content.orderStatus = this.OrderBasicDetails.LastStatusID;
    this.modalRef.content.closeBtnName = 'Cancel';
    this.modalRef.content.StatusChanged.subscribe(res => {

      this.ngOnInit();

    });
  }

  cancelWorkOrder() {

    if (![1, 2, 3, 5].includes(this.OrderBasicDetails.LastStatusID)) {
      return;
    }

    let modelProp: ModalOptions = {

      class: this.translate.currentLang == 'ar' ? "change-order-modal rtl" : "change-order-modal",

      initialState: this.OrderBasicDetails,
    }
    this.modalRef = this.modalService.show(ChangeOrderStatusComponent, modelProp);
    this.modalRef.content.closeBtnName = 'Cancel';
    this.modalRef.content.cancelMode = true;
    this.modalRef.content.StatusChanged.subscribe(res => {
      this.ngOnInit();
    });
  }

  deAssignTanker() {
    let modelProp = {
      class: this.translate.currentLang == 'ar' ? "assigntanker rtl" : "assigntanker",

      initialState: {
        name: 'tanker',
        order: this.OrderBasicDetails,

      }
    }

    this.modalRef = this.modalService.show(DeassignTankerComponent, modelProp);
    this.modalRef.content.deassignWorkorder.subscribe(res => {
      this.modalRef.hide();
      this.ngOnInit();
      // this.router.navigateByUrl(this.router.routerState.snapshot.url, { skipLocationChange: true }).then(() => {
      //   this.router.navigate([this.router.routerState.snapshot.url]);
      // });
    });
  }

  dispatchTanker() {
    let modelProp = {

      class: this.translate.currentLang == 'ar' ? "dispatchTanker rtl" : "dispatchTanker",
      initialState: {
        name: 'dispatch',
        order: this.OrderBasicDetails
      }
    }

    this.modalRef = this.modalService.show(ManualDispatchTankerComponent, modelProp);
    this.modalRef.content.dispatch.subscribe(res => {
      if (res == true) {
        this.modalRef.hide();
        this.ngOnInit();
      }
    });
  }

  onItemSelect(item: any) {
  }

  onLogsChanged(event) {
    this.ChangesLogs = event;
  }
}
