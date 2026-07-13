import { Component, EventEmitter, Output, Input, OnInit, ChangeDetectorRef } from '@angular/core';
// import { TranslateService } from '../shared/Services/translate/translate.service';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '../shared/Services/authentication/authentication.service';
import { Configuration } from '../shared/configurations/shared.config';
import { MenuLinkService } from '../shared/Services/menu/menu-link.service';
import { Router } from '@angular/router';


@Component({
  selector: 'main-header',
  templateUrl: './header.component.html'
})

export class AppHeader implements OnInit {
  logoSrc: any = Configuration.Logo.src;
  userName = '';
  userImage = '';
  @Output() menuStatus = new EventEmitter<Boolean>();
  @Output() hideMenustate = new EventEmitter<Boolean>();
  menuState = false;

  constructor(
    private translate: TranslateService,
    private auth: AuthenticationService,
    private _cd: ChangeDetectorRef,
    private router: Router,
    private menuLink: MenuLinkService) {

  }

  ngOnInit() {
    this.userName = this.auth.getCurrentuserName();
    this.userImage = this.auth.getSublogo();
  }
  toggleMenu() {
    this.menuStatus.emit(!this.menuState);
  }

  hideMenu() {
    this.hideMenustate.emit(false);
  }

  changelang(key: string) {
    this.auth.setCulture(key);
    this.translate.use(key);
    this._cd.markForCheck();
  }

  logout() {
    this.auth.logout();
  }

  redirectToControlPanel() {
    this.router.navigate(['/tms/controlpanel']);
   
  }

  onMyAccountClicked() {
    this.router.navigate(['/tms/controlpanel/my-account']);
  }

}
