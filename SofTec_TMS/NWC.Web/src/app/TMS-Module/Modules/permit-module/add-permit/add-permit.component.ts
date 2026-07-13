import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PermitDTO } from '@tms-models/PermitDTO';
import { PermitSC } from '@tms-models/search-criteria/Permit-search-criteria';
import { StaffModel } from '@tms-models/staff-model';
import { VehicleDTO, VehiclePermitDTO } from '@tms-models/vehicle-dto';
import { PermitService } from '@tms-services/permit.service';
import { LoaderService } from '@tms-shared/loader.service';
import { alertService } from '@tms-shared/Services/alert/alert.service';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-add-permit',
  templateUrl: './add-permit.component.html',
  styleUrls: ['./add-permit.component.scss']
})
export class AddPermitComponent implements OnInit {

  pagePermission: string;
  updateId: string;
  editMode = false;
  SearchCriteria: PermitSC = new PermitSC();
  Dinfo: StaffModel;
  Tinfo: VehiclePermitDTO;
  Pinfo: PermitDTO = new PermitDTO();
  PermitNumber = '';
  show: boolean = false;
  currentLang: string;
  arabic: boolean = false;
  english: boolean = false;
  constructor(private alert: alertService, private router: Router, private mainloading: LoaderService,
    private permitservice: PermitService, private authenticationService: AuthenticationService, private _tran: TranslateService,) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('PermitsNew');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);
    // debugger;
    let urlList = this.router.routerState.snapshot.url.split('/');
    if (urlList[3] === "edit") {
      this.updateId = urlList[4];

      this.editMode = true;
    }
    else {
      this.updateId = null;
      this.editMode = false;
    }
  }

  ngOnInit() {

    this.setdefaults();
    this.currentLang = this._tran.currentLang;
    this._tran.onLangChange.subscribe(a => {
      this.currentLang = a.lang;
    })
    if (this.currentLang == "ar")
      this.arabic = true;
    else
      this.english = true;
    console.log(this.arabic, this.english);
  }
  setdefaults() {
    if (!isNullOrUndefined(this.updateId)) {
      this.mainloading.PreloaderIcreaseCount();
      this.permitservice.GetPermit(this.updateId).subscribe(data => {
        if (data.IsErrorState != true && data.Value != null) {
          console.log(data.Value.TransporterDTO);

          this.Dinfo = data.Value.StaffDTO;
          this.Tinfo = data.Value.TransporterDTO;
          this.Tinfo.InsuranceStartDate = new Date(data.Value.TransporterDTO.InsuranceStartDate);
          this.Tinfo.LicenseExpiryDate = new Date(data.Value.TransporterDTO.LicenseExpiryDate);

          this.Pinfo = data.Value;
          this.Pinfo.StartDate = new Date(data.Value.StartDate);
          this.Pinfo.Expirationdate = new Date(data.Value.Expirationdate);
          this.Pinfo.LastMaintenanceDate = new Date(data.Value.LastMaintenanceDate);
          this.Pinfo.LastValidationDate = new Date(data.Value.LastValidationDate);

          this.show = true;
          //debugger;
        } else {
          this.alert.error("NoDataFound");
        }
      }, err => {
        this.mainloading.PreloaderDecreaseCount();
      }, () => {
        this.mainloading.PreloaderDecreaseCount();
      });

    }

  }

  isValidModelSearch(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.SearchCriteria.DriverIDSearchText)

      || isNullOrUndefined(this.SearchCriteria.TankerNumberSearchText)) {
      validationMessages.push("CustomerAccountBalanceIdMissing");
    }



    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  onSearchSubmit() {
    if (this.isValidModelSearch()) {
      this.mainloading.PreloaderIcreaseCount();
      this.show = false;
      this.permitservice.SearchDriver(this.SearchCriteria.DriverIDSearchText).subscribe(res => {
        if (res.IsErrorState != true && res.Value != null) {
          this.mainloading.PreloaderIcreaseCount();
          this.permitservice.SearchTanker(this.SearchCriteria.TankerNumberSearchText).subscribe(data => {
            if (data.IsErrorState != true && data.Value != null) {
              this.show = true;
              this.Dinfo = res.Value;
              console.log(data.Value);
              this.Tinfo = data.Value;
              debugger;
            } else {
              this.alert.error("NoDataFound");
            }
          }, err => {
            this.mainloading.PreloaderDecreaseCount();
          }, () => {
            this.mainloading.PreloaderDecreaseCount();
          })
        } else {
          this.alert.error("NoDataFound");
        }
      }, err => {
        console.log(err);
        this.mainloading.PreloaderDecreaseCount();
      }, () => {
        this.mainloading.PreloaderDecreaseCount();
      })


    }
  }

  clearSearch() {
    this.SearchCriteria.DriverIDSearchText = '';
    this.SearchCriteria.DriverMobileSearchText = '';
    this.SearchCriteria.TankerNumberSearchText = '';
  }
  isValidModel(): boolean {
    let validationMessages: string[] = [];


    if (!this.Pinfo.Availabletimefrom) {
      validationMessages.push("AvailabletimefromIsRequired");
    }

    if (!this.Pinfo.Availabletimeto) {
      validationMessages.push("AvailabletimetoIsRequired");
    }

    if (!this.Pinfo.CRnumber) {
      validationMessages.push("CRnumberIsRequired");
    }

    if (!this.Pinfo.Discerption) {
      validationMessages.push("DiscerptionIsRequired");
    }


    if (!this.Pinfo.Expirationdate) {
      validationMessages.push("ExpirationdateRequired");
    }

    if (!this.Pinfo.LastMaintenanceDate) {
      validationMessages.push("LastMaintenanceDateRequired");
    }

    if (!this.Pinfo.LastValidationDate) {
      validationMessages.push("LastValidationDateRequired");
    }
    //if (!this.Pinfo.Maramu) {
    //  validationMessages.push("MaramuRequired");
    //}
    if (!this.Pinfo.OrganizationName) {
      validationMessages.push("OrganizationNameRequired");
    }
    if (!this.Pinfo.PermitNumber) {
      validationMessages.push("PermitNumberRequired");
    }
    if (!this.Pinfo.StartDate) {
      validationMessages.push("StartDateRequired");
    }
    if (!this.Pinfo.TripsNumber) {
      validationMessages.push("TripsNumberRequired");
    }

    if (this.Pinfo.TankerCategory == "-1") {
      validationMessages.push("TankerCategoryRequired");
    }
    if (!this.Pinfo.DetectionformFileAttachments || this.Pinfo.DetectionformFileAttachments.length == 0) {
      validationMessages.push("DetectionformFileAttachmentsIsRequired");
    }
    if (!this.Pinfo.DeclarationFileAttachments || this.Pinfo.DeclarationFileAttachments.length == 0) {
      validationMessages.push("DeclarationFileAttachmentsIsRequired");
    }
    //if (!this.Pinfo.OtherFileAttachments || this.Pinfo.OtherFileAttachments.length == 0) {
    //  validationMessages.push("OtherFileAttachmentsIsRequired");
    //}

    if (validationMessages.length > 0) {
      this.alert.errorList(validationMessages);
      return false;
    }
    return true;
  }
  UpdateTanker() {
    this.mainloading.PreloaderIcreaseCount();
    this.permitservice.UpdateTanker(this.Tinfo).subscribe(response => {
      if (response.IsErrorState === true) {
        this.alert.errorList(response.Errors);
      } else {
        this.alert.showSuccess();
        this.navigateToList();
      }

    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });


  }
  UpdateDriver() {
    this.mainloading.PreloaderIcreaseCount();
    this.permitservice.UpdateDriver(this.Dinfo).subscribe(response => {
      if (response.IsErrorState === true) {
        this.alert.errorList(response.Errors);
      } else {
        this.alert.showSuccess();
        this.navigateToList();
      }

    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        this.mainloading.PreloaderDecreaseCount();
      });


  }


  save() {

    if (!this.isValidModel()) return;
    let validationMessages: string[] = [];

    this.Pinfo.DriverID = this.Dinfo.ID;
    this.Pinfo.TransporterID = this.Tinfo.Id;
    this.mainloading.PreloaderIcreaseCount();
    this.permitservice.AddPermit(this.Pinfo).subscribe(response => {
      if (response.IsErrorState === true) {
       
        if (response.ErrorMetadata == "531") {
          validationMessages.push("PermitRepeated");
          this.alert.errorList(validationMessages);
        }
        else
          this.alert.errorList(response.Errors);
      } else {
        this.alert.showSuccess();
        this.navigateToList();
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
    this.navigateToList();
  }
  navigateToList() {
    this.router.navigate(['/tms/permits/list']);
  }
}
