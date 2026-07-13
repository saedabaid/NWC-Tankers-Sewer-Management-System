import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DeviceMeterService } from '../../../Services/device-meter.service';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from '../../../Services/lookup.service';
import { MeterReading } from '../../../Models/meter-reading';
import { Lookup } from '../../../Models/common/lookup';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';


@Component({
  selector: 'app-add-meter-reading',
  templateUrl: './add-meter-reading.component.html',
  styleUrls: ['./add-meter-reading.component.scss']
})
export class AddMeterReadingComponent implements OnInit {

  meterReadingDTO = new MeterReading();
  pagePermission: string = '';

  selectedTime: string;

  StationsList: Lookup<string>[] = [];
  DeviceMeterList: Lookup<string>[] = [];
  bindingModel_Stations: Lookup<string>[] = [];
  bindingModel_DeviceMeters: Lookup<string>[] = [];
  deviceMeterSearchKeyWord = '';

  selectMenuOptions = {
    enableSearchFilter: true,
    singleSelect: true
  };

  station_Loading = false;
  Device_Loading = false;
  SearchStream: SearchStream = new SearchStream();


  constructor(
    private router: Router,
    private deviceMeterService: DeviceMeterService,
    private _alert: alertService,
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private mainloading: LoaderService

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('deviceMeterReading');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

  }

  ngOnInit() {
    this.setDefaultReading();
    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.getStations('');

    this.titleService.setTitle(this.translateService.instant('AddMeterReading'));
  }

  setDefaultReading() {
    this.meterReadingDTO.MeterReading = 0;
    this.meterReadingDTO.ReadingTime = new Date();
    this.selectedTime = this.meterReadingDTO.ReadingTime.toTimeString().substring(0, 5);

  }

  getStations(searchKeyword: string) {
    this.SearchStream.initStream("Station_AddReading", (a) => {
      this.station_Loading = true;
      this.lookupservice.SearchPermittedStations(a).subscribe(res => {
        if (res.Value) {
          this.StationsList = res.Value;
        }
      }
      , err => {
        this.station_Loading = false;
      }
      , () => {
        this.station_Loading = false;
      });

    }).next(searchKeyword);
  }

  getMetersBasedOnStations(searchKeyword: string) {
    this.deviceMeterSearchKeyWord = searchKeyword;
    this.SearchStream.initStream("Device_AddReading", (a) => {
      if (!isNullOrUndefined(this.meterReadingDTO.StationID)) {
        this.Device_Loading = true;
        this.lookupservice.SearchMeterSerial(a, [this.meterReadingDTO.StationID]).subscribe(res => {
          if (res.Value)
            this.DeviceMeterList = res.Value;
        }
        ,err => {
          this.Device_Loading = false;
        }
        ,() => {
          this.Device_Loading = false;
        });
      }
    }).next(searchKeyword);
  };

  onAreaDDLChanged($event) {
    this.meterReadingDTO.StationID = $event.map(m => m.Id)[0];

    this.meterReadingDTO.DeviceMeterID = null;
    this.bindingModel_DeviceMeters = [];
    this.getMetersBasedOnStations(this.deviceMeterSearchKeyWord);
  }

  onDeviceMetersDDLChanged($event) {
    this.meterReadingDTO.DeviceMeterID = $event.map(m => m.Id)[0];
  }


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.meterReadingDTO.StationID) || +this.meterReadingDTO.StationID < 1) {
      validationMessages.push("ChooseStation");
    }
    if (isNullOrUndefined(this.meterReadingDTO.DeviceMeterID) || +this.meterReadingDTO.DeviceMeterID < 1) {
      validationMessages.push("ChooseDeviceMeter");
    }
    if (isNullOrUndefined(this.meterReadingDTO.MeterReading)) {
      validationMessages.push("MeterReadingRequired");
    }
    if (+this.meterReadingDTO.MeterReading < 1) {
      validationMessages.push("MeterReadingMustPositive");
    }

    if (isNullOrUndefined(this.meterReadingDTO.ReadingTime)) {
      validationMessages.push("ReadingDateRequired");
    }
    if (isNullOrUndefined(this.selectedTime)) {
      validationMessages.push("ReadingTimeRequired");
    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  save() {

    if (!this.isValidModel()) return;

    let selectedtimeList = this.selectedTime.split(':');
    this.meterReadingDTO.ReadingTime.setHours(+selectedtimeList[0]);
    this.meterReadingDTO.ReadingTime.setMinutes(+selectedtimeList[1]);

    //alert time zone offset before send
    let modifiedCriteria = Object.assign({}, this.meterReadingDTO);
    modifiedCriteria.ReadingTime = new Date(this.meterReadingDTO.ReadingTime.getTime());
    modifiedCriteria.ReadingTime.setMinutes(modifiedCriteria.ReadingTime.getMinutes() - modifiedCriteria.ReadingTime.getTimezoneOffset());

    this.mainloading.PreloaderIcreaseCount();
    this.deviceMeterService.AddReading(modifiedCriteria).subscribe(response => {
      if (response.IsErrorState === true) {
        this._alert.errorList(response.Errors);
      } else {
        this._alert.showSuccess();
        this.navigateMeterReadingList();
      }

    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });

  }


  close() {
    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.navigateMeterReadingList();
      }
    });
  }

  navigateMeterReadingList() {
    this.router.navigate(['/tms/devicemeter/readingslist']);
  }

  onMeterReadingChanged() {
    if (isNaN(+this.meterReadingDTO.MeterReading) || +this.meterReadingDTO.MeterReading < 0) {
      this.meterReadingDTO.MeterReading = 0;
    }
  }

}
