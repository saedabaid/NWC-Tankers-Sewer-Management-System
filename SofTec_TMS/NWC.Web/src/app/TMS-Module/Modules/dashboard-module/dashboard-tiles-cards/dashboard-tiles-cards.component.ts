import { Component, OnInit, OnDestroy } from '@angular/core';
import { DashboardService } from 'src/app/TMS-Module/Services/dashboard.service';
import { Subscription } from 'rxjs';
import { DashboardSC } from 'src/app/TMS-Module/Models/search-criteria/dashboard-SC.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard-tiles-cards',
  templateUrl: './dashboard-tiles-cards.component.html',
  styleUrls: ['./dashboard-tiles-cards.component.scss']
})
export class DashboardTilesCardsComponent implements OnInit, OnDestroy {

  ImageUrl= "assets/TMSBranding/styles/img/loader.gif";
  searchSubscriper: Subscription;

  startDate: number;
  endDate: number;
  serviceId: number;

  //#region loading & count

  loading_AllOrders = false;
  loading_New = false;
  loading_Assigned = false;
  loading_OutForDelivery = false;
  loading_Cancelled = false;
  loading_Delivered = false;
  loading_FailedToDeliver = false;
  loading_Onhold = false;

  count_AllOrders = 0;
  count_New = 0;
  count_Assigned = 0;
  count_OutForDelivery = 0;
  count_Cancelled = 0;
  count_Delivered = 0;
  count_FailedToDeliver = 0;
  count_Onhold = 0;

  //#endregion

  constructor(
    private dashboardService: DashboardService,
    private router: Router

  ) { }

  ngOnInit() {

    this.searchSubscriper = this.dashboardService.mainSearchClicked$.subscribe(res => {

      if (res != null) {

        this.startDate = (res.DateFrom.getFullYear() * 10000) + (res.DateFrom.getMonth() * 100) + res.DateFrom.getDate();
        this.endDate = (res.DateTo.getFullYear() * 10000) + (res.DateTo.getMonth() * 100) + res.DateTo.getDate();
        this.serviceId =  res.ServiceTypeID;

        this.getCounts_AllOrders(res);
        this.getCounts_New(res);
        this.getCounts_Assigned(res);
        this.getCounts_OutForDelivery(res);
        this.getCounts_Cancelled(res);
        this.getCounts_Delivered(res);
        this.getCounts_FailedToDeliver(res);
        this.getCounts_Onhold(res);
      } else {

        this.startDate = null;
        this.endDate = null;

        this.count_AllOrders = 0;
        this.count_New = 0;
        this.count_Assigned = 0;
        this.count_OutForDelivery = 0;
        this.count_Cancelled = 0;
        this.count_Delivered = 0;
        this.count_FailedToDeliver = 0;
        this.count_Onhold = 0;
      }

    });

  }

  ngOnDestroy(): void {
    this.searchSubscriper.unsubscribe();
  }

  getCounts_AllOrders(filters: DashboardSC) {

    filters.StatusIDs = null;
    this.loading_AllOrders = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_AllOrders = res.Value;

    }
    , err => {
      this.loading_AllOrders = false;
    },() => {
      this.loading_AllOrders = false;
    });

  }

  getCounts_New(filters: DashboardSC) {

    filters.StatusIDs = [1];
    this.loading_New = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_New = res.Value;

    }
    , err => {
      this.loading_New = false;
    },() => {
      this.loading_New = false;
    });

  }

  getCounts_Assigned(filters: DashboardSC) {

    filters.StatusIDs = [5];
    this.loading_Assigned = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_Assigned = res.Value;

    }
    , err => {
      this.loading_Assigned = false;
    },() => {
      this.loading_Assigned = false;
    });

  }

  getCounts_OutForDelivery(filters: DashboardSC) {

    filters.StatusIDs = [6];
    this.loading_OutForDelivery = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_OutForDelivery = res.Value;

    }
    , err => {
      this.loading_OutForDelivery = false;
    },() => {
      this.loading_OutForDelivery = false;
    });

  }

  getCounts_Cancelled(filters: DashboardSC) {

    filters.StatusIDs = [8];
    this.loading_Cancelled = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_Cancelled = res.Value;

    }
    , err => {
      this.loading_Cancelled = false;
    },() => {
      this.loading_Cancelled = false;
    });

  }

  getCounts_Delivered(filters: DashboardSC) {

    filters.StatusIDs = [4];
    this.loading_Delivered = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_Delivered = res.Value;

    }
    , err => {
      this.loading_Delivered = false;
    },() => {
      this.loading_Delivered = false;
    });

  }

  getCounts_FailedToDeliver(filters: DashboardSC) {

    filters.StatusIDs = [3];
    this.loading_FailedToDeliver = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_FailedToDeliver = res.Value;

    }
    , err => {
      this.loading_FailedToDeliver = false;
    },() => {
      this.loading_FailedToDeliver = false;
    });

  }

  getCounts_Onhold(filters: DashboardSC) {

    filters.StatusIDs = [2];
    this.loading_Onhold = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_Onhold = res.Value;

    }
    , err => {
      this.loading_Onhold = false;
    },() => {
      this.loading_Onhold = false;
    });

  }


  tileClicked(statusId) {

    if(this.startDate && this.endDate && this.serviceId) {
      let targetLink = `/tms/order/orderlist/${statusId}-${this.startDate}-${this.endDate}-${this.serviceId}`;
      let url = this.router.serializeUrl(this.router.createUrlTree([targetLink]));
      window.open(url, '_blank');
    }
  }

}
