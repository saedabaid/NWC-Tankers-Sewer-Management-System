import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Lookup } from '@tms-models/common/lookup';
import { SearchResult } from '@tms-models/common/search-result';
import { SearchStream } from '@tms-models/common/search-stream-object.model';
import { PermitDTO } from '@tms-models/PermitDTO';
import { PermitListSC } from '@tms-models/search-criteria/Permit-list-SC';
import { LookupService } from '@tms-services/lookup.service';
import { PermitService } from '@tms-services/permit.service';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { LoaderService } from '@tms-shared/loader.service';
import { alertService } from '@tms-shared/Services/alert/alert.service';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { isNullOrUndefined } from '@tms-shared/utilities/utilities';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
  SearchCriteria: PermitListSC;
  SearchStream: SearchStream = new SearchStream();
  selectMenuOptions: any;
  advancedDiv: string;

  Area_loading = false;
  City_Loading = false;
  Station_Loading = false;
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerStationList: Lookup<string>[] = [];
  searchResult = new SearchResult<PermitDTO>();
  citySearchKeyWord = "";
  stationSearchKeyword = "";
  pagePermission: string = "";
    
    constructor(private _alertservice: alertService, private router: Router,private authServer: AuthenticationService,private permitservice: PermitService,private mainloading: LoaderService,private lookupservice: LookupService) { 
      this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName("PermitsNew");
    }

  ngOnInit() {
    this.setDefaultSearchValues();

    this.load();
  }
  load() {
    this.getAreas();
  }
  setDefaultSearchValues() {
    this.SearchCriteria = new PermitListSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize =
      Configuration.GridSetting.Pagesize;

    this.SearchCriteria.ExpirationdateFrom = new Date();
    this.SearchCriteria.ExpirationdateTo = new Date();

    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Stations = [];

    this.customerCityList = [];
    this.customerStationList = [];
    this.SearchCriteria.DriverID="";
    this.SearchCriteria.DriverMobile="";
    this.SearchCriteria.PermitStatus="-1";
    this.SearchCriteria.TankerCode="";
   
  }
  getAreas(searchKeyword: string = "") {
    this.SearchStream.initStream("AreaDDL_OrderReassignmentReport", (a) => {
      this.Area_loading = true;
      this.lookupservice.getAreasName(a).subscribe(
        (res) => {
          if (res.Value) this.customerAreaList = res.Value;
        },
        (err) => {
          this.Area_loading = false;
        },
        () => {
          this.Area_loading = false;
        }
      );
    }).next(searchKeyword);
  }

  getCity(searchKeyword: string) {
    this.citySearchKeyWord = searchKeyword;
    this.SearchStream.initStream("CityDDL_OrderReassignmentReport", (a) => {
      if (
        !isNullOrUndefined(this.SearchCriteria.AreaIDs) &&
        this.SearchCriteria.AreaIDs.length > 0
      ) {
        this.City_Loading = true;
        this.lookupservice
          .getCityName(a, this.SearchCriteria.AreaIDs)
          .subscribe(
            (res) => {
              if (res.Value) this.customerCityList = res.Value;
            },
            (err) => {
              this.City_Loading = false;
            },
            () => {
              this.City_Loading = false;
            }
          );
      } else {
        this.customerCityList = [];
      }
    }).next(searchKeyword);
  }

  getStations(searchKeyword: string) {
    this.stationSearchKeyword = searchKeyword;
    this.SearchStream.initStream("StationDDL_OrderReassignmentReport", (a) => {
      if (
        !isNullOrUndefined(this.SearchCriteria.CityIDs) &&
        this.SearchCriteria.CityIDs.length > 0
      ) {
        this.Station_Loading = true;
        this.lookupservice
          .GetStationBasedOnCity(a, this.SearchCriteria.CityIDs)
          .subscribe(
            (res) => {
              if (res.Value) this.customerStationList = res.Value;
            },
            (err) => {
              this.Station_Loading = false;
            },
            () => {
              this.Station_Loading = false;
            }
          );
      } else {
        this.customerStationList = [];
      }
    }).next(searchKeyword);
  }

  onAreaDDLChanged(evt) {
    this.SearchCriteria.AreaIDs = evt.map((m) => m.Id);
    this.getCity(this.citySearchKeyWord);
    this.bindingModel_Cities = [];
    this.SearchCriteria.CityIDs = [];

    this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];
  }

  onCityDDLChanged(evt) {
    this.SearchCriteria.CityIDs = evt.map((m) => m.Id);
    this.getStations(this.stationSearchKeyword);

    this.customerStationList = [];
    this.bindingModel_Stations = [];
    this.SearchCriteria.StationIDs = [];
  }

  onStationDDLChanged(evt) {
    this.SearchCriteria.StationIDs = evt.map((m) => m.Id);
  }

  onSearchSubmit() {
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.PageFilter.PageIndex = evt;
    this.searchCaller();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.PageFilter.PageSize = evt;
    this.searchCaller();
  }

  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (
      !isNullOrUndefined(this.SearchCriteria.ExpirationdateFrom) &&
      !isNullOrUndefined(this.SearchCriteria.ExpirationdateTo) &&
      this.SearchCriteria.ExpirationdateTo < this.SearchCriteria.ExpirationdateFrom
    ) {
      validationMessages.push("DateTimeFromMustBeBeforDateTimeTo");
    }

    if (validationMessages.length > 0) {
      this._alertservice.errorList(validationMessages);
      return false;
    }
    return true;
  }

  searchCaller() {
    if (!this.isValidModel()) return;

        //alert time zone offset before send
    let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    if (modifiedCriteria.ExpirationdateFrom) {
      modifiedCriteria.ExpirationdateFrom = new Date(
        this.SearchCriteria.ExpirationdateFrom.getTime()
      );
      modifiedCriteria.ExpirationdateFrom.setMinutes(
        modifiedCriteria.ExpirationdateFrom.getMinutes() -
          modifiedCriteria.ExpirationdateFrom.getTimezoneOffset()
      );
    }
    if (modifiedCriteria.ExpirationdateTo) {
      modifiedCriteria.ExpirationdateTo = new Date(
        this.SearchCriteria.ExpirationdateTo.getTime()
      );
      modifiedCriteria.ExpirationdateTo.setMinutes(
        modifiedCriteria.ExpirationdateTo.getMinutes() -
          modifiedCriteria.ExpirationdateTo.getTimezoneOffset()
      );
    }

    this.mainloading.PreloaderIcreaseCount();
    this.permitservice.GetList(modifiedCriteria).subscribe(
      (res) => {
        //debugger;
        if ((res || ({} as any)).Value) {
          this.searchResult = res.Value;
        } else {
          this.searchResult.Result = [];
          this.searchResult.TotalCount = 0;
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

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }
  editPermit(permitDto: PermitDTO) {
    if (this.authServer.checkAddEditPrivilege(this.pagePermission)) {
      
        this.router.navigate(['/tms/permits/edit/' + permitDto.ID]);
      
    }
  }
}
