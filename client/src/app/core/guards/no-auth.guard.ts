import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { UserService } from '@core/services/user.service';

@Injectable()
export class NoAuthGuard implements CanActivate {
  constructor(public userService: UserService, public router: Router) {}

  canActivate(): boolean {
    const user = this.userService.getUser();
    if (user) {
      this.router.navigate([user.roles[0]]);
      return false;
    }
    return true;
  }
}
