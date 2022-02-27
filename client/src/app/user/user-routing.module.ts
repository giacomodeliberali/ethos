import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BookingsPage } from './pages/bookings-page/bookings.page';
import { UserPageComponent } from './pages/user-page/user-page.component';
import { UserSettingsPage } from './pages/user-settings/user-settings.page';

const routes: Routes = [
  {
    path: '',
    component: UserPageComponent,
  },
  {
    path: 'bookings',
    component: BookingsPage,
  },
  {
    path: 'settings',
    component: UserSettingsPage,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserRoutingModule {}
