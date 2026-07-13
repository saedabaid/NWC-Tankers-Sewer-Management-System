import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { UserService } from "../Services/user.service";
import { LoaderService } from "src/app/shared/loader.service";
import { TranslateService } from "@ngx-translate/core";
import { Title } from "@angular/platform-browser";

@Component({
  selector: "app-landing-page",
  templateUrl: "./landing-page.component.html",
  styleUrls: ["./landing-page.component.scss"],
})
export class LandingPageComponent implements OnInit {
  show_Dashboard = true;
  show_OrderList = true;
  show_Zone = true;
  show_Vehicle = true;
  show_EntryGate = true;
  show_ExitGate = true;
  show_Contracts = true;
  show_Contractors = true;
  show_DeviceReadings = true;
  show_Reports = true;
  show_DeferredOrders = true;
  show_StationSettings = true;
  show_VehicleSettings = true;
  show_UserStationPermissions = true;
  show_SoqyaScheduling = true;
  show_ContractTermsViolations = true;
  show_Customers = true;
  show_MassOrderTransfer = true;
  show_BranchSettings = true;
  show_SewerEntryGate = true;
  show_SewerExitGate = true;
  show_Userlist = true;
  show_changePassword = true;
  show_Permits = true;
  show_StationDashboard = true;

  constructor(
    private _router: Router,
    private activatedsrRoute: ActivatedRoute,
    private authService: AuthenticationService,
    private userService: UserService,
    private trans: TranslateService,
    private titleService: Title
  ) {
    // debugger;
    // let url = this._router.routerState.snapshot.url;
    // let urlList = this._router.routerState.snapshot.url.split('/');
    // const token = this.activatedsrRoute.snapshot.paramMap.get('token');
    const token = this.authService.getToken();

    if (token && token != "") {
      this.authService.saveToken_localStorage(token);
      this.authService.getUserPermissions().subscribe(() => {
        this.load();
      });

      this.userService.GetUserProfile().subscribe((res) => {
        if (!res.IsErrorState) {
          this.authService.saveUserFullName(res.Value.FullName);
          this.authService.saveLogo_localStorage(res.Value.SubLogo);
        }
      });

      //
      const culture = this.activatedsrRoute.snapshot.paramMap.get("culture");
      if (culture && culture != "") {
        const lang = culture == "ar-AE" ? "ar" : "en";
        this.authService.saveCulture_localStorage(lang);
        this.trans.use(lang);
      }
    } else {
      this.load();
    }
  }

  ngOnInit() {
    this.titleService.setTitle(this.trans.instant("TMS"));
    this.trans.onLangChange.subscribe((res) => {
      this.titleService.setTitle(this.trans.instant("TMS"));
    });
  }

  load() {
    this.show_Dashboard = this.havePermission("tmsDashboard");
    this.show_OrderList = this.havePermission("orderlist");
    this.show_Zone = this.havePermission("zonelist");
    this.show_Vehicle = this.havePermission("Transporters");
    this.show_EntryGate = this.havePermission("entrygate");
    this.show_ExitGate = this.havePermission("exitgate");
    this.show_Contracts = this.havePermission("contractlist");
    this.show_Contractors = this.havePermission("contractorlist");
    this.show_DeviceReadings = this.havePermission("deviceMeterReading");
    this.show_Reports = this.havePermission("tmsReports");
    this.show_DeferredOrders = this.havePermission("tmsDeferredOrders");
    this.show_StationSettings = this.havePermission("nwc_StationSettings");
    this.show_VehicleSettings = this.havePermission("nwc_VehicleSettings");
    this.show_UserStationPermissions = this.havePermission(
      "nwc_userLandmarkPermissions"
    );
    this.show_SoqyaScheduling = this.havePermission("tmsSoqyaScheduling");
    this.show_ContractTermsViolations = this.havePermission(
      "tmsContractTermsViolations"
    );
    this.show_Customers = this.havePermission("tmsCustomers");
    this.show_MassOrderTransfer = this.havePermission("tmsMassOrderTransfer");
    this.show_BranchSettings = this.havePermission("tmsBranchSettings");
    this.show_SewerEntryGate = this.havePermission("SewerEntryGate");
    this.show_SewerExitGate = this.havePermission("SewerExitGate");
    this.show_Userlist = this.havePermission("userlist");
    this.show_changePassword = this.havePermission("changePassword");
    
    this.show_Permits = this.havePermission("PermitsList");
    this.show_StationDashboard = this.havePermission("stationDashboard");
  }

  havePermission(pageUniqueName: string): boolean {
    const permission = this.authService.getCurrentUserPermissionByRoleName(
      pageUniqueName
    );
    return this.authService.checkViewPrivilege(permission);
  }

  customRoute(routeTo: string) {
    if (routeTo && routeTo !== "") {
      const targetLink = `/tms/${routeTo}`;
      const url = this._router.serializeUrl(
        this._router.createUrlTree([targetLink])
      );
      console.log(routeTo, url);

      this._router.navigate([url]);

      // window.open(url, '_blank');
    }
  }
}
