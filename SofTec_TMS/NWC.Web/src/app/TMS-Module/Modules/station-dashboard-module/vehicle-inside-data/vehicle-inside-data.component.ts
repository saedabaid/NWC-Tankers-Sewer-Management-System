import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardSC } from '@tms-models/search-criteria/dashboard-SC.model';
import { DashboardService } from '@tms-services/dashboard.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'vehicle-inside-data',
  templateUrl: './vehicle-inside-data.component.html',
  styleUrls: ['./vehicle-inside-data.component.scss']
})
export class VehicleInsideDataComponent implements OnInit {

  ImageUrl= "assets/TMSBranding/styles/img/loader.gif";
  carImageUrl= "assets/TMSBranding/styles/img/truck-front.svg";

  searchSubscriper: Subscription;

  startDate: number;
  endDate: number;
  serviceId: number;

  loading_totalTankers = false;
  count_totalTankers = 0;

  loading_residentialTankers = false;
  count_residentialTankers = 0;

  loading_commercialTankers = false;
  count_commercialTankers = 0;

  loading_governmentalTankers = false;
  count_governmentalTankers = 0;
  
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
        this.getCounts_residentialTankers(res);
        this.getCounts_commercialTankers(res);
        this.getCounts_governmentalTankers(res);
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

  getCounts_residentialTankers(filters: DashboardSC)
  {
    this.count_residentialTankers = 0;
    filters.StatusIDs = [0, 11];
    filters.ClassName = "سكن";
      this.loading_residentialTankers = true;
      this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
        if (!res.IsErrorState)
          this.count_residentialTankers = res.Value;
        console.log(res.Value);

      }
        , err => {
          this.loading_residentialTankers = false;
        }, () => {
          this.loading_residentialTankers = false;
        });
  }

  getCounts_commercialTankers(filters: DashboardSC) {
    this.count_commercialTankers = 0;
    filters.StatusIDs = [0, 11];
    filters.ClassName = "تجار";
    this.loading_commercialTankers = true;
    this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
      if (!res.IsErrorState)
        this.count_commercialTankers = res.Value;

    }
      , err => {
        this.loading_commercialTankers = false;
      }, () => {
        this.loading_commercialTankers = false;
      });
  }

  getCounts_governmentalTankers(filters: DashboardSC) {
    this.count_governmentalTankers = 0;
    filters.StatusIDs = [0, 11];
    filters.ClassName = "حكوم";
    this.loading_governmentalTankers = true;
    this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
      if (!res.IsErrorState)
        this.count_governmentalTankers = res.Value;

    }
      , err => {
        this.loading_governmentalTankers = false;
      }, () => {
        this.loading_governmentalTankers = false;
      });
  }
}
