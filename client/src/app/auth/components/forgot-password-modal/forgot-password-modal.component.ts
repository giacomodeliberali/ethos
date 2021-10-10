import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-forgot-password-modal',
  templateUrl: './forgot-password-modal.component.html',
  styleUrls: ['./forgot-password-modal.component.scss'],
})
export class ForgotPasswordModalComponent {

  forgotPasswordForm: FormGroup = new FormGroup({
    email: new FormControl('', [ Validators.required, Validators.email])
  });

  constructor(private modalCtrl: ModalController) { }

  closeModal(event: 'success' | 'cancel'){
    if(event === 'success'){
      if(this.forgotPasswordForm.valid){
        this.modalCtrl.dismiss({email: this.forgotPasswordForm.get('email').value});
      }
    }else{
      this.modalCtrl.dismiss({user: null});
    }
  }

}
