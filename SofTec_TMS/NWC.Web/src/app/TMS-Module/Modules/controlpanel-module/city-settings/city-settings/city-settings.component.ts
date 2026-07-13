import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { TranslateService } from "@ngx-translate/core";
import { BranchSettingsDTO } from "@tms-models/branch-settings.model";
import { BranchesSearchCriteriaDTO } from "@tms-models/search-criteria/branches-search-criteria";
import { ControlPanelService } from "@tms-services/control-panel.service";
import { Configuration } from "src/app/shared/configurations/shared.config";
import { LoaderService } from "src/app/shared/loader.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";

@Component({
  selector: "app-city-settings",
  templateUrl: "./city-settings.component.html",
  styleUrls: ["./city-settings.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class CitySettingsComponent implements OnInit, OnDestroy {
  pagePermission = "";
  searchCriteriaDTO: BranchesSearchCriteriaDTO =
    new BranchesSearchCriteriaDTO();
  searchResult: BranchSettingsDTO[];
  TotalCount: number;
  selectMenuOptions = {};
  defaultTruckImg = "/assets/TMSBranding/styles/img/tanker-clipart.png";

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private mainLoading: LoaderService,
    private authenticationService: AuthenticationService,
    private controlPanelService: ControlPanelService
  ) {
    this.pagePermission =
      this.authenticationService.getCurrentUserPermissionByRoleName(
        "BranchManagement"
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
    this.titleService.setTitle(this.translateService.instant("CitySettings"));
    this.Search();
  }

  Search() {
    this.mainLoading.PreloaderIcreaseCount();
    this.controlPanelService
      .SearchCitySettings(this.searchCriteriaDTO)
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
    this.searchCriteriaDTO = new BranchesSearchCriteriaDTO();
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

  ngOnDestroy(): void {}
}
