import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MediaService } from "@core/services/media.service";
import { ModalController } from "@ionic/angular";
import { loginToRegister } from "../../animations/login-page.animations";
import { ForgotPasswordModalComponent } from "../../components/forgot-password-modal/forgot-password-modal.component";

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  animations: [loginToRegister]
})
export class LoginPageComponent {

  currentForm: "login" | "register" = "login";

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl('')
  })

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    name: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$')])
  })

  constructor(private modalCtrl: ModalController, private mediaSvc: MediaService) {
  }

  changeForm(event: CustomEvent){
    this.currentForm = event.detail.value;
  }

  async openForgotPasswordModal(event: MouseEvent){
    event.stopPropagation();
    const modal = await this.modalCtrl.create({
      component: ForgotPasswordModalComponent,
      cssClass: this.mediaSvc.isSmartphone ? 'bottom' : '',
      swipeToClose: true,
      mode: "ios"
    });
    await modal.present();
    const { data } = await modal.onWillDismiss();
    if(data?.email){
      // Call reset password service
    }
  }
}
