import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { SharedModule } from '@shared/shared.module';
import { AdminRoutingModule } from './admin-routing.module';
import { CreateEditScheduleModalComponent } from './components/create-edit-schedule-modal/create-edit-schedule-modal.component';
import { DeleteScheduleModalComponent } from './components/delete-schedule-modal/delete-schedule-modal.component';
import { ShowBookingModalComponent } from './components/show-booking-modal/show-booking-modal.component';
import { AdminPageComponent } from './pages/admin-page/admin-page.component';
import { UserSettingsPage } from './pages/user-settings/user-settings.page';

@NgModule({
  declarations: [
    AdminPageComponent,
    ShowBookingModalComponent,
    DeleteScheduleModalComponent,
    CreateEditScheduleModalComponent,
    UserSettingsPage,
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    SharedModule,
    ReactiveFormsModule,
    IonicModule,
  ],
})
export class AdminModule {}
