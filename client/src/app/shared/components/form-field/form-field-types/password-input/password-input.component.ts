import { Component } from '@angular/core';
import { DefaultInputComponent } from '../default-input/default-input.component';

@Component({
  selector: 'app-password-input',
  templateUrl: './password-input.component.html',
  styleUrls: ['../input-base-style.scss', './password-input.component.scss'],
})
export class PasswordInputComponent extends DefaultInputComponent {
  showPassword = false;

  constructor() {
    super();
  }

  changeShowPassword() {
    this.showPassword = !this.showPassword;
    this.type = this.showPassword ? 'text' : 'password';
  }
}
