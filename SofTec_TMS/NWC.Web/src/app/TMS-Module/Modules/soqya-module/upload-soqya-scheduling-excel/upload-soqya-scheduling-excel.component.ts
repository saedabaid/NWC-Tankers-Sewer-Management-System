import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ImportExcelService } from 'src/app/shared/Services/excel/import-excel.service';
//import { BsModalService } from 'ngx-bootstrap';
import { LoaderService } from 'src/app/shared/loader.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { SoqyaService } from '../../../Services/soqya.service';
import { SoqyaScheduleDTO } from 'src/app/TMS-Module/Models/SoqyaScheduleDTO';
import { BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-upload-zone-excel',
  templateUrl: './upload-soqya-scheduling-excel.component.html',
  styleUrls: ['./upload-soqya-scheduling-excel.component.scss']
})
export class UploadSoqyaSchedulingExcelComponent implements OnInit {

  SoqyaSchedulingDays: SoqyaScheduleDTO[] = [];
  @Output() confirm: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private importExcelService: ImportExcelService,
    private modalService: BsModalService,
    private mainloading: LoaderService,
    private _alert: alertService,
    private _ExcelService: ExcelService,
    private SoqyaService: SoqyaService

  ) { }

  ngOnInit() {
  }


  changeExcelFile(file) {

    let res = this.importExcelService.importXLSX(file);
    let SoqyaScheduling = {
      NationalID:[],
      AccountID: [],
      ScheduledDate: [],
      TimeSlotFrom: [],
      TimeSlotTo: [],
      Quantity: [],  
    }

    res.subscribe(r => {
      if (r.rows) {
        this.importExcelService.matchObjects(r, SoqyaScheduling.NationalID, "Customer National ID");
        this.importExcelService.matchObjects(r, SoqyaScheduling.AccountID, "Account ID");
        this.importExcelService.matchObjects(r, SoqyaScheduling.ScheduledDate, "Scheduled Date");
        this.importExcelService.matchObjects(r, SoqyaScheduling.TimeSlotFrom, "Time Slot From");
        this.importExcelService.matchObjects(r, SoqyaScheduling.TimeSlotTo, "Time Slot To");
        this.importExcelService.matchObjects(r, SoqyaScheduling.Quantity, "Quantity");
      

        // add scheduled date in Objects
        for (var i = 0; i < SoqyaScheduling.AccountID.length; i++) {

          let item = new SoqyaScheduleDTO();
          let from = new Date(SoqyaScheduling.TimeSlotFrom[i]);
          let to = new Date(SoqyaScheduling.TimeSlotTo[i]);

          item.AccountId = SoqyaScheduling.AccountID[i];
          item.CustomerIdNumber = SoqyaScheduling.NationalID[i];
          item.ScheduleDate = new Date(SoqyaScheduling.ScheduledDate[i].getTime());
          item.TimeSlotFrom = from.toLocaleTimeString('it-IT');
          item.TimeSlotTo = to.toLocaleTimeString('it-IT');
          item.Quantity = SoqyaScheduling.Quantity[i];
          item.ExcelSheetRowId = (i + 2);


          //alert time zone offset before send
          //let modifiedObject = Object.assign({}, this.SoqyaScheduleDTO);
          //item.ScheduleDate = new Date(item.ScheduleDate.getTime());
          item.ScheduleDate.setMinutes(
            item.ScheduleDate.getMinutes() - item.ScheduleDate.getTimezoneOffset());

          this.SoqyaSchedulingDays.push(item);
        }

        res.unsubscribe();
      }

    })

  }

  saveList() {

    this.mainloading.PreloaderIcreaseCount();
    this.SoqyaService.AddRange(this.SoqyaSchedulingDays).subscribe(res => {
      if (res.IsErrorState == true) {
        this._alert.error(res.ErrorDescription);
      }
      else {

        let excelJson = res.Value.map(value => {
          let r = {
            RowNumber: value.ExcelSheetRowId,
            Error: value.ExcelValidation
          }
          return r;
        });

        this._ExcelService.exportAsExcelFile(excelJson, "FailedSoqyaSchedulingList");

        this.confirm.emit(true);

        //this.contractService.refreshTariffGV$.next();
        this.cancel();
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });

  }

  cancel() {
    this.modalService.hide(1);
  }


}
