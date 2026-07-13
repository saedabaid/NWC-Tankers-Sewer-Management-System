import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { LoaderService } from "src/app/shared/loader.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Lookup } from "src/app/TMS-Module/Models/common/lookup";
import { violationapprovalsDto } from "src/app/TMS-Module/Models/violation-approvals";
import { LookupService } from "../../../Services/lookup.service";
import { VehicleListService } from "../../../Services/vehicle-list.service";
import { SearchStream } from "src/app/TMS-Module/Models/common/search-stream-object.model";
import { EditVehicleStatusDTO } from "@tms-models/edit-vehicle-status-dto";
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { ContractApprovalViolation } from 'src/app/TMS-Module/Models/search-criteria/contract-violation-SC.model';
import { Configuration } from "../../../../shared/configurations/shared.config";
import { SearchResult } from "../../../Models/common/search-result";

@Component({
  selector: 'app-new-contract-violation-approval',
  templateUrl: './new-contract-violation-approval.component.html',
  styleUrls: ['./new-contract-violation-approval.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class NewContractViolationApprovalComponent implements OnInit {
  BranchList: Lookup<string>[] = [];
  SubBranchList: Lookup<string>[] = [];
  staffRoleCategoryList: Lookup<string>[] = [];
  staffList: Lookup<string>[] = [];
  SelectedBranches: Lookup<string>[] = [];
  SelectedSubBranches: Lookup<string>[] = [];
  SelectedStaff: Lookup<string>[] = [];
  LandmarkList: Lookup<string>[] = [];
  SelectedLandmarks: Lookup<string>[] = [];


  advancedDiv = <boolean>false;
  SearchCriteria: ContractApprovalViolation;
  searchResult = new SearchResult<violationapprovalsDto>();
  ViolationApprovalDTO: violationapprovalsDto = new violationapprovalsDto();

  SearchStream: SearchStream = new SearchStream();
  IsEditableMood: boolean = false;
  ViolationID: string;
  validationMessages: string[] = [];
  selectMenuOptions = {
    enableSearchFilter: true,
    singleSelect: true,
  };
  selectMenuOptions2 = {
    enableSearchFilter: false,
    singleSelect: true,
  };
  pagePermission: string = "";
  VehicleType_Loading = false;
  Branch_Loading = false;
  SubBranch_Loading = false;
  Landmark_Loading = false;
  Staff_Loading = false;
  currentTab = 1;
  selectedCategory = null;

  constructor(
    private translateService: TranslateService,
    private router: Router,
    private _alert: alertService,
    private route: ActivatedRoute,
    private lookupService: LookupService,
    private contractService: ContractService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName(
      "tmsContractTermsViolations"
    );
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
  }

  switchTab(tabName) {
    this.currentTab = tabName;
  }

  ngOnInit() {
    this.ViolationID = this.route.snapshot.paramMap.get("ID");
    this.setDefaultSearchValues();
    this.getViolationApprovalList();

  }
  setDefaultSearchValues() {
    this.SearchCriteria = new ContractApprovalViolation();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    // this.SearchCriteria.ViolationDateFrom = new Date();
    // this.SearchCriteria.ViolationDateTo = new Date();
  }

  ngAfterViewInit() {
    this.load();
    this.translateService.onLangChange.subscribe((res) => {
      this.load();
    });
  }

  load() {
    this.getBranches("");
    this.getStaffRoleCategory("");
    if (this.ViolationID != null) {
      this.IsEditableMood = true;
    }
    this.titleService.setTitle(
      this.translateService.instant(
        this.IsEditableMood ? "EditContractViolationApproval" : "NewContractViolationApproval"
      )
    );
  }
  getViolationApprovalList() {
    this.contractService.GetContractApprovalViolation(this.SearchCriteria).subscribe(res => {
      if (res.Value) this.searchResult = res.Value;
    })
  }
  deleteViolationApproval(ViolationApprovalId: number) {
    this._alert.confirmationMessage("ConfirmDelete").subscribe(confirm => {
      if (confirm == true) {
        this.mainloading.PreloaderIcreaseCount();
        this.contractService.DeleteViolationApproval(ViolationApprovalId).subscribe(res => {
          if (!res.IsErrorState) {
            this._alert.success("DeletedSuccessed");
            this.onSearchSubmit();
          }
          else {
            this._alert.errorList(res.Errors);
          }
        }
          , err => {
            this.mainloading.PreloaderDecreaseCount();
          }
          , () => {
            this.mainloading.PreloaderDecreaseCount();
          });
      }
    });

  }
  onSearchSubmit() {
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.LandmarkID = this.ViolationApprovalDTO.Landmark;
    this.getViolationApprovalList();
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.PageFilter.PageIndex = evt;
    this.getViolationApprovalList();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.PageFilter.PageSize = evt;
    this.getViolationApprovalList();
  }
  getBranches(searchKeyword: string) {
    this.Branch_Loading = true;
    this.lookupService.getBranches(searchKeyword).subscribe(
      (res) => {
        if (res.Value) {
          this.BranchList = res.Value;
          if (isNullOrUndefined(this.ViolationApprovalDTO.Branch)) {
            this.SelectedBranches = this.BranchList.filter(
              (x) => x.Id == this.ViolationApprovalDTO.Branch
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
    if ($event.length > 0) {
      this.ViolationApprovalDTO.Branch = $event[0].Id;
      this.SelectedBranches = $event;
      this.getSubBranches("");
    }
  }

  getSubBranches(searchKeyword: string) {
    this.SubBranch_Loading = true;
    this.lookupService.getSubBranches(searchKeyword, [this.ViolationApprovalDTO.Branch]).subscribe(
      (res) => {
        if (res.Value) {
          this.SubBranchList = res.Value;
          if (!isNullOrUndefined(this.ViolationApprovalDTO.SubBranch)) {
            this.SelectedSubBranches = this.SubBranchList.filter(
              (x) => x.Id == this.ViolationApprovalDTO.SubBranch
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
    if ($event.length > 0) {
      this.ViolationApprovalDTO.SubBranch = $event[0].Id;
      this.SelectedSubBranches = $event;
      console.log($event[0].Id);
      this.getLandmarks("");
    }
  }

  getLandmarks(searchKeyword: string) {
    this.Landmark_Loading = true;
    this.lookupService.getLandmarks(searchKeyword, [this.ViolationApprovalDTO.SubBranch]).subscribe(
      (res) => {
        if (res.Value) {
          this.LandmarkList = res.Value;
          if (isNullOrUndefined(this.ViolationApprovalDTO.Landmark)) {
            this.SelectedLandmarks = this.LandmarkList.filter(
              (x) => x.Id == this.ViolationApprovalDTO.Landmark
            );
          }
        }
      },
      () => {
        this.Landmark_Loading = false;
      },
      () => {
        this.Landmark_Loading = false;
      }
    );
  }
  onLandmarkChanged($event) {
    if ($event.length > 0) {
      this.ViolationApprovalDTO.Landmark = $event[0].Id;
      this.SelectedLandmarks = $event;
    }
  }
  LevelNoChanged($event) {
    debugger;
    if ($event.length > 0) {

      this.ViolationApprovalDTO.LevelNo = $event[0];
    }
  }
  StaffChanged($event) {
    debugger
    if ($event.length > 0) {
      this.ViolationApprovalDTO.StaffId = $event[0].Id;
      ;
    }
  }

  getStaffRoleCategory(searchKeyword: string) {
    this.Staff_Loading = true;
    this.lookupService.getStaffRoleCategory().subscribe(
      (res) => {
        if (res.Value) {
          this.staffRoleCategoryList = res.Value;
        }
      },
      (err) => {
        this.Staff_Loading = false;
      },
      () => {
        this.Staff_Loading = false;
      }
    );
  }
  onStaffRoleCategoryDDLChanged(evt) {
    this.selectedCategory = evt.map((m) => m.Id)[0];
    this.getStaff("");
  }

  getStaff(searchKeyword: string) {
    this.SearchStream.initStream("SearchStaff", (a) => {
      this.Staff_Loading = true;
      this.lookupService
        .SearchStaff(a)
        .subscribe(
          (res) => {
            if (res.Value) {
              this.staffList = res.Value;
            }
          },
          (err) => {
            this.Staff_Loading = false;
          },
          () => {
            this.Staff_Loading = false;
          }
        );
    }).next(searchKeyword);
  }

  saveApproval() {
    {
      if (!this.isValidModel()) return;
      this.mainloading.PreloaderIcreaseCount();
      this.contractService.AddViolationApproval(this.ViolationApprovalDTO).subscribe(
        (res) => {
          if (!res.IsErrorState) {
            this._alert.showSuccess();
            this.router.navigate(["/tms/contract/violationapproval/add"]);
          } else {
            debugger;
            if (res.ErrorMetadata == "543") {
              this._alert.error("You Must Add The Previous Level First");
            }
            else if (res.ErrorMetadata == "542") {
              this._alert.error("The Level Is Repeated");
            }
            else
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
  addStaff() {
    this.SelectedStaff.forEach((s) => {
      this.ViolationApprovalDTO.Staff.push({
        Id: s.Id,
        StaffName: s.Name,
        VehicleReceivingDate: new Date(),
      });
    });
    this.SelectedStaff = [];
  }

  deleteStaff(staffId) {
    this.ViolationApprovalDTO.Staff = (this.ViolationApprovalDTO.Staff || []).filter(
      (s) => s.Id !== staffId
    );
  }

  cancel() {
    this._alert.confirmationMessage("ConfirmClose").subscribe((confirm) => {
      if (confirm === true) {
        this.router.navigate(["/tms/vehicle/vehiclelist"]);
      }
    });
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.getViolationApprovalList();
  }

  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.ViolationApprovalDTO.StaffId)) {
      // validationMessages.push("DashboardSelectArea");
      validationMessages.push("ChooseStaff");
    }
    if (isNullOrUndefined(this.ViolationApprovalDTO.Landmark)) {
      // validationMessages.push("DashboardSelectArea");
      validationMessages.push("ChooseLandmark");
    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

}
