import { Component, OnInit, OnDestroy } from "@angular/core";
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";
import { Subscription } from "rxjs";
import { DashboardService } from "../../../Services/dashboard.service";
import { DashboardSC } from "../../../Models/search-criteria/dashboard-SC.model";
import { TranslateService } from "@ngx-translate/core";
//am4core.useTheme(am4themes_animated);

@Component({
  selector: 'app-orders-statuses-chart',
  templateUrl: './orders-statuses-chart.component.html',
  styleUrls: ['./orders-statuses-chart.component.scss']
})
export class OrdersStatusesChartComponent implements OnInit, OnDestroy {

  searchSubscriper: Subscription;
  loading_StatusChart = false;

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

    this.loading_StatusChart = true;
    this.dashboardService.GetOrdersCountGroupByStatus(filters).subscribe(res => {

      if (!res.IsErrorState && res.Value) {

        let chart = am4core.create("ordersStatusChart", am4charts.PieChart);
        chart.rtl = (this.translateService.currentLang == 'ar');

        // Add data
        chart.data = res.Value;

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

        res.Value.forEach(element => {
          pieSeries.colors.list.push(am4core.color(element.Color));
        });

        //   am4core.color("green"),
        //   am4core.color("#D65DB1"),
        //   am4core.color("#FF6F91"),
        //   am4core.color("#FF9671"),
        //   am4core.color("#FFC75F"),
        //   am4core.color("#F9F871"),
        // ];


        // let chart = am4core.create("ordersStatusChart", am4charts.PieChart);

        // // Add data
        // chart.data = res.Value;

        // // Add and configure Series
        // let pieSeries = chart.series.push(new am4charts.PieSeries());
        // pieSeries.dataFields.value = "Count";
        // pieSeries.dataFields.category = "Name";
        // pieSeries.slices.template.stroke = am4core.color("#fff");
        // pieSeries.slices.template.strokeWidth = 2;
        // pieSeries.slices.template.strokeOpacity = 1;

        // // This creates initial animation
        // pieSeries.hiddenState.properties.opacity = 1;
        // pieSeries.hiddenState.properties.endAngle = -90;
        // pieSeries.hiddenState.properties.startAngle = -90;

      }

    }
      , err => {
        this.loading_StatusChart = false;
      }, () => {
        this.loading_StatusChart = false;
      });

  }

  destroyChart() {
    let chart = am4core.create("ordersStatusChart", am4charts.PieChart);
    chart.data = [];
  }

}
