import { Component, OnInit, OnDestroy } from "@angular/core";
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";
import { Subscription } from "rxjs";
import { DashboardService } from "../../../Services/dashboard.service";
import { DashboardSC } from "../../../Models/search-criteria/dashboard-SC.model";
import { TranslateService } from "@ngx-translate/core";
am4core.useTheme(am4themes_animated);

@Component({
  selector: 'app-orders-date-chart',
  templateUrl: './orders-date-chart.component.html',
  styleUrls: ['./orders-date-chart.component.scss']
})
export class OrdersDateChartComponent implements OnInit, OnDestroy {

  searchSubscriper: Subscription;
  loading_OrdersDateChart = false;


  constructor(
    private dashboardService: DashboardService,
    private translateService: TranslateService
  ) {}

  ngOnInit() {

    this.searchSubscriper = this.dashboardService.mainSearchClicked$.subscribe(res => {
      this.destroyChart();
      
      if (res != null) {
        this.loadChart(res);
      } 

    });

  }

  ngOnDestroy(): void {
    this.searchSubscriper.unsubscribe();
  }


  loadChart(filters: DashboardSC) {

    this.loading_OrdersDateChart = true;
    this.dashboardService.GetOrdersCountGroupByDate(filters).subscribe(res => {

      if( !res.IsErrorState && res.Value){

        let chart = am4core.create("OrdersDateChart", am4charts.XYChart);
        chart.rtl = (this.translateService.currentLang == 'ar');

        // Add data
        chart.data = res.Value;

        // Create axes
        let dateAxis = chart.xAxes.push(new am4charts.DateAxis() as any);
        dateAxis.renderer.minGridDistance = 60;

        let valueAxis = chart.yAxes.push(new am4charts.ValueAxis() as any);

        // Create series
        let series = chart.series.push(new am4charts.LineSeries() as any);
        series.dataFields.valueY = "Count";
        series.dataFields.dateX = "Name";
        series.tooltipText = "{value}"

        series.tooltip.pointerOrientation = "vertical";

        chart.cursor = new am4charts.XYCursor();
        chart.cursor.snapToSeries = series;
        chart.cursor.xAxis = dateAxis;

        //chart.scrollbarY = new am4core.Scrollbar();
        chart.scrollbarX = new am4core.Scrollbar();


      }
    }
    , err => {
      this.loading_OrdersDateChart = false;
    }, () => {
      this.loading_OrdersDateChart = false;
    });

  }

  destroyChart() {
    let chart = am4core.create("OrdersDateChart", am4charts.XYChart);
    chart.data = [];
  }

}
