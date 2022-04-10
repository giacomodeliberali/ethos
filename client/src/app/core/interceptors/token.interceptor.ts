import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '@core/services/user.service';
import { Observable, of } from 'rxjs';
import { catchError, mergeMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
// Intercetta tutte le chiamate http e ci aggiunge il jwt e il token per il websocket
export class TokenInterceptor implements HttpInterceptor {
  constructor(private userSvc: UserService, private router: Router) {}
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return this.userSvc.getTokenAsObservable().pipe(
      mergeMap((token) => {
        if (token) {
          request = request.clone({
            setHeaders: {
              authorization: 'Bearer ' + token || '',
            },
          });
        }
        return next.handle(request).pipe(
          catchError((error: HttpErrorResponse) => {
            if (
              this.userSvc
                .getUser()
                .roles.includes(window.location.pathname.split('/')[1]) &&
              error.status === 401
            ) {
              this.userSvc.removeOldAuthentication();
              this.router.navigate(['auth', 'login']);
              return of(null);
            }
            throw error;
          })
        );
      })
    );
  }
}
