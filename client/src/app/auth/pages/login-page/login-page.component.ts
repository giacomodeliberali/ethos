import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseDirective } from '@core/directives';
import {
  IdentityService,
  LoginRequestDto,
  RegisterRequestDto,
} from '@core/services/ethos.generated.service';
import { MediaService } from '@core/services/media.service';
import { UserService } from '@core/services/user.service';
import { ModalController } from '@ionic/angular';
import { LoadingService } from '@shared/services/loading.service';
import { ToastService } from '@shared/services/toast.service';
import { loginToRegister } from '../../animations/login-page.animations';
import { ForgotPasswordModalComponent } from '../../components/forgot-password-modal/forgot-password-modal.component';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  animations: [loginToRegister],
})
export class LoginPageComponent extends BaseDirective {
  currentForm: 'login' | 'register' = 'login';

  loginForm = new FormGroup({
    userNameOrEmail: new FormControl(''),
    password: new FormControl(''),
  });

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    userName: new FormControl('', [Validators.required]),
    fullName: new FormControl('', [Validators.required]),
    password: new FormControl('', [
      Validators.required,
      Validators.pattern(
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$/
      ),
    ]),
  });

  constructor(
    private modalCtrl: ModalController,
    private identityService: IdentityService,
    private userSvc: UserService,
    private loadingSvc: LoadingService,
    private router: Router,
    private toastSvc: ToastService
  ) {
    super();
  }

  changeForm(event: CustomEvent) {
    this.currentForm = event.detail.value;
  }

  async openForgotPasswordModal(event: MouseEvent) {
    event.stopPropagation();
    const modal = await this.modalCtrl.create({
      component: ForgotPasswordModalComponent,
      canDismiss: true,
      backdropDismiss: true,
      breakpoints: [0.25, 0.5],
      initialBreakpoint: 0.25,
    });
    await modal.present();
    const { data } = await modal.onWillDismiss();
    if (data?.email) {
      this.sendForgotPasswordEmail(data.email);
    }
  }

  submitForm() {
    if (this.currentForm === 'login') {
      this.login();
    } else {
      this.register();
    }
  }

  private login() {
    if (this.loginForm.valid) {
      const loginValue: LoginRequestDto = this.loginForm.value;
      this.loadingSvc
        .startLoading(
          this,
          'LOGIN',
          this.identityService.authenticate(loginValue),
          {
            message: 'Sto eseguendo il login',
          }
        )
        .subscribe({
          next: (response) => {
            this.userSvc.setAuthentication(response.user, response.accessToken);
            this.router.navigate([response.user.roles[0]]);
            this.toastSvc.addSuccessToast({
              header: 'Benvenuto!',
              message:
                response.user.roles[0] === 'user'
                  ? 'Inizia a prenotare i tuoi corsi.'
                  : 'Inizia a gestire corsi e prenotazioni.',
            });
          },
          error: (error) =>
            this.toastSvc.addErrorToast({
              message: 'Password o nome utente sbagliati.',
            }),
        });
    }
  }

  private register() {
    if (this.registerForm.valid) {
      const registerValue: RegisterRequestDto = this.registerForm.value;
      // eslint-disable-next-line max-len
      this.loadingSvc
        .startLoading(
          this,
          'REGISTER',
          this.identityService.registerUser(registerValue),
          { message: 'Sto creando il tuo profilo' }
        )
        .subscribe({
          next: (response) => {
            this.toastSvc.addSuccessToast({
              header: 'Utente creato',
              message: `L'utente è stato creato con successo.`,
            });
            this.currentForm = 'login';
          },
          error: (httpError) => {
            this.toastSvc.addErrorToast({
              message:
                httpError?.error?.message ||
                `Errore durante la creazione dell'utente.`,
            });
          },
        });
    }
  }

  private sendForgotPasswordEmail(email: string) {
    // eslint-disable-next-line max-len
    this.loadingSvc
      .startLoading(
        this,
        'REGISTER',
        this.identityService.sendPasswordResetLink(email),
        { message: 'Sto provando ad inviare la mail.' }
      )
      .subscribe({
        next: (response) => {
          this.toastSvc.addSuccessToast({
            header: 'Mail inviata',
            message: `La mail è stata inviata con successo all'indirizzo indicato. Se non la vedi controlla la posta indesiderata.`,
          });
        },
        error: (error) =>
          this.toastSvc.addErrorToast({
            message: `Errore durante l'invio della mail.`,
          }),
      });
  }
}
