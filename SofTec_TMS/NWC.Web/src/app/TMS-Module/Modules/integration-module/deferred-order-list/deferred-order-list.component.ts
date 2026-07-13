import { Component, OnInit } from '@angular/core';
import { DeferredOrderSC } from 'src/app/TMS-Module/Models/search-criteria/deferred-order-SC.model';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { DeferredOrder } from 'src/app/TMS-Module/Models/deferred-order.model';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { WorkOrderSearchService } from 'src/app/TMS-Module/Services/work-order-search.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";

@Component({
  selector: 'app-deferred-list',
  templateUrl: './deferred-order-list.component.html',
  styleUrls: ['./deferred-order-list.component.scss']
})
export class DeferredOrderListComponent implements OnInit {

  advancedDiv = false;
  pagePermission: string = '';

  SearchCriteria: DeferredOrderSC;
  searchResult = new SearchResult<DeferredOrder>();


  constructor(
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private workOrderService: WorkOrderSearchService,
    private _alertservice: alertService,
    private mainloading: LoaderService,
    private _ExcelService: ExcelService

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsDeferredOrders');
    this.authenticationService.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.setDefaultSearchValues();


    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });


    this.GetStatuses();
    this.searchCaller();

  }

  load() {
    this.titleService.setTitle(this.translateService.instant('DeferredOrders'));
  }


  //#region  "For search"

  statusesList: Lookup<number>[] = [];

  bindingModel_Statuses: Lookup<number>[] = [];

  selectMenuOptions = {
    enableSearchFilter: true,
  };

  setDefaultSearchValues() {
    this.SearchCriteria = new DeferredOrderSC();
    this.SearchCriteria.PageFilter.PageIndex = 1;
    this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;

    this.SearchCriteria.DateTimeFrom = new Date();
    this.SearchCriteria.DateTimeTo = new Date();

    this.SearchCriteria.StatusIds = [1];
    if (this.statusesList && this.statusesList.length > 0) {
      this.bindingModel_Statuses = this.statusesList.filter(s => s.Id == 1);
    }

  }

  GetStatuses() {
    this.lookupservice.GetDeferredOrdersStatuses().subscribe(res => {
      if (res.Value) {
        this.statusesList = res.Value.filter(s => s.Id == 1 || s.Id == 3);
        this.bindingModel_Statuses = this.statusesList.filter(s => s.Id == 1);
      }
    });
  }

  onStatusDDLChanged(evt) {
    this.SearchCriteria.StatusIds = evt.map(m => m.Id);
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

    // if (isNullOrUndefined(this.SearchCriteria.DateTimeFrom)) {
    //   validationMessages.push("DateFromRequired");
    // }

    // if (isNullOrUndefined(this.SearchCriteria.DateTimeTo)) {
    //   validationMessages.push("DateToRequired");
    // }

    // if (
    //   !isNullOrUndefined(this.SearchCriteria.DateTimeFrom) &&
    //   !isNullOrUndefined(this.SearchCriteria.DateTimeTo) &&
    //   this.SearchCriteria.DateTimeTo < this.SearchCriteria.DateTimeFrom
    // ) {
    //   validationMessages.push("DateTimeFromMustBeBeforDateTimeTo");
    // }


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
    modifiedCriteria.DateTimeFrom = new Date(this.SearchCriteria.DateTimeFrom.getTime());
    modifiedCriteria.DateTimeFrom.setMinutes(
      modifiedCriteria.DateTimeFrom.getMinutes() - modifiedCriteria.DateTimeFrom.getTimezoneOffset());
    modifiedCriteria.DateTimeTo = new Date(this.SearchCriteria.DateTimeTo.getTime());
    modifiedCriteria.DateTimeTo.setMinutes(
      modifiedCriteria.DateTimeTo.getMinutes() - modifiedCriteria.DateTimeTo.getTimezoneOffset()
    );

    this.mainloading.PreloaderIcreaseCount();
    this.workOrderService.SearchDeferredWorkOrders(modifiedCriteria).subscribe(res => {
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
      })
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  //#endregion "table Pagination and Search"


  CancelOrder(i: number) {
    // if (!this.authServer.checkFullControlPrivilege(this.pagePermission)) {
    //   return;
    // }

    this._alertservice.confirmationMessage("ConfirmCancelDeferredOrder").subscribe(confirm => {
      if (confirm == true) {

        this.workOrderService.CancelDeferredOrder(i).subscribe(res => {

          if (!res.IsErrorState) {
            this._alertservice.success("CancelDeferredOrderSuccessed");
            //refresh
            this.searchCaller();
          }
          else {
            this._alertservice.errorList(res.Errors);
          }
        });
      }

    })
  }


  Excel_Img_Src = "/assets/fmsBranding/styles/img/ic_excel.png";
  HoverExcel: boolean = false;

  onexportOrders() {

    let clonedObj = Object.assign({}, this.SearchCriteria);

    clonedObj.ExcelFlage = true;
    //alert time zone offset before send
    //let modifiedCriteria = Object.assign({}, this.SearchCriteria);
    clonedObj.DateTimeFrom = new Date(this.SearchCriteria.DateTimeFrom.getTime());
    clonedObj.DateTimeFrom.setMinutes(
      clonedObj.DateTimeFrom.getMinutes() - clonedObj.DateTimeFrom.getTimezoneOffset());
    clonedObj.DateTimeTo = new Date(this.SearchCriteria.DateTimeTo.getTime());
    clonedObj.DateTimeTo.setMinutes(
      clonedObj.DateTimeTo.getMinutes() - clonedObj.DateTimeTo.getTimezoneOffset()
    );

    this.Excel_Img_Src = "assets/TMSBranding/styles/img/loader.gif";
    this.workOrderService.SearchDeferredWorkOrders(clonedObj)
      .subscribe(res => {
        if (res.IsErrorState == false) {
          if (
            isNullOrUndefined(res.Value.Result) ||
            res.Value.Result.length == 0
          ) {
            this._alertservice.error("NoDataFound");
            return;
          }

          // let excelJson = res.Value.Result.map(value => {
          //   let r = new DailyOrderSummaryExcel();
          //   r.Station = value.StationName;
          //   r.ServiceType = value.ServiceTypeName;
          //   r.Date = value.CreateDate.toString().substring(0, 10);
          //   r.TotalOrdersCount = value.TotalCount;
          //   r.TotalOrdersSum = value.TotalSum;
          //   r.DeliveredCount = value.DeliveredCount;
          //   r.DeliveredSum = value.DeliveredSum;
          //   r.FailedToDeliverCount = value.FailedToDeliverCount;
          //   r.FailedToDeliverSum = value.FailedToDeliverSum;
          //   r.CancelledCount = value.CancelledCount;
          //   return r;
          // });

          this._ExcelService.exportAsExcelFile(res.Value.Result, this.translateService.instant("DeferredOrders"));
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


}
