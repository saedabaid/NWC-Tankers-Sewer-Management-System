import { Component, OnInit, OnDestroy } from '@angular/core';
import { MeterReadingSearchCriteria } from '../../../Models/search-criteria/meter-reading-SC';
import { MeterReading } from '../../../Models/meter-reading';
import { SearchResult } from '../../../Models/common/search-result';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from '../../../Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { Title } from '@angular/platform-browser';
import { DeviceMeterService } from '../../../Services/device-meter.service';
import { FilterModel } from '../../../Models/common/filter-model';
import { PageFilter } from '../../../Models/common/page-fillter-model';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Lookup } from '../../../Models/common/lookup';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'app-meter-reading-list',
  templateUrl: './meter-reading-list.component.html',
  styleUrls: ['./meter-reading-list.component.scss']
})
export class MeterReadingListComponent implements OnInit, OnDestroy {
  advancedDiv = <boolean>false;
  SearchCriteria: MeterReadingSearchCriteria;
  searchResult = new SearchResult<MeterReading>();

  pagePermission: string = '';
  tableLoading = false;
  DeviceLoading = false;

  DeviceMeterList: Lookup<string>[] = [];
  deviceMeterSearchKeyWord = '';
  bindingModel_DeviceMeters: Lookup<string>[] = [];

  selectMenuOptions = {
    enableSearchFilter: true,    
  };

  SearchStream: SearchStream = new SearchStream();

  constructor(
    private authenticationService: AuthenticationService,
    private deviceMeterService: DeviceMeterService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private _alert: alertService,
    private titleService: Title,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('deviceMeterReading');
    this.authenticationService.checkViewPrivilege(this.pagePermission, true);

  }

  ngOnInit() {
    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });
  }

  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant('MeterReading'));
    this.searchCaller();
    // load Search DDls values
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new MeterReadingSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    let startDate = new Date();
    startDate.setDate(startDate.getDate());
    startDate.setHours(0);
    startDate.setMinutes(0);
    startDate.setSeconds(0);
    this.SearchCriteria.DateTimeFrom = startDate;

    let endDate = new Date();
    endDate.setDate(endDate.getDate() + 1);
    endDate.setHours(0);
    endDate.setMinutes(0);
    endDate.setSeconds(0);
    this.SearchCriteria.DateTimeTo = endDate;

    this.SearchCriteria.DeviceMeterIDs = [];
    this.bindingModel_DeviceMeters = [];
    this.getMetersBasedOnStations(this.deviceMeterSearchKeyWord);

  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }


  searchContractorsNames(name) {
    return this.lookupservice.SearchPermittedStations(name);
  }

  onContractorNameDDLChanged(evt) {
    this.SearchCriteria.FilterModel.SearchKeyword = isNullOrUndefined(evt.Name) ? evt : evt.Name;

    if (!isNullOrUndefined(evt.Id)) {
      this.SearchCriteria.DeviceMeterIDs = [];
      this.bindingModel_DeviceMeters = [];
      this.getMetersBasedOnStations(this.deviceMeterSearchKeyWord, evt.Id);
    }
  }

  getMetersBasedOnStations(searchKeyword: string, stationId: string = null) {
    this.deviceMeterSearchKeyWord = searchKeyword;

    this.SearchStream.initStream("DeviceMeter_ReadingList", (a) => {
      this.DeviceLoading = true;
      this.lookupservice.SearchMeterSerial(a, [stationId]).subscribe(res => {
        if (res.Value)
          this.DeviceMeterList = res.Value;
      }
      , err => {
        this.DeviceLoading = false;
      }
      , () => {
        this.DeviceLoading = false;
      });

    }).next(searchKeyword);
  };

  onDeviceMetersDDLChanged($event) {
    this.SearchCriteria.DeviceMeterIDs = $event.map(m => m.Id);
  }


  //#region "table Pagination and Search"
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

  searchCaller() {

    //alert time zone offset before send
    //let modifiedCriteria: MeterReadingSearchCriteria = JSON.parse(JSON.stringify(this.SearchCriteria));
    let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    modifiedCriteria.DateTimeFrom = new Date(this.SearchCriteria.DateTimeFrom.getTime());
    modifiedCriteria.DateTimeFrom.setMinutes(modifiedCriteria.DateTimeFrom.getMinutes() - modifiedCriteria.DateTimeFrom.getTimezoneOffset());
    modifiedCriteria.DateTimeTo = new Date(this.SearchCriteria.DateTimeTo.getTime());
    modifiedCriteria.DateTimeTo.setMinutes(modifiedCriteria.DateTimeTo.getMinutes() - modifiedCriteria.DateTimeTo.getTimezoneOffset());

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.deviceMeterService.SearchReadingList(modifiedCriteria).subscribe(res => {
      if (res.Value != null) {
        this.searchResult = res.Value;
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
      //this.tableLoading = false;
      this.mainloading.PreloaderDecreaseCount();
    });
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"


  deleteReading(reading: MeterReading) {
    if (!this.authenticationService.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }

    this._alert.confirmationMessage("ConfirmDelete").subscribe(confirm => {
      if (confirm == true) {

        this.mainloading.PreloaderIcreaseCount();
        this.deviceMeterService.DeleteReading(reading.ID).subscribe(res => {
          if (!res.IsErrorState) {
            this._alert.success("DeletedSuccessed");
            this.onSearchSubmit();
          }
          else {
            this._alert.errorList(res.Errors);
          }
        }
        ,err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        ,() => {
          this.mainloading.PreloaderDecreaseCount();
        });
      }

    });

  }


  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

}
