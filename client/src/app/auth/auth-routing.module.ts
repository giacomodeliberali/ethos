import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NoAuthGuard } from '@core/guards';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { ResetPasswordPageComponent } from './pages/reset-password-page/reset-password-page.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginPageComponent,
    data: { animation: 'LoginPage' },
    canActivate: [NoAuthGuard],
  },
  {
    path: 'reset-password',
    component: ResetPasswordPageComponent,
    data: { animation: 'ResetPasswordPage' },
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
