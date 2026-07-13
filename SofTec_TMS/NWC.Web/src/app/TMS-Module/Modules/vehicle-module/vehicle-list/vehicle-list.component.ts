import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { TranslateService } from "@ngx-translate/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { Configuration } from "src/app/shared/configurations/shared.config";
import { LoaderService } from "src/app/shared/loader.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { SearchStream } from "src/app/TMS-Module/Models/common/search-stream-object.model";
import { Lookup } from "../../../Models/common/lookup";
import { VehicleSearchCriteriaDTO } from "../../../Models/search-criteria/vehicle-search-criteria";
import { VehicleListDTO } from "../../../Models/vehicle-list-dto";
import { LookupService } from "../../../Services/lookup.service";
import { VehicleListService } from "../../../Services/vehicle-list.service";
import { UploadVehicleExcelComponent } from "../upload-vehicle-excel/upload-vehicle-excel.component";

@Component({
  selector: "app-vehicle",
  templateUrl: "./vehicle-list.component.html",
  styleUrls: ["./vehicle-list.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class VehicleListComponent implements OnInit, OnDestroy {
  advancedDiv = <boolean>false;
  pagePermission = "";
  VehicleSearchCriteriaDTO: VehicleSearchCriteriaDTO = new VehicleSearchCriteriaDTO();
  vehiclesList: VehicleListDTO[];
  TotalCount: number;
  selectMenuOptions = {};
  BrandList: Lookup<string>[] = [];
  SelectedBrands: Lookup<string>[] = [];
  ModelList: Lookup<string>[] = [];
  SelectedModels: Lookup<string>[] = [];
  YearList: Lookup<string>[] = [];
  SelectedYears: Lookup<string>[] = [];
  VehicleTypeList: Lookup<string>[] = [];
  SelectedVehicleTypes: Lookup<string>[] = [];
  GroupList: Lookup<string>[] = [];
  SelectedGroups: Lookup<string>[] = [];
  StatusList: Lookup<number>[] = [];
  SelectedStatuses: Lookup<number>[] = [];
  BranchList: Lookup<string>[] = [];
  SelectedBranches: Lookup<string>[] = [];
  SubBranchList: Lookup<string>[] = [];
  SelectedSubBranches: Lookup<string>[] = [];
  LandmarkList: Lookup<string>[] = [];
  SelectedLandmarks: Lookup<string>[] = [];

  Brand_Loading = false;
  Model_Loading = false;
  Year_Loading = false;
  VehicleType_Loading = false;
  Group_Loading = false;
  Status_Loading = false;
  Branch_Loading = false;
  SubBranch_Loading = false;
  Landmark_Loading = false;
  SearchStream: SearchStream = new SearchStream();
  defaultTruckImg = "/assets/TMSBranding/styles/img/tanker-clipart.png";

  currentTab = 1;

  constructor(
    private _translate: TranslateService,
    private authServer: AuthenticationService,
    private lookupService: LookupService,
    private vehicleListService: VehicleListService,
    private _alert: alertService,
    private titleService: Title,
    private mainLoading: LoaderService,
    private modalRef: BsModalRef,
    private modalService: BsModalService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName(
      "Transporters"
    );
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.VehicleSearchCriteriaDTO.IsDeleted = false;
    this.VehicleSearchCriteriaDTO.PageFilter.PageSize =
      Configuration.GridSetting.Pagesize;
    this.VehicleSearchCriteriaDTO.PageFilter.PageIndex = 1;

    this.load();
    this._translate.onLangChange.subscribe((res) => {
      this.load();
    });
  }

  load() {
    this.titleService.setTitle(this._translate.instant("Vehicles"));
    this.Search();
  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
    this.getBrands("");
    this.getModels("");
    this.getYears("");
    this.getVehicleTypes("");
    this.getGroups("");
    this.getStatuses("");
    this.getBranches("");
  }

  onVehicleDDLChanged(evt) {
    this.VehicleSearchCriteriaDTO.PlateNoOrCode = isNullOrUndefined(evt.Name)
      ? evt
      : evt.Name;
  }

  getVehicles($event) {
    return this.lookupService.getVehicles($event);
  }

  switchTab(tabName) {
    this.currentTab = tabName;
  }

  Search() {
    this.mainLoading.PreloaderIcreaseCount();
    this.vehicleListService.Search(this.VehicleSearchCriteriaDTO).subscribe(
      (res) => {
        if (res.Value != null) {
          this.vehiclesList = res.Value.Result;
          this.TotalCount = res.Value.TotalCount;
        } else {
          this.vehiclesList = [];
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
    this.VehicleSearchCriteriaDTO = new VehicleSearchCriteriaDTO();
    this.VehicleSearchCriteriaDTO.IsDeleted = false;
    this.VehicleSearchCriteriaDTO.PageFilter.PageSize =
      Configuration.GridSetting.Pagesize;
    this.VehicleSearchCriteriaDTO.PageFilter.PageIndex = 1;

    this.BrandList = [];
    this.SelectedBrands = [];
    this.ModelList = [];
    this.SelectedModels = [];
    this.YearList = [];
    this.SelectedYears = [];
    this.VehicleTypeList = [];
    this.SelectedVehicleTypes = [];
    this.GroupList = [];
    this.SelectedGroups = [];
    this.StatusList = [];
    this.SelectedStatuses = [];
    this.BranchList = [];
    this.SelectedBranches = [];
    this.SubBranchList = [];
    this.SelectedSubBranches = [];
    this.LandmarkList = [];
    this.SelectedLandmarks = [];
    this.Search();
  }

  delete(i: string) {
    if (!this.authServer.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }
    this._alert.confirmationMessage("DeleteMsgVehicle").subscribe((confirm) => {
      if (confirm === true) {
        this.vehicleListService.Delete(i).subscribe((res) => {
          if (!res.IsErrorState) {
            this._alert.success("DeletedSuccessed");
            this.Search();
          } else {
            this._alert.errorList(res.Errors);
          }
        });
      }
    });
  }

  onPageIndexChanged(evt) {
    this.VehicleSearchCriteriaDTO.PageFilter.PageIndex = evt;
    this.Search();
  }

  onPageSizeChanged(evt) {
    this.VehicleSearchCriteriaDTO.PageFilter.PageSize = evt;
    this.Search();
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  uploadExcel() {
    this.modalRef = this.modalService.show(UploadVehicleExcelComponent);
    this.modalRef.content.confirm.subscribe(() => {
      this.Search();
    });
  }

  getBrands(searchKeyword: string) {
    this.SearchStream.initStream("BrandsDDL_VehicleList", (a) => {
      this.Brand_Loading = true;
      this.lookupService.GetTransporterBrands().subscribe(
        (res) => {
          if (res.Value) {
            this.BrandList = res.Value;
          }
        },
        () => {
          this.Brand_Loading = false;
        },
        () => {
          this.Brand_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onBrandChanged($event) {
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.BrandIDs = $event.map((a) => a.Id);
      this.SelectedBrands = $event;
    }
  }

  getModels(searchKeyword: string) {
    this.SearchStream.initStream("ModelsDDL_VehicleList", (a) => {
      this.Model_Loading = true;
      this.lookupService.GetTransporterManufacturer().subscribe(
        (res) => {
          if (res.Value) {
            this.ModelList = res.Value;
          }
        },
        () => {
          this.Model_Loading = false;
        },
        () => {
          this.Model_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onModelChanged($event) {
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.ModelIDs = $event.map((a) => a.Id);
      this.SelectedModels = $event;
    }
  }

  getYears(searchKeyword: string) {
    this.SearchStream.initStream("YearsDDL_VehicleList", (a) => {
      this.Year_Loading = true;
      this.lookupService.GetTransporterProductionYears().subscribe(
        (res) => {
          if (res.Value) {
            this.YearList = res.Value;
          }
        },
        () => {
          this.Year_Loading = false;
        },
        () => {
          this.Year_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onYearChanged($event) {
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.YearIDs = $event.map((a) => a.Id);
      this.SelectedYears = $event;
    }
  }

  getVehicleTypes(searchKeyword: string) {
    this.SearchStream.initStream("VehicleTypesDDL_VehicleList", () => {
      this.VehicleType_Loading = true;
      this.lookupService.GetTransporterTypes().subscribe(
        (res) => {
          if (res.Value) {
            this.VehicleTypeList = res.Value;
          }
        },
        () => {
          this.VehicleType_Loading = false;
        },
        () => {
          this.VehicleType_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onVehicleTypeChanged($event) {
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.VehicleTypeIDs = $event.map((a) => a.Id);
      this.SelectedVehicleTypes = $event;
    }
  }

  getGroups(searchKeyword: string) {
    this.SearchStream.initStream("GroupsDDL_VehicleList", (a) => {
      this.Group_Loading = true;
      this.lookupService.GetTransporterGroups().subscribe(
        (res) => {
          if (res.Value) {
            this.GroupList = res.Value;
          }
        },
        () => {
          this.Group_Loading = false;
        },
        () => {
          this.Group_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onGroupChanged($event) {
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.GroupIDs = $event.map((a) => a.Id);
      this.SelectedGroups = $event;
    }
  }

  getStatuses(searchKeyword: string) {
    this.SearchStream.initStream("StatusesDDL_VehicleList", () => {
      this.Status_Loading = true;
      this.lookupService.GetTransporterStatuses().subscribe(
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
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.StatusIDs = $event.map((a) => a.Id);
      this.SelectedStatuses = $event;
    }
  }

  getBranches(searchKeyword: string) {
    this.SearchStream.initStream("BranchesDDL_VehicleList", (a) => {
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
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.BranchIDs = $event.map((a) => a.Id);
      this.SelectedBranches = $event;
      this.getSubBranches("");
    }
  }

  getSubBranches(searchKeyword: string) {
    this.SearchStream.initStream("SubBranchesDDL_VehicleList", (a) => {
      this.SubBranch_Loading = true;
      this.lookupService.getSubBranches(a, this.VehicleSearchCriteriaDTO.BranchIDs).subscribe(
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
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.SubBranchIDs = $event.map((a) => a.Id);
      this.SelectedSubBranches = $event;
      this.getLandmarks("");
    }
  }

  getLandmarks(searchKeyword: string) {
    this.SearchStream.initStream("LandmarksDDL_VehicleList", (a) => {
      this.Landmark_Loading = true;
      this.lookupService.getLandmarks(a, this.VehicleSearchCriteriaDTO.SubBranchIDs).subscribe(
        (res) => {
          if (res.Value) {
            this.LandmarkList = res.Value;
          }
        },
        () => {
          this.Landmark_Loading = false;
        },
        () => {
          this.Landmark_Loading = false;
        }
      );
    }).next(searchKeyword);
  }
  onLandmarkChanged($event) {
    if ($event.length > 0) {
      this.VehicleSearchCriteriaDTO.LandmarkIDs = $event.map((a) => a.Id);
      this.SelectedLandmarks = $event;
    }
  }
}
