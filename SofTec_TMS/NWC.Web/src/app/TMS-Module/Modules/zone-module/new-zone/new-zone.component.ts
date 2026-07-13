import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { LookupService } from '../../../Services/lookup.service';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { ZoneDTO, Station } from 'src/app/TMS-Module/Models/zoneDTO';
import { zoneListService } from '../../../Services/zone-list.service';
import { ActivatedRoute, Router } from '@angular/router';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LoaderService } from 'src/app/shared/loader.service';


@Component({
  selector: 'app-new-zone',
  templateUrl: './new-zone.component.html',
  styleUrls: ['./new-zone.component.scss'],
  encapsulation: ViewEncapsulation.None

})
export class NewZoneComponent implements OnInit {
  AreaList: Lookup<string>[] = [];
  CityList: Lookup<string>[] = [];
  MainStationList: Lookup<string>[] = [];
  BackupStationList: Lookup<string>[] = [];
  RestrictedTypes: Lookup<string>[] = [];

  PageFilter: PageFilter = new PageFilter();

  error: string = '';
  ZoneDTO: ZoneDTO = new ZoneDTO();
  IsEditableMood: boolean = false;
  ZoneID: string;
  SelectedAreas: Lookup<string>[] = [];
  validationMessages: string[] = [];

  selectMenuOptions = {
    enableSearchFilter: true,
    singleSelect: true
  };

  citySearchKeyWord = '';
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];

  pagePermission: string = '';
  loading_City= false;

  constructor(
    private translateService: TranslateService,
    private router: Router,
    private _alert: alertService,
    private route: ActivatedRoute,
    private lookupservice: LookupService,
    private zoneListService: zoneListService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private mainloading: LoaderService
  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('zonelist');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.ZoneID = this.route.snapshot.paramMap.get("ID");
    //this.ZoneDTO.ZoneWithoutTanker = false;
  }

  ngAfterViewInit() {
    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    })
  }

  load() {
    this.getAreas('');
    if (this.ZoneID != null) {
      this.IsEditableMood = true;
      this.GetZoneDetails();
    }

    else {
      this.GetTransporterTypesForAddingMode();
    }

    this.titleService.setTitle( this.translateService.instant(( this.IsEditableMood ? 'EditZone': 'create new zone' )));
  }

  zoneWithoutTanker(){
    this.ZoneDTO.ZoneWithoutTanker = !this.ZoneDTO.ZoneWithoutTanker;
  }
  GetTransporterTypesForAddingMode() {
    this.lookupservice.GetTransporterTypes().subscribe(TransporterTypes => {
      if (TransporterTypes.Value) {
        this.ZoneDTO.AllowedTankerTypes = TransporterTypes.Value;
      }
    });
  }

  GetTransporterTypesForEditingMoode() {
    this.lookupservice.GetTransporterTypes().subscribe(TransporterTypes => {
      if (TransporterTypes.Value) {
        this.ZoneDTO.AllowedTankerTypes = TransporterTypes.Value;
        for (var r of this.ZoneDTO.RestrictedTankerTypes) {
          var deleted = this.ZoneDTO.AllowedTankerTypes.splice(this.ZoneDTO.AllowedTankerTypes.findIndex(s => s.Id == r.Id), 1);
          r.Name = deleted[0].Name;
        }
      }
    });
  }

  GetZoneDetails() {
    this.mainloading.PreloaderIcreaseCount();
    this.zoneListService.GetZoneDetails(parseInt(this.ZoneID)).subscribe(res => {
      if (res.Value) {
        this.ZoneDTO = res.Value;
        // this.getCityBasedOnArea('');
        // this.GetStationBasedOnCity('');
        this.fillLookupForEditing();
        this.GetTransporterTypesForEditingMoode();
      }
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });
  }

  fillLookupForEditing() {
    this.lookupservice.getCityName(name, [this.ZoneDTO.AreaID]).subscribe(res => {
      if (res.Value) {
        this.CityList = res.Value;
        this.bindingModel_Cities = this.CityList.filter(s => s.Id === this.ZoneDTO.CityID);
        this.bindingModel_Areas = this.AreaList.filter(s => s.Id === this.ZoneDTO.AreaID);
        this.GetStationBasedOnCity('');
      }

    });
  }

  getAreas(name) {
    this.lookupservice.getAreasName(name).subscribe(res => {
      if (res.Value) {
        this.AreaList = res.Value;
        if (this.IsEditableMood && this.bindingModel_Areas.length < 1) {
          this.bindingModel_Areas = this.AreaList.filter(s => s.Id === this.ZoneDTO.AreaID);
        }
      }
    });
  }

  onAreaDDLChanged($event) {
    this.ZoneDTO.AreaID = $event.map(m => m.Id)[0];

    this.ZoneDTO.CityID = null;
    this.bindingModel_Cities = [];
    this.getCityBasedOnArea(this.citySearchKeyWord);

    this.ZoneDTO.MainStation = new Station();
    this.ZoneDTO.BackupStations = [];
  }

  getCityBasedOnArea(name) {
    this.citySearchKeyWord = name;
    if (!isNullOrUndefined(this.ZoneDTO.AreaID)){
      this.loading_City = true;
      this.lookupservice.getCityName(name, [this.ZoneDTO.AreaID]).subscribe(res => {
        if (res.Value)
          this.CityList = res.Value;
      }
      , err => {
        this.loading_City = false;
      }, () => {
        this.loading_City = false;
      });
    }
  };

  onCityDDLChanged($event) {
    this.ZoneDTO.CityID = $event.map(m => m.Id)[0];
    this.GetStationBasedOnCity('');
  }

  GetStationBasedOnCity(name) {
    if (!isNullOrUndefined(this.ZoneDTO.CityID)){
      this.lookupservice.GetAllStationBasedOnCity([this.ZoneDTO.CityID]).subscribe(res => {
        if (res.Value)
          this.BackupStationList = this.MainStationList = res.Value;
      });
    }
  }

  onMainStationChanged($event) {
  }
  onBackupStationChanged($event, index) {
    if ($event.target.value == "null") {
      this.ZoneDTO.BackupStations[index].ID = null;
    }
  }

  addRequest(index: number) {
    const deletedElement = this.ZoneDTO.AllowedTankerTypes.splice(index, 1)
    this.ZoneDTO.RestrictedTankerTypes.push(deletedElement[0]);
  }

  removeRequest(index: number) {
    const deletedElement = this.ZoneDTO.RestrictedTankerTypes.splice(index, 1)
    this.ZoneDTO.AllowedTankerTypes.push(deletedElement[0]);
  }
  ZoneNameChanged($event) {
    this.ZoneDTO.Name = $event.target.value;
  }
  ZoneCodeChanged($event) {
    this.ZoneDTO.Code = $event.target.value;
  }
  BackupStationDistanceChanged($event, StID) {

    this.ZoneDTO.BackupStations[StID].Distance = (parseInt($event.target.value)) ? parseInt($event.target.value) : null;

  }

  MainStationDistanceChanged($event) {
    this.ZoneDTO.MainStation.Distance = (parseInt($event.target.value)) ? parseInt($event.target.value) : null;
  }

  AddBackupStation() {
    this.ZoneDTO.BackupStations.push(new Station())
  }
  saveZone() {
    //verification

    if (this.isValidModel()) {

      if (this.IsEditableMood) {
        this.mainloading.PreloaderIcreaseCount();
        this.zoneListService.Update(this.ZoneDTO).subscribe(res => {
          if (!res.IsErrorState) {
            this._alert.showSuccess();
            // redirect to /tms/zonelist
            this.router.navigate(['/tms/zone/zonelist']);
          }
          else {
            this._alert.errorList(res.Errors);
          }

        }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        })
      }
      else {
        this.mainloading.PreloaderIcreaseCount();
        this.zoneListService.Add(this.ZoneDTO).subscribe(res => {
          if (!res.IsErrorState) {
            this._alert.showSuccess();
            this.router.navigate(['/tms/zone/zonelist']);
          }
          else {
            this._alert.errorList(res.Errors);
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

  }
  cancel() {

    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.router.navigate(['/tms/zone/zonelist']);
      }
    })
  }




  DeleteStation(i: number) {
    this.ZoneDTO.BackupStations.splice(i, 1);
  }
  isValidModel(): boolean {

    this.validationMessages = [];
    //debugger
    if (isNullOrUndefined(this.ZoneDTO.Name) || this.ZoneDTO.Name == "") {
      this.validationMessages.push("InsertZoneName");
    }
    if (isNullOrUndefined(this.ZoneDTO.Code) || this.ZoneDTO.Code == "") {
      this.validationMessages.push("InsertCode");
    }
    if (isNullOrUndefined(this.ZoneDTO.AreaID)) {
      this.validationMessages.push("ChooseArea");
    }
    if (isNullOrUndefined(this.ZoneDTO.CityID)) {
      this.validationMessages.push("ChooseCity");
    }
    if (isNullOrUndefined(this.ZoneDTO.MainStation.ID)) {
      this.validationMessages.push("ChooseMainStation");
    }
    if (isNullOrUndefined(this.ZoneDTO.MainStation.Distance)) {
      this.validationMessages.push("EnterMainStationDistance");
    }
    if (!isNullOrUndefined(this.ZoneDTO.MainStation.Distance) && this.ZoneDTO.MainStation.Distance <= 0) {
      this.validationMessages.push("MainStationDistanceshouldBePostiveNum");
    }
    this.backupStationsHasDistance();

    if (this.ZoneDTO.BackupStations.map(s => s.ID).includes(this.ZoneDTO.MainStation.ID)) {
      this.validationMessages.push("BackupStations!=MainStation");
    }
    if (this.ZoneDTO.AllowedTankerTypes.length < 1) {
      this.validationMessages.push("AtLeastOneAllowedTankerType");
    }

    if (this.hasDuplicates(this.ZoneDTO.BackupStations.map(s => s.ID))) {
      this.validationMessages.push("BackupStationsShouldBeDifferant");
    }


    if (this.validationMessages.length > 0) {
      this._alert.errorList(this.validationMessages);
      return false;
    }
    return true;
  }

  hasDuplicates(array: string[]): boolean {
    return (new Set(array)).size !== array.length;
  }
  backupStationsHasDistance() {
    var flage1 = false;
    var flage2 = false;
    this.ZoneDTO.BackupStations.map(s => {
      if (s.ID) {
        if (isNullOrUndefined(s.Distance) && !flage1) {
          this.validationMessages.push("EnterBackupStationsDistance");
          flage1 = true;
        }
        if (!isNullOrUndefined(s.Distance) && s.Distance <= 0 && !flage2) {
          this.validationMessages.push("BackupStationDistanceshouldBePostiveNum");
          flage2 = true;
        }
        if (flage2 && flage1) {
          return;
        }
      }
      else {
        this.validationMessages.push("ChooseBackupStation");
        return;
      }
    })
  }

}

