import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import 'rxjs/add/operator/filter';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { changePasswordService } from '../../../Services/change-password.service';
import { UserService } from "../../../Services/user.service";
import { ValidatorService } from '../../../Services/validator.service';
import { UserProfile } from '../../../Models/profile.model';
import { Router, ActivatedRoute } from "@angular/router";


@Component({
  selector: 'my-account',
  templateUrl: './my-account.component.html',
  styleUrls: ['./my-account.component.scss']
})
export class MyAccountComponent {
  pagePermission: string = '';
  Name: string = '';
  userProfile: UserProfile;
  UserID: string;
  param: string;

  constructor(private route: ActivatedRoute, private _route: Router, private authServer: AuthenticationService, private fb: FormBuilder, private changePasswordService: changePasswordService, private validatorService: ValidatorService, private _alert: alertService, private userService: UserService
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('UserList');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit(): void {
    this.route.queryParams
      .subscribe(params => {
        console.log(params); // { order: "popular" }

        this.param = params.key;
        console.log(this.param); // popular
      });

    this.load();
  }
  load() {
    this.userService.GetAccountProfile().subscribe((res) => {
      if (!res.IsErrorState) {
        if (res.Value != null) {
          this.userProfile = res.Value;
        }
        else {
          this.userProfile = null;
        }
      }
    });

  }
  onSubmit() {

  }
  customRoute(routeTo: string, value: string) {
    if (routeTo && routeTo !== "") {
      const targetLink = `/tms/${routeTo}`;
      const url = this._route.serializeUrl(
        this._route.createUrlTree([targetLink])
      );
      console.log(routeTo, url);
      this._route.navigate([url], { queryParams: { key: value } });
      // window.open(url, '_blank');
    }
  }

}
