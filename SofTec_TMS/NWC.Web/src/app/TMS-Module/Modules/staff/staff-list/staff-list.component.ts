import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";

import {
  WorkOrderSearchCriteria,
  DateperiodEnum,
  Operator,
} from '@tms-models/search-criteria/work-order-search-criteria';

import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { WorkOrderSearchService } from '@tms-services/work-order-search.service';
import { PageFilter } from '@tms-models/common/page-fillter-model';

import { isNullOrUndefined } from '@tms-shared/utilities/utilities';
import { FilterModel } from '@tms-models/common/filter-model';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { Configuration } from '@tms-configuration/shared.config';
import { Lookup } from '@tms-models/common/lookup';
import { SearchResult } from '@tms-models/common/search-result';
import { OrderDetails } from '@tms-models/order-details';
import { alertService } from '@tms-shared/Services/alert/alert.service';
import { ExcelService } from '@tms-shared/Services/excel/ExcelService';
import { TranslateService } from '@ngx-translate/core';
import { LookupService } from '@tms-services/lookup.service';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderExcel } from '@tms-models/OrderExcel';
import { Title } from '@angular/platform-browser';
import { SearchStream } from '@tms-models/common/search-stream-object.model';
import { LoaderService } from '@tms-shared/loader.service';
import { StaffSearchCriteria } from '@tms-models/search-criteria/staff-search-criteria';
import { StaffService } from '@tms-services/staff-service';
import { StaffModel } from '@tms-models/staff-model';
import { UploadStaffExcelComponent } from '../upload-staff-excel/upload-staff-excel.component';

@Component({
  selector: 'app-staff-list',
  templateUrl: './staff-list.component.html',
  styleUrls: ['./staff-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class StaffListComponent implements OnInit, OnDestroy {

  constructor(
    private mainLoading: LoaderService,
    private router: Router,
    private authServer: AuthenticationService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private titleService: Title,
    private staffService: StaffService,
    private activatedRoute: ActivatedRoute,
    private modalRef: BsModalRef,
    private modalService: BsModalService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName(
      'orderlist',
    );
    this.authServer.checkViewPrivilege(this.pagePermission, true);

    this.tmsCustomerPermission = this.authServer.getCurrentUserPermissionByRoleName(
      'tmsCustomers',
    );

    const urlList = this.router.routerState.snapshot.url.split('/');
    if (!isNullOrUndefined(urlList[4])) {
      this.recievedFilters = urlList[4].split('-');
    }
  }
  dropdownSettings: IDropdownSettings = {};
  advancedDiv = <boolean>false;
  closeBtnName: string;
  SearchCriteria: StaffSearchCriteria;
  tableLoading = false;
  recievedFilters: string[] = [];


  selectMenuOptions = {
    enableSearchFilter: true,
  };
  selectMenuOptions2 = {
    singleSelect: true,
  };
  searchResult = new SearchResult<any>();

  ordersexcel: OrderExcel[] = [];
  HoverExcel = false;
  pagePermission = '';
  tmsCustomerPermission = '';
  intervalAutoRefresh: any;

  recievedServiceId: number;

  //#region "Drop Down lists" *********************************************************************************
  SearchStream: SearchStream = new SearchStream();

  StaffRoleList: Lookup<string>[] = [];
  StaffLandmarkList: Lookup<string>[] = [];
  StaffBranchList: Lookup<string>[] = [];


  Customer_Loading = false;
  Area_loading = false;


  staffModel: StaffModel;
  staffLandmarkList: Lookup<number>[] = [];

  ngOnInit() {
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'Id',
      textField: 'Name',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 1,
      allowSearchFilter: true,
    };

    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe((res) => {
      this.loadDDLsGV();
    });

  }

  ngOnDestroy(): void {
    clearInterval(this.intervalAutoRefresh);
    this.SearchStream.DestroyStreams();
  }

  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant('StaffList'));

    if (!this.recievedFilters || this.recievedFilters.length !== 3) {
      this.searchCaller();
    }

    this.getRoles('');
    this.getBranch('');
    this.getLandmark('');
  }
  uploadExcel() {
    this.modalRef = this.modalService.show(UploadStaffExcelComponent);
    this.modalRef.content.confirm.subscribe(() => {
      this.searchCaller();
    });
  }
  setDefaultSearchValues() {
    this.SearchCriteria = new StaffSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    // this.bindingModel_Roles = [];


  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }


  getRoles(searchKeyword: string) {

    this.SearchStream.initStream('getRoles', (a) => {
      this.Customer_Loading = true;
      this.lookupservice.getStaffRoleCategory().subscribe(res => {
        if (res.Value) {


          this.StaffRoleList = res.Value;
          this.searchCaller();
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


  getBranch(searchKeyword: string) {

    this.SearchStream.initStream('Staff_Branch', (a) => {
      this.Area_loading = true;
      this.lookupservice.getStaffBranch(a).subscribe(
        (res) => {
          if (res.Value) {this.StaffBranchList = res.Value; }
        },
        (err) => {
          this.Area_loading = false;
        },
        () => {
          this.Area_loading = false;
        },
      );
    }).next(searchKeyword);
  }


  getLandmark(searchKeyword: string) {
    this.SearchStream.initStream('Staff_Landmark', (a) => {
      this.Area_loading = true;
      this.lookupservice.getStaffLandmark(a).subscribe(
        (res) => {
          if (res.Value) {this.StaffLandmarkList = res.Value; }
        },
        (err) => {
          this.Area_loading = false;
        },
        () => {
          this.Area_loading = false;
        },
      );
    }).next(searchKeyword);
  }



  onRoleDDLChanged(evt) {
    console.log(evt);

    this.SearchCriteria.Id = evt.map((m) => m.Id);
    this.SearchCriteria.RoleId = evt.map((m) => m.Id);

  }



  onWorkOrderDDLChanged(evt) {
    this.SearchCriteria.FilterModel.SearchKeyword = isNullOrUndefined(evt.Name)
      ? evt
      : evt.Name;
  }

  onBranchDDLChanged(evt) {
    const allocatedBranch = evt.map(m => m.Id);
    this.SearchCriteria.branchId = evt.map(m => m.Id);
    this.getStaffLandmark(allocatedBranch);
  }


  getStaffLandmark(searchKeyword: []) {
    this.lookupservice.GetAllStationBasedOnCity(searchKeyword).subscribe(res => {
      if (res.Value) {
        this.staffLandmarkList = res.Value;
      }
    }
      , err => {
        this.Customer_Loading = false;
      }
      , () => {
        this.Customer_Loading = false;
      });
  }
  onLandmarkDDLChanged(evt) {
    console.log(evt);
    this.SearchCriteria.stationId = evt.map(m => m.Id);
  }
  onSearchSubmit() {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = evt;
    this.searchCaller();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageSize = evt;
    this.searchCaller();
  }

  editStaffItem(id) {
    this.router.navigate([`update/${id}`], {relativeTo: this.activatedRoute});
  }
  deleteStaffItem(id) {
    this.staffService.deleteStaffMember(id).subscribe(res => {
      console.log(res);
      this.searchCaller();
    });
    console.log(id);
  }

  searchCaller() {
   // this.tableLoading = true;
    this.mainLoading.PreloaderIcreaseCount();
    this.lookupservice.searchStaff(this.SearchCriteria).subscribe((res) => {
        this.searchResult.TotalCount = res.Value.TotalCount;
        this.searchResult.Result = res.Value.Result;
        this.tableLoading = false;

        console.log(typeof this.searchResult.Result);
        console.log(this.searchResult.Result);
      },
      () => {
        this.mainLoading.PreloaderDecreaseCount();
      },
      () => {
        this.mainLoading.PreloaderDecreaseCount();
      }
    );
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }


  //
  getZones($event)
  {}

}
