import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
// rxjs
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

// services
import { AuthenticationService } from '../Services/authentication/authentication.service';
import { LoaderService } from '../loader.service';
import { alertService } from '../Services/alert/alert.service';

@Injectable()
export class AuthHttpInterceptor implements HttpInterceptor {
    constructor(
        private Auth: AuthenticationService,
        private loader: LoaderService,
        private alert: alertService) { }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // debugger;
        // if (this.Auth.validUser()) {
        const token = this.Auth.getToken();
        if (token) {
            req = req.clone({
                headers: req.headers.append('Authorization', token)
                    .append('token', token)
                    .append('lang', this.Auth.getCurrentculture())

            });
        }
        // }

        // this.loader.PreloaderIcreaseCount();
        return next.handle(req).pipe(
            map((event: HttpEvent<any>) => {
                if (event instanceof HttpResponse) {
                    const token = event.headers.get('Authorization');
                    if (token) {
                        this.Auth.updateToken(token);
                    }
                    //           this.loader.PreloaderDecreaseCount();
                }
                return event;
            }),
            catchError(x => this.handleAuthError(x)
            ));
    }

    private handleAuthError(err: HttpErrorResponse): Observable<any> {
        // this.loader.PreloaderDecreaseCount();
        // handle your auth error or rethrow
        if (err.status === 401 || err.status === 403) {
            // navigate /delete cookies or whatever
            this.Auth.logout();
            // if you've caught / handled the error, you don't want to rethrow it unless you also want downstream consumers to have to handle it as well.
            return of(err.message);
        } else {
            this.alert.error('NetworkError');
        }
        return of(err);
        // return throwError(err);
    }
}
