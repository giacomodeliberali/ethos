import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserDto } from '@core/services/ethos.generated.service';
import { MediaService } from '@core/services/media.service';
import { UserService } from '@core/services/user.service';
import { ModalController } from '@ionic/angular';
import { LogoutModalComponent } from '../logout-modal/logout-modal.component';
import packageJson from '../../../../../package.json';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss'],
})
export class UserCardComponent implements OnInit {
  @Input()
  user: UserDto;

  version = packageJson.version;

  constructor(
    private modalCtrl: ModalController,
    private userSvc: UserService,
    private router: Router
  ) {}

  ngOnInit() {}

  async logout() {
    const logoutModal = await this.modalCtrl.create({
      component: LogoutModalComponent,
      canDismiss: true,
      backdropDismiss: true,
      breakpoints: [0.35, 0.5],
      initialBreakpoint: 0.35,
    });
    await logoutModal.present();
    const { data } = await logoutModal.onWillDismiss();
    if (data?.logout) {
      this.userSvc.removeOldAuthentication();
      this.router.navigate(['']);
    }
  }
}
