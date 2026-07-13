import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { LookupService } from 'src/app/TMS-Module/Services/lookup.service';
import { Lookup } from 'src/app/TMS-Module/Models/common/lookup';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { passwordDTO } from '../../../Models/passwordDTO';
import { forgotPasswordService } from '../../../Services/forgot-password.service';
import { forkJoin } from 'rxjs';
import { ValidatorService } from '../../../Services/validator.service';

import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import 'rxjs/add/operator/filter';

@Component({
  selector: 'forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
  pagePermission: string = '';
  insertForm: FormGroup;
  Email: FormControl;

  constructor(private route: ActivatedRoute, private authServer: AuthenticationService, private fb: FormBuilder, private forgotPasswordService: forgotPasswordService, private validatorService: ValidatorService, private _alert: alertService) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('UserList');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit(): void {
    // Initialize Controls
    this.Email = new FormControl('', [Validators.required, Validators.email]);

    this.insertForm = this.fb.group({
      Email: this.Email
    });
  }

  onSubmit() {
    let userInfo = this.insertForm.value;
    this.forgotPasswordService.forgotPassword(userInfo.Email).subscribe(
      (result) => {
        if (result.IsErrorState === false) {
          this._alert.success("SavedSuccessed");
        }
        else {
          this._alert.error(result.ErrorDescription)
        }
      }
    );
  }
}
