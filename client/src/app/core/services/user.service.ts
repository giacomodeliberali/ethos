import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto } from './ethos.generated.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private _tokenKey = 'token';
  private _userKey = 'user';

  constructor() {}

  setAuthentication(user: UserDto, token: string) {
    this.setUser(user);
    this.setToken(token);
  }

  setToken(token: string) {
    localStorage.setItem(this._tokenKey, token);
  }

  getToken() {
    return localStorage.getItem(this._tokenKey);
  }

  setUser(user: UserDto) {
    localStorage.setItem(this._userKey, JSON.stringify(user));
  }

  getUser(): UserDto {
    return JSON.parse(localStorage.getItem(this._userKey));
  }

  getTokenAsObservable() {
    return new Observable((subscriber) => {
      subscriber.next(this.getToken());
      subscriber.complete();
    });
  }
}
