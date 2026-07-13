import { Component, OnInit } from '@angular/core';
import { Customer } from 'src/app/TMS-Module/Models/customer.model';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { TranslateService } from '@ngx-translate/core';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { LoaderService } from 'src/app/shared/loader.service';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { CustomerAccount } from 'src/app/TMS-Module/Models/customer-account.model';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { CustomerService } from 'src/app/TMS-Module/Services/customer.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { CustomerSC } from 'src/app/TMS-Module/Models/search-criteria/customer-sc.model';
import { CustomerAccountSC } from 'src/app/TMS-Module/Models/search-criteria/customer-account-SC.model';

@Component({
  selector: 'app-new-customer',
  templateUrl: './new-customer.component.html',
  styleUrls: ['./new-customer.component.scss']
})
export class NewCustomerComponent implements OnInit {

  pagePermission: string;
  updateId: number;
  editMode = false;
  IsArabic = false;

  advancedDiv = false;
  customer = new Customer();
  newlocation = new CustomerAccount();

  constructor(
    private authenticationService: AuthenticationService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private customerService: CustomerService,
    private _alert: alertService,
    private mainloading: LoaderService,
    private titleService: Title,
    private router: Router,

  ) {
    this.pagePermission = this.authenticationService.getCurrentUserPermissionByRoleName('tmsCustomers');
    this.authenticationService.checkAddEditPrivilege(this.pagePermission, true);

    let urlList = this.router.routerState.snapshot.url.split('/');
    if (urlList[3] === "edit") {
      this.updateId = +urlList[4];
      this.editMode = true;
    }
    else {
      this.updateId = null;
      this.editMode = false;
    }

  }

  ngOnInit() {

    this.setDefaultContract();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe(res => {
      this.loadDDLsGV();
    });

  }

  loadDDLsGV() {
    this.IsArabic = (this._translate.currentLang == 'ar');


    this.titleService.setTitle(this._translate.instant(this.updateId ? 'EditCustomer' : 'NewCustomer'));

    // load Search DDls values
    this.getPersonalIDTypes();
    this.getClass();
    this.getServiceTypes();
    //this.getZones('');

  }

  setDefaultContract() {

    if (!isNullOrUndefined(this.updateId) && this.updateId > 0) {
      let filters = new CustomerSC;
      filters.Id = this.updateId;
      filters.PageFilter.PageSize = 1;
      filters.PageFilter.PageIndex = 1;

      this.mainloading.PreloaderIcreaseCount();
      this.customerService.SearchCustomerList(filters).subscribe(res => {
        if (res.Value && res.Value.Result && res.Value.Result.length > 0) {
          this.customer = res.Value.Result[0];

          if (this.customer && this.customer.IDTypeID && +this.customer.IDTypeID > 0) {
            let selectedInvoice = this.PersonalIDTypes.find(s => s.Id == this.customer.IDTypeID);
            if (!isNullOrUndefined(selectedInvoice)) {
              this.bindingModel_PersonalIdTypes.push(selectedInvoice);
            }
          }

          //Accounts
          let accountFilters = new CustomerAccountSC();
          accountFilters.CustomerId = this.updateId;

          this.mainloading.PreloaderIcreaseCount();
          this.customerService.SearchCustomerAccountList(accountFilters).subscribe(res => {
            if (!res.IsErrorState && !isNullOrUndefined(res.Value) && res.Value.Result) {
              this.customer.customerAccounts = res.Value.Result;
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
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

    }
    else {
      this.customer = new Customer();
    }
  }

  ngOnDestroy(): void {
    this.SearchStream.DestroyStreams();
  }

  //#region  "Lookups"
  PersonalIDTypes: Lookup<number>[] = [];
  zoneList: Lookup<number>[] = [];
  customerClassList: Lookup<number>[] = [];
  serviceTypeList: Lookup<number>[] = [];

  bindingModel_PersonalIdTypes: Lookup<number>[] = [];
  bindingModel_Zones: Lookup<number>[] = [];
  bindingModel_Classes: Lookup<number>[] = [];
  bindingModel_ServiceTypes: Lookup<number>[] = [];

  Zone_Loading = false;
  SearchStream: SearchStream = new SearchStream();

  selectMenuOptions = {
    singleSelect: true
  };

  selectMenuOptionsClass = {
    singleSelect: true,
    text: "Class"
  };

  selectMenuOptionsServiceType = {
    singleSelect: true,
    text: "ServiceType"
  };

  selectMenuOptionsZone = {
    enableSearchFilter: true,
    singleSelect: true,
    text: "Zone"
  };


  getPersonalIDTypes() {
    this.lookupservice.GetPersonalIDTypes().subscribe(res => {
      if (res.Value) {
        this.PersonalIDTypes = res.Value;

        // on Edit customer
        if (this.updateId && +this.updateId > 0 && this.customer && this.customer.IDTypeID && +this.customer.IDTypeID > 0) {
          let selectedInvoice = this.PersonalIDTypes.find(s => s.Id == this.customer.IDTypeID);
          if (selectedInvoice) {
            this.bindingModel_PersonalIdTypes.push(selectedInvoice);
          }
        }

      }
    });
  }

  getClass() {
    this.lookupservice.getCustomerClasses().subscribe(res => {
      if (res.Value)
        this.customerClassList = res.Value;
    });
  }

  getServiceTypes() {
    this.lookupservice.getPermittedServicesTypes().subscribe(res => {
      if (res.Value)
        this.serviceTypeList = res.Value;
    });
  }

  getZones(searchKeyword: string) {
    this.SearchStream.initStream("ZoneDDL_addCustomer", (a) => {
      this.Zone_Loading = true;
      this.lookupservice.SearchZonesBasedOnAssignedStations(a).subscribe(res => {
        if (res.Value) {
          this.zoneList = res.Value;

          // on Edit Account | location
          if (this.updateId && +this.updateId > 0 && this.customer
            && this.newlocation && this.newlocation.CL_ZoneID) {
            let selectedInvoice = this.zoneList.find(s => s.Id == this.newlocation.CL_ZoneID);
            if (selectedInvoice) {
              this.bindingModel_Zones.push(selectedInvoice);
            }
          }

        }
      }
        , err => {
          this.Zone_Loading = false;
        }
        , () => {
          this.Zone_Loading = false;
        });
    }).next(searchKeyword);
  }


  onPersonalIDTypeChanged(evt) {
    this.customer.IDTypeID = evt.map(m => m.Id)[0];
  }

  onClassDDLChanged(evt) {
    this.newlocation.CL_ClassID = evt.map(m => m.Id)[0];

    this.newlocation.CL_ClassAr = evt.map(m => m.Name)[0];
    this.newlocation.CL_ClassEn = evt.map(m => m.Name)[0];
  }

  onServiceTypeDDLChanged(evt) {
    this.newlocation.ServiceTypeId = evt.map(m => m.Id)[0];

    this.newlocation.ServiceTypeAr = evt.map(m => m.Name)[0];
    this.newlocation.ServiceTypeEn = evt.map(m => m.Name)[0];
  }

  onZonesDDLChanged(evt) {
    this.newlocation.CL_ZoneID = evt.map(m => m.Id)[0];
    this.newlocation.CL_ZoneName = evt.map(m => m.Name)[0];
  }

  //#endregion  "Lookups"


  filterDeleted(files: CustomerAccount[]) {
    return files.filter(x => !x.IsDeleted);
  }

  deleteLocation(account: CustomerAccount) {
    if (account.ID) {
      account.IsDeleted = true;
    }
    else {
      let index = this.customer.customerAccounts.indexOf(account);
      this.customer.customerAccounts.splice(index, 1);
    }

  }

  editLocation(account: CustomerAccount) {
    this.newlocation.ID = account.ID;
    this.newlocation.CL_Address = account.CL_Address;
    this.newlocation.AccountId_Integration = account.AccountId_Integration;

    this.newlocation.CL_ClassID = account.CL_ClassID;
    this.newlocation.CL_ClassAr = account.CL_ClassAr;
    this.newlocation.CL_ClassEn = account.CL_ClassEn;
    this.bindingModel_Classes = this.customerClassList.filter(a => a.Id == this.newlocation.CL_ClassID);

    this.newlocation.ServiceTypeId = account.ServiceTypeId;
    this.newlocation.ServiceTypeAr = account.ServiceTypeAr;
    this.newlocation.ServiceTypeEn = account.ServiceTypeEn;
    this.bindingModel_ServiceTypes = this.serviceTypeList.filter(a => a.Id == this.newlocation.ServiceTypeId);

    this.newlocation.CL_ZoneID = account.CL_ZoneID;
    this.newlocation.CL_ZoneName = account.CL_ZoneName;
    this.getZones(account.CL_ZoneName);
  }

  clearNewLoction() {
    this.newlocation = new CustomerAccount();
    this.bindingModel_Zones = [];
    this.bindingModel_Classes = [];
    this.bindingModel_ServiceTypes = [];
  }

  isValidLocation(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.newlocation.CL_Address) || this.newlocation.CL_Address == '') {
      validationMessages.push("AddressRequired");
    }
    if (isNullOrUndefined(this.newlocation.CL_ZoneID) || +this.newlocation.CL_ZoneID < 1) {
      validationMessages.push("ZoneIdRequired");
    }
    if (isNullOrUndefined(this.newlocation.CL_ClassID) || this.newlocation.CL_ClassID < 1) {
      validationMessages.push("CustomerIdRequired");
    }
    if (isNullOrUndefined(this.newlocation.ServiceTypeId) || this.newlocation.ServiceTypeId < 1) {
      validationMessages.push("ChooseServiceType");
    }
    if (isNullOrUndefined(this.newlocation.AccountId_Integration) || this.newlocation.AccountId_Integration == '') {
      validationMessages.push("ChooseCustomerAccount");
    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  addLocation() {

    if (!this.isValidLocation()) return;

    if (!this.newlocation.ID) { //add

      this.newlocation.CL_Code = ' ';
      this.newlocation.CL_CategoryID = 1;
      this.newlocation.CL_PriorityID = 5;
      this.newlocation.CL_StatusID = 2;

      this.customer.customerAccounts.push(this.newlocation);
    }
    else { //edit

      let myAccount = this.customer.customerAccounts.find(s => s.ID == this.newlocation.ID);

      myAccount.CL_Address = this.newlocation.CL_Address;
      myAccount.AccountId_Integration = this.newlocation.AccountId_Integration;
      myAccount.CL_ClassID = this.newlocation.CL_ClassID;
      myAccount.ServiceTypeId = this.newlocation.ServiceTypeId;
      myAccount.CL_ZoneID = this.newlocation.CL_ZoneID;

      myAccount.CL_ClassAr = this.newlocation.CL_ClassAr;
      myAccount.CL_ClassEn = this.newlocation.CL_ClassEn;
      myAccount.ServiceTypeAr = this.newlocation.ServiceTypeAr;
      myAccount.ServiceTypeEn = this.newlocation.ServiceTypeEn;
      myAccount.CL_ZoneName = this.newlocation.CL_ZoneName;

    }

    this.clearNewLoction();
  }


  isValidModel(): boolean {
    let validationMessages: string[] = [];

    if (isNullOrUndefined(this.customer.FullName) || this.customer.FullName == '') {
      validationMessages.push("FullNameRequired");
    }
    if (isNullOrUndefined(this.customer.IDTypeID) || +this.customer.IDTypeID < 1) {
      validationMessages.push("IDTypeIDRequired");
    }
    if (isNullOrUndefined(this.customer.IDNumber) || this.customer.IDNumber == '') {
      validationMessages.push("IDNumberRequired");
    }
    // if (isNullOrUndefined(this.customer.Mobile) || this.customer.Mobile == '') {
    //   validationMessages.push("MobileRequired");
    // }

    let exMobile = new RegExp(Configuration.RegExp.ValidMobileNumber);
    if (this.customer.Mobile && !exMobile.test(this.customer.Mobile)) {
      validationMessages.push("InvalidMobileNumber");
    }


    if (isNullOrUndefined(this.customer.customerAccounts) || this.customer.customerAccounts.length < 1) {
      validationMessages.push("AtLeastOneLocationIsRequired");
    }

    if (validationMessages.length > 0) {
      this._alert.errorList(validationMessages);
      return false;
    }
    return true;
  }

  save() {

    if (!this.isValidModel()) return;

    this.customer.Code = ' ';
    this.customer.IntegrationId = 'TMS';
    this.customer.Email = '';


    if (!this.customer.ID) { //add
      this.mainloading.PreloaderIcreaseCount();
      this.customerService.CreateCustomerAndLocations(this.customer).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          this.navigateOrdersList();
        }

      }
        , err => {
          this.mainloading.PreloaderDecreaseCount();
        }
        , () => {
          this.mainloading.PreloaderDecreaseCount();
        });

    } else { //update

      this.mainloading.PreloaderIcreaseCount();
      this.customerService.EditCustomerAndLocations(this.customer).subscribe(response => {
        if (response.IsErrorState === true) {
          this._alert.errorList(response.Errors);
        } else {
          this._alert.showSuccess();
          this.navigateOrdersList();
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


  cancel() {
    this._alert.confirmationMessage("ConfirmClose").subscribe(confirm => {
      if (confirm === true) {
        this.navigateOrdersList();
      }
    });
  }

  navigateOrdersList() {
    //this.router.navigate(['/tms/order/orderlist']);
    this.router.navigate(['/tms/customer/customerlist']);
  }


}
