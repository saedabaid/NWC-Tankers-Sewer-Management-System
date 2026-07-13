import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

//services
import { AuthenticationService } from '../Services/authentication/authentication.service';
import { MenuLinkService } from '../Services/menu/menu-link.service';


@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router,private menuservice: MenuLinkService, private auth: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {

        // if the user is not logged in return
        if (!this.auth.validUser()) {
            return false;
        }
        else {
            return true;
        }
    }
}
