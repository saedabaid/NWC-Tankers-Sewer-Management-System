import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { Router } from '@angular/router';

// services
import { CookieService } from 'ngx-cookie-service';
import { MenuLinkService } from '../menu/menu-link.service';
import { Configuration } from '../../configurations/shared.config';
import { TranslateService } from '@ngx-translate/core';
import { MenuLinks } from '../../Resources/menu.res';
import { StorageService, LOCAL_STORAGE } from 'ngx-webstorage-service';
import { PermissionDataModel } from '../../datamodels/permissions.data.model';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { DescriptiveResponse } from '@tms-models/common/descriptive-response';
import { SignalRService } from '../signalr/signalr.service';

@Injectable()
export class AuthenticationService {
  private str_token = 'token';
  private str_permissions = 'permissions';
  private str_logo = 'logo';
  private str_lang = 'lang';
  private str_name = 'name';
  private str_tabs = 'ts';
  private userPermissions$ = new BehaviorSubject<PermissionDataModel[]>(this.storage.get(this.str_permissions));
  isLogged = true;
  loggedIn$ = new BehaviorSubject<boolean>(this.isLogged);
  currentLanguage$ = new BehaviorSubject<string>(this.str_lang);

  subLogo: string = this.getSublogo();

  constructor(
    private http: HttpClient,
    @Inject(DOCUMENT) private document: Document,
    private CookieSrvc: CookieService,
    private menuservice: MenuLinkService,
    private router: Router,
    private signalRService: SignalRService,
    @Inject(LOCAL_STORAGE) private storage: StorageService,
  ) {}

  // changeLangage(lang: string) {}

  // changeCssFile(lang: string) {}

  private get cookieDomain(): string {
    const domain = Configuration.keys.CookieDomain;
    return domain && domain != '' ? domain : null;
  }

  //TODO: security tabs
  incrementOpenedTabs() {
    // const currentOpenedTabs = this.storage.get(this.str_tabs) || 0;
    // this.storage.set(this.str_tabs, currentOpenedTabs + 1);
  }

  decrementOpenedTabs() {
    // const currentOpenedTabs = this.storage.get(this.str_tabs) || 1;
    // if (currentOpenedTabs <= 1) {
    //   this.doJobIfThisLastTab();
    // } else {
    //   this.storage.set(this.str_tabs, currentOpenedTabs - 1);
    // }
  }

  doJobIfThisLastTab() {
    //this.clear_loaclStorage(true);
  }

  validUser() {
    const token = this.storage.get(this.str_token) || this.CookieSrvc.get(this.str_token);
    if (token && token != null) {
      return true;
    }

    const userdata = this.getCurrentUser();
    if (userdata != null) {
      // this.userName = userdata.split('*')[14];
      this._setLoggedIn(true);
      return true;
    }
    this._setLoggedIn(false);
    return false;
  }

  clear_loaclStorage(clearTabs = false) {
    const currentOpenedTabs = this.storage.get(this.str_tabs) || 0;
    this.storage.clear();
    if (!clearTabs) {
      this.storage.set(this.str_tabs, currentOpenedTabs);
    }
  }

  saveToken_localStorage(token: string) {
    if(Configuration.temporarySession) {
      this.CookieSrvc.set(this.str_token, token)
    } else {
      this.storage.set(this.str_token, token);
    }
    // this.CookieSrvc.deleteAll('/', this.cookieDomain);
  }

  getToken() {
    const token = this.storage.get(this.str_token) || this.CookieSrvc.get(this.str_token);

    return token;
    // return token && token != '' ? token : this.CookieSrvc.get('Authentication');
  }

  updateToken(newToken: string) {
    const token = this.storage.get(this.str_token);
    if (token && token != '') {
      this.saveToken_localStorage(newToken);
    } else {
      this.CookieSrvc.set(
        'Authentication',
        newToken,
        null,
        '/',
        this.cookieDomain,
      );
    }
  }

  savePermissions_localStorage(permissions: PermissionDataModel[]) {
    
    this.storage.set(this.str_permissions, permissions);
    this.userPermissions$.next(permissions);
  }

  getUserPermissions() {
    return this.userPermissions$.asObservable();
  }

  getCurrentUserPermissionByRoleName(pageUniqueName: string) {
    
    const permissions = this.storage.get(
      this.str_permissions,
      ) as PermissionDataModel[];

      if (permissions && permissions.length > 0) {
      const selected = permissions.find(
        (s) => s.PageUniqueName == pageUniqueName,
      );
      return selected ? selected.RoleName : '';
    } else if (this.CookieSrvc.get('SSOPCookie')) {
      try {
        const cooki = this.CookieSrvc.get('SSOPCookie');
        const page = cooki.split('*').find((x) => x.includes(pageUniqueName));
        if (page != null) {
          return page.split(':')[1];
        }
      } catch (err) {
        return null;
      }
    }
    return null;
  }

  checkViewPrivilege(privilege: string, logout: boolean = false): boolean {
    if (
      privilege === 'Full Control' ||
      privilege === 'Add and Edit' ||
      privilege === 'View Only'
    ) {
      return true;
    } else {
      if (logout === true) {
        this.redirectToNonAuthorizePage();
      }
      return false;
    }
  }

  checkAddEditPrivilege(privilege: string, logout: boolean = false): boolean {
    if (privilege === 'Full Control' || privilege === 'Add and Edit') {
      return true;
    } else {
      if (logout === true) {
        this.redirectToNonAuthorizePage();
      }
      return false;
    }
  }

  checkFullControlPrivilege(
    privilege: string,
    logout: boolean = false,
  ): boolean {
    if (privilege === 'Full Control') {
      return true;
    } else {
      if (logout === true) {
        this.redirectToNonAuthorizePage();
      }
      return false;
    }
  }

  saveCulture_localStorage(lang: string) {
    this.storage.set(this.str_lang, lang);
  }

  getCurrentculture(): string {
    const lang = this.storage.get(this.str_lang);
    if (lang && lang != '') {
      return lang;
    } else {
      const userdata = this.getCurrentUser();
      if (userdata) {
        const userdataarray: any[] = userdata.split('*');
        if (userdataarray.length > 3) {
          const culture = userdataarray[4].split('-');
          return culture[0];
        } else {
          return 'en';
        }
      }
    }
    return 'en';
  }

  setCulture(culture: string) {
    // culture = culture && this.getCurrentculture();
    this._setLang(culture);
    this.saveCulture_localStorage(culture);
    const htmlTag = this.document.getElementsByTagName(
      'html',
    )[0] as HTMLHtmlElement;
    htmlTag.dir = culture === 'ar' ? 'rtl' : 'ltr';
    htmlTag.lang = culture === 'ar' ? 'ar' : 'en';

    const headTag = this.document.getElementsByTagName(
      'head',
    )[0] as HTMLHeadElement;
    const existingLink = this.document.getElementById(
      'langCss',
    ) as HTMLLinkElement;
    const bundleName = culture === 'ar' ? 'arabicStyle.css' : 'englishStyle.css';
    if (existingLink) {
      existingLink.href = bundleName;
    } else {
      const newLink = this.document.createElement('link');
      newLink.rel = 'stylesheet';
      newLink.type = 'text/css';
      newLink.id = 'langCss';
      newLink.href = bundleName;
      headTag.appendChild(newLink);
    }

    // this.changeCssFile(culture)

    const token = this.storage.get(this.str_token);
    if (token && token != '') {
      this.storage.set(this.str_lang, culture);
    } else {
      const userdata = this.getCurrentUser();
      if (userdata) {
        const userdataarray: string[] = userdata.split('*');
        if (userdataarray.length > 3) {
          userdataarray[4] = culture;
          this.CookieSrvc.set(
            'SSOCookie',
            userdataarray.join('*'),
            null,
            '/',
            this.cookieDomain,
          );
        }
      }
    }
  }

  saveLogo_localStorage(path: string) {
    this.storage.set(this.str_logo, path);
  }

  getSublogo() {
      const logo = this.storage.get(this.str_logo);
      if (logo && logo != '') {
          return logo;
      } else {
          const Cookie: string = this.getCurrentUser();
          if (Cookie) {
              return Configuration.Ports.AltairPort + Cookie.split('*')[8].replace('~', '');
          } else { return ''; }
      }
  }

  saveUserFullName(fullName: string) {
    this.storage.set(this.str_name, fullName);
  }

  getCurrentuserName(): string {
    const fullName = this.storage.get(this.str_name);
    if (fullName && fullName != '') {
      return fullName;
    } else {
      const userdata = this.getCurrentUser();
      if (userdata) {
        const userdataarray: any[] = userdata.split('*');
        if (userdataarray.length > 14) {
          return userdataarray[14];
        } else {
          return '';
        }
      }
    }
    return '';
  }

  getCurrentUser() {
    
    if (this.CookieSrvc.get('SSOCookie')) {
      try {
        return this.CookieSrvc.get('SSOCookie');
      } catch (err) {
        return null;
      }
    }
    return null;
  }

  private _setLoggedIn(value: boolean) {
    // Update login status subject
    if (this.isLogged !== value) {
      this.loggedIn$.next(value);
      this.isLogged = value;
    }
  }

  private _setLang(value: string) {
    // Update language status subject
    if (this.str_lang !== value) {
      this.currentLanguage$.next(value);
      // this.str_lang = value
    }
  }

  invokeGetUserPermissionsRequest() {
    
   return this.http
      .get(`${Configuration.urls.queryEndpoint + Configuration.urls.User.GetUserPermissions}`)
      .pipe(
        map((response: DescriptiveResponse<PermissionDataModel[]>) => {
          if (!response.IsErrorState) {
            this.savePermissions_localStorage(response.Value);
            this.userPermissions$.next(response.Value);
          }
          return response;

        })
      );
  }

  login(name: string, password: string) {
    const userInfo = { name: name, password: password };


    return this.http
      .post(
        `${Configuration.urls.authenticationEndpoint}user/AuthenticateUser`,
        userInfo,
      )
      .pipe(
        map((response: any) => {
          
          if (!response.IsErrorState) {
            this.saveToken_localStorage(response.Value.Token);
            this.signalRService.connect(response.Value.Token);
            this.invokeGetUserPermissionsRequest().subscribe((res) => {
              this._setLoggedIn(!response.IsErrorState);
              this.router.navigate([`/tms`]);
              this.saveUserFullName(response.Value.Context.Account.Name);
            },
            () => {},
            () => {}
          );
          }
          return response;
        })
      );
  }

  logout() {
    
    this.removeUser();
    this.router.navigate(['/login']);
  }

  removeUser() {
    this.clear_loaclStorage();
    this.CookieSrvc.deleteAll('/', this.cookieDomain);
    this._setLoggedIn(false);
  }

  redirectToNonAuthorizePage() {
    
    this.removeUser();
    this.router.navigate(['/login']);
  }
}
