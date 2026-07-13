import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { LoaderService } from "src/app/shared/loader.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Lookup } from "src/app/TMS-Module/Models/common/lookup";
import { VehicleDTO } from "src/app/TMS-Module/Models/vehicle-dto";
import { LookupService } from "../../../Services/lookup.service";
import { VehicleListService } from "../../../Services/vehicle-list.service";
import { SearchStream } from "src/app/TMS-Module/Models/common/search-stream-object.model";
import { EditVehicleStatusDTO } from "@tms-models/edit-vehicle-status-dto";
import { ZoneListDTO } from "@tms-models/zone-list";

@Component({
  selector: "app-new-vehicle",
  templateUrl: "./new-vehicle.component.html",
  styleUrls: ["./new-vehicle.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class NewVehicleComponent implements OnInit {
  VehicleTypeList: Lookup<string>[] = [];
  BranchList: Lookup<string>[] = [];
  SubBranchList: Lookup<string>[] = [];
  BrandList: Lookup<string>[] = [];
  ModelList: Lookup<string>[] = [];
  YearList: Lookup<string>[] = [];
  DriversList: Lookup<string>[] = [];
  staffRoleCategoryList: Lookup<string>[] = [];
  staffList: Lookup<string>[] = [];
  SelectedVehicleTypes: Lookup<string>[] = [];
  StatusList: Lookup<number>[] = [];
  SelectedStatuses: Lookup<number>[] = [];
  SelectedBranches: Lookup<string>[] = [];
  SelectedSubBranches: Lookup<string>[] = [];
  SelectedBrands: Lookup<string>[] = [];
  SelectedModels: Lookup<string>[] = [];
  SelectedYears: Lookup<string>[] = [];
  SelectedStaff: Lookup<string>[] = [];
  LandmarkList: Lookup<string>[] = [];
  SelectedLandmarks: Lookup<string>[] = [];
  VehicleDTO: VehicleDTO = new VehicleDTO();
  SearchStream: SearchStream = new SearchStream();
  IsEditableMood: boolean = false;
  VehicleID: string;
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
  Status_Loading = false;
  Branch_Loading = false;
  SubBranch_Loading = false;
  Landmark_Loading = false;
  Brand_Loading = false;
  Model_Loading = false;
  Year_Loading = false;
  Staff_Loading = false;
  currentTab = 1;
  selectedCategory = null;

  constructor(
    private translateService: TranslateService,
    private router: Router,
    private _alert: alertService,
    private route: ActivatedRoute,
    private lookupService: LookupService,
    private vehicleListService: VehicleListService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName(
      "Transporters"
    );
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
  }

  switchTab(tabName) {
    this.currentTab = tabName;
  }

  ngOnInit() {
    this.VehicleID = this.route.snapshot.paramMap.get("ID");
  }

  ngAfterViewInit() {
    this.load();
    this.translateService.onLangChange.subscribe((res) => {
      this.load();
    });
  }

  load() {
    this.getVehicleTypes("");
    this.getStatuses("");
    this.getBranches("");
    this.getBrands("");
    this.getModels("");
    this.getYears("");
    this.getStaffRoleCategory("");
    if (this.VehicleID != null) {
      this.IsEditableMood = true;
      this.GetVehicleDetails();
    }
    this.titleService.setTitle(
      this.translateService.instant(
        this.IsEditableMood ? "EditVehicle" : "NewVehicle"
      )
    );
  }
  GetVehicleDetails() {
    this.mainloading.PreloaderIcreaseCount();
    this.vehicleListService.GetOne(this.VehicleID).subscribe(
      (res) => {
        if (res.Value) {
          this.VehicleDTO = res.Value;
          this.getSubBranches("");
          this.getLandmarks("");
          if (this.VehicleTypeList.length > 0) {
            this.SelectedVehicleTypes = this.VehicleTypeList.filter(
              (x) => x.Id == this.VehicleDTO.TransporterType
              );
            }
            if (this.BranchList.length > 0) {
              this.SelectedBranches = this.BranchList.filter(
              (x) => x.Id == this.VehicleDTO.Branch
            );
          }
          if (this.LandmarkList.length > 0) {
            this.SelectedLandmarks = this.LandmarkList.filter(
              (x) => x.Id == this.VehicleDTO.Landmark
            );
          }
          if (this.BrandList.length > 0) {
            this.SelectedBrands = this.BrandList.filter(
              (x) => x.Id == this.VehicleDTO.Brand
            );
          }
          if (this.ModelList.length > 0) {
            this.SelectedModels = this.ModelList.filter(
              (x) => x.Id == this.VehicleDTO.Model
            );
          }
          if (this.StatusList.length > 0) {
            this.SelectedStatuses = this.StatusList.filter(
              (x) => x.Id == this.VehicleDTO.Status
            );
          }
          if (this.YearList.length > 0) {
            this.SelectedYears = this.YearList.filter(
              (x) => x.Id == this.VehicleDTO.ProductionYear
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

  VehicleCodeChanged($event) {
    this.VehicleDTO.Code = $event.target.value;
  }
  VehiclePlateNoChanged($event) {
    this.VehicleDTO.PlateNo = $event.target.value;
  }
  VehicleChassisNoChanged($event) {
    this.VehicleDTO.ChassisNo = $event.target.value;
  }
  VehicleDeviceCodeChanged($event) {
    this.VehicleDTO.DeviceCode = $event.target.value;
  }
  VehicleSIMCardNoChanged($event) {
    this.VehicleDTO.SIMCardNo = $event.target.value;
  }
  VehicleCapacityChanged($event) {
    this.VehicleDTO.Capacity = $event.target.value;
  }
  getVehicleTypes(searchKeyword: string) {
    this.VehicleType_Loading = true;
    this.lookupService.GetTransporterTypes().subscribe(
      (res) => {
        if (res.Value) {
          this.VehicleTypeList = res.Value;
          if (
            isNullOrUndefined(this.VehicleDTO.TransporterType) ||
            this.VehicleDTO.TransporterType === ""
          ) {
            this.SelectedVehicleTypes = this.VehicleTypeList.filter(
              (x) => x.Id == this.VehicleDTO.TransporterType
            );
          }
        }
      },
      () => {
        this.VehicleType_Loading = false;
      },
      () => {
        this.VehicleType_Loading = false;
      }
    );
  }
  onVehicleTypeChanged($event) {
    console.log("onVehicleTypeChanged", $event);
    if ($event.length > 0) {
      this.VehicleDTO.TransporterType = $event[0].Id;
      this.VehicleDTO.TransporterTypeName = $event[0].Name;
      this.SelectedVehicleTypes = $event;
    }
  }

  businessList = {
    0: [0, 0],
    2: [2, 3, 12, 13],
    3: [3, 13],
    6: [6, 2, 13],
    11: [11, 0],
    12: [12, 2, 13],
    13: [13, 2, 3, 12],
  }

  filterStatusesDependOnVehicleSelectedStatus(statusList: any){
    let filteredList = this.businessList[this.VehicleDTO.Status];
    statusList.forEach((value, index) => {
      if(!filteredList.includes(value.Id)){
        console.log(value.Id, value.Name);
        statusList.splice(index, 1);
      }
    });(filteredList);
    return statusList;
  }

  getStatuses(searchKeyword: string) {
    this.Status_Loading = true;
    this.lookupService.GetTransporterStatuses().subscribe(
      (res) => {
        if (res.Value) {
          console.log(res.Value);
          this.StatusList = res.Value;//this.filterStatusesDependOnVehicleSelectedStatus(res.Value);
          if (isNullOrUndefined(this.VehicleDTO.Status)) {
            this.SelectedStatuses = this.StatusList.filter(
              (x) => x.Id == this.VehicleDTO.Status
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
    if ($event.length > 0) {
      this.VehicleDTO.Status = $event[0].Id;
      this.SelectedStatuses = $event;
    }
  }

  getBranches(searchKeyword: string) {
    this.Branch_Loading = true;
    this.lookupService.getBranches(searchKeyword).subscribe(
      (res) => {
        if (res.Value) {
          this.BranchList = res.Value;
          if (isNullOrUndefined(this.VehicleDTO.Branch)) {
            this.SelectedBranches = this.BranchList.filter(
              (x) => x.Id == this.VehicleDTO.Branch
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
      this.VehicleDTO.Branch = $event[0].Id;
      this.SelectedBranches = $event;
      this.getSubBranches("");
    }
  }

  getSubBranches(searchKeyword: string) {
    this.SubBranch_Loading = true;
    this.lookupService.getSubBranches(searchKeyword, [this.VehicleDTO.Branch]).subscribe(
      (res) => {
        if (res.Value) {
          this.SubBranchList = res.Value;
          if (!isNullOrUndefined(this.VehicleDTO.SubBranch)) {
            this.SelectedSubBranches = this.SubBranchList.filter(
              (x) => x.Id == this.VehicleDTO.SubBranch
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
      this.VehicleDTO.SubBranch = $event[0].Id;
      this.SelectedSubBranches = $event;
      console.log($event[0].Id);
      this.getLandmarks("");
    }
  }

  getLandmarks(searchKeyword: string) {
    this.Landmark_Loading = true;
    this.lookupService.getLandmarks(searchKeyword, [this.VehicleDTO.SubBranch]).subscribe(
      (res) => {
        if (res.Value) {
          this.LandmarkList = res.Value;
          if (isNullOrUndefined(this.VehicleDTO.Landmark)) {
            this.SelectedLandmarks = this.LandmarkList.filter(
              (x) => x.Id == this.VehicleDTO.Landmark
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
      this.VehicleDTO.Landmark = $event[0].Id;
      this.SelectedLandmarks = $event;
    }
  }

  getBrands(searchKeyword: string) {
    this.Brand_Loading = true;
    this.lookupService.GetTransporterBrands().subscribe(
      (res) => {
        if (res.Value) {
          this.BrandList = res.Value;
          if (isNullOrUndefined(this.VehicleDTO.Brand)) {
            this.SelectedBrands = this.BrandList.filter(
              (x) => x.Id == this.VehicleDTO.Brand
            );
          }
        }
      },
      () => {
        this.Brand_Loading = false;
      },
      () => {
        this.Brand_Loading = false;
      }
    );
  }
  onBrandChanged($event) {
    if ($event.length > 0) {
      this.VehicleDTO.Brand = $event[0].Id;
      this.SelectedBrands = $event;
    }
  }

  getModels(searchKeyword: string) {
    this.Model_Loading = true;
    this.lookupService.GetTransporterManufacturer().subscribe(
      (res) => {
        if (res.Value) {
          this.ModelList = res.Value;
          if (isNullOrUndefined(this.VehicleDTO.Model)) {
            this.SelectedModels = this.ModelList.filter(
              (x) => x.Id == this.VehicleDTO.Model
            );
          }
        }
      },
      () => {
        this.Model_Loading = false;
      },
      () => {
        this.Model_Loading = false;
      }
    );
  }
  onModelChanged($event) {
    if ($event.length > 0) {
      this.VehicleDTO.Model = $event[0].Id;
      this.SelectedModels = $event;
    }
  }

  getYears(searchKeyword: string) {
    this.Year_Loading = true;
    this.lookupService.GetTransporterProductionYears().subscribe(
      (res) => {
        if (res.Value) {
          this.YearList = res.Value;
          if (isNullOrUndefined(this.VehicleDTO.ProductionYear)) {
            this.SelectedYears = this.YearList.filter(
              (x) => x.Id == this.VehicleDTO.ProductionYear
            );
          }
        }
      },
      () => {
        this.Year_Loading = false;
      },
      () => {
        this.Year_Loading = false;
      }
    );
  }
  onYearChanged($event) {
    if ($event.length > 0) {
      this.VehicleDTO.ProductionYear = $event[0].Id;
      this.SelectedYears = $event;
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
    this.SearchStream.initStream("SearchStaffByCategory", (a) => {
      this.Staff_Loading = true;
      this.lookupService
        .SearchStaffByCategory(a, this.selectedCategory)
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

  saveVehicle() {
    //verification

    if (this.isValidModel()) {
      if (this.IsEditableMood) {
        this.mainloading.PreloaderIcreaseCount();
        this.vehicleListService.Update(this.VehicleDTO).subscribe(
          (res) => {
            if (!res.IsErrorState) {
              this._alert.showSuccess();
              // redirect to /tms/vehiclelist
              this.router.navigate(["/tms/vehicle/vehiclelist"]);
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
      } else {
        this.mainloading.PreloaderIcreaseCount();
        this.vehicleListService.Add(this.VehicleDTO).subscribe(
          (res) => {
            if (!res.IsErrorState) {
              this._alert.showSuccess();
              this.router.navigate(["/tms/vehicle/vehiclelist"]);
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
  }
  addStaff() {
    this.SelectedStaff.forEach((s) => {
      this.VehicleDTO.Staff.push({
        Id: s.Id,
        StaffName: s.Name,
        VehicleReceivingDate: new Date(),
      });
    });
    this.SelectedStaff = [];
  }

  deleteStaff(staffId) {
    this.VehicleDTO.Staff = (this.VehicleDTO.Staff || []).filter(
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

  changeVehicleStatus(){
    let dto = new EditVehicleStatusDTO();
    dto.VehicleStatusID = this.VehicleDTO.Status;
    dto.TransporterID = this.VehicleID;
    console.log(dto.VehicleStatusID, this.VehicleID);
    this.vehicleListService.UpdateVehicleStatus(dto).subscribe(
      (res) => {
        if (!res.IsErrorState) {
          this._alert.showSuccess();
          this.router.navigate(["/tms/vehicle/vehiclelist"]);
        } else {
          this._alert.errorList(res.Errors);
        }
      });
  }

  isValidModel(): boolean {
    this.validationMessages = [];
    if (
      isNullOrUndefined(this.VehicleDTO.Code) ||
      this.VehicleDTO.Code === ""
    ) {
      this.validationMessages.push("InsertVehicleCode");
    }
    if (
      isNullOrUndefined(this.VehicleDTO.PlateNo) ||
      this.VehicleDTO.PlateNo === ""
    ) {
      this.validationMessages.push("InsertVehiclePlateNo");
    }
   

    // if (isNullOrUndefined(this.VehicleDTO.ChassisNo) ||this.VehicleDTO.ChassisNo === "") 
    // {
    //   this.validationMessages.push("InsertVehicleChassisNo");
    // }
    // if ( isNullOrUndefined(this.VehicleDTO.DeviceCode) ||this.VehicleDTO.DeviceCode === "") {
    //   this.validationMessages.push("InsertVehicleDeviceCode");
    // }
    // if (isNullOrUndefined(this.VehicleDTO.SIMCardNo) || this.VehicleDTO.SIMCardNo === "") {
    //   this.validationMessages.push("InsertVehicleSIMCardNo");
    // }

    if (
      isNullOrUndefined(this.VehicleDTO.TransporterType) ||
      this.VehicleDTO.TransporterType === ""
    ) {
      this.validationMessages.push("ChooseVehicleType");
    }
    if (isNullOrUndefined(this.VehicleDTO.Status)) {
      this.validationMessages.push("ChooseStatus");
    }
   
    

    if (this.validationMessages.length > 0) {
      this._alert.errorList(this.validationMessages);
      return false;
    }
    return true;
  }
}
