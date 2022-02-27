import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserDto } from '@core/services/ethos.generated.service';
import { UserService } from '@core/services/user.service';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.page.html',
  styleUrls: ['./user-settings.page.scss'],
})
export class UserSettingsPage implements OnInit {
  currentUser: UserDto;
  constructor(private router: Router, private userSvc: UserService) {}

  ngOnInit() {
    this.currentUser = this.userSvc.getUser();
  }

  goToSchedulePage() {
    this.router.navigate(['user']);
  }
}
