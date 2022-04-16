import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPageComponent } from './pages/admin-page/admin-page.component';
import { AdminsListPage } from './pages/admins-list/admins-list.page';
import { UserSettingsPage } from './pages/user-settings/user-settings.page';

const routes: Routes = [
  {
    path: '',
    component: AdminPageComponent,
  },
  {
    path: 'user-settings',
    component: UserSettingsPage,
  },
  {
    path: 'users',
    component: AdminsListPage,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
