import { Component, OnInit } from '@angular/core';
import { ImportExcelService } from 'src/app/shared/Services/excel/import-excel.service';
import { ContractTariff } from 'src/app/TMS-Module/Models/contract-tariff';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-upload-tariff-excel',
  templateUrl: './upload-tariff-excel.component.html',
  styleUrls: ['./upload-tariff-excel.component.scss']
})
export class UploadTariffExcelComponent implements OnInit {

  tariffs: ContractTariff[] = [];

  constructor(
    private importExcelService: ImportExcelService,
    private contractService: ContractService,
    private modalService: BsModalService,
    private mainloading: LoaderService,
    private _alert: alertService,
    private _ExcelService: ExcelService

  ) { }

  ngOnInit() {
  }


  changeExcelFile(file) {

    let res = this.importExcelService.importXLSX(file);
    let packages = {
      Station: [],
      Zone: [],
      CustomerClass: [],
      ServiceType: [],
      TanckerCapacity: [],
      ChargesM3: [],
      Chargeskm: [],
      AfterFirstkm: [],
      From: [],
      To: [],
    }

    res.subscribe(r => {
      //debugger;
      if (r.rows) {

        this.importExcelService.matchObjects(r, packages.Station, "Station");
        this.importExcelService.matchObjects(r, packages.Zone, "Zone");
        this.importExcelService.matchObjects(r, packages.CustomerClass, "Customer_Class");
        this.importExcelService.matchObjects(r, packages.ServiceType, "Service_Type");
        this.importExcelService.matchObjects(r, packages.TanckerCapacity, "Tancker_Capacity");
        this.importExcelService.matchObjects(r, packages.ChargesM3, "Charges_M3");
        this.importExcelService.matchObjects(r, packages.Chargeskm, "Charges_km");
        this.importExcelService.matchObjects(r, packages.AfterFirstkm, "After_First_km");
        this.importExcelService.matchObjects(r, packages.From, "From");
        this.importExcelService.matchObjects(r, packages.To, "To");

        // add packages ain Objects
        for (var i = 0; i < packages.Station.length; i++) {

          if (isNaN(packages.ChargesM3[i])) {
            packages.ChargesM3[i] = "";
          }

          if (isNaN(packages.Chargeskm[i])) {
            packages.Chargeskm[i] = "";
          }

          if (isNaN(packages.AfterFirstkm[i])) {
            packages.AfterFirstkm[i] = "";
          }

          if (isNaN(packages.TanckerCapacity[i])) {
            packages.TanckerCapacity[i] = "";
          }

          if (packages.From[i]) {
            let fromArr = (packages.From[i] as (string)).split('-');
            packages.From[i] = (+fromArr[0] * 10000) + (+fromArr[1] * 100) + (+fromArr[2]);
          }

          if (packages.To[i]) {
            let ToArr = (packages.To[i] as (string)).split('-');
            packages.To[i] = (+ToArr[0] * 10000) + (+ToArr[1] * 100) + (+ToArr[2]);
          }

          let item = new ContractTariff();

          item.StationName = packages.Station[i];
          item.ZoneName = packages.Zone[i];
          item.CustomerClassName = packages.CustomerClass[i];
          item.ServiceTypeName = packages.ServiceType[i];
          item.TanckerCapacityId = packages.TanckerCapacity[i];
          item.CubicMeterCharge = packages.ChargesM3[i];
          item.DistanceCharge = packages.Chargeskm[i];
          item.AfterFirstKM = packages.AfterFirstkm[i];
          item.DateFromHijri = packages.From[i];
          item.DateToHijri = packages.To[i];

          item.ExcelSheetRowId = (i + 2);
          item.ContractID = this.contractService.updateContractId;

          this.tariffs.push(item);
        }

        res.unsubscribe();
      }

    })

  }

  saveList() {

    this.mainloading.PreloaderIcreaseCount();
    this.contractService.AddTariffRange(this.tariffs).subscribe(res => {
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

        this._ExcelService.exportAsExcelFile(excelJson, "FailedTariffList");

        this.contractService.refreshTariffGV$.next();
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
