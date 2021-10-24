import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { UserService } from '@core/services/user.service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(public userService: UserService, public router: Router) {}

  canActivate(): boolean {
    const token = this.userService.getToken();
    if (!token) {
      this.router.navigate(['auth']);
      return false;
    }
    return true;
  }
}
