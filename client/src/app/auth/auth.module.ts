import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "@shared/shared.module";
import { AuthRoutingModule } from "./auth-routing.module";
import { ForgotPasswordModalComponent } from "./components/forgot-password-modal/forgot-password-modal.component";
import { LoginPageComponent } from "./pages/login-page/login-page.component";



@NgModule({
  declarations: [LoginPageComponent, ForgotPasswordModalComponent],
  imports: [
    CommonModule,
    AuthRoutingModule,
    SharedModule,
    ReactiveFormsModule
  ]
})
export class AuthModule { }