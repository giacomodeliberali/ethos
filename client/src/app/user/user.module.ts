import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { SharedModule } from '@shared/shared.module';
import { BookingsPage } from './pages/bookings-page/bookings.page';
import { UserPageComponent } from './pages/user-page/user-page.component';
import { UserSchedulesPageComponent } from './pages/user-schedules-page/user-schedules-page.component';
import { UserSettingsPage } from './pages/user-settings/user-settings.page';
import { UserRoutingModule } from './user-routing.module';

@NgModule({
  declarations: [
    UserSchedulesPageComponent,
    UserPageComponent,
    UserSettingsPage,
    BookingsPage,
  ],
  imports: [
    CommonModule,
    UserRoutingModule,
    SharedModule,
    ReactiveFormsModule,
    IonicModule,
  ],
})
export class UserModule {}
