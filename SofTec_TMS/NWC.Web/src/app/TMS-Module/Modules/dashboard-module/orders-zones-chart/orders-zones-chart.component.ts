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
  selector: 'app-orders-zones-chart',
  templateUrl: './orders-zones-chart.component.html',
  styleUrls: ['./orders-zones-chart.component.scss']
})
export class OrdersZonesChartComponent implements OnInit, OnDestroy {

  searchSubscriper: Subscription;
  loading_ZonesChart = false;

  constructor(
    private dashboardService: DashboardService,
    private translateService: TranslateService
    ) { }

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

    this.loading_ZonesChart = true;
    this.dashboardService.GetOrdersCountGroupByTop10Zones(filters).subscribe(res => {

      if (!res.IsErrorState && res.Value) {

        let chart = am4core.create("ordersZonesChart", am4charts.XYChart);
        chart.rtl = (this.translateService.currentLang == 'ar');

        // Add data
        chart.data = res.Value;

        let categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis() as any);
        categoryAxis.dataFields.category = "Name";
        categoryAxis.renderer.grid.template.location = 0;
        categoryAxis.renderer.minGridDistance = 30;

        categoryAxis.renderer.labels.template.adapter.add("dy", function (dy, target) {
          if (target.dataItem && target.dataItem.index) {
            return dy + 25;
          }
          return dy;
        });

        let valueAxis = chart.yAxes.push(new am4charts.ValueAxis() as any);

        // Create series
        let series = chart.series.push(new am4charts.ColumnSeries() as any);
        series.dataFields.valueY = "Count";
        series.dataFields.categoryX = "Name";
        series.name = "Count";
        series.columns.template.tooltipText = "{categoryX}: [bold]{valueY}[/]";
        series.columns.template.fillOpacity = .8;

        let columnTemplate = series.columns.template;
        columnTemplate.strokeWidth = 2;
        columnTemplate.strokeOpacity = 1;


      }
    }
      , err => {
        this.loading_ZonesChart = false;
      }, () => {
        this.loading_ZonesChart = false;
      });

  }

  destroyChart() {
    let chart = am4core.create("ordersZonesChart", am4charts.XYChart);
    chart.data = [];
  }

}
