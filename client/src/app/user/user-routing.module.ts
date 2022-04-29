import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BookingsPage } from './pages/bookings-page/bookings.page';
import { UserPageComponent } from './pages/user-page/user-page.component';
import { UserSchedulesPageComponent } from './pages/user-schedules-page/user-schedules-page.component';
import { UserSettingsPage } from './pages/user-settings/user-settings.page';

const routes: Routes = [
  {
    path: '',
    component: UserSchedulesPageComponent,
    children: [
      {
        path: 'schedules',
        component: UserPageComponent,
      },
      {
        path: 'bookings',
        component: BookingsPage,
      },
      {
        path: 'profile',
        component: UserSettingsPage,
      },
      {
        path: '',
        redirectTo: '/user/schedules',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: '',
    redirectTo: '/user/schedules',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserRoutingModule {}
