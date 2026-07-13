import { Component, OnInit, OnDestroy } from '@angular/core';
import { Lookup } from '@tms-models/common/lookup';
import { alertService } from '@tms-shared/Services/alert/alert.service';
import { LookupService } from '@tms-services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { isNullOrUndefined } from '@tms-shared/utilities/utilities';
import { EventWorkOrder } from '@tms-models/events/event-workorder';
import { ActivatedRoute, Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { SearchStream } from '@tms-models/common/search-stream-object.model';
import { LoaderService } from '@tms-shared/loader.service';
import { Configuration } from '@tms-configuration/shared.config';
import { forkJoin } from 'rxjs';
import { StaffModel } from '@tms-models/staff-model';
import { StaffService } from '@tms-services/staff-service';
import { StaffSearchCriteria } from '@tms-models/search-criteria/staff-search-criteria';
import { FilterModel } from '@tms-models/common/filter-model';
import { PageFilter } from '@tms-models/common/page-fillter-model';
import { array } from '@amcharts/amcharts4/core';

@Component({
  selector: 'app-staff-create',
  templateUrl: './staff-create.component.html',
  styleUrls: ['./staff-create.component.scss']
})
export class StaffCreateComponent implements OnInit, OnDestroy {

  staffModel: StaffModel;
  eventOrder: EventWorkOrder;
  customerId: number;
  memberID: string;

  pagePermission = '';
  bulkCreate = false;
  BC_StartingTimeString: string;
  OrdersNoPerMonth: number;



  //#region Drop Down list
  SearchStream: SearchStream = new SearchStream();

  //#region  "Lookups declarations"
  customerNameList: Lookup<number>[] = [];
  staffRoleCategoryList: Lookup<number>[] = [];
  staffRoleCategoryListSelected: Lookup<number>[] = [];
  staffRoleList: Lookup<string>[] = [];
  staffRoleListSelected: Lookup<string>[] = [];
  staffBranchList: Lookup<string>[] = [];
  staffBranchListSelected: Lookup<string>[] = [];
  staffSubBranchList: Lookup<string>[] = [];
  staffSubBranchListSelected: Lookup<string>[] = [];
  staffPermittedBranchList: Lookup<string>[] = [];
  staffPermittedBranchSelectedList: Lookup<string>[] = [];
  staffPermittedSubBranchList: Lookup<string>[] = [];
  staffPermittedSubBranchListSelected: Lookup<string>[] = [];
  staffLandmarkList: Lookup<string>[] = [];
  staffLandmarkListSelected: Lookup<string>[] = [];
  // customerLocationsList: Lookup<number>[] = [];
  customerAccountsList: Lookup<number>[] = [];
  lacationStationsList: Lookup<string>[] = [];
  // customerServiceList: Lookup<number>[] = [];
  accessoriesList: Lookup<number>[] = [];
  categoriesList: Lookup<number>[] = [];
  TanckerCapacityList: Lookup<number>[] = [];
  //#endregion  "Lookups declarations"

  //#region "LookupsBindingModels"
  // bindingModel_CustomerLocations: Lookup<number>[] = [];
  bindingModel_CustomerAccounts: Lookup<number>[] = [];
  bindingModel_lacationStations: Lookup<string>[] = [];
  bindingModel_TanckerCapacity: Lookup<number>[] = [];
  //#endregion "LookupsBindingModels"
  bindingModel_Category: Lookup<number>[] = [];

  selectMenuOptions = {
    enableSearchFilter: false,
  };
  selectMenuOptions2 = {
    enableSearchFilter: true,
    singleSelect: true
  };

  selectMenuOptions3 = {
    enableSearchFilter: false,
    singleSelect: true
  };



  Customer_Loading = false;
  CustomerAccount_Loading = false;
  // CustomerAddress_Loading = false;
  Station_Loading = 0;
  Capacity_Loading = false;
  SearchCriteria: StaffSearchCriteria;



  constructor(
    private staffService: StaffService,
    private _alert: alertService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private router: Router,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private activatedsrRoute: ActivatedRoute,

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('orderlist');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
    this.staffModel = new StaffModel();
    if (this.activatedsrRoute.snapshot.paramMap.has('id')) {
      this.memberID = this.activatedsrRoute.snapshot.paramMap.get('id');
      this.setInitUpdateData(this.memberID);
    }
    this.bulkCreate = this.activatedsrRoute.snapshot.routeConfig.path === 'bulkCreate';
  }

  ngOnInit() {
    this.setInitCreateData();
    this.getStaffRoleCategory('');
    this.getStaffBranch('');

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });

  }

  setInitCreateData() {
    this.staffModel = new StaffModel();
  }

  setInitUpdateData(id: string) {
    this.staffService.getStaffMemberByID(id).subscribe((member) => {
      if (member.Value) {
        this.staffModel = member.Value;
        this.getStaffRoles(this.staffModel.staffRoleCategoryID);
        this.getStaffSubBranch("");
        this.getStaffLandmark("");
        this.getStaffPermittedSubBranch("");
        if (this.staffRoleCategoryList.length > 0) {
          this.staffRoleCategoryListSelected = this.staffRoleCategoryList.filter(
            (x) => x.Id == this.staffModel.staffRoleCategoryID
          );
        }
        if (this.staffBranchList.length > 0) {
          this.staffBranchListSelected = this.staffBranchList.filter(
            (x) => x.Id == this.staffModel.AllocatedBranch
          );
        }
        if (this.staffPermittedBranchList.length > 0) {
          this.staffPermittedBranchSelectedList = this.staffPermittedBranchList.filter(
            (x) => (this.staffModel.PermittedBranch||[]).some((y) => x.Id == y)
          );
        }
      }
    });
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new StaffSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
  }

  loadDDLsGV() {
    const pageTitle = this.memberID ? 'UpdateRole' : 'CreateNewRole';
    this.titleService.setTitle(this._translate.instant(pageTitle));

  }

  getStaffRoleCategory(searchKeyword: string) {
    this.SearchStream.initStream('', (a) => {
      this.Customer_Loading = true;
      this.lookupservice.getStaffRoleCategory().subscribe(res => {
        if (res.Value) {
          this.staffRoleCategoryList = res.Value;
          if (this.staffRoleCategoryList.length > 0) {
            this.staffRoleCategoryListSelected = this.staffRoleCategoryList.filter(
              (x) => x.Id == this.staffModel.staffRoleCategoryID
            );
          }
        }
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }


  getStaffRoles(id: any) {

    this.SearchStream.initStream('getStaffRolesByCategoryID', (a) => {
      this.Customer_Loading = true;
      this.lookupservice.getStaffRolesByCategoryID(a).subscribe(res => {
        if (res.Value) {
          this.staffRoleList = res.Value;
          if (this.staffRoleList.length > 0) {
            this.staffRoleListSelected = this.staffRoleList.filter(
              (x) => x.Id == this.staffModel.staffRoleID
            );
          }
        }
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(id);
  }

  getStaffBranch(searchKeyword: string) {
    this.SearchStream.initStream('getStaffBranch', (a) => {
      this.Customer_Loading = true;
      this.lookupservice.getBranches(a).subscribe(res => {
        if (res.Value) {
          this.staffBranchList = res.Value;
          this.staffPermittedBranchList = res.Value;
          if (this.staffBranchList.length > 0) {
            this.staffBranchListSelected = this.staffBranchList.filter(
              (x) => x.Id == this.staffModel.AllocatedBranch
            );
          }
          if (this.staffPermittedBranchList.length > 0) {
            this.staffPermittedBranchSelectedList = this.staffPermittedBranchList.filter(
              (x) => (this.staffModel.PermittedBranch||[]).some((y) => x.Id == y)
            );
          }
        }
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }

  getSelctedItemsOnList(fullArray: Lookup<any>[], selctedIDs: string[]) {
    return selctedIDs.map(item => {
      return fullArray.filter(e => e.Id === item && e)[0];
    })
  }

  getStaffSubBranch(searchKeyword: string) {
    this.SearchStream.initStream('getStaffSubBranch', (a) => {
      this.Customer_Loading = true;
      this.lookupservice.getSubBranches(a, [this.staffModel.AllocatedBranch]).subscribe(res => {
        if (res.Value) {
          this.staffSubBranchList = res.Value;
          if (this.staffSubBranchList.length > 0) {
            this.staffSubBranchListSelected = this.staffSubBranchList.filter(
              (x) => x.Id == this.staffModel.AllocatedSubBranch
            );
          }
        }
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }

  getStaffPermittedSubBranch(searchKeyword: string) {
    this.SearchStream.initStream('getStaffPermittedSubBranch', (a) => {
      this.Customer_Loading = true;
      this.lookupservice.getSubBranches(a, this.staffModel.PermittedBranch).subscribe(res => {
        if (res.Value) {
          this.staffPermittedSubBranchList = res.Value;
          if (this.staffPermittedSubBranchList.length > 0) {
            this.staffPermittedSubBranchListSelected = this.staffPermittedSubBranchList.filter(
              (x) => (this.staffModel.PermittedSubBranch||[]).some((y) => x.Id == y)
            );
          }
        }
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }

  getStaffLandmark(searchKeyword: "") {
    this.lookupservice.getLandmarks(searchKeyword, [this.staffModel.AllocatedSubBranch]).subscribe(res => {
      if (res.Value) {
        this.staffLandmarkList = res.Value;
        if (this.staffLandmarkList.length > 0) {
          this.staffLandmarkListSelected = this.staffLandmarkList.filter(
            (x) => x.Id == this.staffModel.AllocatedLandmark
          );
        }
      }
    }
      , err => {
        this.Customer_Loading = false;
      }
      , () => {
        this.Customer_Loading = false;
      });
  }

  onStaffRolesDDLChanged(evt) {
    this.staffRoleListSelected = evt;
    this.staffModel.staffRoleID = evt.map(m => m.Id)[0];
  }

  onStaffRoleCategoryDDLChanged(evt) {
    this.staffRoleCategoryListSelected = evt;
    this.staffModel.staffRoleCategoryID = evt.map(m => m.Id)[0];
    this.getStaffRoles(this.staffModel.staffRoleCategoryID);
  }

  ongetStaffBranchDDLChanged(evt) {
    this.staffModel.AllocatedBranch = evt[0].Id;
    this.staffBranchListSelected = evt;
    this.getStaffSubBranch("");
  }

  ongetStaffSubBranchDDLChanged(evt) {
    this.staffModel.AllocatedSubBranch = evt[0].Id;
    this.getStaffLandmark("")
  }

  ongetStaffLandmarkDDLChanged(evt) {
    this.staffModel.AllocatedLandmark = evt[0].Id;
  }

  ongetStaffPermittedBranchhDDLChanged(evt) {
    this.staffModel.PermittedBranch = evt.map((x) => x.Id);
    if ((this.staffModel.PermittedBranch || []).length)
      this.getStaffPermittedSubBranch("");
    else this.staffPermittedSubBranchList = [];
  }

  ongetStaffPermittedSubBranchhDDLChanged(evt) {
    this.staffModel.PermittedSubBranch = evt.map((x) => x.Id);
  }

  isValidModel(): boolean {
    const validationMessages: string[] = [];

    if (isNullOrUndefined(this.staffModel.FirstName) || this.staffModel.FirstName === '') {
      validationMessages.push('AddFirstName');
    }
    if (isNullOrUndefined(this.staffModel.MiddleName) || this.staffModel.MiddleName === '') {
      validationMessages.push('AddMiddleName');
    }
    if (isNullOrUndefined(this.staffModel.LastName) || this.staffModel.LastName === '') {
      validationMessages.push('AddLastName');
    }
    if (isNullOrUndefined(this.staffModel.personalID) || this.staffModel.personalID === '') {
      validationMessages.push('AddpersonalID');
    }
    const ex = new RegExp(Configuration.RegExp.ValidMobileNumber);
    if (this.staffModel.mobileNumber && !ex.test(this.staffModel.mobileNumber)) {
      validationMessages.push('InvalidMobileNumber');
    }



    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  save() {
    this.staffModel.username = this.staffModel.personalID;

    if(this.memberID && this.memberID !== '') {
      this.staffService.updateStaffMember(this.staffModel).subscribe(res => this.navigateToList())
    } else {
      this.staffService.createNewStaffMember(this.staffModel).subscribe(res => this.navigateToList())
    }

    console.log(this.staffModel);
  }


  cancel() {
    this._alert.confirmationMessage('ConfirmClose').subscribe(confirm => {
      if (confirm === true) {
        this.navigateToList();
      }
    });
  }

  navigateToList() {
    this.router.navigate(['/tms/staff']);
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }


}
