import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '../../Services/authentication/authentication.service';
import { Configuration } from '../../../shared/configurations/shared.config';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  logoSrc: any = Configuration.Logo.src;
  isLogged: boolean;
  username;
  password;
  loadingIcon = false;
  errorObj = {
    ErrorMetadata: 0,
    Message: ''
  };

  constructor(
    private _translate: TranslateService,
    private router: Router,
    private authenticationService: AuthenticationService) {
    this.authenticationService.loggedIn$.subscribe(res => {
      this.isLogged = res;
      if (this.isLogged) {
        this.router.navigate(['/tms/defaultpage']);
      }
    });
  }



  ngOnInit() {
    console.log(this.isLogged);
  }

  login() {
    //debugger;
    this.loadingIcon = true;
    this.authenticationService.login(this.username, this.password)
      .subscribe(
        res => {
         
        if (res.IsErrorState && res.IsErrorState != undefined) {
          this.loadingIcon = false;
          this.errorObj = {
            ErrorMetadata: 3,
            Message: this._translate.instant('wrongUsernameOrPassword')
          };
        }      
      }, (err => {
        this.loadingIcon = false;
        console.log('rr', err)
      }));      
  }
}
