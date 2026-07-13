import { Component, OnInit, ViewEncapsulation, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { LookupService } from '../../../../Services/lookup.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { searchCriteriaContractStationDTO  } from 'src/app/TMS-Module/Models/search-criteria/search-Criteria-Contract-StationDTO';
import { ContractStationListDTO } from 'src/app/TMS-Module/Models/Contract-Station-ListDTO';
import { ActivatedRoute } from '@angular/router';
import { ContactPersonDTO } from 'src/app/TMS-Module/Models/contact-personDTO';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { TranslateService } from '@ngx-translate/core';
import { ContractStationDTO } from 'src/app/TMS-Module/Models/contract-stationDTO';
import { isNullOrUndefined } from 'src/app/shared/utilities/utilities';
import { LoaderService } from 'src/app/shared/loader.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';

@Component({
  selector: 'stations-details',
  templateUrl: './stations-details.component.html',
  styleUrls: ['./stations-details.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class StationsDetailsComponent implements OnInit, OnDestroy {

  advancedDiv = false;
  validationMessages: string[] = [];
  AreaList: Lookup<string>[] = [];
  CityList: Lookup<string>[] = [];
  StationList: Lookup<string>[] = [];

  SelectedAreas: Lookup<string>[] = [];
  SelectedCites: Lookup<string>[] = [];
  SelectedStation: Lookup<string>[] = [];

  PersonalIDTypes: Lookup<number>[] = [];


  StationListResult: SearchResult<ContractStationListDTO> = new SearchResult<ContractStationListDTO>();

  searchCriteriaContractStationDTO: searchCriteriaContractStationDTO = new searchCriteriaContractStationDTO();
  editMoode = false ;
  authServer: any;
  selectMenuOptions = {
    enableSearchFilter: true,
  };

  Area_Loading = false;
  City_Loading = false;
  Station_Loading = false;
  SearchStream: SearchStream = new SearchStream();


  constructor(private translateService: TranslateService,
    private _alert: alertService,
    private router: Router,
    private route: ActivatedRoute,
    private LookupService: LookupService,
    private ContractService: ContractService,
    private mainloading: LoaderService
    ) { }

  ngOnInit() {

    this.searchCriteriaContractStationDTO.ContractStationDTO = new ContractStationDTO();
    this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson = new ContactPersonDTO();

    this.searchCriteriaContractStationDTO.ContractStationDTO.ContractID = parseInt(this.route.snapshot.paramMap.get('Id')) ;
    this.searchCriteriaContractStationDTO.PageFilter.PageIndex = 1;
    this.searchCriteriaContractStationDTO.PageFilter.PageSize =  Configuration.GridSetting.Pagesize;



  }
  ngAfterViewInit() {
    this.load();
    this.translateService.onLangChange.subscribe(res => {
          this.load();
        });
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  load() {
    this.getArea('');
    this.Search();
    this.getPersonalIDTypes();
  }

  getPersonalIDTypes() {
    this.LookupService.GetPersonalIDTypes().subscribe(res => {
      if (res.Value) {
        this.PersonalIDTypes = res.Value;
      }});
  }

  getArea(searchKeyword: string ) {
    this.SearchStream.initStream('AreaDDL_contractStations', (a) => {
      this.Area_Loading = true;
      this.LookupService.getAreasName(a).subscribe(res => {
        if (res.Value) {
          this.AreaList = res.Value;
        }

      }
      , err => {
        this.Area_Loading = false;
      }
      , () => {
        this.Area_Loading = false;
      });
    }).next(searchKeyword);
  }

onAreaChanged($event) {
  if ($event.length > 0) {
    this.SelectedAreas = $event;
    this.searchCriteriaContractStationDTO.ContractStationDTO.AreaIDs = this.SelectedAreas.map(a => a.Id);
    this.getCityBasedOnArea('');
  } else {
    this.CityList = [];
    this.SelectedCites = [];
    this.StationList = [];
    this.SelectedStation = [];
  }


}

getCityBasedOnArea(searchKeyword: string) {
  this.SearchStream.initStream('CityDDL_contractStations', (a) => {
    this.City_Loading = true;
    this.LookupService.getCityName(a, this.SelectedAreas.map(a => a.Id) ).subscribe(res => {
      if (res.Value) {
        this.CityList = res.Value;
      }
    }
    , err => {
      this.City_Loading = false;
    }
    , () => {
      this.City_Loading = false;
    });
  }).next(searchKeyword);
}


onCityChanged($event) {


  if ($event.length > 0) {
    this.SelectedCites = $event;
    this.searchCriteriaContractStationDTO.ContractStationDTO.CityIDs = this.SelectedCites.map(a => a.Id);
    this.GetStationBasedOnCity('');
  } else {

    this.StationList = [];
    this.SelectedStation = [];
  }
}

  GetStationBasedOnCity(searchKeyword: string) {
    this.SearchStream.initStream('StationDDL_contractStations', (a) => {
      this.Station_Loading = true;
      this.LookupService.GetStationBasedOnCity(a , this.SelectedCites.map(a => a.Id) ).subscribe(res => {
        if (res.Value) {
          this.StationList = res.Value;
        }
      }
      , err => {
        this.Station_Loading = false;
      }
      , () => {
        this.Station_Loading = false;
      });
    }).next(searchKeyword);
  }

onStationChanged( $event) {
  this.SelectedStation = $event;
  this.searchCriteriaContractStationDTO.ContractStationDTO.StationIDs = this.SelectedStation.map(a => a.Id);

}

onPersonalIDTypeChanged($event) {
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.PersonalIDType = $event.target.value;
}

Search() {
  this.mainloading.PreloaderIcreaseCount();
  this.ContractService.SearchStattionList( this.searchCriteriaContractStationDTO ).subscribe(res => {
    if (res.IsErrorState == false ) {

      this.StationListResult = res.Value;
    }
  }
  , err => {
    this.mainloading.PreloaderDecreaseCount();
  }
  , () => {
    this.mainloading.PreloaderDecreaseCount();
  });
}

Save() {

  if ( this.isValidModel() ) {
    if ( this.editMoode ) {
      this.mainloading.PreloaderIcreaseCount();
      this.ContractService.UpdateStation( this.searchCriteriaContractStationDTO.ContractStationDTO).subscribe(res => {
        if (! res.IsErrorState) {
          this._alert.showSuccess();
          this.Clear();

        } else {
          this._alert.errorList(res.Errors);
        }
      }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });
    } else {
      this.mainloading.PreloaderIcreaseCount();
      this.ContractService.AddStation( this.searchCriteriaContractStationDTO.ContractStationDTO).subscribe(res => {

        if (! res.IsErrorState) {
          if (res.Value.failed > 0) {
           // let msg = this.translateService.instant(res.Value.failed.toString()) + ":" + this.translateService.instant("failed")
           // +this.translateService.instant(res.Value.success.toString())  +":" +this.translateService.instant("success") ;
            let msg = `${this.translateService.instant('success')}: ${res.Value.success}, ${this.translateService.instant('failed')}: ${res.Value.failed}`;
            res.Value.message.forEach(element => {
              msg += ',' +  this.translateService.instant(element);
          });

            this._alert.warning(msg);
          } else {
            this._alert.showSuccess();
          }

          this.Clear();

        } else {
          this._alert.errorList(res.Errors);
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
Clear() {
  this.editMoode = false;

  this.SelectedAreas = [];
  this.SelectedCites = [];
  this.SelectedStation = [];
  this.searchCriteriaContractStationDTO.ContractStationDTO.AreaIDs = [];
  this.searchCriteriaContractStationDTO.ContractStationDTO.CityIDs = [];
  this.searchCriteriaContractStationDTO.ContractStationDTO.StationIDs = [];


  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson = new  ContactPersonDTO() ;

  this.Search();
}

edit( s: ContractStationListDTO ) {

 if ( this.editMoode == false) {
  this.editMoode = true ;

  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.ID = s.ContactPersonID ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.FirstName = s.FirstName ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.SecondName = s.SecondName ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LastName = s.LastName ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Position = s.Position ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.PersonalIDType = s.PersonalIDType ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Mobile = s.Mobile ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Email = s.Email ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.PersonalIDNumber = s.PersonalIDNumber ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LandlineNumber = s.LandlineNumber ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.PersonAddressPostalCode = s.PersonAddressPostalCode ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.PersonAddress = s.PersonAddress ;
  this.searchCriteriaContractStationDTO.ContractStationDTO.StationIDs = [s.StationID];
  this.searchCriteriaContractStationDTO.ContractStationDTO.ContractStationID = s.contractStationID;

  this.SelectedStation = [{Id: s.StationID , Name: s.stationName} as Lookup<string>];
  this.SelectedCites = [{Id: s.branchId , Name: s.BranchName} as Lookup<string>];
  this.SelectedAreas = [{Id: s.AreaId , Name: this.AreaList.find(x => x.Id == s.AreaId).Name }  as Lookup<string>];
  this.GetStationBasedOnCity('');
  this.getCityBasedOnArea('');
 }


}

delete( s: ContractStationListDTO) {
  if ( this.editMoode ) {
    this._alert.error('CannotDeleteUntilSaving');
  } else {
    this._alert.confirmationMessage('DeleteMsgContractStation').subscribe(confirm => {
      if (confirm == true) {
        this.mainloading.PreloaderIcreaseCount();
        this.ContractService.DeleteStation( s ).subscribe(res => {

        if (! res.IsErrorState) {
        this._alert.success('DeletedSuccessed');
        this.Search();
        } else {
          this._alert.showError();
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


isValidModel(): boolean {

  this.validationMessages = [];
 
  if ( this.searchCriteriaContractStationDTO.ContractStationDTO.StationIDs.length <= 0 ) {
    this.validationMessages.push('ChooseStation');
  }
 const  EmptyContactPerson = new ContactPersonDTO();
// EmptyContactPerson.CreatedDate = EmptyContactPerson.UpdatedDate=null;
  if (! this.isEquivalent(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson , EmptyContactPerson )  ) {

    if ( isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.FirstName)
      || this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.FirstName == '' ) {
      this.validationMessages.push('InsertFristName');
    }
    if (isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LastName)
      || this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LastName == '') {
      this.validationMessages.push('InsertLastName');
    }
    if ( !isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.FirstName)
          && this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.FirstName.length > 100) {
      this.validationMessages.push('ExceedMaxCharFristName');
    }
    if ( ! isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LastName)
          && this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LastName.length > 100) {
      this.validationMessages.push('ExceedMaxCharLastName');
    }
    if ( ! isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.SecondName)
      && this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.SecondName.length > 100) {
      this.validationMessages.push('ExceedMaxCharSecondName');
    }
    if ( ! isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Position)
      && this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Position.length > 50) {
      this.validationMessages.push('ExceedMaxCharPosition');
    }
    if ( ! isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.PersonAddressPostalCode)
      && this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.PersonAddressPostalCode.length > 10) {
      this.validationMessages.push('ExceedMaxCharPersonAddressPostalCode');
    }

    if ( !( isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Email)
            || this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Email == '') ) {
      if (/^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/.test(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Email) == false) {
        this.validationMessages.push('InvalidEmail');
      }
    }
    if ( !(isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Mobile)
          || this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Mobile == '') ) {
      if (/^[+]*[-\s\./0-9]*$/.test(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.Mobile) == false) {
        this.validationMessages.push('InvalidMobileNumber');
      }
    }
    if ( !(isNullOrUndefined(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LandlineNumber)
        || this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LandlineNumber == '') ) {
      if (/^[-\s\./0-9]*$/.test(this.searchCriteriaContractStationDTO.ContractStationDTO.ContactPerson.LandlineNumber) == false) {
      this.validationMessages.push('InvalidLandlineNumber');
      }
    }

  }

  if (this.validationMessages.length > 0) {
    this._alert.errorList(this.validationMessages);
    return false;
  }
  return true;
}


onPageIndexChanged(evt) {
  this.searchCriteriaContractStationDTO.PageFilter.PageIndex = evt;
  this.Search();
}

onPageSizeChanged(evt) {
  this.searchCriteriaContractStationDTO.PageFilter.PageSize = evt;
  this.Search();
}
  close() {
    this.ContractService.changeTab$.next('contractlist');
  }

  backBtn() {
    this.ContractService.changeTab$.next('contract');
  }

  nextBtn() {
    if (this.StationListResult.Result.length <= 0) {
      this._alert.error('NoStationAdded');
    } else {
      this.ContractService.changeTab$.next('price');
    }

  }
  isEquivalent(a, b) {

    // Create arrays of property names
    const aProps = Object.getOwnPropertyNames(a);
    const bProps = Object.getOwnPropertyNames(b);

    // If number of properties is different,
    // objects are not equivalent
    if (aProps.length != bProps.length) {
        return false;
    }

    for (let i = 0; i < aProps.length; i++) {
        const propName = aProps[i];
        if (propName == 'ID') {
          continue;
        }

        // If values of same property are not equal,
        // objects are not equivalent
        // Get all prop names of Bike
        if (a[propName] == '') {
          a[propName] = null;
        }
        if (b[propName] == '') {
          b[propName] = null;
        }

        if ( a[propName] !== b[propName] ) {
            return false;
        }
    }

    // If we made it this far, objects
    // are considered equivalent
    return true;
}



}
