import {
  Component,
  ViewEncapsulation,
  OnInit,
  ChangeDetectorRef,
  ChangeDetectionStrategy,
  Inject,
  OnDestroy,
  HostListener,
} from '@angular/core'

// Services
import { AuthorizationService } from './shared/Services/authorization/authorization.service'
import { AuthenticationService } from './shared/Services/authentication/authentication.service'
import { TranslateService } from '@ngx-translate/core'
// modules
// data Model
import { Subscription } from 'rxjs'
import { PermissionDataModel } from './shared/datamodels/permissions.data.model'
import { MenuLinkService } from './shared/Services/menu/menu-link.service'
import { LoaderService } from './shared/loader.service'
import { Router } from '@angular/router'
import { DOCUMENT } from '@angular/common'
import { SignalRService } from './shared/Services/signalr/signalr.service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: [
    '../assets/fmsBranding/styles/sass/rtl.scss',
    './app.component.scss',
  ],
  encapsulation: ViewEncapsulation.None,
})
export class AppComponent implements OnInit, OnDestroy {
  @HostListener('window:beforeunload', ['$event']) onBeforeUnload(event) {
    this.AuthenticationService.decrementOpenedTabs();
  }
  displayLoader = false
  displayLoaderInSub: Subscription
  menuState = false
  submenuState: boolean
  Permissions: PermissionDataModel[] = []
  isLogged = false
  currentculture = 'ar' //Default language


  constructor(
    private router: Router,
    private AuthenticationService: AuthenticationService,
    private AuthorizationService: AuthorizationService,
    public menuservice: MenuLinkService,
    private cd: ChangeDetectorRef,
    private loaderService: LoaderService,
    public translateService: TranslateService,
    public signalRService: SignalRService,
  ) {
    AuthenticationService.incrementOpenedTabs()
    translateService.addLangs(['en', 'ar'])
    translateService.setDefaultLang(this.currentculture)
    const browserLang = translateService.getBrowserLang()
    this.currentculture = this.AuthenticationService.getCurrentculture()
    this.AuthenticationService.setCulture(this.currentculture)
    translateService.use(this.currentculture);
    this.AuthenticationService.loggedIn$.subscribe(
      (res) => (this.isLogged = res)
    );
    signalRService.connected.subscribe((isConnected) => {
      if (isConnected) {
        signalRService.connection.invoke("Ping", "Messageee");
        signalRService.connection.on("Ping", (x) => console.log(x));
      }
    });
  }


  subscriber() {
    this.displayLoaderInSub = this.loaderService.loaderCount$.subscribe(
      (displayLoader) => {
        if (displayLoader == 0) {
          if (this.displayLoader != false) {
            this.displayLoader = false
            this.cd.detectChanges()
          }
        } else {
          if (this.displayLoader != true) {
            this.displayLoader = true
            this.cd.detectChanges()
          }
        }
      },
    )
  }

  ngOnInit(): void {

    this.AuthenticationService.currentLanguage$.subscribe(
      (res) => (this.currentculture = res)
    )
    // this._hubConnection = new signalR.HubConnectionBuilder()
    //   .withUrl('http://localhost:42068/notify')
    //   // .configureLogging(signalR.LogLevel.Information)
    //   .build();

    //  this._hubConnection.start().catch(err => console.error(err.toString()));

    // this._hubConnection.on('BroadcastMessage', (type: any , PayLoad:any) => {
    //   const received = `Received: ${type + ' ' + PayLoad}`;
    //   console.log(received);
    // });

    this.subscriber()
    // Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    // Add 'implements OnInit' to the class.
    // 


    if (this.AuthenticationService.validUser()) {
      
      const sso: string = this.AuthenticationService.getCurrentUser()

      if (sso) {
        const UserId = sso.split('*')[0]
        const SubId = sso.split('*')[1]
        this.AuthorizationService.getPermissions(UserId, SubId).subscribe(
          (res) => {
            this.Permissions = res
          },
          (err) => {
            console.log(err)
          },
        )
      }
    } else {
      
      this.router.navigate(['/login'])
    }
  }
  toggleMenu() {
    this.menuState = !this.menuState
    if (!this.menuState) {
      this.submenuState = false
    }
  }
  hideMenu() {
    this.menuState = false
    this.submenuState = false
  }
  toggleSubmenu($event) {
    this.submenuState = $event
  }

  ngOnDestroy() {
    this.displayLoaderInSub.unsubscribe()
  }
}
