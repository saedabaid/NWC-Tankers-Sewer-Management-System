import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { zoneListService } from '../../../Services/zone-list.service';
import { LookupService } from '../../../Services/lookup.service';
import { Lookup } from '../../../Models/common/lookup';
//import { ZoneSearchCriteriaDTO } from '../../../Models/zone-search-criteria';
import { Configuration } from 'src/app/shared/configurations/shared.config';
//import { ZoneListDTO } from '../../../Models/contract-station-listDTO';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { ZoneSearchCriteriaDTO } from '../../../Models/search-criteria/zone-search-criteria';
import { ZoneListDTO } from '../../../Models/zone-listDTO';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { Subject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';
//import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { UploadZoneExcelComponent } from '../upload-zone-excel/upload-zone-excel.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-zone',
  templateUrl: './zoneList.component.html',
  styleUrls: ['./zoneList.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ZoneListComponent implements OnInit, OnDestroy {
  advancedDiv = <boolean>false;
  // ZonesByNameOrCode: Lookup<number>[] = [];
  // SelectedZonesByNameOrCode: Lookup<number>[] = [];
  ZoneSearchCriteriaDTO: ZoneSearchCriteriaDTO = new ZoneSearchCriteriaDTO();
  zonesList: ZoneListDTO[];
  TotalCount: number;
  selectMenuOptions = {
    enableSearchFilter: true,    
  };
  AreaList: Lookup<string>[] = [];
  CityList: Lookup<string>[] = [];
  StationList: Lookup<string>[] = [];
  SelectedAreas: Lookup<string>[] = [];
  SelectedCites: Lookup<string>[] = [];
  SelectedStation: Lookup<string>[] = [];
  pagePermission: string = '';

  Area_Loading = false;
  City_Loading = false;
  Station_Loading = false;
  SearchStream: SearchStream = new SearchStream();
  tableLoading = false;

  constructor(private _translate: TranslateService,
    private authServer: AuthenticationService,
    private LookupService: LookupService,
    private zoneListService: zoneListService,
    private _alert: alertService,
    private titleService: Title,
    private mainloading: LoaderService,
    private modalRef: BsModalRef,
    private modalService: BsModalService
    ) {

    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('zonelist');
    this.authServer.checkViewPrivilege(this.pagePermission, true);

  }

  ngOnInit() {
    this.ZoneSearchCriteriaDTO.IsDeleted = false;
    this.ZoneSearchCriteriaDTO.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.ZoneSearchCriteriaDTO.PageFilter.PageIndex = 1;

    this.load();
    this._translate.onLangChange.subscribe(res => {
      this.load();
    });

  }
  load() {

    this.titleService.setTitle(this._translate.instant('Zones'));

    // this.LookupService.getZonesByNameOrCode().subscribe(res => {
    //   if (res.IsErrorState == false) {
    //     this.ZonesByNameOrCode = res.Value;
    //   }
    // })

    this.Search();
  }
  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
    this.getArea('');
  }

  getArea(searchKeyword: string) {
    this.SearchStream.initStream("AreaDDL_ZoneList", (a) => {
      this.Area_Loading = true;
      this.LookupService.getAreasName(a).subscribe(res => {
        if (res.Value) {
          this.AreaList = res.Value;
        }
      }
      ,err => {
        this.Area_Loading = false;
      }
      ,() => {
        this.Area_Loading = false;
      });
    }).next(searchKeyword);
  }

  onAreaChanged($event) {
    if ($event.length > 0) {
      this.ZoneSearchCriteriaDTO.AreaIDs = $event.map(a => a.Id);
      this.SelectedAreas = $event;
      this.getCityBasedOnArea('');
    }
    else {
      this.CityList = [];
      this.SelectedCites = [];
      this.StationList = [];
      this.SelectedStation = [];
    }

  }

  getCityBasedOnArea(searchKeyword: string) {
    this.SearchStream.initStream("CityDDL_ZoneList", (a) => {
      this.City_Loading = true;
      this.LookupService.getCityName(a, this.ZoneSearchCriteriaDTO.AreaIDs).subscribe(res => {
        if (res.Value)
          this.CityList = res.Value;
      }
      ,err => {
        this.City_Loading = false;
      }
      ,() => {
        this.City_Loading = false;
      });
    }).next(searchKeyword);
  };


  onCityChanged($event) {
    if ($event.length > 0) {
      this.ZoneSearchCriteriaDTO.CityIDs = $event.map(a => a.Id);
      this.SelectedCites = $event;
      this.GetStationBasedOnCity('');
    }
    else {

      this.StationList = [];
      this.SelectedStation = [];
    }

  }

  GetStationBasedOnCity(searchKeyword: string) {
    this.SearchStream.initStream("StationDDl_ZoneList", (a) => {
      this.Station_Loading = true;
      this.LookupService.GetStationBasedOnCity(a, this.ZoneSearchCriteriaDTO.CityIDs).subscribe(res => {
        if (res.Value)
          this.StationList = res.Value;
      }
      ,err => {
        this.Station_Loading = false;
      }
      ,() => {
        this.Station_Loading = false;
      });
    }).next(searchKeyword);
  }

  onStationChanged($event) {
    this.ZoneSearchCriteriaDTO.StationIDs = $event.map(a => a.Id);
    this.SelectedStation = $event;
  }
  searchZonesByNameOrCode($event) {

    return this.LookupService.SearchZonesByNameOrCode($event)

  }

  onZoneDDLChanged(evt) {
    this.ZoneSearchCriteriaDTO.NameOrCode = isNullOrUndefined(evt.Name) ? evt : evt.Name;
  }

  Search() {

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.zoneListService.Search(this.ZoneSearchCriteriaDTO).subscribe(res => {
      if (res.Value != null) {
        this.zonesList = res.Value.Result;
        this.TotalCount = res.Value.TotalCount;
      }
      else {
        this.zonesList = [];
        this.TotalCount = 0;
      }
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      //this.tableLoading= false;
      this.mainloading.PreloaderDecreaseCount();
    })
  }

  Clear() {
    this.ZoneSearchCriteriaDTO = new ZoneSearchCriteriaDTO();
    this.ZoneSearchCriteriaDTO.IsDeleted = false;
    this.ZoneSearchCriteriaDTO.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.ZoneSearchCriteriaDTO.PageFilter.PageIndex = 1;

    this.CityList = [];
    this.StationList = [];
    this.SelectedAreas = [];
    this.SelectedCites = [];
    this.SelectedStation = [];
    this.Search();
  }

  delete(i: number) {
    if (!this.authServer.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }

    this._alert.confirmationMessage("DeleteMsgZone").subscribe(confirm => {
      if (confirm == true) {
        //delete function
        this.zoneListService.Delete(i).subscribe(res => {

          if (!res.IsErrorState) {
            this._alert.success("DeletedSuccessed");
            //refresh
            this.Search();
          }
          else {
            this._alert.errorList(res.Errors);
          }


        });
      }

    })
  }
  onPageIndexChanged(evt) {
    this.ZoneSearchCriteriaDTO.PageFilter.PageIndex = evt;
    this.Search();
  }

  onPageSizeChanged(evt) {
    this.ZoneSearchCriteriaDTO.PageFilter.PageSize = evt;
    this.Search();
  }


  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  uploadExcel() {

    this.modalRef = this.modalService.show(UploadZoneExcelComponent);

    this.modalRef.content.confirm.subscribe(() => {
      this.Search();
    });

  }



}

