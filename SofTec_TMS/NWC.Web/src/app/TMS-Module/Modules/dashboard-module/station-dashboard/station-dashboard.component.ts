import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { DashboardService } from "src/app/TMS-Module/Services/dashboard.service";

@Component({
  selector: "app-station-dashboard",
  templateUrl: "./station-dashboard.component.html",
  styleUrls: ["./station-dashboard.component.scss"],
})
export class StationDashboardComponent implements OnInit, OnDestroy {
  ImageUrl = "assets/TMSBranding/styles/img/loader.gif";
  station = { isOnline: true } as any;
  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {}

  ngOnDestroy(): void {}
}
