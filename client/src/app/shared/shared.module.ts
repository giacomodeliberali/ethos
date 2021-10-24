import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { InputComponent } from './components/input/input.component';
import { LoadingToastComponent } from './components/loading-toast/loading-toast.component';
import { LoadingComponent } from './components/loading-toast/loading.component';
import { LogoutModalComponent } from './components/logout-modal/logout-modal.component';
import { ModalDefaultContentComponent } from './components/modal-default-content/modal-default-content.component';
import { UnsavedChangesComponent } from './components/unsaved-changes/unsaved-changes.component';

@NgModule({
  declarations: [
    InputComponent,
    LoadingToastComponent,
    LogoutModalComponent,
    ModalDefaultContentComponent,
    UnsavedChangesComponent,
    LoadingComponent,
  ],
  imports: [CommonModule, IonicModule, ReactiveFormsModule],
  exports: [
    InputComponent,
    LoadingToastComponent,
    LogoutModalComponent,
    ModalDefaultContentComponent,
    UnsavedChangesComponent,
  ],
})
export class SharedModule {}
