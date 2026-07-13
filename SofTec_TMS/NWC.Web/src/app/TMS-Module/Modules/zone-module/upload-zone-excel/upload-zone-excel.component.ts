import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ImportExcelService } from 'src/app/shared/Services/excel/import-excel.service';
//import { BsModalService } from 'ngx-bootstrap';
import { LoaderService } from 'src/app/shared/loader.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { ExcelService } from 'src/app/shared/Services/excel/ExcelService';
import { ZoneDTO, Station } from 'src/app/TMS-Module/Models/zoneDTO';
import { zoneListService } from 'src/app/TMS-Module/Services/zone-list.service';
import { BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-upload-zone-excel',
  templateUrl: './upload-zone-excel.component.html',
  styleUrls: ['./upload-zone-excel.component.scss']
})
export class UploadZoneExcelComponent implements OnInit {

  zones: ZoneDTO[] = [];
  @Output() confirm: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private importExcelService: ImportExcelService,
    private modalService: BsModalService,
    private mainloading: LoaderService,
    private _alert: alertService,
    private _ExcelService: ExcelService,
    private _zoneListService: zoneListService

  ) { }

  ngOnInit() {
  }


  changeExcelFile(file) {

    let res = this.importExcelService.importXLSX(file);
    let packages = {
      ZoneName: [],
      ZoneCode: [],
      City: [],
      MainStation: [],
      MainStationDistance: [],
      BackupStation: [],
      BackupStationDistance: [],
      IntegrationID: [],
      
    }

    res.subscribe(r => {
      if (r.rows) {

        this.importExcelService.matchObjects(r, packages.ZoneName, "Zone");
        this.importExcelService.matchObjects(r, packages.ZoneCode, "Zone_Code");
        this.importExcelService.matchObjects(r, packages.City, "City");
        this.importExcelService.matchObjects(r, packages.MainStation, "Main_Station");
        this.importExcelService.matchObjects(r, packages.MainStationDistance, "Main_Station_Distance");
        this.importExcelService.matchObjects(r, packages.BackupStation, "Backup_Station");
        this.importExcelService.matchObjects(r, packages.BackupStationDistance, "Backup_Station_distance");
        this.importExcelService.matchObjects(r, packages.IntegrationID, "Integration_ID");

        // add packages ain Objects
        for (var i = 0; i < packages.ZoneCode.length; i++) {

          let item = new ZoneDTO();

          item.Name = packages.ZoneName[i];
          item.Code = packages.ZoneCode[i];
          item.CityName = packages.City[i];
          item.MainStation.StationName = packages.MainStation[i];
          item.MainStation.Distance = packages.MainStationDistance[i];
          item.IntegrationID = packages.IntegrationID[i];
          item.ExcelSheetRowId = (i + 2);
          
          let bakupSta = new Station()
          bakupSta.StationName = packages.BackupStation[i];
          bakupSta.Distance = packages.BackupStationDistance[i];
          
          if(bakupSta.StationName && bakupSta.StationName != '' 
              && bakupSta.Distance) {
            item.BackupStations.push(bakupSta);
          }

          this.zones.push(item);
        }

        res.unsubscribe();
      }

    })

  }

  saveList() {

    this.mainloading.PreloaderIcreaseCount();
    this._zoneListService.AddRange(this.zones).subscribe(res => {
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

        this._ExcelService.exportAsExcelFile(excelJson, "FailedZoneList");

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
