import { Component, OnInit } from '@angular/core';
import { FilterModel } from '@tms-models/common/filter-model';
import { PageFilter } from '@tms-models/common/page-fillter-model';
import { SearchResult } from '@tms-models/common/search-result';
import { OrderDetails } from '@tms-models/order-details';
import { DateperiodEnum, WorkOrderSearchCriteria } from '@tms-models/search-criteria/work-order-search-criteria';
import { DashboardService } from '@tms-services/dashboard.service';
import { WorkOrderSearchService } from '@tms-services/work-order-search.service';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { Subscription } from 'rxjs';

@Component({
  selector: 'table-day-order',
  templateUrl: './table-day-order.component.html',
  styleUrls: ['./table-day-order.component.scss']
})
export class TableDayOrderComponent implements OnInit {

  SearchCriteria: WorkOrderSearchCriteria;
  searchSubscriper: Subscription;
  searchResult = new SearchResult<OrderDetails>();
  timeFromStr: string;
  timeToStr: string;
  
  constructor(
    private workOrderService: WorkOrderSearchService,
    private dashboardService: DashboardService,
  ) { }

  ngOnInit() {
    this.setDefaultSearchValues();

    this.searchSubscriper = this.dashboardService.mainSearchClicked$.subscribe(res => {
      if (res != null) {
        this.SearchCriteria.DateTimeFrom = res.DateFrom;
        this.SearchCriteria.DateTimeTo = res.DateTo;
        this.SearchCriteria.StationIDs = res.StationIDs;
        this.searchData();
      }
    });
  }

  ngOnDestroy(): void {
    this.searchSubscriper.unsubscribe();
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new WorkOrderSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize =
      Configuration.GridSetting.Pagesize;

    this.SearchCriteria.DatePeriod = DateperiodEnum.ScheduleDate;
    this.SearchCriteria.StatusIDs = [4];

    let startDate = new Date();
    startDate.setDate(startDate.getDate() - 1);
    this.SearchCriteria.DateTimeFrom = startDate;
    this.timeFromStr = startDate.toTimeString().substring(0, 5);

    let endDate = new Date();
    endDate.setDate(endDate.getDate() + 1);
    endDate.setHours(0);
    endDate.setMinutes(0);
    endDate.setSeconds(0);
    this.SearchCriteria.DateTimeTo = endDate;
    this.timeToStr = endDate.toTimeString().substring(0, 5);
  }

  searchData(){

    let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    modifiedCriteria.DateTimeFrom = new Date(
      this.SearchCriteria.DateTimeFrom.getTime()
    );
    modifiedCriteria.DateTimeFrom.setMinutes(
      modifiedCriteria.DateTimeFrom.getMinutes() -
        modifiedCriteria.DateTimeFrom.getTimezoneOffset()
    );
    modifiedCriteria.DateTimeTo = new Date(
      this.SearchCriteria.DateTimeTo.getTime()
    );
    modifiedCriteria.DateTimeTo.setMinutes(
      modifiedCriteria.DateTimeTo.getMinutes() -
        modifiedCriteria.DateTimeTo.getTimezoneOffset()
    );

    this.workOrderService.GetDailyOrderDetailsReport(modifiedCriteria).subscribe(
      res => {
        if (res.Value != null) {
          this.searchResult = res.Value;
          console.log(res.Value);
        } else {
          this.searchResult.Result = [];
          this.searchResult.TotalCount = 0;
        }
      },
      err => {
        
      },
      () => {
        //this.tableLoading = false;
        
      }
    );
  }

  calculateTime(date1: any, date2: any){
    const diffInMs = Date.parse(date1) - Date.parse(date2);
    const diffInHours = diffInMs / 1000 / 60 / 60;
    return Math.round(diffInHours * 100) / 100;
  }
}
