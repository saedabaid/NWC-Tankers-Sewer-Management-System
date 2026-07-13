import { Component, OnInit } from '@angular/core';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { SoqyaScheduleSC } from 'src/app/TMS-Module/Models/search-criteria/Soqya-search-criteria';
import { SoqyaScheduleDTO, SoqyaBalanceDTO } from 'src/app/TMS-Module/Models/SoqyaScheduleDTO';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { SoqyaService } from '../../../Services/soqya.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { LoaderService } from 'src/app/shared/loader.service';
//import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { UploadSoqyaSchedulingExcelComponent } from '../upload-soqya-scheduling-excel/upload-soqya-scheduling-excel.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-soqya-scheduling',
  templateUrl: './soqya-scheduling.component.html',
  styleUrls: ['./soqya-scheduling.component.scss']
})
export class SoqyaSchedulingComponent implements OnInit {

  pagePermission: string
  IsArabic = false;
  advancedDiv = false;

  SearchCriteria: SoqyaScheduleSC = new SoqyaScheduleSC();
  searchResult = new SearchResult<SoqyaScheduleDTO>();
  editMoode: boolean = false;
  show: boolean = false;


  RepeateCommingMonth: boolean = false;
  AddingArea: boolean = false;
  SoqyaBalance: SoqyaBalanceDTO = new SoqyaBalanceDTO();
  SoqyaScheduleDTO: SoqyaScheduleDTO = new SoqyaScheduleDTO();


  constructor(
    private lookupService: LookupService,
    private _translate: TranslateService,
    private SoqyaService: SoqyaService,
    private titleService: Title,
    private alert: alertService,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService,
    private modalRef: BsModalRef,
    private modalService: BsModalService
  ) {

    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName("tmsSoqyaScheduling");
    this.authenticationService.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {

    this.fillDayNumber();
    this.resetAddEditData();

    this.resetListData(true);

    this.load();
    this._translate.onLangChange.subscribe(res => {
      this.load();
    });

  }


  load() {
    //this.SearchCustomers('');
    this.titleService.setTitle(this._translate.instant('SoqyaScheduling'));
    this.IsArabic = (this._translate.currentLang == 'ar');
    this.GetMonthYearList();
  }

  resetAddEditData() {
    this.editMoode = false;
    this.RepeateCommingMonth = false;

    this.SoqyaScheduleDTO.Quantity = 0;
    this.SoqyaScheduleDTO.ScheduleDate = null;
    this.SoqyaScheduleDTO.TimeSlotFrom = "08:00";
    this.SoqyaScheduleDTO.TimeSlotTo = "16:00";

    this.SoqyaScheduleDTO.MonthYearAddIds = [];
    this.SoqyaScheduleDTO.ScheculeDayListAdd = [];

    this.bindingModel_nextMonthYearList = [];
    this.bindingModel_DayNumbers = [];

  }

  resetListData(forceReset: boolean = false) {

    if (forceReset || this.AddingArea) {
      this.AddingArea = false;

      this.SoqyaBalance = new SoqyaBalanceDTO();

      this.SearchCriteria.PageFilter = new PageFilter();
      this.SearchCriteria.PageFilter.PageIndex = 1;
      this.SearchCriteria.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
      this.SearchCriteria.MonthYear = null;

      this.searchResult.Result = [];
      this.searchResult.TotalCount = 0;

      this.resetAddEditData();

    }
  }


  //#region "Drop Down lists" *********************************************************************************
  SearchStream: SearchStream = new SearchStream();

  //customerId: number;
  //CustomerAccountId: number;

  stationList: Lookup<string>[] = [];
  customerNameList: Lookup<number>[] = [];
  customerAccountList: Lookup<number>[] = [];
  customerLocationList: Lookup<number>[] = [];
  customerAccountStationsList: Lookup<number>[] = [];
  //customerBalancesList: Lookup<number>[] = [];
  MonthYearList: Lookup<number>[] = [];
  DayNumbers: Lookup<number>[] = [];
  //nextCustomerBalancesList: Lookup<number>[] = [];
  nextMonthYearList: Lookup<number>[] = [];

  //bindingModel_nextCustomerBalancesList: Lookup<number>[] = [];
  bindingModel_MonthYearList: Lookup<number>[] = [];
  bindingModel_nextMonthYearList: Lookup<number>[] = [];
  bindingModel_DayNumbers: Lookup<number>[] = [];

  Customer_Loading: boolean = false;
  CustomerAccount_Loading: boolean = false;
  nextBalance_Loading: boolean = false;
  Balance_Loading: boolean = false;

  selectMenuOptions = {
    enableSearchFilter: true,
    singleSelect: true,
  };

  selectMenuOptions2 = {
    enableSearchFilter: false,
  }
  selectMenuOptions3 = {
    enableSearchFilter: false,
    singleSelect: true,
  }

  SearchPermittedStations(searchKeyword: string) {

    this.lookupService.SearchPermittedSoqyaStations(searchKeyword).subscribe(res => {
      if (res.Value != null) {
        this.stationList = res.Value;

      }
      else {
        this.stationList = [];

      }
    });
  }

  GetMonthYearList() {
    this.lookupService.GetCommingMonthYear().subscribe(res => {
      if (res.Value != null && res.Value.length > 0)
        this.MonthYearList = res.Value;

      else
        this.MonthYearList = [];
    });
  }

  SearchCustomers(searchKeyword: string) {
    this.SearchStream.initStream("CustomerDDL_SoqyaScheduling", (a) => {

      this.customerAccountList = [];
      this.customerLocationList = [];
      //this.customerBalancesList = [];
      this.SearchCriteria.CustomerAccountId = null;
      this.bindingModel_MonthYearList = [];
      this.resetListData();

      this.Customer_Loading = true;
      this.lookupService.SearchSoqyaCustomers(a).subscribe(res => {
        if (res.Value)
          this.customerNameList = res.Value;
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }

  onCustomerDDLChanged(evt) {

    let customerId = evt.map(m => m.Id)[0];
    //this.customerId = evt.map(m => m.Id)[0];

    //this.customerBalancesList = [];
    this.SearchCriteria.CustomerAccountId = null;
    this.bindingModel_MonthYearList = [];
    this.resetListData();

    if (!isNullOrUndefined(customerId)) {
      this.CustomerAccount_Loading = true;
      this.lookupService.GetCustomerAccountsSoqya(customerId).subscribe(res => {
        if (res.Value != null && res.Value.length > 0)
          this.customerAccountList = res.Value;

        else
          this.customerAccountList = [];
      }
        , err => {
          this.CustomerAccount_Loading = false;
        }
        , () => {
          this.CustomerAccount_Loading = false;
        });

      this.lookupService.GetCustomerLocations(customerId).subscribe(res => {
        if(res.Value != null && res.Value.length > 0)
          this.customerLocationList = res.Value;
          else
          this.customerLocationList = [];
      });

    }
    else {
      this.customerAccountList = [];
      this.customerLocationList = [];
    }

  }


  oncustomerAccountDDLChanged(evt) {
    this.show = false;
    //this.CustomerAccountId = evt.map(m => m.Id)[0];
    this.SoqyaScheduleDTO.CustomerAccountId = evt.map(m => m.Id)[0];

    this.SearchCriteria.CustomerAccountId = evt.map(m => m.Id)[0];

    this.bindingModel_MonthYearList = [];
    this.resetListData();

    this.customerAccountStationsList = [];
    let customerAccountId = evt.map(m => m.Id)[0];

    if(!isNullOrUndefined(customerAccountId)){
      this.lookupService.getCustomerAccountStations(customerAccountId).subscribe(res =>{
        if(res.Value != null && res.Value.length > 0){
          this.customerAccountStationsList = res.Value;
          this.show = true;
        }
        else
        this.customerAccountStationsList = [];
      });
    }
    else
    this.customerAccountStationsList = [];
  }

  onMonthYearDDLChanged(evt) {
    this.resetListData();

    this.SearchCriteria.MonthYear = evt.map(m => m.Id)[0];

    if (this.SearchCriteria.MonthYear && this.SearchCriteria.MonthYear > 0) {
      this.nextMonthYearList = this.MonthYearList.filter(a => a.Id > this.SearchCriteria.MonthYear);
    }

  }


  fillDayNumber() {
    var day1 = new Lookup<number>();
    day1.Id = 1;
    day1.Name = "1st";
    this.DayNumbers.push(day1);

    var day2 = new Lookup<number>();
    day2.Id = 2;
    day2.Name = "2nd";
    this.DayNumbers.push(day2);

    var day3 = new Lookup<number>();
    day3.Id = 3;
    day3.Name = "3rd";
    this.DayNumbers.push(day3);

    var day4 = new Lookup<number>();
    day4.Id = 4;
    day4.Name = "4th";
    this.DayNumbers.push(day4);

    var day5 = new Lookup<number>();
    day5.Id = 5;
    day5.Name = "5th";
    this.DayNumbers.push(day5);

    var day6 = new Lookup<number>();
    day6.Id = 6;
    day6.Name = "6th";
    this.DayNumbers.push(day6);

    var day7 = new Lookup<number>();
    day7.Id = 7;
    day7.Name = "7th";
    this.DayNumbers.push(day7);

    var day8 = new Lookup<number>();
    day8.Id = 8;
    day8.Name = "8th";
    this.DayNumbers.push(day8);

    var day9 = new Lookup<number>();
    day9.Id = 9;
    day9.Name = "9th";
    this.DayNumbers.push(day9);

    var day10 = new Lookup<number>();
    day10.Id = 10;
    day10.Name = "10th";
    this.DayNumbers.push(day10);

    var day11 = new Lookup<number>();
    day11.Id = 11;
    day11.Name = "11th";
    this.DayNumbers.push(day11);

    var day12 = new Lookup<number>();
    day12.Id = 12;
    day12.Name = "12th";
    this.DayNumbers.push(day12);

    var day13 = new Lookup<number>();
    day13.Id = 13;
    day13.Name = "13th";
    this.DayNumbers.push(day13);

    var day14 = new Lookup<number>();
    day14.Id = 14;
    day14.Name = "14th";
    this.DayNumbers.push(day14);

    var day15 = new Lookup<number>();
    day15.Id = 15;
    day15.Name = "15th";
    this.DayNumbers.push(day15);

    var day16 = new Lookup<number>();
    day16.Id = 16;
    day16.Name = "16ht";
    this.DayNumbers.push(day16);

    var day17 = new Lookup<number>();
    day17.Id = 17;
    day17.Name = "17th";
    this.DayNumbers.push(day17);

    var day18 = new Lookup<number>();
    day18.Id = 18;
    day18.Name = "18th";
    this.DayNumbers.push(day18);

    var day19 = new Lookup<number>();
    day19.Id = 19;
    day19.Name = "19th";
    this.DayNumbers.push(day19);

    var day20 = new Lookup<number>();
    day20.Id = 20;
    day20.Name = "20th";
    this.DayNumbers.push(day20);

    var day21 = new Lookup<number>();
    day21.Id = 21;
    day21.Name = "21st";
    this.DayNumbers.push(day21);

    var day22 = new Lookup<number>();
    day22.Id = 22;
    day22.Name = "22nd";
    this.DayNumbers.push(day22);

    var day23 = new Lookup<number>();
    day23.Id = 23;
    day23.Name = "23rd";
    this.DayNumbers.push(day23);

    var day24 = new Lookup<number>();
    day24.Id = 24;
    day24.Name = "24th";
    this.DayNumbers.push(day24);

    var day25 = new Lookup<number>();
    day25.Id = 25;
    day25.Name = "25th";
    this.DayNumbers.push(day25);

    var day26 = new Lookup<number>();
    day26.Id = 26;
    day26.Name = "26th";
    this.DayNumbers.push(day26);

    var day27 = new Lookup<number>();
    day27.Id = 27;
    day27.Name = "27th";
    this.DayNumbers.push(day27);

    var day28 = new Lookup<number>();
    day28.Id = 28;
    day28.Name = "28th";
    this.DayNumbers.push(day28);

    var day29 = new Lookup<number>();
    day29.Id = 29;
    day29.Name = "29th";
    this.DayNumbers.push(day29);

    var day30 = new Lookup<number>();
    day30.Id = 30;
    day30.Name = "30th";
    this.DayNumbers.push(day30);

    var day31 = new Lookup<number>();
    day31.Id = 31;
    day31.Name = "31th";
    this.DayNumbers.push(day31);
  }

  onDayNumbersDDLChanged(evt: Lookup<number>[]) {
    this.SoqyaScheduleDTO.ScheculeDayListAdd = evt.map(r => r.Id);
  }



  onRepeateCommingMonth() {
    this.RepeateCommingMonth = !this.RepeateCommingMonth;
    if (this.RepeateCommingMonth == false) {
      this.SoqyaScheduleDTO.MonthYearAddIds = [];
    }
  }

  onNextMonthYearDDLChanged(evt) {
    this.SoqyaScheduleDTO.MonthYearAddIds = evt.map(m => m.Id);
  }

  //#endregion "Drop Down lists"



  onPageIndexChanged(evt) {
    this.SearchCriteria.PageFilter.PageIndex = evt;
    this.search();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.PageFilter.PageSize = evt;
    this.search();
  }

  isValidModelSearch(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.SearchCriteria.CustomerAccountId)) {
      validationMessages.push("CustomerAccountBalanceIdMissing");
    }



    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  search() {

    if (this.isValidModelSearch()) {
      this.AddingArea = true;
      this.getBalanc();

      this.mainloading.PreloaderIcreaseCount();
      this.SoqyaService.SearchSoqyaSchedules(this.SearchCriteria).subscribe(res => {
        if (res.IsErrorState != true && res.Value.Result != null) {
          this.searchResult.Result = res.Value.Result;
          this.searchResult.TotalCount = res.Value.TotalCount;

          this.searchResult.Result.forEach(a => {
            a.ScheduleDate = new Date(a.ScheduleDate);
          });

        } else {
          this.searchResult.Result = [];
          this.searchResult.TotalCount = 0;
        }
      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        })
    }

  }

  getBalanc() {

    let sendMonthYear = this.SearchCriteria.MonthYear ? this.SearchCriteria.MonthYear : 0;

    this.SoqyaService.GetBalanceAndUsed(this.SearchCriteria.CustomerAccountId, sendMonthYear).subscribe(res => {
      if (res.Value != null) {
        this.SoqyaBalance.Balance = res.Value.Balance;
        this.SoqyaBalance.UsedQuantity = res.Value.UsedQuantity;
      }
    });

  }

  cancel() {
    this.resetAddEditData();
  }




  edit(SoqyaSchedule: SoqyaScheduleDTO) {
    this.editMoode = true;
    this.SoqyaScheduleDTO.Id = SoqyaSchedule.Id;
    //this.SoqyaScheduleDTO.CustomerId = SoqyaSchedule.CustomerId;
    this.SoqyaScheduleDTO.CustomerAccountId = SoqyaSchedule.CustomerAccountId;
    //this.SoqyaScheduleDTO.CustomerBalanceId = SoqyaSchedule.CustomerBalanceId;
    this.SoqyaScheduleDTO.Quantity = SoqyaSchedule.Quantity;
    this.SoqyaScheduleDTO.ScheduleDate = new Date(SoqyaSchedule.ScheduleDate);
    this.SoqyaScheduleDTO.TimeSlotFrom = SoqyaSchedule.TimeSlotFrom;
    this.SoqyaScheduleDTO.TimeSlotTo = SoqyaSchedule.TimeSlotTo;


  }

  delete(Id: number) {
    this.alert.confirmationMessage("DeleteMsgSoqyaSchadule").subscribe(confirm => {
      if (confirm == true) {
        this.mainloading.PreloaderIcreaseCount();
        this.SoqyaService.DeleteSoqyeScheduleRecord(Id).subscribe(res => {
          if (res.Value == true) {
            this.alert.showSuccess();
            this.search();
          }
          else {
            this.alert.showError();
          }
        }
          , err => {
            this.mainloading.PreloaderDecreaseCount();
          }
          , () => {
            this.mainloading.PreloaderDecreaseCount();
          })


      }
    })

  }




  isValidModel(): boolean {

    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.SearchCriteria.MonthYear) || +this.SearchCriteria.MonthYear < 1) {
      validationMessages.push("MonthYearRequired");
    }

    if (isNullOrUndefined(this.SoqyaScheduleDTO.TimeSlotFrom)) {
      validationMessages.push("TimeSlotFromMissed");
    }
    if (isNullOrUndefined(this.SoqyaScheduleDTO.TimeSlotTo)) {
      validationMessages.push("TimeSlotToMissed");
    }
    if (isNullOrUndefined(this.SoqyaScheduleDTO.Quantity)) {
      validationMessages.push("QuantityMissed");
    }
    if (+this.SoqyaScheduleDTO.Quantity <= 0) {
      validationMessages.push("InvalidQuantity");
    }

    if (this.editMoode) {
      if (isNullOrUndefined(this.SoqyaScheduleDTO.ScheduleDate)) {
        validationMessages.push("ScheduleDateMissed");
      }
    } else {
      if (this.SoqyaScheduleDTO.ScheculeDayListAdd.length == 0) {
        validationMessages.push("ScheduleDateMissed");
      }
    }


    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  save() {

    if (!(this.pagePermission == 'Full Control') && !(this.pagePermission == 'Add and Edit')) {
      this.alert.error("NotPermitted")
      return;
    }

    if (!isNullOrUndefined(this.SearchCriteria.MonthYear)
      && this.SoqyaScheduleDTO.MonthYearAddIds.findIndex(s => s == this.SearchCriteria.MonthYear) == -1) {
      this.SoqyaScheduleDTO.MonthYearAddIds.push(this.SearchCriteria.MonthYear);
    }

    if (this.isValidModel()) {
      if (this.editMoode) {

        //alert time zone offset before send
        let modifiedObject = Object.assign({}, this.SoqyaScheduleDTO);
        modifiedObject.ScheduleDate = new Date(this.SoqyaScheduleDTO.ScheduleDate.getTime());
        modifiedObject.ScheduleDate.setMinutes(
          modifiedObject.ScheduleDate.getMinutes() - modifiedObject.ScheduleDate.getTimezoneOffset());

        this.mainloading.PreloaderIcreaseCount();
        this.SoqyaService.EditSoqyeScheduleRecord(modifiedObject).subscribe(res => {
          if (!res.IsErrorState) {
            this.search();
            this.alert.showSuccess();
            this.resetAddEditData();
            //this.editMoode = false;
          } else {
            this.alert.errorList(res.Errors);
          }
        }
          , err => {
            this.mainloading.PreloaderDecreaseCount();
          }
          , () => {
            this.mainloading.PreloaderDecreaseCount();
          })
      } else {
        this.mainloading.PreloaderIcreaseCount();
        this.SoqyaService.AddSoqyaSchedule(this.SoqyaScheduleDTO).subscribe(res => {
          if (!res.IsErrorState) {
            this.search();
            this.alert.showSuccess();
            this.resetAddEditData();
          } else {
            this.alert.errorList(res.Errors);
          }
        }
          , err => {
            this.mainloading.PreloaderDecreaseCount();
          }
          , () => {
            this.mainloading.PreloaderDecreaseCount();
          });
      }

    }

  }


  todayDate = new Date();

  canEdit(item: SoqyaScheduleDTO) {
    if (
      ((this.pagePermission == 'Full Control') || (this.pagePermission == 'Add and Edit'))
      && item.ScheduleDate > this.todayDate
      && !item.WorkOrderId
    ) {
      return true;
    }
  }

  canDelete(item: SoqyaScheduleDTO) {
    if (
      this.pagePermission == 'Full Control'
      && item.ScheduleDate > this.todayDate
      && !item.WorkOrderId
    ) {
      return true;
    }
  }

  uploadExcel() {
    this.modalRef = this.modalService.show(UploadSoqyaSchedulingExcelComponent);

    this.modalRef.content.confirm.subscribe(() => {
      this.alert.showSuccess();
    });
  }


  onQuantityChanged() {
    if (!this.SoqyaScheduleDTO.Quantity || +this.SoqyaScheduleDTO.Quantity < 0) {
      this.SoqyaScheduleDTO.Quantity = 0;
    }
    this.SoqyaScheduleDTO.Quantity = Math.floor(this.SoqyaScheduleDTO.Quantity);
  }


}
