import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardSC } from '@tms-models/search-criteria/dashboard-SC.model';
import { DashboardService } from '@tms-services/dashboard.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'main-vehicle-data',
  templateUrl: './main-vehicle-data.component.html',
  styleUrls: ['./main-vehicle-data.component.scss']
})
export class MainVehicleDataComponent implements OnInit {

  ImageUrl= "assets/TMSBranding/styles/img/loader.gif";
  carImageUrl= "assets/TMSBranding/styles/img/truck-front.svg";

  searchSubscriper: Subscription;

  startDate: number;
  endDate: number;
  serviceId: number;

  //#region loading & count

  loading_totalTankers = false;
  count_totalTankers = 0;

  loading_availableTankers = false;
  count_availableTankers = 0;

  loading_inServiceTankers = false;
  count_inServiceTankers = 0;
  //#endregion

  constructor(
    private dashboardService: DashboardService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.searchSubscriper = this.dashboardService.mainSearchClicked$.subscribe(res => {

      if (res != null) {

        this.startDate = (res.DateFrom.getFullYear() * 10000) + (res.DateFrom.getMonth() * 100) + res.DateFrom.getDate();
        this.endDate = (res.DateTo.getFullYear() * 10000) + (res.DateTo.getMonth() * 100) + res.DateTo.getDate();
        this.serviceId =  res.ServiceTypeID;

        this.getCounts_AllTankers(res);
        this.getCounts_AvalaibleTankers(res);
        this.getCounts_InServiceTankers(res);
        // this.getCounts_InServiceTankers(res);
        //this.getCounts_residential(res);
      } else {

        this.startDate = null;
        this.endDate = null;

      }

    });
  }

  getCounts_AllTankers(filters: DashboardSC) {
    this.count_totalTankers = 0;
    filters.StatusIDs = [0, 11];
    this.loading_totalTankers = true;
    this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
      if (!res.IsErrorState)
        this.count_totalTankers = res.Value;
    }
      , err => {
        this.loading_totalTankers = false;
      }, () => {
        this.loading_totalTankers = false;
      });
  }

  getCounts_AvalaibleTankers(filters: DashboardSC)
  {
    this.count_availableTankers = 0;
    filters.StatusIDs = [0];
      this.loading_availableTankers = true;
      this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
        if (!res.IsErrorState)
          this.count_availableTankers = res.Value;
        console.log(res.Value);

      }
        , err => {
          this.loading_availableTankers = false;
        }, () => {
          this.loading_availableTankers = false;
        });
  }

  getCounts_InServiceTankers(filters: DashboardSC) {
    this.count_inServiceTankers = 0;
    filters.StatusIDs = [11];
    this.loading_inServiceTankers = true;
    this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
      if (!res.IsErrorState)
        this.count_inServiceTankers = res.Value;

    }
      , err => {
        this.loading_inServiceTankers = false;
      }, () => {
        this.loading_inServiceTankers = false;
      });
  }



}
