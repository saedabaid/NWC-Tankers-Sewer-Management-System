import { Component, OnInit } from '@angular/core';
import { UserStationPermissionDTO } from 'src/app/TMS-Module/Models/user-landmark-permissionDTO';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { Title } from '@angular/platform-browser';
import { ControlPanelService } from 'src/app/TMS-Module/Services/control-panel.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { stream } from 'xlsx/types';
import { UserStationPermissionSC } from 'src/app/TMS-Module/Models/search-criteria/user-station-permissionSC';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { LoaderService } from 'src/app/shared/loader.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-user-station-permission',
  templateUrl: './user-station-permission.component.html',
  styleUrls: ['./user-station-permission.component.scss']
})

export class UserStationPermissionComponent implements OnInit {
  advancedDiv = false;
  pagePermission: string = '';
  tableLoading = false;
  recievedServiceId: number;
  TotalCount: number;
  searchCriteria: UserStationPermissionSC = new UserStationPermissionSC();
  searchResult = new SearchResult<UserStationPermissionDTO>();

  IsArabic = false;

  stationList: Lookup<string>[] = [];
  serviceList: Lookup<number>[] = [];

  selectMenuOptions = {
    enableSearchFilter: true,    
  };
  selectMenuOptions2 = {
    enableSearchFilter: false,
  };

  selectMenuOptionsSelectedLandmarks = {
    enableSearchFilter: false,
    text: 'AssignedStations'
  };

  constructor(private _translate: TranslateService,
    private authServer: AuthenticationService,
    private lookupService: LookupService,
    private controlPanelService: ControlPanelService,
    private alertService: alertService,
    private titleService: Title,
    private mainloading: LoaderService
  ) {

    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('nwc_userLandmarkPermissions');
    this.authServer.checkFullControlPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.searchCriteria.PageFilter.PageIndex = 1;
    this.searchCriteria.PageFilter.PageSize = 5; //Configuration.GridSetting.Pagesize;

    this.load();
    this.IsArabic = this._translate.currentLang == "ar";
    this._translate.onLangChange.subscribe(res => {
      this.load();
      this.IsArabic = res.lang == "ar";

    });
  }

  load() {
    this.titleService.setTitle(this._translate.instant('UserStationPermission'));
    forkJoin(
      [
        this.lookupService.GetUserStations(''),
        this.lookupService.getServicesTypes()
      ]
    ).subscribe(([userStation, serviceTypes]) => {

      if (userStation.Value)
        this.stationList = userStation.Value;

      if (serviceTypes.Value)
        this.serviceList = serviceTypes.Value;

      this.searchUserPermittedLandmarks();
    }
      , err => {
        // this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        // this.mainloading.PreloaderDecreaseCount();
      });


  }

  getWorkOrderNumbers(name) {
    if (!name) return [];
    return this.lookupService.SearchStaff(name);
  }

  onWorkOrderDDLChanged(evt) {
    this.searchCriteria.UserName = !evt.Name ? evt : evt.Name;
  }





  getStations(name) {
    this.lookupService.GetUserStations(name).subscribe(res => {
      if (res.Value)
        this.stationList = res.Value;
    });
  }

  searchUserPermittedLandmarks() {
    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.controlPanelService.getUserPermittedLandmarks(this.searchCriteria).subscribe(res => {
      if (res.Value != null && res.Value.Result != null) {
        //console.log("getUserPermittedLandmarks",res.Value);
        this.searchResult = res.Value;

        this.searchResult.Result.forEach(element => {
          element.PermittedLandmarks = [];

          if (element.DBPermittedLandmarks && element.DBPermittedLandmarks.length) {
            element.PermittedLandmarks = element.DBPermittedLandmarks.map(m => {
              let arr = m.split(':');
              let n = new Lookup<string>()
              n.Id = arr[0];
              n.Name = arr[1];
              return n;
            });
          }

        });

        this.searchResult.Result.forEach(m => {
          m.PermittedServices = this.serviceList.filter(a => m.PermittedServiceIDs.includes(a.Id));
        });
      }
      else {
        this.searchResult.Result = [];
        this.searchResult.TotalCount = 0
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
        //this.tableLoading = false;
      });
  }

  // onStationDDLChange(evt, item: UserStationPermissionDTO) {
  //   // var marks = evt as Lookup<string>[];
  //   // item.PermittedLandmarks = item.PermittedLandmarks.concat(
  //   //   marks.filter(s => item.PermittedLandmarks.findIndex(a => a.Id == s.Id) == -1)
  //   // );
  // }

  // onSelected_StationDDLChange(evt, item: UserStationPermissionDTO) {
  //   //item.bindingModel_SelectedLandmarks = evt.map(m => m.Id);
  //   // var marks = evt as Lookup<string>[];
  //   // // remove object from landmarks list
  //   // marks.forEach(element => {
  //   //   let index = item.PermittedLandmarks.indexOf(element);
  //   //   item.PermittedLandmarks.splice(index, 1);
  //   // });
  //   // item.bindingModel_SelectedLandmarks = [];
  // }

  removeStations(item: UserStationPermissionDTO) {
    // remove object from landmarks list
    item.bindingModel_SelectedLandmarks.forEach(element => {
      let index = item.PermittedLandmarks.findIndex(s => s.Id.toLowerCase() == element.Id.toLowerCase());
      item.PermittedLandmarks.splice(index, 1);
    });
    item.bindingModel_SelectedLandmarks = [];
  }

  addStations(item: UserStationPermissionDTO) {
    var marks = item.bindingModel_Landmarks;
    item.PermittedLandmarks = item.PermittedLandmarks.concat(
      marks.filter(s => item.PermittedLandmarks.findIndex(a => a.Id.toLowerCase() == s.Id.toLowerCase()) == -1)
    );
  }

  onServiceDDLChange(evt, item: UserStationPermissionDTO) {
    item.PermittedServiceIDs = evt.map(m => m.Id);
  }

  onSearchSubmit() {
    this.searchCriteria.PageFilter.PageIndex = 1;
    this.searchUserPermittedLandmarks();
  }

  onPageIndexChanged(evt) {
    this.searchCriteria.PageFilter.PageIndex = evt;
    this.searchUserPermittedLandmarks();
  }

  onPageSizeChanged(evt) {
    this.searchCriteria.PageFilter.PageSize = evt;
    //this.searchUserPermittedLandmarks();
  }

  onSave() {
    let send = this.searchResult.Result.filter(a => a.CheckToSave)
      .map(m => {
        let r = new UserStationPermissionDTO();
        r.StaffID = m.StaffID;
        r.PermittedServiceIDs = m.PermittedServiceIDs;
        r.PermittedLandmarkIDs = m.PermittedLandmarks.map(ff => ff.Id);
        return r;
      });

    this.mainloading.PreloaderIcreaseCount();

    this.controlPanelService.saveUserPermittedLandmarks(send).subscribe(res => {

      if (res.IsErrorState === false)
        this.alertService.success("SavedSuccessed");

    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });

    //console.log(send);
  }

  clearSearch() {
    //this.setDefaultSearchValues();
    this.searchCriteria = new UserStationPermissionSC();
    this.searchCriteria.PageFilter.PageIndex = 1;
    this.searchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;


    this.searchUserPermittedLandmarks();
  }

}
