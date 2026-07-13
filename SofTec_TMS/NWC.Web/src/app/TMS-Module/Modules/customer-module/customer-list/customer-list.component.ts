import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { LoaderService } from 'src/app/shared/loader.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { isNullOrUndefined } from 'src/app/shared/utilities/utilities';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { Customer } from 'src/app/TMS-Module/Models/customer.model';
import { CustomerSC } from 'src/app/TMS-Module/Models/search-criteria/customer-sc.model';
import { CustomerService } from 'src/app/TMS-Module/Services/customer.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {


  advancedDiv = false;
  pagePermission: string = '';
  tableLoading = false;

  SearchCriteria: CustomerSC;
  searchResult = new SearchResult<Customer>();
  IsArabic = false;

  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private _alertservice: alertService,
    private mainloading: LoaderService,
    private _ExcelService: ExcelService,
    private customerService: CustomerService,
    private router: Router,

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsCustomers');
    this.authenticationService.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.setDefaultSearchValues();

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.getAreas();
    this.getServiceTypes();

    this.searchCaller();

    this.IsArabic = (this.translateService.currentLang == 'ar');
    this.titleService.setTitle(this.translateService.instant('TMS_CustomerList'));
  }

  showAdvanced() {
    this.advancedDiv = !this.advancedDiv;
  }

  //#region  "For search"
  SearchStream: SearchStream = new SearchStream();

  customerAreaList: Lookup<string>[] = [];
  customerCityList: Lookup<string>[] = [];
  customerZoneList: Lookup<number>[] = [];
  //customerStationList: Lookup<string>[] = [];
  customerServiceList: Lookup<number>[] = [];

  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];
  //bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_ServiceTypes: Lookup<number>[] = [];

  citySearchKeyWord = '';
  zoneSearchKeyWord = '';
  //stationSearchKeyword = '';

  timeFromStr: string;
  timeToStr: string;

  Area_loading = false;
  City_Loading = false;
  Zone_Loading = false;
  //Station_Loading = false;

  selectMenuOptions = {
    enableSearchFilter: true,
  };

  selectMenuOptions2 = {
    singleSelect: true
  };

  setDefaultSearchValues() {
    this.SearchCriteria = new CustomerSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    // redraw DDL selections
    this.bindingModel_Areas = [];
    this.bindingModel_Cities = [];
    this.bindingModel_Zones = [];
    //this.bindingModel_Stations = [];
    this.bindingModel_ServiceTypes = [];

    this.customerCityList = [];
    this.customerZoneList = [];
    //this.customerStationList = [];

  }

  getAreas(searchKeyword: string = '') {
    this.SearchStream.initStream("AreaDDL_TankerMovementReport", (a) => {
      this.Area_loading = true;
      this.lookupservice.getAreasName(a).subscribe(res => {
        if (res.Value)
          this.customerAreaList = res.Value;
      }
        , err => {
          this.Area_loading = false;
        }
        , () => {
          this.Area_loading = false;
        });
    }).next(searchKeyword);
  };

  getCity(searchKeyword: string) {
    this.citySearchKeyWord = searchKeyword;
    this.SearchStream.initStream("CityDDL_TankerMovementReport", (a) => {
      if (!isNullOrUndefined(this.SearchCriteria.AreaIDs) && this.SearchCriteria.AreaIDs.length > 0) {
        this.City_Loading = true;
        this.lookupservice.getCityName(a, this.SearchCriteria.AreaIDs).subscribe(res => {
          if (res.Value)
            this.customerCityList = res.Value;
        }
          , err => {
            this.City_Loading = false;
          }
          , () => {
            this.City_Loading = false;
          });
      }
      else {
        this.customerCityList = [];
      }
    }).next(searchKeyword);
  };

  //depends on cityIds
  getZones(searchKeyword: string) {
    this.zoneSearchKeyWord = searchKeyword;
    this.SearchStream.initStream("ZoneDDL_createOrder", a => {
      if (
        !isNullOrUndefined(this.SearchCriteria.CityIDs) &&
        this.SearchCriteria.CityIDs.length > 0
      ) {
        this.Zone_Loading = true;
        this.lookupservice
          .getZoneName(a, this.SearchCriteria.CityIDs)
          .subscribe(
            res => {
              if (res.Value) this.customerZoneList = res.Value;
            },
            err => {
              this.Zone_Loading = false;
            },
            () => {
              this.Zone_Loading = false;
            }
          );
      } else {
        this.customerZoneList = [];
      }
    }).next(searchKeyword);
  }

  // getStations(searchKeyword: string) {
  //   this.stationSearchKeyword = searchKeyword;
  //   this.SearchStream.initStream("StationDDL_TankerMovementReport", (a) => {
  //     if (!isNullOrUndefined(this.SearchCriteria.CityIDs) && this.SearchCriteria.CityIDs.length > 0) {
  //       this.Station_Loading = true;
  //       this.lookupservice.GetStationBasedOnCity(a, this.SearchCriteria.CityIDs).subscribe(res => {
  //         if (res.Value)
  //           this.customerStationList = res.Value;
  //       }
  //         , err => {
  //           this.Station_Loading = false;
  //         }
  //         , () => {
  //           this.Station_Loading = false;
  //         });
  //     }
  //     else {
  //       this.customerStationList = [];
  //     }
  //   }).next(searchKeyword);
  // }

  getServiceTypes() {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value) this.customerServiceList = res.Value;
    });
  }

  onAreaDDLChanged(evt) {
    this.SearchCriteria.AreaIDs = evt.map(m => m.Id);
    this.getCity(this.citySearchKeyWord);
    this.bindingModel_Cities = [];
    this.SearchCriteria.CityIDs = [];

    this.customerZoneList = [];
    this.bindingModel_Zones = [];
    this.SearchCriteria.ZoneId = null;
    //this.customerStationList = [];
    //this.bindingModel_Stations = [];
    //this.SearchCriteria.StationIDs = [];
  }

  onCityDDLChanged(evt) {
    this.SearchCriteria.CityIDs = evt.map(m => m.Id);
    this.getZones(this.zoneSearchKeyWord);
    //this.getStations(this.stationSearchKeyword);

    this.customerZoneList = [];
    this.bindingModel_Zones = [];
    this.SearchCriteria.ZoneId = null;
    //this.customerStationList = [];
    //this.bindingModel_Stations = [];
    //this.SearchCriteria.StationIDs = [];
  }

  onZoneDDLChanged(evt) {
    this.SearchCriteria.ZoneId = evt.map(m => m.Id)[0];
  }
  // onStationDDLChanged(evt) {
  //   this.SearchCriteria.StationIDs = evt.map(m => m.Id);
  // }

  onServiceTypeDDLChanged(evt) {
    this.SearchCriteria.ServiceTypeId = evt.map(m => m.Id)[0];
  }

  //#endregion "For Search"


  //#region "table Pagination and Search"
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

    if (isNullOrUndefined(this.SearchCriteria.AreaIDs) || this.SearchCriteria.AreaIDs.length < 1) {
      // validationMessages.push("DashboardSelectArea");
    } else if (isNullOrUndefined(this.SearchCriteria.CityIDs) || this.SearchCriteria.CityIDs.length < 1) {
      validationMessages.push("ChooseCity");
    }
    else if (isNullOrUndefined(this.SearchCriteria.ZoneId) || this.SearchCriteria.ZoneId < 1) {
      validationMessages.push("ZoneIdRequired");
    }
    // else if (isNullOrUndefined(this.SearchCriteria.StationIDs) || this.SearchCriteria.StationIDs.length < 1) {
    //   validationMessages.push("StationIsRequired");
    // }

    if (validationMessages.length > 0) {
      this._alertservice.errorList(validationMessages);
      return false;
    }
    return true;
  }


  searchCaller() {
    if (!this.isValidModel()) return;

    //this.tableLoading = true;
    this.mainloading.PreloaderIcreaseCount();
    this.customerService.SearchCustomerList(this.SearchCriteria).subscribe(res => {
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
        this.mainloading.PreloaderDecreaseCount();
        //this.tableLoading = false;
      })
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"


  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }


  Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";
  HoverExcel: boolean = false;

  onexportOrders() {

    let clonedObj = Object.assign({}, this.SearchCriteria);
    clonedObj.ExcelFlage = true;

    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.customerService.SearchCustomerList(clonedObj)
      .subscribe(res => {
        if (!res || res.IsErrorState == false) {
          if (
            isNullOrUndefined(res.Value.Result) ||
            res.Value.Result.length == 0
          ) {
            this._alertservice.error("NoDataFound");
            return;
          }

          let excelJson = res.Value.Result.map(value => {
            let r = {
              FullName: value.FullName,
              IDNumber: value.IDNumber,
              Mobile: value.Mobile

            }
            return r;
          });

          this._ExcelService.exportAsExcelFile(excelJson, this.translateService.instant("TMS_CustomerList"));
        }
      }
        , err => {
          this.Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";
        }
        , () => {
          this.Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";
        });
  }

  mouseEnterExcel() {
    this.HoverExcel = true;
  }
  mouseLeaveExcel() {
    this.HoverExcel = false;
  }

  editCustomer(contract: Customer) {
    if (this.authenticationService.checkAddEditPrivilege(this.pagePermission)) {
      this.router.navigate(['/tms/customer/edit/' + contract.ID]);
    }
  }

  deleteCustomer(customerId: Customer) {
    if (!this.authenticationService.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }

    this._alertservice.confirmationMessage("ConfirmDelete").subscribe(confirm => {
      if (confirm == true) {
        this.mainloading.PreloaderIcreaseCount();
        this.customerService.DeleteCustomer(customerId.ID).subscribe(res => {
          if (!res.IsErrorState) {
            this._alertservice.success("DeletedSuccessed");
            this.onSearchSubmit();
          }
          else {
            this._alertservice.errorList(res.Errors);
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




}
