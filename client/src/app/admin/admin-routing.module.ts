import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPageComponent } from './pages/admin-page/admin-page.component';
import { AdminSchedulesPageComponent } from './pages/admin-schedules-page/admin-schedules-page.component';
import { AdminsListPage } from './pages/admins-list/admins-list.page';
import { UserSettingsPage } from './pages/user-settings/user-settings.page';

const routes: Routes = [
  {
    path: '',
    component: AdminPageComponent,
    children: [
      {
        path: 'schedules',
        component: AdminSchedulesPageComponent,
      },
      {
        path: 'users',
        component: AdminsListPage,
      },
      {
        path: 'profile',
        component: UserSettingsPage,
      },
      {
        path: '',
        redirectTo: '/admin/schedules',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: '',
    redirectTo: '/admin/schedules',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
