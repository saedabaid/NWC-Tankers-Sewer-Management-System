import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { FilterModel } from '@tms-models/common/filter-model';
import { PageFilter } from '@tms-models/common/page-fillter-model';
import { SearchResult } from '@tms-models/common/search-result';
import { StaffSearchCriteria } from '@tms-models/search-criteria/staff-search-criteria';
import { ControlPanelService } from '@tms-services/control-panel.service';
import { LookupService } from '@tms-services/lookup.service';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { Lookup } from '../../../Models/common/lookup';
import { VehicleSearchCriteriaDTO } from 'src/app/TMS-Module/Models/search-criteria/vehicle-search-criteria';
import { VehicleListService } from 'src/app/TMS-Module/Services/vehicle-list.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { VehicleListDTO } from '../../../Models/vehicle-list-dto';
import { VehicleDataDTO } from '../../../Models/VehicleDataDTO';
import { VehicleType } from '../../../Models/vehicle-type';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";

@Component({
  selector: 'app-vehicle-types',
  templateUrl: './vehicle-types.component.html',
  styleUrls: ['./vehicle-types.component.scss']
})
export class VehicleTypesComponent implements OnInit {
  pagePermission = '';
  tableLoading = false;
  Customer_Loading = false;
  dropdownSettings: IDropdownSettings = {};
  advancedDiv = <boolean>false;
  SearchCriteria = new VehicleSearchCriteriaDTO();
  items: Lookup<number>[] = [];
  Name: string = '';
  VehicleList: VehicleType[];
  TotalCount: number;

  constructor(
    private authServer: AuthenticationService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private router: Router,
    private titleService: Title,
    private cpService: ControlPanelService, private mainloading: LoaderService,
    private VehicleListService: VehicleListService,
    private _alert: alertService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('orderlist');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }


  ngOnInit() {
    this.titleService.setTitle(this._translate.instant('VehicleTypes'));
    this._translate.onLangChange.subscribe((res) => {
      this.titleService.setTitle(this._translate.instant('VehicleTypes'));
    });
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.searchVehicleType();
    this.getVehicleTypes();
  }

  getVehicleTypes() {
    this.lookupservice.getVehicleTypes().subscribe(res => this.items = res.Value.Value);
  }

  deleteItem(id) {
    this._alert.confirmationMessage("DeleteMsgUser").subscribe(confirm => {
      if (confirm == true) {
        this.cpService.deleteVehicleType(id).subscribe((res) => {
          if (!res.IsErrorState) {
            this._alert.success("DeletedSuccessed");
            this.searchVehicleType();
          } else {
            this._alert.errorList(res.Errors);
          }
        });
      }
    })
  }
  Clear() {
    this.SearchCriteria = new VehicleSearchCriteriaDTO();
    this.SearchCriteria.IsDeleted = false;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.searchVehicleType();
  }

  onUserDDLChanged(evt) {
    this.SearchCriteria.Name = isNullOrUndefined(evt.Name) ? evt : evt.Name;
  }
  onPageIndexChanged(evt) {
    this.SearchCriteria.PageFilter.PageIndex = evt;
    this.searchVehicleType();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.PageFilter.PageSize = evt;
    this.searchVehicleType();
  }
  searchVehicleType(Name: string = "") {
    //this.tableLoading = true;
    this.SearchCriteria.Name = Name;
    this.mainloading.PreloaderIcreaseCount();
    this.VehicleListService.searchVehicleType(this.SearchCriteria).subscribe(res => {
      debugger;
      if (res.Value != null) {
      
        this.VehicleList = res.Value.Result;
        this.TotalCount = res.Value.TotalCount;
      }
      else {
        this.VehicleList = [];
        this.TotalCount = 0
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
        //this.tableLoading = false;
      })
  }

}
