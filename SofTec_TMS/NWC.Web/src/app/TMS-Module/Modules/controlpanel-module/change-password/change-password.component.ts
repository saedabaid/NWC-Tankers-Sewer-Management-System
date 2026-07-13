import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { passwordDTO } from '../../../Models/passwordDTO';
import { changePasswordService } from '../../../Services/change-password.service';
import { forkJoin } from 'rxjs';
import { ValidatorService } from '../../../Services/validator.service';

import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';
import { ActivatedRoute } from '@angular/router';
import { PasswordValidators } from './password.validators';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import 'rxjs/add/operator/filter';

@Component({
  selector: 'change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent {
  pagePermission: string = '';
  Id: string = '';
  updatePasswordForm: FormGroup;
  oldPassword: FormControl;
  newPassword: FormControl;
  cnewPassword: FormControl;

  constructor(private route: ActivatedRoute,private authServer: AuthenticationService, private fb: FormBuilder, private changePasswordService: changePasswordService, private validatorService: ValidatorService, private _alert: alertService)
  {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('UserList');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit(): void {
   
    this.oldPassword = new FormControl('', [Validators.required]);
    this.newPassword = new FormControl('', [Validators.required, Validators.maxLength(15), Validators.minLength(8)]);
    this.cnewPassword = new FormControl('', [Validators.required, this.validatorService.MustMatch(this.newPassword)]);
   // this.Name = this.route.snapshot.paramMap.get('Key');
    this.route.queryParams
      .subscribe(params => {
        console.log(params); // { order: "popular" }
        this.Id = params.key;
        console.log(this.Id); // popular
      });
   
    this.updatePasswordForm = this.fb.group({
      oldPassword: this.oldPassword,
      newPassword: this.newPassword,
      cnewPassword: this.cnewPassword,
      Id: this.Id
    });
  }

  onSubmit() {
    if (this.updatePasswordForm.valid) {
      let userDetails = this.updatePasswordForm.value;

      this.changePasswordService.changePassword(userDetails).subscribe(
        (result) => {
         
          if (result.IsErrorState === false) {
            this._alert.success("SavedSuccessed");
          }
          else {
            this._alert.error(result.ErrorDescription)
          }
        },
      
      );
    }
  }
}
