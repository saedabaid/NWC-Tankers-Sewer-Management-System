import { Component, OnInit } from '@angular/core';
import { SearchResult } from '@tms-models/common/search-result';
import { ZonePriceSCDTO } from '@tms-models/search-criteria/zone-price-search-criteria';
import { ZonePriceListDTO } from '@tms-models/zone-price-listDTO';
import { DashboardService } from '@tms-services/dashboard.service';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { Subscription } from 'rxjs';

@Component({
  selector: 'table-price-data',
  templateUrl: './table-price-data.component.html',
  styleUrls: ['./table-price-data.component.scss']
})
export class TablePriceDataComponent implements OnInit {

  ImageUrl= "assets/TMSBranding/styles/img/loader.gif";
  searchSubscriper: Subscription;

  SearchCriteria: ZonePriceSCDTO;
  zonePriceList !: SearchResult<ZonePriceListDTO>;
  constructor(
    private dashboardService: DashboardService,
  ) { }

  ngOnInit() {
    this.searchSubscriper = this.dashboardService.mainSearchClicked$.subscribe(res => {

      if (res != null) {
        this.SearchCriteria = new ZonePriceSCDTO();
        this.SearchCriteria.PageFilter.PageIndex = 1;
        this.SearchCriteria.PageFilter.PageSize =
          Configuration.GridSetting.Pagesize;
        this.SearchCriteria.StationIDs = res.StationIDs;

        this.getAreasWithNoPrices();
      }
    });
  }
  
  getAreasWithNoPrices(){
    this.dashboardService.GetAreasWithNoPrices(this.SearchCriteria)
    .subscribe((res) => {
      if(!res.IsErrorState){
        this.zonePriceList = res.Value;
      }
      else{
        this.zonePriceList.Result = [];
          this.zonePriceList.TotalCount = 0;
      }
    },
    (err) => {
    },
    () => {
    }
    )
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.PageFilter.PageIndex = evt;
    this.getAreasWithNoPrices();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.PageFilter.PageSize = evt;
    //this.getAreasWithNoPrices();
  }
}
