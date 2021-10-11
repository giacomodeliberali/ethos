import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserService } from '@core/services/user.service';
import { Observable } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
// Intercetta tutte le chiamate http e ci aggiunge il jwt e il token per il websocket
export class TokenInterceptor implements HttpInterceptor {
  constructor(public userSvc: UserService) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.userSvc.getTokenAsObservable().pipe(
      mergeMap((token) => {
        if (token) {
          request = request.clone({
            setHeaders: {
              authorization: 'Bearer ' + token || ''
            },
          });
        }
        return next.handle(request);
      }),
    );
  }
}
