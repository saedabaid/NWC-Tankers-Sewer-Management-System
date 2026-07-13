import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { BranchSettingsDTO } from "@tms-models/branch-settings.model";
import { ControlPanelService } from "@tms-services/control-panel.service";
import { LoaderService } from "src/app/shared/loader.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";

@Component({
  selector: "app-new-edit-city-settings",
  templateUrl: "./new-edit-city-settings.component.html",
  styleUrls: ["./new-edit-city-settings.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class NewEditCitySettingsComponent implements OnInit {
  branchSettingsDTO: BranchSettingsDTO = new BranchSettingsDTO();
  IsEditableMood: boolean = false;
  CitySettingsId: string;
  validationMessages: string[] = [];
  pagePermission: string = "";

  constructor(
    private translateService: TranslateService,
    private router: Router,
    private _alert: alertService,
    private route: ActivatedRoute,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService,
    private controlPanelService: ControlPanelService
  ) {
    this.pagePermission =
      this.authenticationService.getCurrentUserPermissionByRoleName(
        "BranchManagement"
      );
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    const citySettingsId = this.route.snapshot.paramMap.get("cityId");
    this.CitySettingsId = citySettingsId == "new" ? null : citySettingsId;
  }

  ngAfterViewInit() {
    this.load();
    this.translateService.onLangChange.subscribe((res) => {
      this.load();
    });
  }

  load() {
    if (this.CitySettingsId != null) {
      this.IsEditableMood = true;
      this.GetCitySettingDetails();
    }
    this.titleService.setTitle(
      this.translateService.instant(
        this.IsEditableMood ? "EditCitySettings" : "AddCitySettings"
      )
    );
  }

  GetCitySettingDetails() {
    this.mainloading.PreloaderIcreaseCount();
    this.controlPanelService.GetCitySetting(this.CitySettingsId).subscribe(
      (res) => {
        if (res.Value) {
          this.branchSettingsDTO = res.Value;
        }
      },
      (err) => {
        this.mainloading.PreloaderDecreaseCount();
      },
      () => {
        this.mainloading.PreloaderDecreaseCount();
      }
    );
  }

  cancel() {
    this.router.navigate(["/tms/controlpanel/city-settings"]);
  }

  save() {
    if (this.isValidModel()) {
      this.mainloading.PreloaderIcreaseCount();
      const methodName = this.IsEditableMood
        ? "UpdateCitySettings"
        : "AddCitySettings";
      this.controlPanelService[methodName](this.branchSettingsDTO).subscribe(
        (res) => {
          if (!res.IsErrorState) {
            this._alert.showSuccess();
            this.router.navigate(["/tms/controlpanel/city-settings"]);
          } else {
            this._alert.errorList(res.Errors);
          }
        },
        (err) => {
          this.mainloading.PreloaderDecreaseCount();
        },
        () => {
          this.mainloading.PreloaderDecreaseCount();
        }
      );
    }
  }

  BranchSettingsNameChanged($event) {
    this.branchSettingsDTO.name = $event.target.value;
  }
  ApprovalLevelsCountChanged($event) {
    this.branchSettingsDTO.ApprovalLevelsCount = $event.target.value;
  }
  
  BranchSettingsTankerQuotaNoChanged($event) {
    console.log($event);
    this.branchSettingsDTO.TankerQuotaNo = $event.target.value;
  }
  BranchSettingsAutoCancelationNewOrdersHoursChanged($event) {
    this.branchSettingsDTO.AutoCancelationNewOrdersHours = $event.target.value;
  }
  BranchSettingsAutoCancelationOnHoldOrdersHoursChanged($event) {
    this.branchSettingsDTO.AutoCancelationOnHoldOrdersHours =
      $event.target.value;
  }

  isValidModel(): boolean {
    this.validationMessages = [];
    if (
      isNullOrUndefined(this.branchSettingsDTO.name) ||
      this.branchSettingsDTO.name === ""
    ) {
      this.validationMessages.push("InsertVehicleCode");
    }

    if (this.validationMessages.length > 0) {
      this._alert.errorList(this.validationMessages);
      return false;
    }
    return true;
  }
}
