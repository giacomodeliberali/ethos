import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto } from './ethos.generated.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor() { }

  setAuthentication(user: UserDto, token: string){
    this.setUser(user);
    this.setToken(token);
  }

  setToken(token: string){
    localStorage.setItem('token', token);
  }

  getToken(): string{
    return localStorage.getItem('token');
  }

  setUser(user: UserDto){
    localStorage.setItem('user', JSON.stringify(user));
  }

  getUser(): UserDto{
    return JSON.parse(localStorage.getItem('user'));
  }

  getTokenAsObservable(){
    return new Observable(subscriber => {
      subscriber.next(this.getToken());
      subscriber.complete();
    });
  }
}
