import { Component, OnInit } from '@angular/core';
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";
import { Subscription } from 'rxjs';
import { DashboardService } from 'src/app/TMS-Module/Services/dashboard.service';
import { TranslateService } from '@ngx-translate/core';
import { DashboardSC } from 'src/app/TMS-Module/Models/search-criteria/dashboard-SC.model';
import { DashboardXYChartDTO } from 'src/app/TMS-Module/Models/common/DashboardXYChart.model';

@Component({
  selector: 'app-orders-execute-time-chart',
  templateUrl: './orders-execute-time-chart.component.html',
  styleUrls: ['./orders-execute-time-chart.component.scss']
})
export class OrdersExecuteTimeChartComponent implements OnInit {

  searchSubscriper: Subscription;
  loading_ExecuteTimeChart = false;

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

    this.loading_ExecuteTimeChart = true;
    this.dashboardService.GetOrdersCountGroupByExecuteTime(filters).subscribe(res => {

      if (!res.IsErrorState && res.Value) {

        let translatedValue = res.Value.map(v => {
          let c = new DashboardXYChartDTO();
          c.Count = v.Count;
          c.Name = this.translateService.instant(v.Name);
          return c;
        });

        let chart = am4core.create("ordersExecuteTimeChart", am4charts.PieChart);
        chart.rtl = (this.translateService.currentLang == 'ar');

        // Add data
        chart.data = translatedValue;

        // Add and configure Series
        let pieSeries = chart.series.push(new am4charts.PieSeries());
        pieSeries.dataFields.value = "Count";
        pieSeries.dataFields.category = "Name";
        pieSeries.innerRadius = am4core.percent(50);
        pieSeries.ticks.template.disabled = true;
        pieSeries.labels.template.disabled = true;

        let rgm = new am4core.RadialGradientModifier();
        rgm.brightnesses.push(-0.8, -0.8, -0.5, 0, - 0.5);
        pieSeries.slices.template.fillModifier = rgm;
        pieSeries.slices.template.strokeModifier = rgm;
        pieSeries.slices.template.strokeOpacity = 0.4;
        pieSeries.slices.template.strokeWidth = 0;

        chart.legend = new am4charts.Legend();
        chart.legend.position = "right";

        pieSeries.colors.list = [];
        pieSeries.colors.list.push(am4core.color("#00ae8d"));
        pieSeries.colors.list.push(am4core.color("#026CB7"));
        pieSeries.colors.list.push(am4core.color("#FFFF00"));
        pieSeries.colors.list.push(am4core.color("#FF9F02"));
        pieSeries.colors.list.push(am4core.color("#FF0000"));
        
        // res.Value.forEach(element => {
        //   pieSeries.colors.list.push(am4core.color(element.Color));
        // });

      }

    }
      , err => {
        this.loading_ExecuteTimeChart = false;
      }, () => {
        this.loading_ExecuteTimeChart = false;
      });

  }

  destroyChart() {
    let chart = am4core.create("ordersExecuteTimeChart", am4charts.PieChart);
    chart.data = [];
  }


}
