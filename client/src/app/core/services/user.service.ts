import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto } from './ethos.generated.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private tokenKey = 'token';
  private userKey = 'user';

  constructor() {}

  setAuthentication(user: UserDto, token: string) {
    this.setUser(user);
    this.setToken(token);
  }

  setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
  }

  getToken() {
    return localStorage.getItem(this.tokenKey);
  }

  setUser(user: UserDto) {
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }

  getUser(): UserDto {
    return JSON.parse(localStorage.getItem(this.userKey));
  }

  getTokenAsObservable() {
    return new Observable((subscriber) => {
      subscriber.next(this.getToken());
      subscriber.complete();
    });
  }
}
