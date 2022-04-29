import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserDto } from './ethos.generated.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private tokenKey = 'token';
  private userKey = 'user';

  private currentUser$ = new BehaviorSubject<UserDto | null>(
    JSON.parse(localStorage.getItem(this.userKey))
  );

  private token$ = new BehaviorSubject<string | null>(
    localStorage.getItem(this.tokenKey)
  );

  setAuthentication(user: UserDto, token: string) {
    this.setUser(user);
    this.setToken(token);
  }

  removeOldAuthentication() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.currentUser$.next(null);
    this.token$.next(null);
  }

  setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
    this.token$.next(token);
  }

  getToken() {
    return this.token$.value;
  }

  setUser(user: UserDto) {
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.currentUser$.next(user);
  }

  getUser(): UserDto | null {
    return this.currentUser$.value;
  }

  getTokenAsObservable() {
    return this.token$.asObservable();
  }

  isAuthenticated$() {
    return this.currentUser$.asObservable().pipe(map((user) => !!user));
  }
}
