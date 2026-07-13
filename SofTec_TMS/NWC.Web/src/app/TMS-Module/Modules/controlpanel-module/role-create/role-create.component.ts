import { Component, OnInit } from '@angular/core';
import { Lookup } from '@tms-models/common/lookup';
import { SearchStream } from '@tms-models/common/search-stream-object.model';
import { RoleModel, MyRoleModel } from '@tms-models/role-model';
import { LookupService } from '@tms-services/lookup.service';
import { StaffService } from '../../../Services/staff-service';
import { PagesDTO } from '../../../Models/PagesDTO';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { FormGroup, FormBuilder, Validators, FormControl, FormArray, NgForm } from '@angular/forms';
import { of } from 'rxjs';
import { Router, ActivatedRoute } from "@angular/router";
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'app-role-create',
  templateUrl: './role-create.component.html',
  styleUrls: ['./role-create.component.scss']
})


export class RoleCreateComponent implements OnInit {
  SearchStream: SearchStream = new SearchStream();
  Customer_Loading = false;
  roleModel: RoleModel;
  PagesDTO: PagesDTO[];
  MyRoleModel: MyRoleModel[];
  form: FormArray;
  radioSelected: string;
  param: string;
  
  staffRoleCategoryList: Lookup<number>[] = [];

  pagesList: Lookup<number>[] = [];
  selectMenuOptions = {
    enableSearchFilter: false,
  };
  selectMenuOptions2 = {
    enableSearchFilter: false,
    singleSelect: true
  };
  StaffSelectedCategory: Lookup<number>[] = [];
  StaffSelectedRoleName: Lookup<string>[] = [];
  selectMenuOptions3 = {
    enableSearchFilter: false,
    singleSelect: true
  };
  constructor(
    private lookupservice: LookupService, private staffService: StaffService,
    private _alert: alertService, private fb: FormBuilder, private route: ActivatedRoute, private mainloading: LoaderService
  ) {
    this.roleModel = new RoleModel();

  }
  SelectedAreas: Lookup<string>[] = [];
  SelectedPages: Lookup<string>[] = [];

  player = [
    
  ];
  ngOnInit() {
    this.form = this.fb.array([]);
    this.route.queryParams
      .filter(params => params.key)
      .subscribe(params => {
        console.log(params); 
        this.param = params.key;
      });
    this.getStaffRoleCategory('');
    this.getPagesList("");
    this.GetPages();

    this.GetRoles();

    console.log(this.player);
    this.GetStaffSelectedCategory();
    this.GetStaffSelectedRoleName();
    this.GetStaffDefaultPage();
  }


  GetStaffDefaultPage() {
    let param = this.param;
    this.lookupservice.GetStaffDefaultPage(param).subscribe(res => {
      if (res.Value)
        this.SelectedPages = res.Value;

    });
  }
  GetStaffSelectedRoleName() {
    let param = this.param;
    this.lookupservice.GetStaffSelectedRoleName(param).subscribe(res => {
      if (res.Value)
        this.roleModel.roleName = res.Value;
        this.roleModel.descr  = res.Value;

    });
  }
  GetStaffSelectedCategory() {
    let param = this.param;
    this.lookupservice.GetStaffSelectedCategory(param).subscribe(res => {
      if (res.Value)
        this.StaffSelectedCategory = res.Value;
    });
  }

  getStaffRoleCategory(searchKeyword: string) {
    this.SearchStream.initStream('', (a) => {
      this.Customer_Loading = true;
      this.lookupservice.getStaffRoleCategory().subscribe(res => {
        if (res.Value) {
          this.staffRoleCategoryList = res.Value;
          debugger;

        }
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
    }).next(searchKeyword);
  }

  onStaffRoleCategoryDDLChanged(evt) {
    this.roleModel.staffRoleCategoryID = evt.map(m => m.Id)[0];
  }
  GetRoles() {
    this.staffService.GetAllRoles().subscribe((res) => {
      if (!res.IsErrorState) {
        this.MyRoleModel = res.Value;
        debugger;
      //  this._alert.success("lockUser");
      } else {
        this._alert.errorList(res.Errors);
      }
    });

  }
  onSubmit() {
    if (this.form.valid) {
      let userDetails = this.form.value;
      let param = this.param;
      let roleModel = this.roleModel;
      this.mainloading.PreloaderIcreaseCount();

      this.staffService.changePermission(userDetails, roleModel, param).subscribe(
        (result) => {
          if (result.IsErrorState === false) {
            this._alert.success("SavedSuccessed");
          }
          else {
            this._alert.error(result.ErrorDescription)
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

  GetPages() {
    let param2 = this.param;
    this.staffService.getPages(param2).subscribe((res) => {
     
      var i = 0;
      if (!res.IsErrorState) {
        this.PagesDTO = res.Value;
        this.PagesDTO.map((nestedObject) => {
          i = 0;
         
          if (typeof nestedObject.Pages != 'undefined') {
            nestedObject.Pages.map((x) => {
              i++;
             
              this.player.push({ ModuleId: nestedObject.ModuleID, ModuleName: nestedObject.Name, ID: x.ID, Name: x.Name, status: this.MyRoleModel, exist: x.exist });
              if (i == 1)
              this.form.push(
                this.addFormGroup(nestedObject.ModuleID, nestedObject.Name, x.ID, x.Name, x.status, x.exist)
                );
              else
                this.form.push(
                  this.addFormGroup(nestedObject.ModuleID, '', x.ID, x.Name, x.status, x.exist)
                );
            });

          }
        })

      } else {
        this._alert.error(res.ErrorDescription);
      }
    });

    console.log(this.player.length);
  }
  private addFormGroup(ModuleID?: string, ModuleName?: string, pageid?: string, Name?: string, status?: string, exist?: boolean): FormGroup {
    return this.fb.group({
      ModuleID,
      ModuleName,
      pageid,
      Name,
      status,
      exist
    });
  }
  getPagesList(e) {
      this.Customer_Loading = true;
      this.lookupservice.getPagesList().subscribe(res => {
        if (res.Value) {
        
          this.pagesList = res.Value;
        }
      }
        , err => {
          this.Customer_Loading = false;
        }
        , () => {
          this.Customer_Loading = false;
        });
  }

  onPagesListDDLChanged(evt) {
    this.roleModel.DefaultPageId = evt.map(m => m.Id)[0];
  }



  save() {
    console.log('save');
  }

}
