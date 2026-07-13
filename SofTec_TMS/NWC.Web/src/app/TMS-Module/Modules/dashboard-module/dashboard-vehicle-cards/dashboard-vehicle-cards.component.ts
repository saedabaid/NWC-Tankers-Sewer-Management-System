import { Component, OnInit, OnDestroy } from '@angular/core';
import { DashboardService } from 'src/app/TMS-Module/Services/dashboard.service';
import { Subscription } from 'rxjs';
import { DashboardSC } from 'src/app/TMS-Module/Models/search-criteria/dashboard-SC.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard-vehicle-cards',
  templateUrl: './dashboard-vehicle-cards.component.html',
  styleUrls: ['./dashboard-vehicle-cards.component.scss']
})
export class DashboardVehicleCardsComponent implements OnInit, OnDestroy {

  ImageUrl= "assets/TMSBranding/styles/img/loader.gif";
  searchSubscriper: Subscription;

  startDate: number;
  endDate: number;
  serviceId: number;

  //#region loading & count

  loading_AllVehicle = false;
  loading_avalaible = false;
  loading_assigned = false;
  loading_residential = false;
  loading_commercial = false;

  count_AllVehicle = 0;
  count_avalaible = 0;
  count_assigned = 0;
  count_residential = 0;
  count_commercial= 0;


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

        this.getCounts_AllVehicles(res);
        this.getCounts_avalaible(res);
        this.getCounts_AssignedVehicle(res);
 this.getCounts_commercial(res);
        //this.getCounts_residential(res);
      } else {

        this.startDate = null;
        this.endDate = null;

      }

    });

  }

  ngOnDestroy(): void {
    this.searchSubscriper.unsubscribe();
  }

  getCounts_AllVehicles(filters: DashboardSC) {
    this.count_AllVehicle = 0;
    filters.StatusIDs = [0, 11];
    this.loading_AllVehicle = true;
    this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
      if (!res.IsErrorState)
        this.count_AllVehicle = res.Value;
    }
      , err => {
        this.loading_AllVehicle = false;
      }, () => {
        this.loading_AllVehicle = false;
      });
  }
  getCounts_avalaible(filters: DashboardSC)
  {
    this.count_avalaible = 0;
    filters.StatusIDs = [0];
    filters.ClassName = "سكن";
      this.loading_avalaible = true;
      this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
        if (!res.IsErrorState)
          this.count_avalaible = res.Value;
        console.log(res.Value);

      }
        , err => {
          this.loading_avalaible = false;
        }, () => {
          this.loading_avalaible = false;
        });
  }

  getCounts_AssignedVehicle(filters: DashboardSC) {
    this.count_assigned = 0;
    filters.StatusIDs = [11];
    filters.ClassName = "سكن";
    this.loading_assigned = true;
    this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
      if (!res.IsErrorState)
        this.count_assigned = res.Value;

    }
      , err => {
        this.loading_assigned = false;
      }, () => {
        this.loading_assigned = false;
      });
  }

  //getCounts_residential(filters: DashboardSC) {
  //  this.count_residential = 0;
  //  filters.StatusIDs = [0, 11];
  //  filters.ClassName ="سكن";
  //  this.loading_residential = true;
  //  this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
  //    if (!res.IsErrorState)
  //      this.count_residential = res.Value;
  //  }
  //    , err => {
  //      this.loading_residential = false;
  //    }, () => {
  //      this.loading_residential = false;
  //    });
  //}

  getCounts_commercial(filters: DashboardSC) {
    this.count_commercial = 0;
    filters.StatusIDs = [0, 11];
    filters.ClassName = "تجار";
    this.loading_commercial = true;
    this.dashboardService.GetVehiclesCountGroupByStatus(filters).subscribe(res => {
      if (!res.IsErrorState)
        this.count_commercial = res.Value;
    }
      , err => {
        this.loading_commercial = false;
      }, () => {
        this.loading_commercial = false;
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
