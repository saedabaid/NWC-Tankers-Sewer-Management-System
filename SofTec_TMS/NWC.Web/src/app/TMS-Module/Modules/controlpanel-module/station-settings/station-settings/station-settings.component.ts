import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { TranslateService } from "@ngx-translate/core";
import { StationSettingsNWC } from "@tms-models/station-settings-nwc.model";
import { StationSettingsSC } from "src/app/TMS-Module/Models/search-criteria/station-settings-SC.model";
import { ControlPanelService } from "@tms-services/control-panel.service";
import { Configuration } from "src/app/shared/configurations/shared.config";
import { LoaderService } from "src/app/shared/loader.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { SearchStream } from "src/app/TMS-Module/Models/common/search-stream-object.model";
import { Lookup } from "../../../../Models/common/lookup";
import { LookupService } from "@tms-services/lookup.service";

@Component({
  selector: "app-station-settings",
  templateUrl: "./station-settings.component.html",
  styleUrls: ["./station-settings.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class StationSettingsComponent implements OnInit, OnDestroy {
  pagePermission = "";
  searchCriteriaDTO: StationSettingsSC = new StationSettingsSC();
  searchResult: StationSettingsNWC[];
  TotalCount: number;
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
  SearchStream: SearchStream = new SearchStream();
  defaultTruckImg = "/assets/TMSBranding/styles/img/tanker-clipart.png";

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private mainLoading: LoaderService,
    private authenticationService: AuthenticationService,
    private lookupService: LookupService,
    private controlPanelService: ControlPanelService
  ) {
    this.pagePermission =
      this.authenticationService.getCurrentUserPermissionByRoleName(
        "nwc_StationSettings"
      );
    this.authenticationService.checkFullControlPrivilege(
      this.pagePermission,
      true
    );
  }

  ngOnInit() {
    this.searchCriteriaDTO.IsDeleted = false;
    this.searchCriteriaDTO.PageFilter.PageSize =
      Configuration.GridSetting.Pagesize;
    this.searchCriteriaDTO.PageFilter.PageIndex = 1;
    this.load();
    this.translateService.onLangChange.subscribe((res) => {
      this.load();
    });
  }

  load() {
    this.titleService.setTitle(
      this.translateService.instant("StationsSettings")
    );
    this.Search();
    this.getBranches("");
    this.getSubBranches("");
    this.getStatuses("");
    this.getCustomerClasses("");
    this.getServiceTypes("");
  }

  Search() {
    this.mainLoading.PreloaderIcreaseCount();
    this.controlPanelService
      .getStationNWCSettings(this.searchCriteriaDTO)
      .subscribe(
        (res) => {
          if (res.Value != null) {
            this.searchResult = res.Value.Result;
            this.TotalCount = res.Value.TotalCount;
          } else {
            this.searchResult = [];
            this.TotalCount = 0;
          }
        },
        () => {
          this.mainLoading.PreloaderDecreaseCount();
        },
        () => {
          this.mainLoading.PreloaderDecreaseCount();
        }
      );
  }

  Clear() {
    this.searchCriteriaDTO = new StationSettingsSC();
    this.searchCriteriaDTO.IsDeleted = false;
    this.searchCriteriaDTO.PageFilter.PageSize =
      Configuration.GridSetting.Pagesize;
    this.searchCriteriaDTO.PageFilter.PageIndex = 1;
    this.Search();
  }

  onPageIndexChanged(evt) {
    this.searchCriteriaDTO.PageFilter.PageIndex = evt;
    this.Search();
  }

  onPageSizeChanged(evt) {
    this.searchCriteriaDTO.PageFilter.PageSize = evt;
    this.Search();
  }
  getBranches(searchKeyword: string) {
    this.SearchStream.initStream("BranchesDDL_StationSettings", (a) => {
      this.Branch_Loading = true;
      this.lookupService.getBranches(a).subscribe(
        (res) => {
          if (res.Value) {
            this.BranchList = res.Value;
          }
        },
        () => {
          this.Branch_Loading = false;
        },
        () => {
          this.Branch_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onBranchChanged($event) {
    this.searchCriteriaDTO.AreaIds =
      $event.length > 0 ? $event.map((a) => a.Id) : null;
    this.SelectedBranches = $event;
    this.getSubBranches("");
  }

  getSubBranches(searchKeyword: string) {
    this.SearchStream.initStream("SubBranchesDDL_StationSettings", (a) => {
      this.SubBranch_Loading = true;
      this.lookupService
        .getSubBranches(a, this.searchCriteriaDTO.AreaIds)
        .subscribe(
          (res) => {
            if (res.Value) {
              this.SubBranchList = res.Value;
            }
          },
          () => {
            this.SubBranch_Loading = false;
          },
          () => {
            this.SubBranch_Loading = false;
          }
        );
    }).next(searchKeyword);
  }
  onSubBranchChanged($event) {
    this.searchCriteriaDTO.CityIds =
      $event.length > 0 ? $event.map((a) => a.Id) : null;
    this.SelectedSubBranches = $event;
  }

  getStatuses(searchKeyword: string) {
    this.SearchStream.initStream("StatusesDDL_StationSettings", (a) => {
      this.Status_Loading = true;
      this.lookupService.GetStationStatuses().subscribe(
        (res) => {
          if (res.Value) {
            this.StatusList = res.Value;
          }
        },
        () => {
          this.Status_Loading = false;
        },
        () => {
          this.Status_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onStatusChanged($event) {
    this.searchCriteriaDTO.Status = $event.length > 0 ? $event[0].Id : null;
    this.SelectedStatuses = $event;
  }

  getCustomerClasses(searchKeyword: string) {
    this.SearchStream.initStream("CustomerClassesDDL_StationSettings", (a) => {
      this.CustomerClass_Loading = true;
      this.lookupService.getCustomerClasses().subscribe(
        (res) => {
          if (res.Value) {
            this.CustomerClassList = res.Value;
          }
        },
        () => {
          this.CustomerClass_Loading = false;
        },
        () => {
          this.CustomerClass_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onCustomerClassChanged($event) {
    this.searchCriteriaDTO.CustomerClass =
      $event.length > 0 ? $event[0].Id : null;
    this.SelectedCustomerClasses = $event;
  }

  getServiceTypes(searchKeyword: string) {
    this.SearchStream.initStream("ServiceTypesDDL_StationSettings", (a) => {
      this.ServiceType_Loading = true;
      this.lookupService.getServicesTypes().subscribe(
        (res) => {
          if (res.Value) {
            this.ServiceTypeList = res.Value;
          }
        },
        () => {
          this.ServiceType_Loading = false;
        },
        () => {
          this.ServiceType_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onServiceTypeChanged($event) {
    this.searchCriteriaDTO.ServiceType =
      $event.length > 0 ? $event[0].Id : null;
    this.SelectedServiceTypes = $event;
  }

  ngOnDestroy(): void {}
}
