import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { BaseDirective } from "@core/directives";
import { AccountsService } from "@core/services/ethos.generated.service";
import { LoadingService } from "@shared/services/loading.service";
import { ToastService } from "@shared/services/toast.service";
import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

@Component({
  selector: 'app-reset-password-page',
  templateUrl: './reset-password-page.component.html',
  styleUrls: ['./reset-password-page.component.scss'],
})
export class ResetPasswordPageComponent extends BaseDirective {
  private email: string;
  private resetToken: string;

  newPasswordForm = new FormGroup({
    password: new FormControl('', [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$/)])
  })

  constructor(
    private router: Router, 
    private route: ActivatedRoute,
    private accountsSvc: AccountsService,
    private loadingSvc: LoadingService,
    private toastSvc: ToastService) { 
      super();
      this.email = this.route.snapshot.queryParamMap.get('email')
      this.resetToken = this.route.snapshot.queryParamMap.get('resetToken')
    }

  resetPassword(){
    if(this.newPasswordForm.valid){
      new Observable(subscriber => {
        subscriber.next([this.email, this.resetToken]);
        subscriber.complete();
      }).pipe(
        map(([email, token]) => {
          console.log(email, token, this.email, this.resetToken)
          if(!email || !token)
            throw new Error();
          return [email, token]
        }),
        switchMap(_ => this.loadingSvc.startLoading(this, 'RESET_PASSWORD', this.accountsSvc.resetPassword({
          email: this.email,
          resetToken: this.resetToken,
          newPassword: this.newPasswordForm.get('password').value
        }), { message: 'Sto reimpostando la password.'}))).subscribe({
          next: _ => {
            this.toastSvc.addSuccessToast({
              header: 'Password cambiata',
              message: 'La password è stata cambiata con successo.'
            });
            this.router.navigate(['auth', 'login']);
          },
          error: _ => {
            this.toastSvc.addErrorToast({
              message: 'Il link utilizzato è sbagliato o il codice di reset non è più valido.'
            });
            this.router.navigate(['auth', 'login']);
          }
        })
    }
    this.router.navigate(['auth', 'login']);
  }

}
