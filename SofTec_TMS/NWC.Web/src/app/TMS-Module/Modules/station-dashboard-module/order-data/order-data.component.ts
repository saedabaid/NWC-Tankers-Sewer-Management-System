import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardSC } from '@tms-models/search-criteria/dashboard-SC.model';
import { DashboardService } from '@tms-services/dashboard.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-order-data',
  templateUrl: './order-data.component.html',
  styleUrls: ['./order-data.component.scss']
})
export class OrderDataComponent implements OnInit {

  ImageUrl= "assets/TMSBranding/styles/img/loader.gif";

  ImgAll ="assets/TMSBranding/styles/img/svg/all.png";
  ImgNew = "assets/TMSBranding/styles/img/svg/new.png";
  ImgAss = "assets/TMSBranding/styles/img/svg/assigned.png";
  ImgDlv = "assets/TMSBranding/styles/img/svg/delivery.png";
  ImgArr = "assets/TMSBranding/styles/img/svg/location.png";
  ImgDvd = "assets/TMSBranding/styles/img/svg/delivered.png";
  ImgHld = "assets/TMSBranding/styles/img/svg/hold.png";
  
  searchSubscriper: Subscription;

  startDate: number;
  endDate: number;
  serviceId: number;

  //#region loading & count

  loading_AllOrders = false;
  loading_New = false;
  loading_Assigned = false;
  loading_OutForDelivery = false;
  loading_Arrived = false;
  loading_Delivered = false;
  loading_FailedToDeliver = false;
  loading_Onhold = false;

  count_AllOrders = 0;
  count_New = 0;
  count_Assigned = 0;
  count_OutForDelivery = 0;
  count_Arrived = 0;
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
        this.count_Arrived = 0;
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

    filters.StatusIDs = [7];
    this.loading_Arrived = true;
    this.dashboardService.GetWorkOrdersCountPerStatus(filters).subscribe(res => {
      if(!res.IsErrorState)
        this.count_Arrived = res.Value;

    }
    , err => {
      this.loading_Arrived = false;
    },() => {
      this.loading_Arrived = false;
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

}
