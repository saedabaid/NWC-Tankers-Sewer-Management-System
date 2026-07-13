import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { AttachmentDTO } from 'src/app/shared/datamodels/attachment-dto';
import { Router } from '@angular/router';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { ContractorService } from '../../../Services/contractor.service';
import { Lookup } from '../../../Models/common/lookup';
import { LookupService } from '../../../Services/lookup.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Contractor } from '../../../Models/contractor';
import { ContractorSearchCriteria } from '../../../Models/search-criteria/contractor-search-criteria';
import { LoaderService } from 'src/app/shared/loader.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';


@Component({
  selector: 'app-contractor-create',
  templateUrl: './contractor-create.component.html',
  styleUrls: ['./contractor-create.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ContractorCreateComponent implements OnInit, OnDestroy {


  contractorDTO = new Contractor();

  ListFiles: AttachmentDTO[] = [];

  updateContractorId: number;
  updateMode = false;
  pagePermission: string = '';



  PersonalIDTypes: Lookup<number>[] = [];
  AreaList: Lookup<string>[] = [];
  CityList: Lookup<string>[] = [];
  bindingModel_Areas: Lookup<string>[] = [];
  bindingModel_Cities: Lookup<string>[] = [];
  citySearchKeyWord = '';

  selectMenuOptions = {
    enableSearchFilter: true,
    singleSelect: true
  };

  Area_Loading = false;
  City_Loading = false;

  SearchStream: SearchStream = new SearchStream();


  constructor(
    private router: Router,
    private contractorService: ContractorService,
    private _alert: alertService,
    private translateService: TranslateService,
    private titleService: Title,
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private mainloading: LoaderService
  ) {

    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('contractlist');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

    let urlList = this.router.routerState.snapshot.url.split('/');
    if (urlList[3] === "edit") {
      this.updateContractorId = +urlList[4];
      this.updateMode = true;
    }
    else {
      this.updateContractorId = null;
      this.updateMode = false;
    }

  }

  ngOnInit() {

    this.setDefaultContract();
    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.getPersonalIDTypes();
    this.getAreas('');

    this.titleService.setTitle(this.translateService.instant((this.updateMode ? 'UpdateContractor' : 'NewContractor')));
  }

  getPersonalIDTypes() {
    this.lookupservice.GetPersonalIDTypes().subscribe(res => {
      if (res.Value) {
        this.PersonalIDTypes = res.Value;
      }
    });
  }

  getAreas(searchKeyword: string) {
    this.SearchStream.initStream("AreaDDL_CreateOrder", (a) => {
      this.Area_Loading = true;
      this.lookupservice.getAreasName(a).subscribe(res => {
        if (res.Value) {
          this.AreaList = res.Value;
          if (this.updateMode && this.bindingModel_Areas.length < 1) {
            this.bindingModel_Areas = this.AreaList.filter(s => s.Id === this.contractorDTO.AreaId);
          }
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

  getCityBasedOnArea(searchKeyword: string) {
    this.citySearchKeyWord = searchKeyword;
    this.SearchStream.initStream("CityDDL_CreateOrder", (a) => {
    if (!isNullOrUndefined(this.contractorDTO.AreaId)) {
        this.City_Loading = true;
        this.lookupservice.getCityName(a, [this.contractorDTO.AreaId]).subscribe(res => {
        if (res.Value)
          this.CityList = res.Value;
        }
        , err => {
          this.City_Loading = false;
        }
        , () => {
          this.City_Loading = false;
        });
      }
    }).next(searchKeyword);
  };

  onAreaDDLChanged($event) {
    this.contractorDTO.AreaId = $event.map(m => m.Id)[0];

    this.contractorDTO.CompanyAddressCityID = null;
    this.bindingModel_Cities = [];
    this.getCityBasedOnArea(this.citySearchKeyWord);
  }

  onCityDDLChanged($event) {
    this.contractorDTO.CompanyAddressCityID = $event.map(m => m.Id)[0];
  }


  setDefaultContract() {
    //this.contractorDTO = new Contractor;

    if (this.updateMode) {
      let filters = new ContractorSearchCriteria;
      filters.Id = this.updateContractorId;
      filters.FilterModel.PageFilter.PageSize = 1;
      filters.FilterModel.PageFilter.PageIndex = 1;

      this.mainloading.PreloaderIcreaseCount();
      this.contractorService.searchContractorList(filters).subscribe(res => {
        if (res.IsErrorState == false && res.Value.Result.length > 0) {
          this.contractorDTO = res.Value.Result[0];

          // choose Area and push it to the list
          if (!isNullOrUndefined(this.contractorDTO.AreaId)) {
            let selectedArea = this.AreaList.find(s => s.Id == this.contractorDTO.AreaId);
            if (!isNullOrUndefined(selectedArea)) {
              this.bindingModel_Areas.push(selectedArea);
            }
            else {
              let newSelectedArea = new Lookup<string>();
              newSelectedArea.Id = this.contractorDTO.AreaId;
              newSelectedArea.Name = this.contractorDTO.AreaName;

              this.AreaList.push(newSelectedArea);
              this.bindingModel_Areas.push(newSelectedArea);
            }
          }


          // choose city and push it to the list
          if (!isNullOrUndefined(this.contractorDTO.CompanyAddressCityID)) {
            let selectedCity = this.CityList.find(s => s.Id == this.contractorDTO.CompanyAddressCityID);
            if (!isNullOrUndefined(selectedCity)) {
              this.bindingModel_Cities.push(selectedCity);
            }
            else {
              let newSelectedCity = new Lookup<string>();
              newSelectedCity.Id = this.contractorDTO.CompanyAddressCityID;
              newSelectedCity.Name = this.contractorDTO.CityName;

              this.CityList.push(newSelectedCity);
              this.bindingModel_Cities.push(newSelectedCity);
            }
          }

        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

      //Attachments
      this.mainloading.PreloaderIcreaseCount();
      this.contractorService.GetContractorAttachments(this.updateContractorId).subscribe(res => {
        if (!res.IsErrorState && !isNullOrUndefined(res.Value)) {
          this.ListFiles = res.Value;
        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

    }
    else {
      this.contractorDTO.PersonalIDType = null;
    }
  }


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.contractorDTO.ContractorFullName)) {
      validationMessages.push("ContractorNameRequired");
    }
    if (isNullOrUndefined(this.contractorDTO.Code)) {
      validationMessages.push("ContractorCodeRequired");
    }
    if (isNullOrUndefined(this.contractorDTO.CommercialIDNumber)) {
      validationMessages.push("CommercialIDNumberRequired");
    }
    if (isNullOrUndefined(this.contractorDTO.TaxNumber)) {
      validationMessages.push("TaxNumberRequired");
    }
    if (isNullOrUndefined(this.contractorDTO.MOI)) {
      validationMessages.push("MOIRequired");
    }

    if (isNullOrUndefined(this.contractorDTO.AreaId)) {
      validationMessages.push("ChooseArea");
    }
    if (isNullOrUndefined(this.contractorDTO.CompanyAddressCityID)) {
      validationMessages.push("ChooseCity");
    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }



  save() {
    this.contractorDTO.ContractorAttachments = this.ListFiles;

    if (!this.isValidModel()) return;

    // edit
    if (!isNullOrUndefined(this.contractorDTO.ID)) {
      this.mainloading.PreloaderIcreaseCount();
      this.contractorService.EditContractor(this.contractorDTO).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          this.navigateContractorList();
        }
      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

    }
    else // new
    {
      this.mainloading.PreloaderIcreaseCount();
      this.contractorService.addContractor(this.contractorDTO).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          this.navigateContractorList();
        }

      }
      ,err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      ,() => {
        this.mainloading.PreloaderDecreaseCount();
      });

    }


  }


  close() {
    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.navigateContractorList();
      }
    });
  }


  navigateContractorList() {
    this.router.navigate(['/tms/contractor/contractorlist']);
  }


  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

}
