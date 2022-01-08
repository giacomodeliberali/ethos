import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { DateTimeInputComponent } from './components/form-field/form-field-types/date-time-input/date-time-input.component';
import { DefaultInputComponent } from './components/form-field/form-field-types/default-input/default-input.component';
import { PasswordInputComponent } from './components/form-field/form-field-types/password-input/password-input.component';
import { TextareaInputComponent } from './components/form-field/form-field-types/textarea-input/textarea-input.component';
import { FormFieldComponent } from './components/form-field/form-field.component';
import { LoadingToastComponent } from './components/loading-toast/loading-toast.component';
import { LoadingComponent } from './components/loading-toast/loading.component';
import { LogoutModalComponent } from './components/logout-modal/logout-modal.component';
import { ModalDefaultContentComponent } from './components/modal-default-content/modal-default-content.component';
import { ScheduleCardComponent } from './components/schedule-card/schedule-card.component';
import { UnsavedChangesComponent } from './components/unsaved-changes/unsaved-changes.component';
import { WeekDaySelectorComponent } from './components/week-day-selector/week-day-selector.component';

@NgModule({
  declarations: [
    FormFieldComponent,
    LoadingToastComponent,
    LogoutModalComponent,
    ModalDefaultContentComponent,
    UnsavedChangesComponent,
    LoadingComponent,
    ScheduleCardComponent,
    WeekDaySelectorComponent,
    DefaultInputComponent,
    PasswordInputComponent,
    TextareaInputComponent,
    DateTimeInputComponent,
  ],
  imports: [CommonModule, IonicModule, ReactiveFormsModule],
  exports: [
    FormFieldComponent,
    LoadingToastComponent,
    LogoutModalComponent,
    ModalDefaultContentComponent,
    UnsavedChangesComponent,
    ScheduleCardComponent,
    WeekDaySelectorComponent,
  ],
})
export class SharedModule {}
