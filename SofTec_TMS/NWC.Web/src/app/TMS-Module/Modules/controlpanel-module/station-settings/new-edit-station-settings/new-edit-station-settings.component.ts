import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { StationSettingsNWC } from "@tms-models/station-settings-nwc.model";
import { ControlPanelService } from "@tms-services/control-panel.service";
import { LoaderService } from "src/app/shared/loader.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";

import { Lookup } from "../../../../Models/common/lookup";
import { LookupService } from "@tms-services/lookup.service";
import { StationDefaultTankersDTO } from "@tms-models/station-default-tankersDTO";
@Component({
  selector: "app-new-edit-station-settings",
  templateUrl: "./new-edit-station-settings.component.html",
  styleUrls: ["./new-edit-station-settings.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class NewEditStationSettingsComponent implements OnInit {
  stationSettingsDTO: StationSettingsNWC = new StationSettingsNWC();
  IsEditableMood: boolean = false;
  StationSettingsId: string;
  validationMessages: string[] = [];
  pagePermission: string = "";
  selectMenuOptions = {};
  selectMenuOptions2 = { singleSelect: true };

  BranchList: Lookup<string>[] = [];
  SelectedBranches: Lookup<string>[] = [];
  Branch_Loading = false;
  SubBranchList: Lookup<string>[] = [];
  SelectedSubBranches: Lookup<string>[] = [];
  SubBranch_Loading = false;
  StatusList: Lookup<number>[] = [];
  SelectedStatuses: Lookup<number>[] = [];
  Status_Loading = false;
  CustomerClassList: Lookup<number>[] = [];
  SelectedCustomerClasses: Lookup<number>[] = [];
  CustomerClass_Loading = false;
  ServiceTypeList: Lookup<number>[] = [];
  SelectedServiceTypes: Lookup<number>[] = [];
  ServiceType_Loading = false;

  constructor(
    private translateService: TranslateService,
    private router: Router,
    private _alert: alertService,
    private route: ActivatedRoute,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupService: LookupService,
    private mainloading: LoaderService,
    private controlPanelService: ControlPanelService
  ) {
    this.pagePermission =
      this.authenticationService.getCurrentUserPermissionByRoleName(
        "nwc_StationSettings"
      );
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    const stationSettingsId = this.route.snapshot.paramMap.get("stationId");
    this.StationSettingsId =
      stationSettingsId == "new" ? null : stationSettingsId;
  }

  ngAfterViewInit() {
    this.load();
    this.translateService.onLangChange.subscribe((res) => {
      this.load();
    });
  }

  load() {
    this.getBranches("");
    this.getStatuses("");
    this.getCustomerClasses("");
    this.getServiceTypes("");
    if (this.StationSettingsId != null) {
      this.IsEditableMood = true;
      this.GetStationSettingDetails();
    }
    else
      this.GetTankerCapacities();
    this.titleService.setTitle(
      this.translateService.instant(
        this.IsEditableMood ? "EditStationSettings" : "AddStationSettings"
      )
    );
  }

  GetStationSettingDetails() {
    this.mainloading.PreloaderIcreaseCount();
    this.controlPanelService
      .getStationNWCSetting(this.StationSettingsId)
      .subscribe(
        (res) => {
          if (res.Value) {
            this.stationSettingsDTO = res.Value;
            this.GetTankerCapacitiesForEditableMode();
            this.getSubBranches("");
            if (this.BranchList && this.BranchList.length) {
              this.SelectedBranches = this.BranchList.filter(
                (x) => x.Id == this.stationSettingsDTO.AreaId
              );
            }
            if (this.SubBranchList && this.SubBranchList.length) {
              this.SelectedSubBranches = this.SubBranchList.filter(
                (x) => x.Id == this.stationSettingsDTO.CityId
              );
            }
            if (this.StatusList && this.StatusList.length) {
              this.SelectedStatuses = this.StatusList.filter(
                (x) => x.Id == this.stationSettingsDTO.StatusId
              );
            }
            if (this.CustomerClassList && this.CustomerClassList.length) {
              this.SelectedCustomerClasses = this.CustomerClassList.filter(
                (x) => this.stationSettingsDTO.CustomerClassIds.includes(x.Id)
              );
            }
            if (this.ServiceTypeList && this.ServiceTypeList.length) {
              this.SelectedServiceTypes = this.ServiceTypeList.filter((x) =>
                this.stationSettingsDTO.StationServiceIds.includes(x.Id)
              );
            }
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
    this.router.navigate(["/tms/controlpanel/station-settings"]);
  }

  save() {
    if (this.isValidModel()) {
      this.mainloading.PreloaderIcreaseCount();
      this.controlPanelService.saveStationNWCSettings(this.stationSettingsDTO).subscribe(
        (res) => {
          if (!res.IsErrorState) {
            this._alert.showSuccess();
            this.router.navigate(["/tms/controlpanel/station-settings"]);
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

  StationNameChanged($event) {
    this.stationSettingsDTO.StationName = $event.target.value;
  }

  getBranches(searchKeyword: string) {
    this.Branch_Loading = true;
    this.lookupService.getBranches(searchKeyword).subscribe(
      (res) => {
        if (res.Value) {
          this.BranchList = res.Value;
          if (this.stationSettingsDTO.AreaId) {
            this.SelectedBranches = this.BranchList.filter(
              (x) => x.Id == this.stationSettingsDTO.AreaId
            );
          }
        }
      },
      () => {
        this.Branch_Loading = false;
      },
      () => {
        this.Branch_Loading = false;
      }
    );
  }
  onBranchChanged($event) {
    this.stationSettingsDTO.AreaId = $event.length > 0 ? $event[0].Id : null;
    this.SelectedBranches = $event;
    this.getSubBranches("");
  }

  getSubBranches(searchKeyword: string) {
    this.SubBranch_Loading = true;
    this.lookupService
      .getSubBranches(searchKeyword, [this.stationSettingsDTO.AreaId])
      .subscribe(
        (res) => {
          if (res.Value) {
            this.SubBranchList = res.Value;
            if (this.stationSettingsDTO.CityId) {
              this.SelectedSubBranches = this.SubBranchList.filter(
                (x) => x.Id == this.stationSettingsDTO.CityId
              );
            }
          }
        },
        () => {
          this.SubBranch_Loading = false;
        },
        () => {
          this.SubBranch_Loading = false;
        }
      );
  }
  onSubBranchChanged($event) {
    this.stationSettingsDTO.CityId = $event.length > 0 ? $event[0].Id : null;
    this.SelectedSubBranches = $event;
  }

  getStatuses(e) {
    this.Status_Loading = true;
    this.lookupService.GetStationStatuses().subscribe(
      (res) => {
        if (res.Value) {
          this.StatusList = res.Value;
          if (this.stationSettingsDTO.StatusId) {
            this.SelectedStatuses = this.StatusList.filter(
              (x) => x.Id == this.stationSettingsDTO.StatusId
            );
          }
        }
      },
      () => {
        this.Status_Loading = false;
      },
      () => {
        this.Status_Loading = false;
      }
    );
  }
  onStatusChanged($event) {
    this.stationSettingsDTO.StatusId = $event.length > 0 ? $event[0].Id : null;
    this.SelectedStatuses = $event;
  }

  getCustomerClasses(e) {
    this.CustomerClass_Loading = true;
    this.lookupService.getCustomerClasses().subscribe(
      (res) => {
        if (res.Value) {
          this.SelectedServiceTypes = this.ServiceTypeList.filter((x) =>
          this.stationSettingsDTO.StationServiceIds != null ? this.stationSettingsDTO.StationServiceIds.includes(x.Id): null
          );
          this.CustomerClassList = res.Value;
          if (
            this.stationSettingsDTO.CustomerClassIds &&
            this.stationSettingsDTO.CustomerClassIds.length
          ) {
            this.SelectedCustomerClasses = this.CustomerClassList.filter((x) =>
              this.stationSettingsDTO.CustomerClassIds.includes(x.Id)
            );
          }
        }
      },
      () => {
        this.CustomerClass_Loading = false;
      },
      () => {
        this.CustomerClass_Loading = false;
      }
    );
  }
  onCustomerClassChanged($event) {
    this.stationSettingsDTO.CustomerClassIds = $event.map((a) => a.Id);
    this.SelectedCustomerClasses = $event;
  }

  getServiceTypes(e) {
    this.ServiceType_Loading = true;
    this.lookupService.getServicesTypes().subscribe(
      (res) => {
        if (res.Value) {
          this.ServiceTypeList = res.Value;
          if (
            this.stationSettingsDTO.StationServiceIds &&
            this.stationSettingsDTO.StationServiceIds.length
          ) {
            this.SelectedServiceTypes = this.ServiceTypeList.filter((x) =>
              this.stationSettingsDTO.StationServiceIds.includes(x.Id)
            );
          }
        }
      },
      () => {
        this.ServiceType_Loading = false;
      },
      () => {
        this.ServiceType_Loading = false;
      }
    );
  }
  onServiceTypeChanged($event) {
    this.stationSettingsDTO.StationServiceIds = $event.map((a) => a.Id);
    this.SelectedServiceTypes = $event;
  }

  isValidModel(): boolean {
    this.validationMessages = [];
    if (
      isNullOrUndefined(this.stationSettingsDTO.StationName) ||
      this.stationSettingsDTO.StationName === ""
    ) {
      this.validationMessages.push("InsertStationName");
    }

    if (this.validationMessages.length > 0) {
      this._alert.errorList(this.validationMessages);
      return false;
    }
    return true;
  }

  GetTankerCapacities(){
    this.lookupService.GetTanckerCapacities().subscribe(caps => {
      if(caps.Value){
        this.stationSettingsDTO.AllCapacities = caps.Value;
      }
    })
  }
  GetTankerCapacitiesForEditableMode(){
    this.lookupService.GetTanckerCapacities().subscribe(caps => {
      if (caps.Value) {
        this.stationSettingsDTO.AllCapacities = caps.Value;
        for (var r of this.stationSettingsDTO.SelectedCapacities) {
          var deleted = this.stationSettingsDTO.AllCapacities.splice(this.stationSettingsDTO.AllCapacities.findIndex(s => s.Id == r.Id), 1);
          r.Name = deleted[0].Name;
        }
      }
    });
  }

  addRequest(index: number) {
    const deletedElement = this.stationSettingsDTO.AllCapacities.splice(index, 1)
    this.stationSettingsDTO.SelectedCapacities.push(deletedElement[0]);
  }

  removeRequest(index: number) {
    const deletedElement = this.stationSettingsDTO.SelectedCapacities.splice(index, 1)
    this.stationSettingsDTO.AllCapacities.push(deletedElement[0]);
  }

  // saveDefault(){
  //   this.controlPanelService.saveStationDefaultTankers(this.StationDefaultTankersDTO)
  //   .subscribe(res => {
  //     if(!res.IsErrorState){
  //       this._alert.showSuccess();
  //     }
  //   })
  // }

  // cancelDefault(){

  // }
}
